using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private static int boardWidth = 26; // Number of units horizontally along the Pacman game board.
    private static int boardHeight = 29; // Number of units vertically along the Pacman game board.
    public GameObject[,] pills = new GameObject[boardWidth, boardHeight];
    public GameObject[,] nodes = new GameObject[boardWidth, boardHeight];

	void Start ()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject obj in objects)
        {
            Vector2 pos = obj.transform.position;

            if (obj.transform.parent != null && (obj.transform.parent.gameObject.name.Equals("Dots") || obj.transform.parent.gameObject.name.Equals("Power Pills"))) // Only consider game objects that are part of the game board / maze.
            {
                //Vector2Int board = WorldToBoard(pos);
                Vector2Int board = new Vector2Int();
                float xf, yf;

                xf = (pos.x * 12.5f) + 12.5f; // Each x unit in the game board is 0.08 Unity units across and starts at -1.
                yf = (pos.y * 12.5f) + 14.0f; // Each y unit in the game board is 0.08 Unity units across and starts at -1.12.

                board.x = Mathf.RoundToInt(xf);
                board.y = Mathf.RoundToInt(yf);

                pills[board.x, board.y] = obj;

                Debug.Log(board.x + ", " + board.y + ", " + pos.x + ", " + pos.y + ", " + (pos.x * 12.5f) + ", " + (pos.y * 12.5f) + ", " + ((pos.x * 12.5f) + 12.5f) + ", " + ((pos.y * 12.5f) + 14.0f) + ", " + Mathf.RoundToInt((pos.x * 12.5f) + 12.5f) + ", " + Mathf.RoundToInt((pos.y * 12.5f) + 14.0f) + ", " + obj.name);
            }

            if (obj.transform.parent != null && (obj.transform.parent.gameObject.name.Equals("Nodes"))) // Only consider game objects that are part of the game board / maze.
            {
                Vector2Int board = WorldToBoard(pos);
                nodes[board.x, board.y] = obj;
            }
        }
	}

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
