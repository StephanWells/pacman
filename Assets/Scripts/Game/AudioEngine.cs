using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEngine : MonoBehaviour
{
    private const float variationPreparationTime = 2f;

    private static GameObject audioObj;

    private static AudioSource consumeSFX;
    private static AudioSource startSFX;
    public static AudioSource[] levelMusic;
    public static AudioSource[] transitions;

    private static float stopTimer = 0;
    private static float stopTime = 0;
    private static float varTimer = 0;
    private static float varTime = 0;
    private static bool needToStop = false;
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

    public static void Start()
    {
        audioObj = GameObject.Find("Audio"); // Gets the object that the AudioEngine component is attached to.

        transitions = new AudioSource[8];

        // Load audio.
        LoadSoundEffects();
        LoadTransitions();
        LoadLevelMusic();

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
    }

    private static AudioSource[] LoadL1Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L1music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/regular/L1-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/regular/L1-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/regular/L1-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L1/regular/L1-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L1music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L1music;
    }

    private static AudioSource[] LoadL2Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L2music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/regular/L2-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/regular/L2-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/regular/L2-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L2/regular/L2-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L2music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L2music;
    }

    private static AudioSource[] LoadL3Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L3music = new AudioSource[2];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L3/regular/L3-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L3music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L3/regular/L3-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L3music[1] = tempMusicObj.GetComponent<AudioSource>();

        return L3music;
    }

    private static AudioSource[] LoadL4Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L4music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/regular/L4-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/regular/L4-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/regular/L4-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L4/regular/L4-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L4music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L4music;
    }

    private static AudioSource[] LoadL5Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L5music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/regular/L5-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/regular/L5-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/regular/L5-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L5/regular/L5-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L5music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L5music;
    }

    private static AudioSource[] LoadL6Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L6music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/regular/L6-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/regular/L6-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/regular/L6-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L6/regular/L6-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L6music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L6music;
    }

    private static AudioSource[] LoadL7Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L7music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/regular/L7-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/regular/L7-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/regular/L7-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L7/regular/L7-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L7music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L7music;
    }

    private static AudioSource[] LoadL8Music()
    {
        GameObject tempMusicObj;
        AudioSource[] L8music = new AudioSource[4];

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/regular/L8-1", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8music[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/regular/L8-2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8music[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/regular/L8-3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8music[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/L8/regular/L8-4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        L8music[3] = tempMusicObj.GetComponent<AudioSource>();

        return L8music;
    }

    private static void LoadTransitions()
    {
        GameObject tempMusicObj;

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L1 to L2", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[0] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L2 to L3", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[1] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L3 to L4", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[2] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L4 to L5", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[3] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L5 to L6", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[4] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L6 to L7", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[5] = tempMusicObj.GetComponent<AudioSource>();

        tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L7 to L8", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[6] = tempMusicObj.GetComponent<AudioSource>();

        /*tempMusicObj = Instantiate(Resources.Load("Prefabs/Music/Transitions/L8 to L8", typeof(GameObject)) as GameObject);
        tempMusicObj.transform.parent = audioObj.transform;
        transitions[7] = tempMusicObj.GetComponent<AudioSource>();*/
    }

    private static void LoadLevelMusic()
    {
        Debug.Log("Loading Level " + GameBoard.level + " music.");

        switch (GameBoard.level)
        {
            case 1:
                levelMusic = LoadL1Music();
                level = GameBoard.level;

                levelCount = 0;
            break;

            case 2:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL2Music();
                level = GameBoard.level;

                levelCount = 0;
            break;

            case 3:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL3Music();
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
                level = GameBoard.level;

                levelCount = 0;
                varTimer = 0 - (nextSong.clip.length - nextSong.time);
                varTimer = 0;
                varTime = levelMusic[levelCount].clip.length - variationPreparationTime;
            break;

            case 5:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL5Music();
                level = GameBoard.level;

                levelCount = 0;
                varTimer = 0 - (nextSong.clip.length - nextSong.time);
                varTimer = 0;
                varTime = levelMusic[levelCount].clip.length - variationPreparationTime;
            break;

            case 6:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL6Music();
                level = GameBoard.level;

                levelCount = 0;
                varTimer = 0 - (nextSong.clip.length - nextSong.time);
                varTimer = 0;
                varTime = levelMusic[levelCount].clip.length - variationPreparationTime;
            break;

            case 7:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL7Music();
                level = GameBoard.level;

                levelCount = 0;
                varTimer = 0 - (nextSong.clip.length - nextSong.time);
                varTimer = 0;
                varTime = levelMusic[levelCount].clip.length - variationPreparationTime;
            break;

            case 8:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelMusic = LoadL8Music();
                level = GameBoard.level;

                levelCount = 0;
                varTimer = 0 - (nextSong.clip.length - nextSong.time);
                varTimer = 0;
                varTime = levelMusic[levelCount].clip.length - variationPreparationTime;
            break;
        }
    }

    // Updates the audio engine to the next level.
    public void UpdateLevel()
    {
        Debug.Log("Updating to new level.");
        LoadLevelMusic(); // Loads the music for the next level.
        levelCount = 0; // Initialises the music to the first clip/variation.

        float startingPoint = 0;

        // Sets the starting point of the clip.
        if (level == 4)
        {
            startingPoint = levelMusic[levelCount].clip.length / 4; // (if we're in level 4, we want to start from bar 3 of the variation)
        }
        else if (level > 4)
        {
            startingPoint = levelMusic[levelCount].clip.length / 2; // (if we're in level 5 onwards, we want to start from the middle of the variation)
        }
        else
        {
            startingPoint = 0; // (if we're in levels 1/2/3, we want to start from the beginning of the clip)
        }

        SwapSongAfter(levelMusic[levelCount], currentSong.GetComponent<MusicSource>().bars, startingPoint); // Swap to the level's first song.
        transitioning = false;
    }

    private void Update()
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
        varTimer += Time.deltaTime; // Add the time of the last frame to the variation timer.

        if (varTimer >= varTime) // Is it time to schedule a new variation?
        {
            Debug.Log("Variation timer threshold reached!");

            varTimer = 0 - variationPreparationTime; // Reset the variation timer.
            int rand = Random.Range(0, levelMusic.Length); // Choose a new random index.

            if (rand == levelCount) // If the index is the same as the one we currently have.
            {
                rand = rand < levelMusic.Length - 1 ? rand + 1 : rand - 1; // Change it.
            }

            SwapSongAfter(levelMusic[rand], levelMusic[levelCount].GetComponent<MusicSource>().bars, 0); // Schedule the new variation.
            levelCount = rand; // Set the current index to be the new one.
            varTime = levelMusic[rand].clip.length - variationPreparationTime; // Set the variation timer threshold to be as long as the next track (minus a preparation time).
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
                SwapSongAfter(transitions[level - 1], 1, 0); // Finish the bar and start the transition from the beginning.
            }
            else // If we're in an odd bar.
            {
                SwapSongAfter(transitions[level - 1], 1, transitions[level - 1].clip.length / 4f); // Finish the bar and start the transition from bar 2.
            }
        }
        else // If we're doing 8-bar variations/
        {
            if (levelMusic[levelCount].GetComponent<MusicSource>().IsInEvenBar()) // If we're in in an even bar.
            {
                SwapSongAfter(transitions[level - 1], 1, 0); // Finish the bar and transition.
            }
            else // If we're in an odd bar.
            {
                SwapSongAfter(transitions[level - 1], 2, 0); // Finish the current bar and the next and then transition.
            }
        }

        return stopTime;
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
        newSong.PlayScheduled(AudioSettings.dspTime + stopTime); // Schedule the song.
        Debug.Log(newSong.name + " is scheduled.");
        newSong.time = startingPoint; // Set the starting point of the scheduled song.
        StartCoroutine(StopSongAfter(currentSong, stopTime)); // Schedule the current song to stop.
    }

    IEnumerator StopSongAfter(AudioSource songIn, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (songIn != null) // If the song we wanted to stop hasn't been destroyed with the level change.
        {
            currentSong = nextSong; // The scheduled track is now playing and can be stored in the currentSong variable.
            Debug.Log("Stopped " + songIn.name);
            StartCoroutine(FadeMusic(songIn, 0.01f)); // Stop the song.
        }
    }

    // Method to fade out music given a fadeTime parameter.
    IEnumerator FadeMusic(AudioSource songIn, float fadeTime)
    {
        float startVolume = songIn.volume;

        while (songIn.volume > 0)
        {
            songIn.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        songIn.Stop();
        songIn.volume = startVolume;
    }
}