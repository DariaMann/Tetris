using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour {
    [SerializeField]
    float timing;
    [SerializeField]
    bool isPaused;
    [SerializeField]
    bool exitController;
    [SerializeField]
    int scene;
    [SerializeField]
     GameObject menu;
    [SerializeField]
    GameObject YesNo;
    [SerializeField]
    GameObject Options;
    [SerializeField]
    GameObject MainMenu;
    void Start () {
        isPaused = false;
    }
	
	void Update () {
        if (scene == 2)
        {
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
    }
    public void PlayButton()
    {
        Application.LoadLevel("Play");

    }
    public void OptionButton()
    {
        Options.SetActive(true);
        MainMenu.SetActive(false);
    }
    public void BackToMenuButton()
    {
        Options.SetActive(false);
        MainMenu.SetActive(true);
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
    public void ExitButton()
    {
        MainMenu.SetActive(false);
        YesNo.SetActive(true);
    }
    public void NoButton()
    {
        MainMenu.SetActive(true);
        YesNo.SetActive(false);
    }
}
