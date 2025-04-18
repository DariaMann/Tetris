﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.IO;
using UnityEngine.UI;
using System;
using JetBrains.Annotations;
using TMPro;

public class SaveScores : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> ScoreText;
    [SerializeField] private List<TextMeshProUGUI> Record;
    
    [SerializeField, CanBeNull] private TextMeshProUGUI scoreAndRecordText;
    
    [SerializeField] private int scene;

    private string path;
    private int currentScore = 0;
    private int record = 0;
    
    public int CurrentScore
    {
        get => currentScore;
        set => currentScore = value;
    }

    public void Awake()
    {
        if (scene == 1)
        {
            path = Application.persistentDataPath + "/ScoresTetris.xml";
        }
        else if (scene == 2)
        {
            path = Application.persistentDataPath + "/ScoresSnake.xml";
        }
        else if (scene == 3)
        {
            path = Application.persistentDataPath + "/Scores2048.xml";
        }
        
        else if (scene == 4)
        {
            path = Application.persistentDataPath + "/ScoresLines98.xml";
        }

        record = GameHelper.LoadRecordData(path);
        ChangeScore(currentScore);
    }
    
    public void Save()
    {
        if (record < currentScore)
        {
            GameHelper.SaveRecordData(path, currentScore);
            
            record = currentScore;
            foreach (var rec in Record)
            {
                //todo: Продумать выделение победы рекорда
//                rec.text = "<color=red>" + record + "</color>";
                rec.text = record.ToString();
            }
            if (scoreAndRecordText != null)
            {
//                scoreAndRecordText.text = currentScore + "/<color=red>" + record + "</color>";
                scoreAndRecordText.text = currentScore + "/" + record;
            }
            GameServicesManager.ReportScore(record, GameHelper.GameType);
            //todo: Новый рекорд! подсветить
        }
        else
        {
            foreach (var rec in Record)
            {
                rec.text = record.ToString();
            }
            if (scoreAndRecordText != null)
            {
                scoreAndRecordText.text = currentScore + "/" + record;
            }
        }
    }

    public void ChangeScore(int sc, bool isAdd = true)
    {
        if (isAdd)
        {
            if (sc != 0)
            {
                sc = currentScore + sc;
            }
        }
        currentScore = sc;
        foreach (var score in ScoreText)
        {
            score.text = currentScore.ToString();
        }
        if (scoreAndRecordText != null)
        {
            scoreAndRecordText.text = currentScore + "/" + record;
        }
        Save();
        if (GameHelper.GameType == MiniGameType.Tetris)
        {
            if(currentScore >= 10) GameServicesManager.UnlockAchieve(AchivementServices.Tetris10Points);
            if(currentScore >= 50) GameServicesManager.UnlockAchieve(AchivementServices.Tetris50Points);
            if(currentScore >= 100) GameServicesManager.UnlockAchieve(AchivementServices.Tetris100Points);
            if(currentScore >= 500) GameServicesManager.UnlockAchieve(AchivementServices.Tetris500Points);
            if(currentScore >= 1000) GameServicesManager.UnlockAchieve(AchivementServices.Tetris1000Points);
        }
        else if (GameHelper.GameType == MiniGameType.Snake)
        {
            if(currentScore >= 10) GameServicesManager.UnlockAchieve(AchivementServices.Snake10Points);
            if(currentScore >= 50) GameServicesManager.UnlockAchieve(AchivementServices.Snake50Points);
            if(currentScore >= 100) GameServicesManager.UnlockAchieve(AchivementServices.Snake100Points);
            if(currentScore >= 200) GameServicesManager.UnlockAchieve(AchivementServices.Snake200Points);
            if(currentScore >= 300) GameServicesManager.UnlockAchieve(AchivementServices.Snake300Points);
        }
        else if (GameHelper.GameType == MiniGameType.G2048)
        {
            if(currentScore >= 1000) GameServicesManager.UnlockAchieve(AchivementServices.G1000Points);
            if(currentScore >= 5000) GameServicesManager.UnlockAchieve(AchivementServices.G5000Points);
            if(currentScore >= 10000) GameServicesManager.UnlockAchieve(AchivementServices.G10000Points);
            if(currentScore >= 50000) GameServicesManager.UnlockAchieve(AchivementServices.G50000Points);
            if(currentScore >= 100000) GameServicesManager.UnlockAchieve(AchivementServices.G100000Points);
            if(currentScore >= 150000) GameServicesManager.UnlockAchieve(AchivementServices.G150000Points);
            if(currentScore >= 200000) GameServicesManager.UnlockAchieve(AchivementServices.G200000Points);
        }
    }
}
