using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public enum Mode { SCATTER, CHASE, FRIGHTENED, RECOVERY, IDLE };
    public enum Ghost { BLINKY, PINKY, INKY, CLYDE };

    private float frightenedTime = 0f;
    private float frightenedSlow = 0f;
    private float ghostSpeed = 0f;
    private float recoveryFactor = 0f;

    public bool canMove = true;

    public Node startingPosition;
    public Node homeNode;
    
    public float[] timers = new float[8];
    public Mode[] modes = new Mode[9];
    public Ghost ghost;

    private int stateIteration = 0;
    private static int consumedGhosts = 0;
    private float modeChangeTimer = 0;
    private float frightenedTimer = 0;
    private float frightenedFactor = 1f;

    private GameObject ghostHouse;
    private GameObject pacMan;
    private GameObject blinky;
    private GameBoard gameBoard;

    private AnimationController.State ghostState;
    private Mode currentMode, previousMode;

    private AnimationController animator;

    private Vector2 ghostDirection, nextDirection;
    private Node currentNode, previousNode, targetNode;

    void Start()
    {
        pacMan = GameObject.Find("Pacman");
        blinky = GameObject.Find("Blinky");
        ghostHouse = GameObject.Find("StartNodePinky");
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        animator = this.GetComponent<AnimationController>();

        SetDifficulty(GameBoard.level);

        currentMode = Mode.IDLE;
        ghostState = AnimationController.State.STILL;

        currentNode = startingPosition;
        previousNode = currentNode;
        targetNode = ChooseNextNode();
    }

    private void SetDifficulty(int level)
    {
        Level currentLevel = GameBoard.levels[level - 1];

        ghostSpeed = currentLevel.GetGhostSpeed();
        frightenedTime = currentLevel.GetGhostFrightenedTime();
        frightenedSlow = currentLevel.GetGhostFrightenedSlow();
        recoveryFactor = currentLevel.GetGhostRecoveryFactor();

        modes = currentLevel.GetDefaultGhostModes();
        timers = currentLevel.GetGhostTimers(ghost);

        timers[1] = 7; modes[1] = Mode.SCATTER;
        timers[3] = 7; modes[3] = Mode.SCATTER;
        timers[5] = 5; modes[5] = Mode.SCATTER;
        timers[7] = 5; modes[7] = Mode.SCATTER;

        timers[2] = 20; modes[2] = Mode.CHASE;
        timers[4] = 20; modes[4] = Mode.CHASE;
        timers[6] = 20; modes[6] = Mode.CHASE;   
    }

    public void Restart()
    {
        transform.position = startingPosition.transform.position;
        modeChangeTimer = 0;
        frightenedTimer = 0;
        stateIteration = 0;
        frightenedFactor = 1f;

        currentMode = Mode.IDLE;
        previousMode = Mode.IDLE;
        ghostState = AnimationController.State.RECOVERED;
        animator.SetAnimatorState(ghostState);
        ghostState = AnimationController.State.STILL;

        currentNode = startingPosition;
        previousNode = currentNode;
        targetNode = ChooseNextNode();
    }

	void Update()
    {
        if (canMove)
        {
            ModeUpdate();
            Move();
            CheckCollision();
        }
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
        
        for (int i = 0; i < currentNode.neighbouringNodes.Length; i++)
        {
            Vector2 tempDir = currentNode.validDirections[i];
            Node tempNode = currentNode.neighbouringNodes[i];
            bool isValidDirection = tempDir != ghostDirection * -1;

            if ((isValidDirection && !tempNode.isGhostNode) || (isValidDirection && currentMode == Mode.RECOVERY) || (IsInGhostHouse()))
            {
                foundNodes.Add(tempNode);
                foundNodeDirections.Add(tempDir);
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

    void ModeUpdate()
    {
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
                EnterFrightened2Mode();
            }

            if (frightenedTimer >= frightenedTime)
            {
                ExitFrightenedMode();
            }
        }
        else
        {
            if (previousNode == ghostHouse.GetComponent<Node>())
            {
                ExitRecoveryMode();
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

    void CheckCollision()
    {
        Rect ghostRect = new Rect(this.transform.position, this.transform.GetComponent<SpriteRenderer>().sprite.bounds.size * 0.5f);
        Rect pacmanRect = new Rect(pacMan.transform.position, pacMan.transform.GetComponent<SpriteRenderer>().sprite.bounds.size * 0.5f);

        if (ghostRect.Overlaps(pacmanRect))
        {
            if (currentMode == Mode.FRIGHTENED)
            {
                Consume();
            }
            else if (currentMode != Mode.RECOVERY)
            {
                gameBoard.StartDeath();
            }
        }
    }

    void Consume()
    {
        consumedGhosts++;
        StartCoroutine(Pause(1.0f));
        Score.Ghost(consumedGhosts);
        EnterRecoveryMode();
    }

    public void EnterFrightenedMode()
    {
        if (currentMode != Mode.IDLE && currentMode != Mode.RECOVERY)
        {
            consumedGhosts = 0;
            frightenedTimer = 0;
            ChangeMode(Mode.FRIGHTENED);
            ghostState = AnimationController.State.FRIGHTENED;
            frightenedFactor = frightenedSlow;
            ReverseDirection();
        }
    }

    public void EnterFrightened2Mode()
    {
        ghostState = AnimationController.State.FRIGHTENED2;
    }

    public void EnterRecoveryMode()
    {
        ChangeMode(Mode.RECOVERY);
        ghostState = AnimationController.State.RECOVERY;
        frightenedFactor = recoveryFactor;
    }

    void ExitFrightenedMode()
    {
        ChangeMode(previousMode);
        ghostState = AnimationController.State.RECOVERED;
        frightenedFactor = 1f;
    }

    void ExitRecoveryMode()
    {
        modeChangeTimer = 0;
        stateIteration = 1;
        ChangeMode(modes[stateIteration]);
        ghostState = AnimationController.State.RECOVERED;
        frightenedFactor = 1f;
    }

    bool IsInGhostHouse()
    {
        return currentNode.name.Equals("StartNodePinky") || currentNode.name.Equals("StartNodeInky") || currentNode.name.Equals("StartNodeClyde");
    }

    bool IsLeavingInGhostHouse()
    {
        return previousNode.name.Equals("StartNodePinky") || previousNode.name.Equals("StartNodeInky") || previousNode.name.Equals("StartNodeClyde");
    }

    void ReverseDirection()
    {
        if (!IsLeavingInGhostHouse())
        {
            ghostDirection *= -1;

            Node temp = targetNode;
            targetNode = previousNode;
            previousNode = temp;
            currentNode = null;
        }
    }

    IEnumerator Pause(float delay)
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        pacMan.GetComponent<SpriteRenderer>().enabled = false;

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<Animator>().enabled = false;
            ghost.GetComponent<GhostController>().canMove = false;
        }

        pacMan.GetComponent<Animator>().enabled = false;
        pacMan.GetComponent<PacmanController>().canMove = false;

        gameBoard.GhostScorePopup(Score.GetGhostScore(consumedGhosts), this.transform.position);

        yield return new WaitForSeconds(delay);

        Resume();
    }

    void Resume()
    {
        this.GetComponent<SpriteRenderer>().enabled = true;
        pacMan.GetComponent<SpriteRenderer>().enabled = true;

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<Animator>().enabled = true;
            ghost.GetComponent<GhostController>().canMove = true;
        }

        pacMan.GetComponent<Animator>().enabled = true;
        pacMan.GetComponent<PacmanController>().canMove = true;

        gameBoard.GhostScorePopupRemove(Score.GetGhostScore(consumedGhosts));
    }
}