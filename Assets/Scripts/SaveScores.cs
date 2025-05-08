using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.IO;
using UnityEngine.UI;
using System;
using Assets.SimpleLocalization;
using JetBrains.Annotations;
using TMPro;

public class SaveScores : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> ScoreText;
    [SerializeField] private List<TextMeshProUGUI> Record;
    [SerializeField] private List<TextMeshProUGUI> maximums = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> maximumTexts = new List<TextMeshProUGUI>();
    
    [SerializeField, CanBeNull] private TextMeshProUGUI scoreAndRecordText;
    
    [SerializeField] private int scene;
    [SerializeField] private MiniGameType gameType;

    private string path;
    private int currentScore = 0;
    private int maximum = 0;

    public int CurrentRecord { get; set; } = 0;
    
    public int Maximum { get; set; } = 0;
    
    public bool IsWin { get; set; }

    public int CurrentScore
    {
        get => currentScore;
        set => currentScore = value;
    }

    public void Awake()
    {
//        if (scene == 1)
//        {
//            path = Application.persistentDataPath + "/ScoresTetris.xml";
//        }
//        else if (scene == 2)
//        {
//            path = Application.persistentDataPath + "/ScoresSnake.xml";
//        }
////        else if (scene == 3)
////        {
////            path = Application.persistentDataPath + "/Scores2048.xml";
////        }
//        
//        else if (scene == 4)
//        {
//            path = Application.persistentDataPath + "/ScoresLines98.xml";
//        } 
//        
//        else if (scene == 5)
//        {
//            path = Application.persistentDataPath + "/ScoresBlocks.xml";
//        } 
//        
//        Debug.Log(path);
//        CurrentRecord = GameHelper.LoadRecordData(path, "score");
//        if (scene == 3)
//        {
//            Maximum = GameHelper.LoadRecordData(path, "maximum");
//        }
        switch (gameType)
        {
            case MiniGameType.G2048:
            {
                CurrentRecord = GameHelper.Save2048.Record;
                Maximum = GameHelper.Save2048.Maximum;
                break;
            } 
            case MiniGameType.Tetris:
            {
                CurrentRecord = GameHelper.SaveTetris.Record;
                break;
            } 
            case MiniGameType.Snake:
            {
                CurrentRecord = GameHelper.SaveSnake.Record;
                break;
            } 
            case MiniGameType.Lines98:
            {
                CurrentRecord = GameHelper.SaveLines98.Record;
                break;
            } 
            case MiniGameType.Blocks:
            {
                CurrentRecord = GameHelper.SaveBlocks.Record;
                break;
            } 
        }

        ChangeScore(currentScore);
    }
    
    public void Save()
    {
        if (CurrentRecord < currentScore)
        {
            if (CurrentRecord > 0 || currentScore > 0)
            {
                IsWin = true;
            }

            if (gameType == MiniGameType.G2048)
            {
                GameHelper.Save2048.Record = currentScore;
                GameHelper.Save2048.Maximum = maximum;
                JsonHelper.Save2048(GameHelper.Save2048);
            }
            else if (gameType == MiniGameType.Tetris)
            {
                GameHelper.SaveTetris.Record = currentScore;
                JsonHelper.SaveTetris(GameHelper.SaveTetris);
            }
            else if (gameType == MiniGameType.Snake)
            {
                GameHelper.SaveSnake.Record = currentScore;
                JsonHelper.SaveSnake(GameHelper.SaveSnake);
            }
            else if (gameType == MiniGameType.Lines98)
            {
                GameHelper.SaveLines98.Record = currentScore;
                JsonHelper.SaveLines98(GameHelper.SaveLines98);
            }
            else if (gameType == MiniGameType.Blocks)
            {
                GameHelper.SaveBlocks.Record = currentScore;
                JsonHelper.SaveBlocks(GameHelper.SaveBlocks);
            }
            
            CurrentRecord = currentScore;
            foreach (var rec in Record)
            {
                //todo: Продумать выделение победы рекорда
//                rec.text = "<color=red>" + record + "</color>";
                rec.text = CurrentRecord.ToString();
            }
            if (scoreAndRecordText != null)
            {
//                scoreAndRecordText.text = currentScore + "/<color=red>" + record + "</color>";
                scoreAndRecordText.text = currentScore + "/" + CurrentRecord;
            }
            GameServicesManager.ReportScore(CurrentRecord, GameHelper.GameType);
            //todo: Новый рекорд! подсветить
        }
        else
        {
            IsWin = false;
            foreach (var rec in Record)
            {
                rec.text = CurrentRecord.ToString();
            }
            if (scoreAndRecordText != null)
            {
                scoreAndRecordText.text = currentScore + "/" + CurrentRecord;
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
            scoreAndRecordText.text = currentScore + "/" + CurrentRecord;
        }
        Save();
        if (GameHelper.GameType == MiniGameType.Tetris)
        {
            if(currentScore >= 10) GameServicesManager.UnlockAchieve(AchivementServices.Tetris10Points);
            if(currentScore >= 50) GameServicesManager.UnlockAchieve(AchivementServices.Tetris50Points);
            if(currentScore >= 100) GameServicesManager.UnlockAchieve(AchivementServices.Tetris100Points);
            if(currentScore >= 500) GameServicesManager.UnlockAchieve(AchivementServices.Tetris500Points);
            if(currentScore >= 1000) GameServicesManager.UnlockAchieve(AchivementServices.Tetris1000Points);
            if(currentScore >= 1500) GameServicesManager.UnlockAchieve(AchivementServices.Tetris1500Points);
            if(currentScore >= 2000) GameServicesManager.UnlockAchieve(AchivementServices.Tetris2000Points);
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
        else if (GameHelper.GameType == MiniGameType.Lines98)
        {
            if(currentScore >= 100) GameServicesManager.UnlockAchieve(AchivementServices.Lines98100Points);
            if(currentScore >= 200) GameServicesManager.UnlockAchieve(AchivementServices.Lines98200Points);
            if(currentScore >= 300) GameServicesManager.UnlockAchieve(AchivementServices.Lines98300Points);
            if(currentScore >= 400) GameServicesManager.UnlockAchieve(AchivementServices.Lines98400Points);
            if(currentScore >= 500) GameServicesManager.UnlockAchieve(AchivementServices.Lines98500Points);
            if(currentScore >= 700) GameServicesManager.UnlockAchieve(AchivementServices.Lines98700Points);
            if(currentScore >= 900) GameServicesManager.UnlockAchieve(AchivementServices.Lines98900Points);
            if(currentScore >= 1000) GameServicesManager.UnlockAchieve(AchivementServices.Lines981000Points);
            if(currentScore >= 2000) GameServicesManager.UnlockAchieve(AchivementServices.Lines982000Points);
            if(currentScore >= 3000) GameServicesManager.UnlockAchieve(AchivementServices.Lines983000Points);
            if(currentScore >= 4000) GameServicesManager.UnlockAchieve(AchivementServices.Lines984000Points);
            if(currentScore >= 5000) GameServicesManager.UnlockAchieve(AchivementServices.Lines985000Points);
            if(currentScore >= 6000) GameServicesManager.UnlockAchieve(AchivementServices.Lines986000Points);
            if(currentScore >= 7000) GameServicesManager.UnlockAchieve(AchivementServices.Lines987000Points);
        }
        else if (GameHelper.GameType == MiniGameType.Blocks)
        {
            if(currentScore >= 100) GameServicesManager.UnlockAchieve(AchivementServices.Blocks100Points);
            if(currentScore >= 300) GameServicesManager.UnlockAchieve(AchivementServices.Blocks300Points);
            if(currentScore >= 500) GameServicesManager.UnlockAchieve(AchivementServices.Blocks500Points);
            if(currentScore >= 700) GameServicesManager.UnlockAchieve(AchivementServices.Blocks700Points);
            if(currentScore >= 900) GameServicesManager.UnlockAchieve(AchivementServices.Blocks900Points);
            if(currentScore >= 1000) GameServicesManager.UnlockAchieve(AchivementServices.Blocks1000Points);
            if(currentScore >= 1500) GameServicesManager.UnlockAchieve(AchivementServices.Blocks1500Points);
            if(currentScore >= 2000) GameServicesManager.UnlockAchieve(AchivementServices.Blocks2000Points);
            if(currentScore >= 3000) GameServicesManager.UnlockAchieve(AchivementServices.Blocks3000Points);
            if(currentScore >= 4000) GameServicesManager.UnlockAchieve(AchivementServices.Blocks4000Points);
            if(currentScore >= 5000) GameServicesManager.UnlockAchieve(AchivementServices.Blocks5000Points);
            if(currentScore >= 6000) GameServicesManager.UnlockAchieve(AchivementServices.Blocks6000Points);
        }
    }

    public void ChangeMaximum(int max)
    {
        if (maximums.Count <= 0)
        {
            return;
        }
        maximum = max;
        foreach (var maxText in maximums)
        {
            maxText.text = maximum.ToString();
        }      
        foreach (var maxText in maximumTexts)
        {
            maxText.text = LocalizationManager.Localize("2048.maximum") + ": " + maximum;
        }
    }
}
