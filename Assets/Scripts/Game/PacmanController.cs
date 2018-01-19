﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    public float playerSpeed;

    public bool canMove = true;

    public Node startingPosition;
    public Vector2 playerDirection;

    private AnimationController.State playerState;
    private AnimationController animator;
    private Vector2 nextDirection;
    private GameObject[] ghosts = new GameObject[4];
    private Node currentNode, previousNode, targetNode;
    private GameBoard gameBoard;

    // Called when the game starts.
    void Start()
    {
        // Initialising values.
        animator = this.GetComponent<AnimationController>();
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        
        ghosts[0] = GameObject.Find("Blinky");
        ghosts[1] = GameObject.Find("Pinky");
        ghosts[2] = GameObject.Find("Inky");
        ghosts[3] = GameObject.Find("Clyde");

        playerDirection = Vector2.right;
        playerState = AnimationController.State.MOVING;
        currentNode = startingPosition;
        ChangePosition(playerDirection);
    }

    public void Restart()
    {
        this.transform.position = startingPosition.transform.position;
        playerDirection = Vector2.right;
        nextDirection = Vector2.right;
        playerState = AnimationController.State.ALIVE;
        playerState = AnimationController.State.MOVING;
        currentNode = startingPosition;
        ChangePosition(playerDirection);
    }

    // Called every tick.
    void Update()
    {
        if (canMove)
        {
            CheckInput();
            Move();
            ConsumePellet();
        }
    }

    // Handles player input (arrow keys).
    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangePosition(Vector2.right);
            playerState = AnimationController.State.MOVING;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangePosition(Vector2.up);
            playerState = AnimationController.State.MOVING;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangePosition(Vector2.left);
            playerState = AnimationController.State.MOVING;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePosition(Vector2.down);
            playerState = AnimationController.State.MOVING;
        }
    }

    // Updates the player's next direction, and make it the player's current direction if it's valid.
    void ChangePosition(Vector2 dir)
    {
        nextDirection = dir; // Update the direction we want to go to.

        if (dir == (-1) * playerDirection) // If the player wants to turn back while halfway to a node.
        {
            // Swap the player's direction and nodes.
            playerDirection = dir;
            Node temp = targetNode;
            targetNode = previousNode;
            previousNode = temp;
            currentNode = null;
        }

        if (currentNode != null) // If we're on a node.
        {
            Node moveToNode = CanMove(dir);
            
            if (moveToNode != null) // If it's valid.
            {
                // Update the direction and nodes.
                playerDirection = dir;
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }

    // Move Pacman in the player's current direction.
    void Move()
    {
        if (targetNode != currentNode && targetNode != null) // If the game thinks we have not yet reached our target node.
        {
            if (gameBoard.Overshot(previousNode, targetNode, transform.localPosition)) // If we reached our target node.
            {
                currentNode = targetNode; // Update the node information.

                // Handling left/right portals.
                if (currentNode.GetComponent<Tile>() != null && currentNode.GetComponent<Tile>().isPortal)
                {
                    this.transform.localPosition = currentNode.GetComponent<Tile>().portalReceiver.transform.position;
                    currentNode = currentNode.GetComponent<Tile>().portalReceiver.GetComponent<Node>();
                }
                else
                {
                    this.transform.localPosition = currentNode.transform.position;
                }

                Node moveToNode = CanMove(nextDirection);

                // Updates the direction to the desired one if it's valid, or checks if Pacman can keep going if the desired direction isn't valid.
                if (moveToNode != null)
                {
                    playerDirection = nextDirection;
                }
                else
                {
                    moveToNode = CanMove(playerDirection);
                }

                // Updates node information if there's a node Pacman can go to or else brings him to a halt.
                if (moveToNode != null)
                {
                    targetNode = moveToNode;
                    previousNode = currentNode;
                    currentNode = null;
                }
                else
                {
                    playerState = AnimationController.State.STILL;
                }
            }
            else
            {
                transform.localPosition += (Vector3)(playerDirection * playerSpeed) * Time.deltaTime * (playerState == AnimationController.State.STILL ? 0.0f : 1.0f); // Move Pacman.
            }

            animator.SetAnimatorDirection(playerDirection);
            animator.SetAnimatorState(playerState);
        }
    }

    // Handles eating dots / power pills and removing them from screen.
    void ConsumePellet()
    {
        GameObject obj = gameBoard.GetTileAtPosition(this.transform.position);

        if (obj != null)
        {
            Tile tile = obj.GetComponent<Tile>();

            if (tile != null)
            {
                if (tile.isPowerPill && !tile.consumed)
                {
                    for (int i = 0; i < ghosts.Length; i++)
                    {
                        ghosts[i].GetComponent<GhostController>().enterFrightenedMode();
                    }
                }

                obj.GetComponent<Tile>().consume();
            }
        }
    }

    // Returns a node in the given direction if there is a valid one, otherwise returning null.
    Node CanMove(Vector2 dir)
    {
        Node nextNode = null;

        for (int i = 0; i < currentNode.neighbouringNodes.Length; i++) // Go through the neighbours of our current node.
        {
            if (currentNode.validDirections[i] == dir && !currentNode.neighbouringNodes[i].isGhostNode) // If the direction we want to go in is part of the valid directions (has a node where it can go to).
            {
                nextNode = currentNode.neighbouringNodes[i]; // Choose that node to go to.

                break;
            }
        }

        return nextNode;
    }
}