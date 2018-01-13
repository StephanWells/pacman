using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private static int boardWidth = 26; // Number of units horizontally along the Pacman game board.
    private static int boardHeight = 29; // Number of units vertically along the Pacman game board.
    public int totalPellets = 0;
    public int pelletsConsumed = 0;
    public int score = 0;
    public GameObject[,] pellets = new GameObject[boardWidth, boardHeight]; // Locations of the dots and power pills.
    public GameObject[,] nodes = new GameObject[boardWidth, boardHeight]; // Locations of the back-end movement nodes/waypoints.

	void Start ()
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
	
	void Update ()
    {
		
	}
}
