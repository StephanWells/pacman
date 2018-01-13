using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isPortal;
    public bool isDot;
    public bool isPowerPill;
    public bool consumed;
    public GameObject portalReceiver;
    private GameBoard gameBoard;

    private void Start()
    {
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
    }

    public void consume()
    {
        if (!consumed && (isDot || isPowerPill))
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            consumed = true;
            gameBoard.score++;
            gameBoard.pelletsConsumed++;
        }
    }
}