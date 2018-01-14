using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    enum Mode { SCATTER, CHASE, FRIGHTENED };

    public float ghostSpeed = 0.4f;
    public Node startingPosition;

    private int[] timers = { 7, 20, 7, 20, 5, 20, 5};
    private Mode[] modes = { Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE };

    private int stateIteration = 0;
    private float modeChangeTimer = 0;

    Mode currentMode;
    Mode previousMode;

    private GameObject pacMan;
    private GameBoard gameBoard;

    private Animator animator;
    private Vector2 ghostDirection, nextDirection;
    private Node currentNode, previousNode, targetNode;

    void Start ()
    {
        pacMan = GameObject.Find("Pacman");
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        animator = this.GetComponent<Animator>();
        currentMode = Mode.SCATTER;

        currentNode = startingPosition;
        previousNode = currentNode;
        targetNode = ChooseNextNode();
    }

	void Update ()
    {
        ModeUpdate();
        Move();
	}

    void Move()
    {
        if (targetNode != currentNode && targetNode != null)
        {
            if (gameBoard.Overshot(previousNode, targetNode, this.transform.position))
            {
                currentNode = targetNode;

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

                targetNode = ChooseNextNode();
                previousNode = currentNode;
                currentNode = null;
            }
            else
            {
                transform.localPosition += (Vector3)ghostDirection * ghostSpeed * Time.deltaTime;
            }
        }
    }

    Node ChooseNextNode()
    {
        Vector2 pacmanPosition = pacMan.transform.position;
        Vector2 targetTile = gameBoard.WorldToBoard(pacmanPosition);
        Debug.Log(gameBoard.WorldToBoard(pacmanPosition));
        Node moveToNode = null;

        List<Node> foundNodes = new List<Node>();
        List<Vector2> foundNodeDirections = new List<Vector2>();

        for (int i = 0; i < currentNode.neighbouringNodes.Length; i++)
        {
            if (currentNode.validDirections[i] != ghostDirection * -1)
            {
                foundNodes.Add(currentNode.neighbouringNodes[i]);
                foundNodeDirections.Add(currentNode.validDirections[i]);
            }
        }

        if (foundNodes.Count == 1)
        {
            moveToNode = foundNodes[0];
            ghostDirection = foundNodeDirections[0];
        }
        else if (foundNodes.Count >= 1)
        {
            float leastDistance = float.MaxValue;

            for (int i = 0; i < foundNodes.Count; i++)
            {
                float distance = gameBoard.GetSquaredDistance(gameBoard.WorldToBoard(foundNodes[i].transform.position), targetTile);

                if (distance < leastDistance)
                {
                    leastDistance = distance;
                    moveToNode = foundNodes[i];
                    ghostDirection = foundNodeDirections[i];
                }
            }
        }

        return moveToNode;
    }

    void ModeUpdate()
    {
        if (currentMode != Mode.FRIGHTENED)
        {
            modeChangeTimer += Time.deltaTime;

            if (stateIteration == 7)
            {
                ChangeMode(modes[7]);
            }
            else
            {
                if (modeChangeTimer > timers[stateIteration])
                {
                    stateIteration++;
                    modeChangeTimer = 0;
                    ChangeMode(modes[stateIteration]);
                }
            }
        }
    }

    void ChangeMode(Mode mode)
    {
        previousMode = currentMode;
        currentMode = mode;
    }
}