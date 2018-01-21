﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lives : MonoBehaviour
{
    public enum LivesID { LIFE1 = 1, LIFE2 = 2, LIFE3 = 3 };
    public LivesID life;

    public void UpdateLives()
    {
        if (GameBoard.pacmanLives < (int)life)
        {
            this.GetComponent<Image>().enabled = false;
        }
    }

    public static void LoseALife()
    {
        GameBoard.pacmanLives--;
    }

    public static void ResetLives()
    {
        GameBoard.pacmanLives = GameBoard.defaultLives;
    }
}
