using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{

    public float playerSpeed;
    private Vector2 playerDirection;
    private Animator animator;
    public Node currentNode;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        Node node = GetNodeAtPosition(this.transform.localPosition);

        if (node != null)
        {
            currentNode = node;
            Debug.Log(currentNode);
        }
        else
        {
            Debug.Log("null!");
        }
    }

    void Update()
    {
        CheckInput();
        //Move();
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            playerDirection = Vector2.right;
            SetAnimatorState("Right");
            MoveToNode(playerDirection);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerDirection = Vector2.up;
            SetAnimatorState("Up");
            MoveToNode(playerDirection);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playerDirection = Vector2.left;
            SetAnimatorState("Left");
            MoveToNode(playerDirection);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            playerDirection = Vector2.down;
            SetAnimatorState("Down");
            MoveToNode(playerDirection);
        }
    }

    void Move()
    {
        transform.localPosition += (Vector3)(playerDirection * playerSpeed) * Time.deltaTime;
    }

    void SetAnimatorState(string state)
    {
        animator.SetBool("Right", false);
        animator.SetBool("Left", false);
        animator.SetBool("Up", false);
        animator.SetBool("Down", false);
        animator.SetBool(state, true);
    }

    void MoveToNode (Vector2 dir)
    {
        Node nextNode = CanMove(dir);
        Debug.Log(nextNode);

        if (nextNode != null)
        {
            transform.localPosition = nextNode.transform.position;
            currentNode = nextNode;
        }
    }

    Node CanMove (Vector2 dir)
    {
        Node nextNode = null;

        for (int i = 0; i < currentNode.neighbouringNodes.Length; i++) // Go through the neighbours of our current node.
        {
            Debug.Log("Number of neighbouring nodes: " + currentNode.neighbouringNodes.Length);
            if (currentNode.validDirections[i] == dir) // If the direction we want to go in is part of the valid directions (has a node where it can go to).
            {
                nextNode = currentNode.neighbouringNodes[i]; // Choose that node to go to.

                break;
            }
        }

        return nextNode;
    }

    Node GetNodeAtPosition (Vector2 pos)
    {
        int x = (int)((pos.x / 0.08) + 12.5f); // Each x unit in the game board is 0.08 Unity units across and starts at -1.
        int y = (int)((pos.y / 0.08) + 14.0f); // Each y unit in the game board is 0.08 Unity units across and starts at -1.12.
        GameObject tile = GameObject.Find("GameBoard").GetComponent<GameBoard>().board[x, y];

        if (tile != null)
        {
            return tile.GetComponent<Node>();
        }

        return null;
    }
}
