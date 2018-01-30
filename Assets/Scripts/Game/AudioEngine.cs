using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class AudioEngine : MonoBehaviour
{
    private const float variationPreparationTime = 2f;

    private static GameObject audioObj;

    private static AudioSource consumeSFX;
    private static AudioSource startSFX;
    private static AudioSource deathSFX;
    private static AudioSource ghostEatSFX;
    public static AudioSource[] levelMusic;
    public static AudioSource[] levelFdMusic;
    public static AudioSource[] levelDeath;
    public static AudioSource[,] transitions;

    private static float stopTime;
    private static float varTimer;
    private static float varTime;
    private static bool variations = false;
    private static bool transitioning = false;

    private static int level = GameBoard.level;
    private static int levelCount;
    private static AudioSource currentSong;
    private static AudioSource nextSong;

    // Plays the sound effect when you consume a ghost / bonus item.
    public static void PlayConsume()
    {
        consumeSFX.Play();
    }

    // Plays the sound effect when you start the game.
    public static void PlayStart()
    {
        startSFX.Play();
    }

    public static void PlayDeath()
    {
        deathSFX.Play();
    }

    public static void PlayGhostEat()
    {
        ghostEatSFX.Play();
    }

    public static void Start()
    {
        audioObj = GameObject.Find("Audio"); // Gets the object that the AudioEngine component is attached to.

        transitions = new AudioSource[8, 4];

        // Load audio.
        LoadSoundEffects();
        LoadTransitions();
        LoadLevelMusic();

        // Initialise variables.
        stopTime = 0;
        varTimer = 0;
        varTime = 0;
        variations = false;
        transitioning = false;

        levelMusic[levelCount].Play(); // Play the first level's initial song.
        currentSong = nextSong = levelMusic[levelCount]; // Initialise currentSong and nextSong values.
    }

    // Methods for loading the music and sound effect prefabs.
    private static void LoadSoundEffects()
    {
        GameObject consumeObj = Instantiate(Resources.Load("Prefabs/Sound Effects/Consume", typeof(GameObject)) as GameObject);
        consumeObj.transform.parent = audioObj.transform;
        consumeSFX = consumeObj.GetComponent<AudioSource>();

        GameObject startObj = Instantiate(Resources.Load("Prefabs/Sound Effects/StartGame", typeof(GameObject)) as GameObject);
        startObj.transform.parent = audioObj.transform;
        startSFX = startObj.GetComponent<AudioSource>();

        GameObject deathObj = Instantiate(Resources.Load("Prefabs/Sound Effects/Death", typeof(GameObject)) as GameObject);
        deathObj.transform.parent = audioObj.transform;
        deathSFX = deathObj.GetComponent<AudioSource>();

        GameObject ghostEatObj = Instantiate(Resources.Load("Prefabs/Sound Effects/GhostEating 110BPM", typeof(GameObject)) as GameObject);
        ghostEatObj.transform.parent = audioObj.transform;
        ghostEatSFX = ghostEatObj.GetComponent<AudioSource>();
    }

    private static AudioSource[] LoadL1Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L1Music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/regular/L1-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1Music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/regular/L1-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1Music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/regular/L1-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1Music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/regular/L1-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1Music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L1Music;
    }

    private static AudioSource[] LoadL1FdMusic()
    {
        GameObject tempMusicObj;
        AudioSource[] L1FdMusic = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/fd/L1-1_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1FdMusic[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/fd/L1-2_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1FdMusic[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/fd/L1-3_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1FdMusic[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/fd/L1-4_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1FdMusic[3] = tempMusicObj.GetComponent<AudioSource>();

        return L1FdMusic;
    }

    private static AudioSource[] LoadL2Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L2Music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/regular/L2-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2Music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/regular/L2-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2Music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/regular/L2-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2Music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/regular/L2-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2Music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L2Music;
    }

    private static AudioSource[] LoadL2FdMusic()
    {
        GameObject tempMusicObj;
        AudioSource[] L2FdMusic = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/fd/L2-1_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2FdMusic[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/fd/L2-2_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2FdMusic[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/fd/L2-3_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2FdMusic[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/fd/L2-4_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2FdMusic[3] = tempMusicObj.GetComponent<AudioSource>();

        return L2FdMusic;
    }

    private static AudioSource[] LoadL3Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L3Music = new AudioSource[2];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L3/regular/L3-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L3Music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L3/regular/L3-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L3Music[1] = tempMusicObj.GetComponent<AudioSource>();

        return L3Music;
    }

    private static AudioSource[] LoadL3FdMusic()
    {
        GameObject tempMusicObj;
        AudioSource[] L3FdMusic = new AudioSource[2];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L3/fd/L3-1_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L3FdMusic[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L3/fd/L3-2_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L3FdMusic[1] = tempMusicObj.GetComponent<AudioSource>();

        return L3FdMusic;
    }

    private static AudioSource[] LoadL4Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L4Music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/regular/L4-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4Music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/regular/L4-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4Music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/regular/L4-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4Music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/regular/L4-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4Music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L4Music;
    }

    private static AudioSource[] LoadL4FdMusic()
    {
        GameObject tempMusicObj;
        AudioSource[] L4FdMusic = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/fd/L4-1_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4FdMusic[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/fd/L4-2_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4FdMusic[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/fd/L4-3_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4FdMusic[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/fd/L4-4_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4FdMusic[3] = tempMusicObj.GetComponent<AudioSource>();

        return L4FdMusic;
    }

    private static AudioSource[] LoadL5Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L5Music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/regular/L5-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5Music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/regular/L5-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5Music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/regular/L5-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5Music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/regular/L5-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5Music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L5Music;
    }

    private static AudioSource[] LoadL5FdMusic()
    {
        GameObject tempMusicObj;
        AudioSource[] L5FdMusic = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/fd/L5-1_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5FdMusic[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/fd/L5-2_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5FdMusic[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/fd/L5-3_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5FdMusic[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/fd/L5-4_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5FdMusic[3] = tempMusicObj.GetComponent<AudioSource>();

        return L5FdMusic;
    }

    private static AudioSource[] LoadL6Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L6Music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/regular/L6-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6Music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/regular/L6-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6Music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/regular/L6-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6Music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/regular/L6-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6Music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L6Music;
    }

    private static AudioSource[] LoadL6FdMusic()
    {
        GameObject tempMusicObj;
        AudioSource[] L6FdMusic = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/fd/L6-1_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6FdMusic[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/fd/L6-2_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6FdMusic[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/fd/L6-3_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6FdMusic[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/fd/L6-4_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6FdMusic[3] = tempMusicObj.GetComponent<AudioSource>();

        return L6FdMusic;
    }

    private static AudioSource[] LoadL7Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L7Music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/regular/L7-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7Music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/regular/L7-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7Music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/regular/L7-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7Music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/regular/L7-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7Music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L7Music;
    }

    private static AudioSource[] LoadL7FdMusic()
    {
        GameObject tempMusicObj;
        AudioSource[] L7FdMusic = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/fd/L7-1_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7FdMusic[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/fd/L7-2_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7FdMusic[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/fd/L7-3_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7FdMusic[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/fd/L7-4_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7FdMusic[3] = tempMusicObj.GetComponent<AudioSource>();

        return L7FdMusic;
    }

    private static AudioSource[] LoadL8Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L8Music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/regular/L8-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8Music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/regular/L8-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8Music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/regular/L8-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8Music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/regular/L8-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8Music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L8Music;
    }

    private static AudioSource[] LoadL8FdMusic()
    {
        GameObject tempMusicObj;
        AudioSource[] L8FdMusic = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/fd/L8-1_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8FdMusic[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/fd/L8-2_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8FdMusic[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/fd/L8-3_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8FdMusic[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/fd/L8-4_fd", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8FdMusic[3] = tempMusicObj.GetComponent<AudioSource>();

        return L8FdMusic;
    }

    private static AudioSource[] LoadL1ToL4Death()
    {
        GameObject tempMusicObj;
        AudioSource[] deathMusic = new AudioSource[10];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1-L4 death/Pluck", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        deathMusic[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1-L4 death/Pluck2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        deathMusic[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1-L4 death/Pad", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        deathMusic[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1-L4 death/Pulse", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        deathMusic[3] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1-L4 death/Sine", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        deathMusic[4] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1-L4 death/Square", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        deathMusic[5] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1-L4 death/Bass", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        deathMusic[6] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1-L4 death/DrumsLow", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        deathMusic[7] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1-L4 death/DrumsMid", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        deathMusic[8] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1-L4 death/DrumsHigh", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        deathMusic[9] = tempMusicObj.GetComponent<AudioSource>();

        return deathMusic;
    }

    private static void LoadTransitions()
    {
        GameObject tempMusicObj;

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L1 to L2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[0, 0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L2 to L3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[1, 0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L3 to L4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[2, 0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L4 to L5 (1)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[3, 0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L4 to L5 (2)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[3, 1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L4 to L5 (3)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[3, 2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L4 to L5 (4)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[3, 3] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L5 to L6 (1)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[4, 0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L5 to L6 (2)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[4, 1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L5 to L6 (3)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[4, 2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L5 to L6 (4)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[4, 3] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L6 to L7 (1)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[5, 0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L6 to L7 (2)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[5, 1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L6 to L7 (3)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[5, 2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L6 to L7 (4)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[5, 3] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L7 to L8 (1)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[6, 0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L7 to L8 (2)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[6, 1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L7 to L8 (3)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[6, 2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L7 to L8 (4)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[6, 3] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L8 to L8 (1)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[7, 0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L8 to L8 (2)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[7, 1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L8 to L8 (3)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[7, 2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L8 to L8 (4)", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[7, 3] = tempMusicObj.GetComponent<AudioSource>();
    }

    private static void LoadLevelMusic()
    {
        Debug.Log("Loading Level " + GameBoard.level + " music.");

        switch (GameBoard.level)
        {
            case 1:
                levelMusic = LoadL1Music();
                levelFdMusic = LoadL1FdMusic();
                levelDeath = LoadL1ToL4Death();
                level = GameBoard.level;

                levelCount = 0;
            break;

            case 2:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL2Music();
                levelFdMusic = LoadL2FdMusic();
                levelDeath = LoadL1ToL4Death();
                level = GameBoard.level;

                levelCount = 0;
            break;

            case 3:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL3Music();
                levelFdMusic = LoadL3FdMusic();
                levelDeath = LoadL1ToL4Death();
                level = GameBoard.level;

                levelCount = 0;
            break;

            case 4:
                variations = true;

                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL4Music();
                levelFdMusic = LoadL4FdMusic();
                levelDeath = LoadL1ToL4Death();
                level = GameBoard.level;

                levelCount = 0;
            break;

            case 5:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL5Music();
                levelFdMusic = LoadL5FdMusic();
                level = GameBoard.level;

                break;

            case 6:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL6Music();
                levelFdMusic = LoadL6FdMusic();
                level = GameBoard.level;

                break;

            case 7:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL7Music();
                levelFdMusic = LoadL7FdMusic();
                level = GameBoard.level;

                break;

            case 8:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL8Music();
                levelFdMusic = LoadL8FdMusic();

                break;
        }
    }

    // Updates the audio engine to the next level.
    public void UpdateLevel()
    {
        Debug.Log("Updating to new level.");
        LoadLevelMusic(); // Loads the music for the next level.

        float startingPoint = 0;

        // Sets the starting point of the clip.
        if (level == 4)
        {
            startingPoint = levelMusic[levelCount].clip.length / 4; // (if we're in level 4, we want to start from bar 3 of the variation)
        }
        else if (level > 4)
        {
            startingPoint = 4 * levelMusic[levelCount].clip.length / levelMusic[levelCount].GetComponent<MusicSource>().bars; // (if we're in level 5 onwards, we want to start from bar 5)
        }
        else
        {
            startingPoint = 0; // (if we're in levels 1/2/3, we want to start from the beginning of the clip)
        }

        if (variations)
        {
            varTimer = 0 - (nextSong.clip.length - nextSong.time);
            varTime = levelMusic[levelCount].clip.length - variationPreparationTime - startingPoint;

            Debug.Log("Setting the varTimer to " + varTimer + " and the varTime to " + varTime + ".");
        }
        
        SwapSongAfter(levelMusic[levelCount], currentSong.GetComponent<MusicSource>().bars, startingPoint); // Swap to the level's first song.

        transitioning = false;
    }

    private void FixedUpdate()
    {
        if (!variations) // If we're not past level 3.
        {
            CheckPellets(); // Change clips based on pellet thresholds.
        }
        else // If we're past level 3.
        {
            PlayVariation(); // Change clips based on choosing a random variation after the current one is done playing.
        }
    }

    public void CheckPellets()
    {
        if (GameBoard.pelletsConsumed > levelMusic[levelCount].GetComponent<MusicSource>().pelletsHigh) // Check pellet threshold.
        {
            Debug.Log("Pellets threshold reached!");
            levelCount++;
            SwapSongAfter(levelMusic[levelCount], 2, 0); // Swap to the next song.
        }
    }

    public void PlayVariation()
    {
        if (!transitioning)
        {
            varTimer += Time.fixedDeltaTime; // Add the time of the last frame to the variation timer.

            if (varTimer >= varTime) // Is it time to schedule a new variation?
            {
                int rand = Random.Range(0, levelMusic.Length); // Choose a new random index.

                varTimer = 0 - variationPreparationTime; // Reset the variation timer.
                varTime = levelMusic[rand].clip.length - variationPreparationTime; // Set the variation timer threshold to be as long as the next track (minus a preparation time).

                Debug.Log("Variation timer threshold reached! Setting varTimer to " + varTimer + " and varTime to " + varTime + ".");

                if (rand == levelCount) // If the index is the same as the one we currently have.
                {
                    rand = rand < levelMusic.Length - 1 ? rand + 1 : rand - 1; // Change it.
                }

                SwapSongAfter(levelMusic[rand], levelMusic[levelCount].GetComponent<MusicSource>().bars, 0); // Schedule the new variation.
                levelCount = rand; // Set the current index to be the new one.
            }
        }
    }

    // Method called by game board when it's time to transition. Returns how long the game board needs to wait.
    public float Transition()
    {
        Debug.Log("Transitioning to next level!");
        transitioning = true;

        if (level < 4)
        {
            if (levelMusic[levelCount].GetComponent<MusicSource>().IsInEvenBar()) // If we're in an even bar.
            {
                SwapSongAfter(transitions[level - 1, 0], 1, 0); // Finish the bar and start the transition from the beginning.
            }
            else // If we're in an odd bar.
            {
                SwapSongAfter(transitions[level - 1, 0], 1, transitions[level - 1, 0].clip.length / transitions[level - 1, 0].GetComponent<MusicSource>().bars); // Finish the bar and start the transition from bar 2.
            }
        }
        else // If we're doing 8-bar variations/
        {
            if (levelMusic[levelCount].GetComponent<MusicSource>().IsInEvenBar()) // If we're in in an even bar.
            {
                SwapSongAfter(transitions[level - 1, levelCount], 1, 0); // Finish the bar and transition.
            }
            else // If we're in an odd bar.
            {
                SwapSongAfter(transitions[level - 1, levelCount], 2, 0); // Finish the current bar and the next and then transition.
            }
        }

        return stopTime;
    }

    public void DeathStart()
    {
        transitioning = true;
        StopAllCoroutines();
        Debug.Log("Current: " + currentSong.name + ". Next: " + nextSong.name);
        currentSong.Stop();
        nextSong.Stop();
        PlayDeath();
    }

    public float Death()
    {
        bool[] boolArray = currentSong.GetComponent<MusicSource>().GetBoolArray();
        bool variation = currentSong.GetComponent<MusicSource>().isVariation;

        for (int i = 0; i < levelDeath.Length; i++)
        {
            if (boolArray[i] || variation)
            {
                levelDeath[i].Play();
            }
        }

        return levelDeath[0].clip.length;
    }

    public float AfterDeath()
    {
        varTimer = 0;
        transitioning = false;
        currentSong = nextSong = levelFdMusic[levelCount];
        currentSong.Play();

        if (!variations)
        {
            SwapSongAfter(levelMusic[levelCount], currentSong.GetComponent<MusicSource>().bars, 0);
        }

        varTime = currentSong.clip.length - variationPreparationTime;

        return currentSong.GetComponent<MusicSource>().timePerBar;
    }

    // Schedules the current track to stop and the next track to start.
    public void SwapSongAfter(AudioSource newSong, int bars, float startingPoint)
    {
        Debug.Log("Swapping from " + currentSong.name + " to " + newSong.name + ".");

        if (nextSong != currentSong) // If there is a song already scheduled.
        {
            Debug.Log("Next song " + nextSong.name + " is not equal to current song " + currentSong.name + "!");

            nextSong.Stop(); // Cancel that song.
        }

        nextSong = newSong; // Set the scheduled song.

        stopTime = currentSong.GetComponent<MusicSource>().GetTimeUntilBarsEnd(bars); // Set how long the program will need to wait till we swap songs.

        //newSong.PlayScheduled(AudioSettings.dspTime + stopTime); // Schedule the song.
        /*StartCoroutine*/Timing.RunCoroutine(StartSongAfter(newSong, stopTime));
        Debug.Log("Swapping to " + newSong.name + " in " + stopTime + ".");
        newSong.time = startingPoint; // Set the starting point of the scheduled song.

        if (currentSong.GetComponent<MusicSource>().isTransition)
        {
            Timing.RunCoroutine(StopSongAfterTransition(currentSong, stopTime));
        }
        else
        {
            /*StartCoroutine*/
            Timing.RunCoroutine(StopSongAfter(currentSong, stopTime)); // Schedule the current song to stop.
        }
    }

    IEnumerator<float> StopSongAfter(AudioSource songIn, float delay)
    {
        yield return Timing.WaitForSeconds(delay);

        if (songIn != null) // If the song we wanted to stop hasn't been destroyed with the level change.
        {
            currentSong = nextSong; // The scheduled track is now playing and can be stored in the currentSong variable.
            Debug.Log("Stopped " + songIn.name + ". Current song: " + currentSong);
            /*StartCoroutine*/Timing.RunCoroutine(FadeMusicOut(songIn, 0.2f)); // Stop the song.
        }
    }

    IEnumerator<float> StopSongAfterTransition(AudioSource songIn, float delay)
    {
        yield return Timing.WaitForSeconds(delay);

        if (songIn != null) // If the song we wanted to stop hasn't been destroyed with the level change.
        {
            currentSong = nextSong; // The scheduled track is now playing and can be stored in the currentSong variable.
            Debug.Log("(Transition) Stopped " + songIn.name + ". Current song: " + currentSong);
        }
    }

    IEnumerator<float> StartSongAfter(AudioSource songIn, float delay)
    {
        yield return Timing.WaitForSeconds(delay);

        if (songIn != null) // If the song we wanted to stop hasn't been destroyed with the level change.
        {
            Debug.Log("Started " + songIn.name);
            /*StartCoroutine*/Timing.RunCoroutine(FadeMusicIn(songIn, 0.02f)); // Stop the song.
        }
    }

    // Method to fade out music given a fadeTime parameter.
    IEnumerator<float> FadeMusicOut(AudioSource songIn, float fadeTime)
    {
        float startVolume = songIn.volume;

        while (songIn.volume > 0)
        {
            songIn.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return 0;
        }

        songIn.Stop();
        songIn.volume = startVolume;
    }

    IEnumerator<float> FadeMusicIn(AudioSource songIn, float fadeTime)
    {
        float startVolume = songIn.volume;
        songIn.volume = 0;
        songIn.Play();

        while (songIn.volume < startVolume)
        {
            songIn.volume += startVolume * Time.deltaTime / fadeTime;

            yield return 0;
        }

        songIn.volume = startVolume;
    }
}