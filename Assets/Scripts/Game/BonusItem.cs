using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusItem : MonoBehaviour
{
    private float timer;
    private float currentLifeTime;
    private GameBoard gameBoard;

	void Start()
    {
        timer = Random.Range(9f, 10f);
        currentLifeTime = 0f;
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        this.name = "BonusItem";
        gameBoard.consumables[14, 13] = this.gameObject;
	}

	void Update()
    {
		if (currentLifeTime < timer && !this.gameObject.GetComponent<Tile>().consumed)
        {
            currentLifeTime += Time.deltaTime;
        }
        else if (currentLifeTime >= timer)
        {
            Destroy(this.gameObject);
        }
	}
}
