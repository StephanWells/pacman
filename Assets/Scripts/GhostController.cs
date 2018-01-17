using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public enum Mode { SCATTER, CHASE, FRIGHTENED, IDLE };
    public enum Ghost { BLINKY, PINKY, INKY, CLYDE };

    public float ghostSpeed = 0.4f;
    public Node startingPosition;
    public Node homeNode;

    public int[] timers = new int[8];
    public Mode[] modes = new Mode[8];
    public Ghost ghost;

    private int stateIteration = 0;
    private float modeChangeTimer = 0;
    private AnimationController.State ghostState;

    Mode currentMode;

    private GameObject pacMan;
    private GameObject blinky, pinky, inky, clyde;
    private GameBoard gameBoard;
    private AnimationController animator;

    private Vector2 ghostDirection, nextDirection;
    private Node currentNode, previousNode, targetNode;

    void Start ()
    {
        pacMan = GameObject.Find("Pacman");
        blinky = GameObject.Find("Blinky");
        pinky = GameObject.Find("Pinky");
        inky = GameObject.Find("Inky");
        clyde = GameObject.Find("Clyde");
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        animator = this.GetComponent<AnimationController>();
        currentMode = Mode.IDLE;
        ghostState = AnimationController.State.STILL;

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
                transform.localPosition += (Vector3)(ghostDirection * ghostSpeed) * Time.deltaTime * (currentMode == Mode.IDLE ? 0.0f : 1.0f);
            }
        }

        animator.SetAnimatorDirection(ghostDirection);
        animator.SetAnimatorState(ghostState);
    }

    // Blinky follows Pacman.
    Vector2 GetBlinkyChaseTile()
    {
        Vector2 pacmanPosition = pacMan.transform.position;
        Vector2 targetTile = gameBoard.WorldToBoard(pacmanPosition);

        return targetTile;
    }

    // Pinky goes four tiles ahead of Pacman.
    Vector2 GetPinkyChaseTile()
    {
        Vector2 pacmanPosition = pacMan.transform.position;
        Vector2 pacmanDirection = pacMan.GetComponent<PacmanController>().playerDirection;
        Vector2 targetTile = gameBoard.WorldToBoard(pacmanPosition) + 4 * pacmanDirection;

        return targetTile;
    }

    // Inky considers the tile two tiles ahead of Pacman, then draws a direction vector from it to Blinky, doubles that vector, and chooses the tile that the vector lands at.
    Vector2 GetInkyChaseTile()
    {
        Vector2 pacmanPosition = pacMan.transform.position;
        Vector2 pacmanDirection = pacMan.GetComponent<PacmanController>().playerDirection;
        Vector2 pacmanTile = gameBoard.WorldToBoard(pacmanPosition) + 2 * pacmanDirection;
        Vector2 blinkyPos = gameBoard.WorldToBoard(blinky.transform.position);
        Vector2 blinkyPacman = (pacmanTile - blinkyPos) * 2;
        Vector2 targetTile = blinkyPos + blinkyPacman;

        return targetTile;
    }

    // Clyde chases Pacman Blinky-style if he's fewer than 8 units away from him, or runs to his home node if he's over 8 unites away from him.
    Vector2 GetClydeChaseTile()
    {
        Vector2 pacmanPosition = gameBoard.WorldToBoard(pacMan.transform.position);
        Vector2 ghostPosition = gameBoard.WorldToBoard(this.transform.position);
        float distance = gameBoard.GetSquaredDistance(pacmanPosition, ghostPosition);
        Vector2 targetTile = Vector2.zero;

        if (distance <= 64f)
        {
            targetTile = pacmanPosition;
        }
        else
        {
            targetTile = gameBoard.WorldToBoard(homeNode.transform.position);
        }

        return targetTile;
    }

    Vector2 GetGhostScatterTile()
    {
        return gameBoard.WorldToBoard(homeNode.transform.position);
    }

    public Vector2 GetTargetTile()
    {
        Vector2 targetTile = Vector2.zero;

        switch (currentMode)
        {
            case Mode.SCATTER:
                targetTile = GetGhostScatterTile();
            break;

            case Mode.CHASE:
                switch (ghost)
                {
                    case Ghost.BLINKY:
                        targetTile = GetBlinkyChaseTile();
                    break;

                    case Ghost.PINKY:
                        targetTile = GetPinkyChaseTile();
                    break;

                    case Ghost.INKY:
                        targetTile = GetInkyChaseTile();
                    break;

                    case Ghost.CLYDE:
                        targetTile = GetClydeChaseTile();
                    break;
                }
            break;

            case Mode.FRIGHTENED:

            break;

            case Mode.IDLE:
                targetTile = gameBoard.WorldToBoard(this.transform.position);
            break;
        }

        return targetTile;
    }

    Node ChooseNextNode()
    {
        Vector2 targetTile = GetTargetTile();
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

            if (stateIteration == 1)
            {
                ghostState = AnimationController.State.MOVING;
            }

            if (stateIteration == timers.Length)
            {
                ChangeMode(modes[timers.Length]);
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
        currentMode = mode;
    }
}