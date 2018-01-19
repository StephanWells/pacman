using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDebug : MonoBehaviour
{
    public enum Ghost { BLINKY, PINKY, INKY, CLYDE };
    public Ghost ghostType;
    GhostController blinky, pinky, inky, clyde;
    GameBoard gameBoard;

    void Start ()
    {
        blinky = GameObject.Find("Blinky").GetComponent<GhostController>();
        pinky = GameObject.Find("Pinky").GetComponent<GhostController>();
        inky = GameObject.Find("Inky").GetComponent<GhostController>();
        clyde = GameObject.Find("Clyde").GetComponent<GhostController>();
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
    }
	
	void Update ()
    {
        Vector2 targetTile = this.transform.position;

        switch (ghostType)
        {
            case Ghost.BLINKY:
                targetTile = blinky.GetTargetTile();
            break;

            case Ghost.PINKY:
                targetTile = pinky.GetTargetTile();
            break;

            case Ghost.INKY:
                targetTile = inky.GetTargetTile();
            break;

            case Ghost.CLYDE:
                targetTile = clyde.GetTargetTile();
            break;
        }

        this.transform.position = gameBoard.BoardToWorld(targetTile);
    }
}
