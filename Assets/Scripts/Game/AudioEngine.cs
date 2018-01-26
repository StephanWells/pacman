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

    private static int level = GameBoard.level;
    private static int levelCount;
    private static int currentVariation = 0;
    private static AudioSource oldSong;

    public static void PlayConsume()
    {
        consumeSFX.Play();
    }

    public static void PlayStart()
    {
        startSFX.Play();
    }

    public static void Start()
    {
        audioObj = GameObject.Find("Audio");
        DontDestroyOnLoad(audioObj);

        transitions = new AudioSource[7];

        LoadSoundEffects();
        LoadTransitions();
        LoadLevelMusic();

        levelMusic[levelCount].Play();
    }

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
    }

    private static void LoadLevelMusic()
    {
        switch (GameBoard.level)
        {
            case 1:
                levelCount = 0;
                levelMusic = LoadL1Music();
            break;

            case 2:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelCount = 0;
                levelMusic = LoadL2Music();
            break;

            case 3:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelCount = 0;
                levelMusic = LoadL3Music();
            break;

            case 4:
                variations = true;

                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelCount = 0;
                levelMusic = LoadL4Music();
                varTime = levelMusic[levelCount].clip.length - variationPreparationTime;
            break;

            case 5:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelCount = 1;
                levelMusic = LoadL5Music();
                varTime = levelMusic[levelCount].clip.length - variationPreparationTime;
            break;

            case 6:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelCount = 2;
                levelMusic = LoadL6Music();
                varTime = levelMusic[levelCount].clip.length - variationPreparationTime;
            break;

            case 7:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelCount = 3;
                levelMusic = LoadL7Music();
                varTime = levelMusic[levelCount].clip.length - variationPreparationTime;
            break;

            case 8:
                foreach (AudioSource audio in levelMusic)
                {
                    Destroy(audio.gameObject);
                }

                levelCount = 3;
                levelMusic = LoadL8Music();
                varTime = levelMusic[levelCount].clip.length - variationPreparationTime;
            break;
        }
    }

    public static void UpdateLevel()
    {
        level = GameBoard.level;
        LoadLevelMusic();
        levelCount = 0;
        SwapSongAfter(transitions[level - 2], levelMusic[levelCount], transitions[level - 2].GetComponent<MusicSource>().bars, 0);
    }

    private void Update()
    {
        if (!variations)
        {
            CheckPellets();
        }
        else
        {
            PlayVariation();
        }
        
        CheckStop();
    }

    public void CheckPellets()
    {
        MusicSource currentSong = levelMusic[levelCount].GetComponent<MusicSource>();

        if (GameBoard.pelletsConsumed > currentSong.pelletsHigh)
        {
            SwapSongAfter(levelMusic[levelCount], levelMusic[levelCount + 1], 2, 0);
            levelCount++;
        }
    }

    public void PlayVariation()
    {
        varTimer += Time.deltaTime;

        if (varTimer >= varTime)
        {
            varTimer = 0 - variationPreparationTime;
            int rand = Random.Range(0, levelMusic.Length);

            if (rand == currentVariation)
            {
                rand = rand < levelMusic.Length - 1 ? rand + 1 : rand - 1;
            }

            SwapSongAfter(levelMusic[currentVariation], levelMusic[rand], levelMusic[currentVariation].GetComponent<MusicSource>().bars, 0);
            currentVariation = rand;
            varTime = levelMusic[rand].clip.length - variationPreparationTime;
        }
    }

    public void CheckStop()
    {
        if (needToStop)
        {
            stopTimer += Time.deltaTime;

            if (stopTimer >= stopTime)
            {
                stopTimer = 0;
                needToStop = false;
                StartCoroutine(FadeMusic(oldSong, 0.01f));
            }
        }
    }

    public static float Transition()
    {
        //if (levelMusic[levelCount].time >= levelMusic[levelCount].clip.length / 2f)
        if (!levelMusic[levelCount].GetComponent<MusicSource>().IsInEvenBar())
        {
            SwapSongAfter(levelMusic[levelCount], transitions[level - 1], 1, 0);
        }
        else
        {
            SwapSongAfter(levelMusic[levelCount], transitions[level - 1], 1, transitions[level - 1].clip.length / 4f);
        }

        return stopTime;
    }

    public static void SwapSongAfter(AudioSource oldSongIn, AudioSource newSong, int bars, float startingPoint)
    {
        Debug.Log("Swapping from " + oldSongIn.name + " to " + newSong.name + ".");

        needToStop = true;
        oldSong = oldSongIn;
        stopTime = oldSong.GetComponent<MusicSource>().GetTimeUntilBarsEnd(bars);
        newSong.PlayScheduled(AudioSettings.dspTime + stopTime);
        newSong.time = startingPoint;
    }

    public static void StopMusic(AudioSource songIn)
    {
        songIn.Stop();
    }

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

    /*public static void PlayMusic()
    {
        if (!levelMusic[levelCount].isPlaying && canTransition)
        {
            levelMusic[levelCount].Play();
        }
    }*/
}