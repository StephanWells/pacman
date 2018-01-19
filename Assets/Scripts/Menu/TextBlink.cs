using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlink : MonoBehaviour
{
    public float blinkTimer = 1.0f;
    public Color colour;
    private float runningTime;

    void Start()
    {
        runningTime = blinkTimer;
    }

    void Update ()
    {
        runningTime -= Time.deltaTime;

        if (runningTime >= blinkTimer * 0.5)
        {
            this.GetComponent<Text>().color = colour;
        }
        else if (runningTime <= blinkTimer * 0.5 && runningTime >= 0)
        {
            this.GetComponent<Text>().color = Color.black;
        }
        else
        {
            runningTime = blinkTimer;
        }
	}
}