using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour {
    public float timing;
    public bool isPaused;
    public bool exitController;
    public GameObject menu;
    public GameObject YesNo;
    void Start () {
        isPaused = false;
    }
	
	void Update () {
        Time.timeScale = timing;
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused == false)
        {
            isPaused = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused == true)
        {
            isPaused = false;
        }
        if (isPaused == true)
        {
            timing = 0;
            if (!exitController)
            {
                menu.SetActive(true);
            }
            else
            {
                menu.SetActive(false);
            }
        }
        else if (isPaused == false)
        {
            timing = 1;
            menu.SetActive(false);
        }
    }
    public void PlayButton()
    {
        Application.LoadLevel("Play");

    }
    public void OptionButton()
    {
        

    }
    public void ResumeButton(bool state)
    {
        isPaused = state;
    }
    public void BackButton()
    {
        Application.LoadLevel("Menu");

    }
    public void PauseButton()
    {
        isPaused = true;
        if (isPaused == true)
        {
            timing = 0;
            if (!exitController)
            {
                menu.SetActive(true);
            }
            else
            {
                menu.SetActive(false);
            }
        }
        

    }
    public void QuitButton()
    {
        Application.Quit();
    }
}
