using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOver : MonoBehaviour
{
    [SerializeField, CanBeNull] private GameObject maximumParam;
    [SerializeField] private List<GameObject> confetti = new List<GameObject>();
    
    public bool IsGameOver { get; set; }

    private void Start()
    {
        SetData();
    }

    public void SetData()
    {
        if (maximumParam != null)
        {
            maximumParam.SetActive(GameHelper.GameType == MiniGameType.G2048);
        }
    }
    
    public void ShowGameOverPanel(bool isShow, bool isWin = false)
    {
        if (isShow)
        {
            if (isWin)
            {
                int randomIndex = Random.Range(0, confetti.Count);
                confetti[randomIndex].SetActive(true);
            }
            else
            {
                foreach (var conf in confetti)
                {
                    conf.SetActive(false);
                }
            }
            gameObject.SetActive(true);
            IsGameOver = true;
        }
        else
        {
            foreach (var conf in confetti)
            {
                conf.SetActive(false);
            }
            gameObject.SetActive(false);
            IsGameOver = false;
        }
    }
}