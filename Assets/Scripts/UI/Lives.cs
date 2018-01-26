using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lives : MonoBehaviour
{
    public enum LivesID { LIFE1 = 1, LIFE2 = 2, LIFE3 = 3 };
    public LivesID life;
    
    public const int defaultLives = 3;
    public static int pacmanLives = defaultLives;
    static GameObject[] lives;

    void Start()
    {
        lives = GameObject.FindGameObjectsWithTag("Life");
    }

    public static void UpdateLives()
    {
        foreach (GameObject lifeObj in lives)
        {
            if (pacmanLives < (int)lifeObj.GetComponent<Lives>().life)
            {
                lifeObj.GetComponent<Image>().enabled = false;
            }
            else
            {
                lifeObj.GetComponent<Image>().enabled = true;
            }
        }
    }

    public static void AddALife()
    {
        if (pacmanLives < 3)
        {
            pacmanLives++;
        }
    }
    public static void LoseALife()
    {
        pacmanLives--;
    }

    public static void ResetLives()
    {
        pacmanLives = defaultLives;
    }
}
