using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{
    private const int boardWidth = 26; // Number of units horizontally along the Pacman game board.
    private const int boardHeight = 29; // Number of units vertically along the Pacman game board.
    private const int pelletsInLevel = 246;

    public static int defaultLives = 3;
    public static int pacmanLives = defaultLives;
    private GameObject[] lives;

    public static int level = 1;

    public int totalPellets = 0;
    public int pelletsConsumed = 0;

    public GameObject[,] pellets = new GameObject[boardWidth, boardHeight]; // Locations of the dots and power pills.
    public GameObject[,] nodes = new GameObject[boardWidth, boardHeight]; // Locations of the back-end movement nodes/waypoints.

    bool startDeath = false;
    private bool shouldBlink = false;

    private const float blinkInterval = 0.33f;
    private float blinkIntervalTimer = 0;

    public Sprite mazeBlue;
    public Sprite mazeWhite;

    public Text readyText;

	void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject obj in objects)
        {
            Vector2 pos = obj.transform.position;

            if (obj.GetComponent<Tile>() != null && (obj.GetComponent<Tile>().isDot || obj.GetComponent<Tile>().isPowerPill)) // Only consider game objects that are pellets (dots or power pills).
            {
                Vector2Int board = WorldToBoard(pos);
                pellets[board.x, board.y] = obj;
                totalPellets++;
            }

            if (obj.transform.parent != null && (obj.transform.parent.gameObject.name.Equals("Nodes"))) // Only consider game objects that are movement nodes.
            {
                Vector2Int board = WorldToBoard(pos);
                nodes[board.x, board.y] = obj;
            }
        }

        lives = GameObject.FindGameObjectsWithTag("Life");

        StartGame();
	}

    public void StartGame()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<SpriteRenderer>().enabled = false;
            ghost.GetComponent<Animator>().enabled = false;
            ghost.GetComponent<GhostController>().canMove = false;
        }

        foreach (GameObject life in lives)
        {
            life.GetComponent<Lives>().UpdateLives();
        }

        GameObject pacMan = GameObject.Find("Pacman");

        pacMan.GetComponent<SpriteRenderer>().enabled = false;
        pacMan.GetComponent<Animator>().enabled = false;
        pacMan.GetComponent<PacmanController>().canMove = false;

        StartCoroutine(ShowObjectsAfter(2.0f));
    }

    IEnumerator ShowObjectsAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<SpriteRenderer>().enabled = true;
        }

        GameObject pacMan = GameObject.Find("Pacman");

        pacMan.GetComponent<SpriteRenderer>().enabled = true;

        readyText.GetComponent<Text>().enabled = true;

        StartCoroutine(StartGameAfter(2.0f));
    }

    IEnumerator StartGameAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<Animator>().enabled = true;
            ghost.GetComponent<GhostController>().canMove = true;
        }

        GameObject pacMan = GameObject.Find("Pacman");

        pacMan.GetComponent<Animator>().enabled = true;
        pacMan.GetComponent<PacmanController>().canMove = true;

        readyText.GetComponent<Text>().enabled = false;
    }

    public void Restart()
    {
        GameObject pacMan = GameObject.Find("Pacman");
        pacMan.GetComponent<PacmanController>().Restart();
        pacMan.GetComponent<SpriteRenderer>().enabled = true;
        
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostController>().Restart();
            ghost.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void StartDeath()
    {
        if (!startDeath)
        {
            startDeath = true;
        }

        Lives.LoseALife();

        foreach (GameObject life in lives)
        {
            life.GetComponent<Lives>().UpdateLives();
        }

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostController>().canMove = false;
        }

        GameObject pacMan = GameObject.Find("Pacman");
        pacMan.GetComponent<PacmanController>().canMove = false;
        pacMan.GetComponent<Animator>().enabled = false;

        StartCoroutine(ProcessDeathAfter(1.0f));
    }

    IEnumerator ProcessDeathAfter (float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<SpriteRenderer>().enabled = false;
        }

        StartCoroutine(ProcessDeathAnimation(2.1f));
    }

    IEnumerator ProcessDeathAnimation(float delay)
    {
        GameObject pacMan = GameObject.Find("Pacman");

        pacMan.transform.GetComponent<Animator>().enabled = true;
        pacMan.GetComponent<AnimationController>().SetAnimatorState(AnimationController.State.DEAD);

        yield return new WaitForSeconds(delay);

        pacMan.GetComponent<SpriteRenderer>().enabled = false;

        if (pacmanLives <= 0)
        {
            readyText.GetComponent<Text>().text = "GAME OVER";
            readyText.GetComponent<Text>().color = new Color(153f / 255f, 0, 0);
            readyText.GetComponent<Text>().enabled = true;

            StartCoroutine(ProcessGameOver(2f));
        }
        else
        {
            readyText.GetComponent<Text>().text = "READY!";
            readyText.GetComponent<Text>().color = new Color(1, 1, 0);
            readyText.GetComponent<Text>().enabled = true;

            StartCoroutine(ProcessStart(1.5f));
        }
    }

    IEnumerator ProcessGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Score.score > HighScore.highScore)
        {
            HighScore.UpdateHighScore();
        }

        Score.ResetScore();
        Lives.ResetLives();
        level = 1;

        SceneManager.LoadScene("GameMenu");
    }

    IEnumerator ProcessStart(float delay)
    {
        GameObject pacMan = GameObject.Find("Pacman");

        yield return new WaitForSeconds(delay);

        readyText.GetComponent<Text>().enabled = false;

        Restart();

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostController>().canMove = true;
        }

        pacMan.GetComponent<PacmanController>().canMove = true;
        startDeath = false;
    }

    void Update()
    {
        CheckPellets();
        CheckBlink();
    }

    void CheckPellets()
    {
        if (pelletsConsumed >= pelletsInLevel)
        {
            StartCoroutine(ProcessWin(1f));
        }
    }

    IEnumerator ProcessWin(float delay)
    {
        GameObject pacMan = GameObject.Find("Pacman");
        pacMan.GetComponent<PacmanController>().canMove = false;
        pacMan.GetComponent<Animator>().enabled = false;

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostController>().canMove = false;
            ghost.GetComponent<Animator>().enabled = false;
        }

        yield return new WaitForSeconds(delay);

        StartCoroutine(BlinkBoard(2f));
    }

    IEnumerator BlinkBoard(float delay)
    {
        GameObject pacMan = GameObject.Find("Pacman");
        pacMan.GetComponent<SpriteRenderer>().enabled = false;

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<SpriteRenderer>().enabled = false;
        }

        shouldBlink = true;

        yield return new WaitForSeconds(delay);

        shouldBlink = false;

        NextLevel();
    }

    private void CheckBlink()
    {
        if (shouldBlink)
        {
            if (blinkIntervalTimer < blinkInterval)
            {
                blinkIntervalTimer += Time.deltaTime;
            }
            else
            {
                SpriteRenderer mazeSprite = GameObject.Find("Maze").GetComponent<SpriteRenderer>();
                blinkIntervalTimer = 0;

                if (mazeSprite.sprite == mazeBlue)
                {
                    mazeSprite.sprite = mazeWhite;
                }
                else
                {
                    mazeSprite.sprite = mazeBlue;
                }
            }
        }
    }

    private void NextLevel()
    {
        level++;
        SceneManager.LoadScene("Game");
    }

    // Adds a ghost score popup at the given location.
    public void GhostScorePopup(int score, Vector2 location)
    {
        GameObject scoreImage = GameObject.Find("GhostScorePopup" + score.ToString());
        scoreImage.GetComponent<SpriteRenderer>().enabled = true;
        scoreImage.transform.position = location;
    }

    public void ghostScorePopupRemove(int score)
    {
        GameObject scoreImage = GameObject.Find("GhostScorePopup" + score.ToString());
        scoreImage.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Translates unity coordinates to coordinates on the game board.
    public Vector2Int WorldToBoard(Vector2 coord)
    {
        Vector2Int board = new Vector2Int();
        float xf, yf;

        xf = (coord.x * 12.5f) + 12.5f; // Each x unit in the game board is 0.08 Unity units across and starts at -1.
        yf = (coord.y * 12.5f) + 14.0f; // Each y unit in the game board is 0.08 Unity units across and starts at -1.12.

        board.x = Mathf.RoundToInt(xf);
        board.y = Mathf.RoundToInt(yf);

        return board;
    }

    // Translates board coordinates to coordinates in the unity world.
    public Vector2 BoardToWorld(Vector2 coord)
    {
        Vector2 world = new Vector2Int();
        float xf, yf;

        xf = (coord.x - 12.5f) / 12.5f; // Each x unit in the game board is 0.08 Unity units across and starts at -1.
        yf = (coord.y - 14.0f) / 12.5f; // Each y unit in the game board is 0.08 Unity units across and starts at -1.12.

        world = new Vector2(xf, yf);

        return world;
    }

    // Returns the pill object at a given position on the game board.
    public GameObject GetTileAtPosition(Vector2 pos)
    {
        Vector2Int board = WorldToBoard(pos);
        GameObject tile = pellets[board.x, board.y];

        if (tile != null)
        {
            return tile;
        }

        return null;
    }

    // Returns the node at a given position on the game board.
    public Node GetNodeAtPosition(Vector2 pos)
    {
        Vector2Int board = WorldToBoard(pos);
        GameObject tile = nodes[board.x, board.y];

        if (tile != null)
        {
            return tile.GetComponent<Node>();
        }

        return null;
    }

    // Returns the length from a node to the coordinates parameter.
    public float LengthFromNode(Node node, Vector2 pos)
    {
        Vector2 vec = pos - (Vector2)node.transform.position;

        return vec.sqrMagnitude;
    }

    // Checks if an entity can go further or not based on its previous and target node, and its current position.
    public bool Overshot(Node previous, Node target, Vector2 currentPos)
    {
        float nodeToTarget = LengthFromNode(previous, target.transform.position);
        float nodeToSelf = LengthFromNode(previous, currentPos);

        return nodeToSelf >= nodeToTarget;
    }

    // Returns the squared distance between two points.
    public float GetSquaredDistance (Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        return /*Mathf.Sqrt(*/dx * dx + dy * dy/*)*/;
    }
}