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

    // Called when the game starts.
    void Start()
    {
        pacMan = GameObject.Find("Pacman");
        blinky = GameObject.Find("Blinky");
        ghostHouse = GameObject.Find("StartNodePinky");
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        animator = this.GetComponent<AnimationController>();

        SetDifficulty();

        currentMode = Mode.IDLE;
        ghostState = AnimationController.State.STILL;

        currentNode = startingPosition;
        previousNode = currentNode;
        targetNode = ChooseNextNode();
    }

    // Sets the difficulty of the ghosts.
    private void SetDifficulty()
    {
        Level currentLevel = GameBoard.levels[GameBoard.level - 1];

        ghostSpeed = currentLevel.GetGhostSpeed(); // How fast the ghost is.
        frightenedTime = currentLevel.GetGhostFrightenedTime(); // How long it spends in frightened mode.
        frightenedSlow = currentLevel.GetGhostFrightenedSlow(); // How slow it is in frightened mode.
        recoveryFactor = currentLevel.GetGhostRecoveryFactor(); // How fast it is in recovery mode.

        modes = currentLevel.GetDefaultGhostModes();
        timers = currentLevel.GetGhostTimers(ghost);

        //// Scatter behaviour timers.
        //timers[1] = timers[0]; modes[1] = Mode.SCATTER;
        //timers[3] = 7; modes[3] = Mode.SCATTER;
        //timers[5] = 5; modes[5] = Mode.SCATTER;
        //timers[7] = 5; modes[7] = Mode.SCATTER;

        //// Chase behaviour timers.
        //timers[2] = 20; modes[2] = Mode.CHASE;
        //timers[4] = 20; modes[4] = Mode.CHASE;
        //timers[6] = 20; modes[6] = Mode.CHASE;   
    }

    // Resets all ghost variables.
    public void Restart()
    {
        SetDifficulty();
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
        animator.SetAnimatorState(ghostState);
        animator.SetAnimatorDirection(Vector2.up);

        currentNode = startingPosition;
        previousNode = currentNode;
        targetNode = ChooseNextNode();
    }

    // Called every tick.
	void Update()
    {
        if (canMove)
        {
            ModeUpdate();
            Move();
            CheckCollision();
        }
    }

    // Method that moves the ghost in its direction.
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

    // Chooses which movement node to go to next based on the target tile.
    Node ChooseNextNode()
    {
        Vector2 targetTile = GetTargetTile(); // Get the ghost's unique target tile.
        Node moveToNode = null;

        List<Node> foundNodes = new List<Node>();
        List<Vector2> foundNodeDirections = new List<Vector2>();
        
        // Get all the nodes the ghost can move to.
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

        if (foundNodes.Count == 1) // If it can only go to one node.
        {
            moveToNode = foundNodes[0];
            ghostDirection = foundNodeDirections[0];
        }
        else if (foundNodes.Count >= 1) // If it has multiple nodes to choose from, then choose the node with the least distance to the target tile.
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

    // Pinky aims four tiles ahead of Pacman.
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

    // Each ghost has its own home node.
    Vector2 GetGhostScatterTile()
    {
        return gameBoard.WorldToBoard(homeNode.transform.position);
    }

    // Figures out what target tile each individual ghost wants to aim for.
    public Vector2 GetTargetTile()
    {
        Vector2 targetTile = Vector2.zero;

        switch (currentMode)
        {
            // Scattering ghosts hang around their home node.
            case Mode.SCATTER:
                targetTile = GetGhostScatterTile();
            break;

            // Each ghost has its own unique behaviour.
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
            
            // Frightened ghosts just run to their home node.
            case Mode.FRIGHTENED:
                targetTile = GetGhostScatterTile();
            break;
            
            // Aim for the ghost house when the ghost wants to recover.
            case Mode.RECOVERY:
                targetTile = gameBoard.WorldToBoard(ghostHouse.transform.position);
            break;
            
            // Stay still when idle.
            case Mode.IDLE:
                targetTile = gameBoard.WorldToBoard(this.transform.position);
            break;
        }

        return targetTile;
    }

    // Method for updating the behaviour of each ghost.
    void ModeUpdate()
    {
        if (currentMode != Mode.FRIGHTENED && currentMode != Mode.RECOVERY) // If ghosts are not in modes that interrupt their normal behaviour.
        {
            modeChangeTimer += Time.deltaTime; // Increment the behaviour timer.

            if (stateIteration == 1) // If the ghost has exited idle state.
            {
                ghostState = AnimationController.State.MOVING;
            }

            if (stateIteration == timers.Length) // If the ghost has entered permanent chase state.
            {
                ChangeMode(modes[timers.Length]);
            }
            else
            {
                if (modeChangeTimer > timers[stateIteration]) // If it's time to change modes.
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

    // Method to change to a given mode.
    void ChangeMode(Mode mode)
    {
        if (currentMode != Mode.FRIGHTENED && currentMode != Mode.RECOVERY)
        {
            previousMode = currentMode;
        }
        
        currentMode = mode;
    }

    // Checks if the ghost collided with Pacman using box collision detection.
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

    // Called when a ghost is eaten by Pacman.
    void Consume()
    {
        AudioEngine.PlayConsume();
        consumedGhosts++;
        StartCoroutine(Pause(1.0f));
        Score.Ghost(consumedGhosts);
        EnterRecoveryMode();
    }

    // Enters the frightened state after a power pill is eaten.
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

    // Enter blinking state indicating frightened mode is nearly over.
    public void EnterFrightened2Mode()
    {
        ghostState = AnimationController.State.FRIGHTENED2;
    }

    // Enters the recovery state (eyes) after being eaten.
    public void EnterRecoveryMode()
    {
        ChangeMode(Mode.RECOVERY);
        ghostState = AnimationController.State.RECOVERY;
        frightenedFactor = recoveryFactor;
    }

    // Exits the frightened state and reverts the ghosts to normal.
    void ExitFrightenedMode()
    {
        ChangeMode(previousMode);
        ghostState = AnimationController.State.RECOVERED;
        frightenedFactor = 1f;
    }

    // Exits the recovery state and reverts the ghosts to normal.
    void ExitRecoveryMode()
    {
        modeChangeTimer = 0;
        stateIteration = 1;
        ChangeMode(modes[stateIteration]);
        ghostState = AnimationController.State.RECOVERED;
        frightenedFactor = 1f;
    }

    // Booleans for checking whether the ghosts are in or leaving the ghost house in the middle.
    bool IsInGhostHouse()
    {
        return currentNode.name.Equals("StartNodePinky") || currentNode.name.Equals("StartNodeInky") || currentNode.name.Equals("StartNodeClyde");
    }

    bool IsLeavingInGhostHouse()
    {
        return previousNode.name.Equals("StartNodePinky") || previousNode.name.Equals("StartNodeInky") || previousNode.name.Equals("StartNodeClyde");
    }

    // Reverse the direction of the ghost (called when a power pill is eaten).
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

    // Pauses all ghost and temporarily disables the sprite for the one that got eaten.
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

    // Allows ghosts to move again after a pause.
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