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
