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

    private AudioEngine audioEngine;

    public static int level = 1;
    public static Level[] levels;

    public static int totalPellets = 0;
    public static int pelletsConsumed = 0;

    public GameObject[,] consumables = new GameObject[boardWidth, boardHeight]; // Locations of the dots and power pills.
    public GameObject[,] nodes = new GameObject[boardWidth, boardHeight]; // Locations of the back-end movement nodes/waypoints.

    GameObject bonusItem;

    private bool startDeath = false;
    private bool shouldBlink = false;

    private float blinkInterval;
    private float blinkIntervalTimer;

    private bool didSpawnBonusItem1;
    private bool didSpawnBonusItem2;

    public Sprite mazeBlue;
    public Sprite mazeWhite;

    public Text readyText;

	void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
        totalPellets = 0;
        pelletsConsumed = 0;

        foreach (GameObject obj in objects)
        {
            Vector2 pos = obj.transform.position;

            if (obj.GetComponent<Tile>() != null && (obj.GetComponent<Tile>().isDot || obj.GetComponent<Tile>().isPowerPill)) // Only consider game objects that are pellets (dots or power pills).
            {
                Vector2Int board = WorldToBoard(pos);
                consumables[board.x, board.y] = obj;
                totalPellets++;
                obj.GetComponent<Tile>().consumed = false;
                obj.GetComponent<SpriteRenderer>().enabled = true;
            }

            if (obj.transform.parent != null && (obj.transform.parent.gameObject.name.Equals("Nodes"))) // Only consider game objects that are movement nodes.
            {
                Vector2Int board = WorldToBoard(pos);
                nodes[board.x, board.y] = obj;
            }
        }

        audioEngine = GameObject.Find("Audio").GetComponent<AudioEngine>();

        if (level == 1)
        {
            Initialise();
        }

        startDeath = false;
        shouldBlink = false;
        blinkIntervalTimer = 0;
        bonusItem = null;
        didSpawnBonusItem1 = false;
        didSpawnBonusItem2 = false;

        if (bonusItem != null)
        {
            Destroy(bonusItem.gameObject);
        }

        StartGame();
	}

    static void Initialise()
    {
        AudioEngine.Start();

        levels = new Level[8];

        levels[0] = new Level(0.60f, 0.50f, 12f, 0.30f, 1.2f, 0f, 0f, 4f, 8f, 7f, 7f, 5f, 5f, 10f);
        levels[1] = new Level(0.70f, 0.55f, 08f, 0.35f, 1.4f, 0f, 0f, 4f, 8f, 7f, 6f, 5f, 5f, 12f);
        levels[2] = new Level(0.80f, 0.60f, 06f, 0.40f, 1.5f, 0f, 0f, 3f, 7f, 6f, 6f, 5f, 4f, 14f);
        levels[3] = new Level(0.90f, 0.65f, 05f, 0.45f, 1.6f, 0f, 0f, 3f, 6f, 5f, 5f, 5f, 4f, 16f);
        levels[4] = new Level(1.00f, 0.75f, 04f, 0.50f, 1.7f, 0f, 0f, 2f, 5f, 5f, 5f, 4f, 4f, 18f);
        levels[5] = new Level(1.10f, 0.85f, 03f, 0.55f, 1.8f, 0f, 0f, 2f, 4f, 5f, 4f, 3f, 3f, 20f);
        levels[6] = new Level(1.20f, 0.95f, 03f, 0.60f, 1.9f, 0f, 0f, 1f, 3f, 4f, 4f, 2f, 2f, 20f);
        levels[7] = new Level(1.30f, 1.10f, 03f, 0.65f, 2.0f, 0f, 0f, 1f, 2f, 0f, 0f, 0f, 0f, 20f);
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

        Lives.UpdateLives();

        GameObject pacMan = GameObject.Find("Pacman");

        pacMan.GetComponent<SpriteRenderer>().enabled = false;
        pacMan.GetComponent<Animator>().enabled = false;
        pacMan.GetComponent<PacmanController>().canMove = false;

        float musicDelay = (60f / (float)AudioEngine.levelMusic[0].GetComponent<MusicSource>().BPM) * 2;

        StartCoroutine(ShowObjectsAfter(musicDelay));
    }

    IEnumerator ShowObjectsAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<SpriteRenderer>().enabled = true;
            ghost.GetComponent<Animator>().enabled = true;
        }

        GameObject pacMan = GameObject.Find("Pacman");

        pacMan.GetComponent<SpriteRenderer>().enabled = true;
        pacMan.GetComponent<Animator>().enabled = true;

        readyText.GetComponent<Text>().enabled = true;

        float musicDelay = (60f / (float)AudioEngine.levelMusic[0].GetComponent<MusicSource>().BPM) * 2;

        StartCoroutine(StartGameAfter(musicDelay));
    }

    IEnumerator StartGameAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostController>().canMove = true;
        }

        GameObject pacMan = GameObject.Find("Pacman");

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

        if (bonusItem != null)
        {
            Destroy(bonusItem.gameObject);
            bonusItem = null;
        }

        Lives.LoseALife();
        Lives.UpdateLives();
        audioEngine.DeathStart();

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

        float songDelay = audioEngine.Death();

        StartCoroutine(ProcessDeathAnimation(songDelay));
    }

    IEnumerator ProcessDeathAnimation(float delay)
    {
        GameObject pacMan = GameObject.Find("Pacman");

        pacMan.transform.GetComponent<Animator>().enabled = true;
        pacMan.GetComponent<AnimationController>().SetAnimatorState(AnimationController.State.DEAD);

        yield return new WaitForSeconds(delay);

        pacMan.GetComponent<SpriteRenderer>().enabled = false;

        if (Lives.pacmanLives <= 0)
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

            float songDelay = audioEngine.AfterDeath();
            StartCoroutine(ProcessStart(songDelay));
        }
    }

    public IEnumerator ProcessGameOver(float delay)
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

    void SpawnBonusItem()
    {
        bonusItem = Instantiate(Resources.Load("Prefabs/BonusItem_" + level.ToString(), typeof(GameObject)) as GameObject);

        consumables[12, 12] = bonusItem;
    }

    void CheckPellets()
    {
        if (pelletsConsumed >= pelletsInLevel)
        {
            pelletsConsumed = 0;
            float delay = (float)audioEngine.Transition();
            StartCoroutine(ProcessWin(delay));
        }
        else if (pelletsConsumed >= 70 && pelletsConsumed < 170 && !didSpawnBonusItem1)
        {
            didSpawnBonusItem1 = true;
            SpawnBonusItem();
        }
        else if (pelletsConsumed >= 170 && !didSpawnBonusItem2)
        {
            didSpawnBonusItem2 = true;
            SpawnBonusItem();
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

        blinkInterval = (60f / (float)AudioEngine.transitions[level - 1, 0].GetComponent<MusicSource>().BPM);

        StartCoroutine(BlinkBoard(blinkInterval * 4));
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

        GameObject.Find("Maze").GetComponent<SpriteRenderer>().sprite = mazeBlue;
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
        level = level == 8 ? 8 : level + 1;
        audioEngine.UpdateLevel();
        Start();

        GameObject pacMan = GameObject.Find("Pacman");
        pacMan.GetComponent<PacmanController>().Restart();

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostController>().Restart();
        }
    }

    // Adds a ghost score popup at the given location.
    public void GhostScorePopup(int score, Vector2 location)
    {
        GameObject scoreImage = GameObject.Find("GhostScorePopup" + score.ToString());
        scoreImage.GetComponent<SpriteRenderer>().enabled = true;
        scoreImage.transform.position = location;
    }

    public void GhostScorePopupRemove(int score)
    {
        GameObject scoreImage = GameObject.Find("GhostScorePopup" + score.ToString());
        scoreImage.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void BonusScorePopup(int score, Vector2 location)
    {
        GameObject scoreImage = GameObject.Find("BonusScorePopup" + score.ToString());
        scoreImage.GetComponent<SpriteRenderer>().enabled = true;
        scoreImage.transform.position = location;
    }

    public void BonusScorePopupRemove(int score)
    {
        GameObject scoreImage = GameObject.Find("BonusScorePopup" + score.ToString());
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
        GameObject tile = consumables[board.x, board.y];

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