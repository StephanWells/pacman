using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private bool isPaused = false;
    private float timer = 0;

	void Update()
    {
		if (Input.anyKey || isPaused)
        {
            if (!isPaused)
            {
                PlayStart();
                isPaused = true;
            }
            else
            {
                timer += Time.deltaTime;
            }
            
            if (timer >= 1f)
            {
                SceneManager.LoadScene("Game");
            }
        }
	}

    void PlayStart()
    {
        GameObject startObj = Instantiate(Resources.Load("Prefabs/Sound Effects/StartGame", typeof(GameObject)) as GameObject);
        AudioSource startSFX = startObj.GetComponent<AudioSource>();

        startSFX.Play();
    }
}