using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour
{
    public static int highScore = 0;

    void Update()
    {
        this.GetComponent<Text>().text = highScore.ToString();
    }

    public static void UpdateHighScore()
    {
        highScore = Score.score;
    }
}