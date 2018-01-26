using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    public int bars;
    public int BPM;
    public int level;
    public int pelletsLow;
    public int pelletsHigh;

    public bool isTransition;
    public bool isVariation;

    public bool pluck;
    public bool pluck2;
    public bool pad;
    public bool pulse;
    public bool sine;
    public bool square;
    public bool bass;
    public bool drumsLow;
    public bool drumsMid;
    public bool drumsHigh;

    public float timePerBar;

    private void Start()
    {
        timePerBar = 4f * (60f / (float)BPM);
    }

    public bool[] GetBoolArray()
    {
        bool[] boolArray = { pluck, pluck2, pad, pulse, sine, square, bass, drumsLow, drumsMid, drumsHigh };

        return boolArray;
    }

    public float GetTimeUntilBarsEnd(int bars)
    {
        return (bars * timePerBar) - this.GetComponent<AudioSource>().time % (bars * timePerBar);
    }

    public int CurrentBar()
    {
        return Mathf.FloorToInt(this.GetComponent<AudioSource>().time / timePerBar) + 1;
    }

    public bool IsInEvenBar()
    {
        return CurrentBar() % 2 == 0;
    }
}