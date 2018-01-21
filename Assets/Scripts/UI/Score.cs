using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    GameBoard gameBoard;
    public static int score = 0;
    public const int dotScore = 10;
    public const int powerPillScore = 50;
    public const int ghostScore = 200;

    void Start()
    {
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
    }

    void Update()
    {
        this.GetComponent<Text>().text = score.ToString();
	}

    public static void Dot()
    {
        score += dotScore;
    }

    public static void PowerPill()
    {
        score += powerPillScore;
    }

    public static void Ghost(int consumed)
    {
        score += GetGhostScore(consumed);
    }

    public static int GetGhostScore(int consumed)
    {
        return ghostScore * (int)(Mathf.Pow(2.0f, consumed - 1));
    }

    public static void ResetScore()
    {
        score = 0;
    }
}
