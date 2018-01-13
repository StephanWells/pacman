using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    public float playerSpeed;
    private Vector2 playerDirection, nextDirection;
    private Animator animator;
    public Node currentNode, previousNode, targetNode;

    // Called when the game starts.
    void Start()
    {
        // Initialising values.
        animator = this.GetComponent<Animator>();
        Node node = GetNodeAtPosition(this.transform.localPosition);
        playerDirection = Vector2.zero;

        if (node != null)
        {
            currentNode = node;
        }
        else
        {
            Debug.Log("Error! Starting point has no node attached to it.");
        }
    }

    // Called every tick.
    void Update()
    {
        CheckInput();
        Move();
        ConsumePellet();
    }

    // Handles player input (arrow keys).
    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangePosition(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangePosition(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangePosition(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePosition(Vector2.down);
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
            if (Overshot()) // If we reached our target node.
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
                    playerDirection = Vector2.zero;
                }
            }
            else
            {
                transform.localPosition += (Vector3)(playerDirection * playerSpeed) * Time.deltaTime; // Move Pacman.
            }

            SetAnimatorState(playerDirection);
        }
    }

    // Updates the animation Pacman's currently on based on the direction parameter.
    void SetAnimatorState(Vector2 state)
    {
        animator.SetBool("Right", false);
        animator.SetBool("Left", false);
        animator.SetBool("Up", false);
        animator.SetBool("Down", false);
        animator.SetBool("Idle", false);

        if (state == Vector2.right)
        {
            animator.SetBool("Right", true);
        }
        else if (state == Vector2.left)
        {
            animator.SetBool("Left", true);
        }
        else if (state == Vector2.up)
        {
            animator.SetBool("Up", true);
        }
        else if (state == Vector2.down)
        {
            animator.SetBool("Down", true);
        }
        else if (state == Vector2.zero)
        {
            animator.SetBool("Idle", true);
        }
    }

    // Handles eating dots / power pills and removing them from screen.
    void ConsumePellet()
    {
        GameObject obj = GetTileAtPosition(this.transform.position);

        if (obj != null)
        {
            Tile tile = obj.GetComponent<Tile>();

            if (tile != null)
            {
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
            if (currentNode.validDirections[i] == dir) // If the direction we want to go in is part of the valid directions (has a node where it can go to).
            {
                nextNode = currentNode.neighbouringNodes[i]; // Choose that node to go to.

                break;
            }
        }

        return nextNode;
    }

    // Returns the pill object at a given position on the game board.
    GameObject GetTileAtPosition(Vector2 pos)
    {
        GameBoard gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        Vector2Int board = gameBoard.WorldToBoard(pos);
        GameObject tile = gameBoard.pellets[board.x, board.y];

        if (tile != null)
        {
            return tile;
        }

        return null;
    }

    // Returns the node at a given position on the game board.
    Node GetNodeAtPosition(Vector2 pos)
    {
        GameBoard gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        Vector2Int board = gameBoard.WorldToBoard(pos);
        GameObject tile = gameBoard.nodes[board.x, board.y];

        if (tile != null)
        {
            return tile.GetComponent<Node>();
        }

        return null;
    }

    // Checks if Pacman can go further or not based on its current position.
    bool Overshot()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf >= nodeToTarget;
    }

    // Returns the length from the previous node to the target coordinates parameter.
    float LengthFromNode(Vector2 targetPos)
    {
        Vector2 vec = targetPos - (Vector2)previousNode.transform.position;

        return vec.sqrMagnitude;
    }
}