using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static int score = 0;
    private static int scoreTemp = 0;
    public const int dotScore = 10;
    public const int powerPillScore = 50;
    public const int ghostScore = 200;

    void Update()
    {
        this.GetComponent<Text>().text = score.ToString();

        if (scoreTemp >= 10000)
        {
            Lives.AddALife();
            Lives.UpdateLives();
            scoreTemp = 0;
        }
	}

    public static void Dot()
    {
        score += dotScore;
        scoreTemp += dotScore;
    }

    public static void PowerPill()
    {
        score += powerPillScore;
        scoreTemp += powerPillScore;
    }

    public static void Ghost(int consumed)
    {
        score += GetGhostScore(consumed);
        scoreTemp += GetGhostScore(consumed);
    }
    
    public static void BonusItem(Tile bonusItem)
    {
        score += bonusItem.pointValue;
        scoreTemp += bonusItem.pointValue;
    }

    public static int GetGhostScore(int consumed)
    {
        return ghostScore * (int)(Mathf.Pow(2.0f, consumed - 1));
    }

    public static void ResetScore()
    {
        score = 0;
        scoreTemp = 0;
    }
}
