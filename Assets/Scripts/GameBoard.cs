using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private static int boardWidth = 26; // Number of units horizontally along the Pacman game board.
    private static int boardHeight = 29; // Number of units vertically along the Pacman game board.
    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

	void Start ()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject obj in objects)
        {
            Vector2 pos = obj.transform.position;
            int x, y; // x and y indices of the GameBoard array.

            if (obj.transform.parent != null && (obj.transform.parent.gameObject.name.Equals("Nodes") || obj.transform.parent.gameObject.name.Equals("Dots") || obj.transform.parent.gameObject.name.Equals("Power Pills"))) // Only consider game objects that are part of the game board / maze.
            {
                x = (int)((pos.x / 0.08) + 12.5f); // Each x unit in the game board is 0.08 Unity units across and starts at -1.
                y = (int)((pos.y / 0.08) + 14.0f); // Each y unit in the game board is 0.08 Unity units across and starts at -1.12.
                board[x, y] = obj;
            }
        }
	}
	
	void Update ()
    {
		
	}
}
