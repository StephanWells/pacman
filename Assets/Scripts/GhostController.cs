using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public enum Mode { SCATTER, CHASE, FRIGHTENED, RECOVERY, IDLE };
    public enum Ghost { BLINKY, PINKY, INKY, CLYDE };
    private const float frightenedTime = 10f;
    private const float frightenedSlow = 0.5f;

    public float ghostSpeed = 0.4f;
    public float frightenedFactor = 1f;
    public Node startingPosition;
    public Node homeNode;

    public int[] timers = new int[8];
    public Mode[] modes = new Mode[8];
    public Ghost ghost;

    private int stateIteration = 0;
    private float modeChangeTimer = 0;
    private float frightenedTimer = 0;
    private AnimationController.State ghostState;
    private GameObject ghostHouseEntrance;
    private GameObject ghostHouse;

    Mode currentMode, previousMode;

    private GameObject pacMan;
    private GameObject blinky;
    private GameBoard gameBoard;
    private AnimationController animator;

    private Vector2 ghostDirection, nextDirection;
    private Node currentNode, previousNode, targetNode;

    void Start()
    {
        pacMan = GameObject.Find("Pacman");
        blinky = GameObject.Find("Blinky");
        ghostHouseEntrance = GameObject.Find("StartNodeBlinky");
        ghostHouse = GameObject.Find("StartNodePinky");
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        animator = this.GetComponent<AnimationController>();
        currentMode = Mode.IDLE;
        ghostState = AnimationController.State.STILL;

        currentNode = startingPosition;
        previousNode = currentNode;
        targetNode = ChooseNextNode();
    }

	void Update()
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
                transform.localPosition += (Vector3)(ghostDirection * ghostSpeed) * Time.deltaTime * (currentMode == Mode.IDLE ? 0.0f : 1.0f) * frightenedFactor;
            }
        }

        animator.SetAnimatorDirection(ghostDirection);
        animator.SetAnimatorState(ghostState);
    }

    Node ChooseNextNode()
    {
        Vector2 targetTile = GetTargetTile();
        Node moveToNode = null;

        List<Node> foundNodes = new List<Node>();
        List<Vector2> foundNodeDirections = new List<Vector2>();

        if (ghost == Ghost.BLINKY)
        {
            Debug.Log("ChooseNextNode currentnode: " + currentNode);
            Debug.Log("ChooseNextNode previousnode: " + previousNode);
            Debug.Log("ChooseNextNode ghosthousebool: " + isInGhostHouse());
            Debug.Log(currentNode.name);
        }
        
        for (int i = 0; i < currentNode.neighbouringNodes.Length; i++)
        {
            Vector2 tempDir = currentNode.validDirections[i];
            Node tempNode = currentNode.neighbouringNodes[i];
            bool isValidDirection = tempDir != ghostDirection * -1;

            if ((isValidDirection && !tempNode.isGhostNode) || (isValidDirection && currentMode == Mode.RECOVERY) || (isInGhostHouse()))
            {
                foundNodes.Add(tempNode);
                foundNodeDirections.Add(tempDir);
            }
        }

        if (ghost == Ghost.BLINKY)
        {
            Debug.Log("ChooseNextNode nodecount: " + foundNodes.Count);
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
                targetTile = GetGhostScatterTile();
                CheckCollision();
            break;

            case Mode.RECOVERY:
                targetTile = gameBoard.WorldToBoard(ghostHouse.transform.position);
            break;

            case Mode.IDLE:
                targetTile = gameBoard.WorldToBoard(this.transform.position);
            break;
        }

        return targetTile;
    }

    void CheckCollision()
    {
        Rect ghostRect = new Rect(this.transform.position, this.transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);
        Rect pacmanRect = new Rect(pacMan.transform.position, pacMan.transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);

        if (ghostRect.Overlaps(pacmanRect))
        {
            enterRecoveryMode();
        }
    }

    void ModeUpdate()
    {
        //if (ghost == Ghost.BLINKY)
        //{
        //    Debug.Log("Current node: " + currentNode + ". Previous node: " + previousNode + ". Target node: " + targetNode + ". Current mode: " + currentMode + ".");
        //}

        if (currentMode != Mode.FRIGHTENED && currentMode != Mode.RECOVERY)
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
        else if (currentMode == Mode.FRIGHTENED)
        {
            frightenedTimer += Time.deltaTime;

            if (frightenedTimer >= frightenedTime * 0.7 && ghostState == AnimationController.State.FRIGHTENED)
            {
                enterFrightened2Mode();
            }

            if (frightenedTimer >= frightenedTime)
            {
                exitFrightenedMode();
            }
        }
        else
        {
            if (previousNode == ghostHouse.GetComponent<Node>())
            {
                exitRecoveryMode();
            }
        }
    }

    void ChangeMode(Mode mode)
    {
        if (currentMode != Mode.FRIGHTENED && currentMode != Mode.RECOVERY)
        {
            previousMode = currentMode;
        }
        
        currentMode = mode;
    }

    public void enterFrightenedMode()
    {
        if (currentMode != Mode.IDLE && currentMode != Mode.RECOVERY)
        {
            frightenedTimer = 0;
            ChangeMode(Mode.FRIGHTENED);
            ghostState = AnimationController.State.FRIGHTENED;
            frightenedFactor = frightenedSlow;
            reverseDirection();
        }
    }

    public void enterFrightened2Mode()
    {
        ghostState = AnimationController.State.FRIGHTENED2;
    }

    public void enterRecoveryMode()
    {
        ChangeMode(Mode.RECOVERY);
        ghostState = AnimationController.State.RECOVERY;
        frightenedFactor = 1f;
    }

    void exitFrightenedMode()
    {
        ChangeMode(previousMode);
        ghostState = AnimationController.State.RECOVERED;
        frightenedFactor = 1f;
    }

    void exitRecoveryMode()
    {
        modeChangeTimer = 0;
        stateIteration = 1;
        ChangeMode(modes[stateIteration]);
        ghostState = AnimationController.State.RECOVERED;
    }

    bool isInGhostHouse()
    {
        return currentNode.name.Equals("StartNodePinky") || currentNode.name.Equals("StartNodeInky") || currentNode.name.Equals("StartNodeClyde");
    }

    bool isLeavingInGhostHouse()
    {
        return previousNode.name.Equals("StartNodePinky") || previousNode.name.Equals("StartNodeInky") || previousNode.name.Equals("StartNodeClyde");
    }

    void reverseDirection()
    {
        if (!isLeavingInGhostHouse())
        {
            ghostDirection *= -1;

            Node temp = targetNode;
            targetNode = previousNode;
            previousNode = temp;
            currentNode = null;
        }
    }
}