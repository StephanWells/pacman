using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isPortal;
    public bool isDot;
    public bool isPowerPill;
    public bool consumed;
    public bool isBonusItem;
    public int pointValue;
    public GameObject portalReceiver;
    private GameBoard gameBoard;

    private void Start()
    {
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
    }

    public void Consume()
    {
        if (!consumed)
        {
            consumed = true;

            if (isDot || isPowerPill)
            {
                this.GetComponent<SpriteRenderer>().enabled = false;
                GameBoard.pelletsConsumed++;
            }
            else if (isBonusItem)
            {
                AudioEngine.PlayConsume();
                StartCoroutine(Pause(1.0f));
            }
        }
    }

    IEnumerator Pause(float delay)
    {
        GameObject pacMan = GameObject.Find("Pacman");

        pacMan.GetComponent<SpriteRenderer>().enabled = false;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<Animator>().enabled = false;
            ghost.GetComponent<GhostController>().canMove = false;
        }

        pacMan.GetComponent<Animator>().enabled = false;
        pacMan.GetComponent<PacmanController>().canMove = false;

        gameBoard.BonusScorePopup(pointValue, this.transform.position);

        yield return new WaitForSeconds(delay);

        Resume();
    }

    void Resume()
    {
        GameObject pacMan = GameObject.Find("Pacman");

        pacMan.GetComponent<SpriteRenderer>().enabled = true;

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<Animator>().enabled = true;
            ghost.GetComponent<GhostController>().canMove = true;
        }

        pacMan.GetComponent<Animator>().enabled = true;
        pacMan.GetComponent<PacmanController>().canMove = true;

        gameBoard.BonusScorePopupRemove(pointValue);
        Destroy(this.gameObject);
    }
}