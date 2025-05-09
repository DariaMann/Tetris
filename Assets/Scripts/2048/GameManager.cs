﻿using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField] private TileBoard board;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private Button undoButton;

    public static GameManager Instance { get; private set; }

    public Stack<Step2048> EventSteps { get; set; } = new Stack<Step2048>();

    public SaveScores SaveScores
    {
        get => saveScores;
        set => saveScores = value;
    }

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void Start()
    {
        LoadLastPlay();
        CheckUndoButtonState();
    }
    
    void OnApplicationQuit()
    {
        SaveLastPlay();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveLastPlay();
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
        SaveLastPlay();
    }

    private void LoadLastPlay()
    {
        SaveData2048 saveData = GameHelper.Save2048.SaveData2048;
        if (saveData == null)
        {
            NewGame();
            return;
        }
        
        SaveScores.ChangeScore(saveData.Score);
        saveScores.IsWin = saveData.IsWin;
        ChangeMaximumNumber(GameHelper.Save2048.Maximum);
        // hide game over screen
        gameOver.ShowGameOverPanel(false);

        // update board state
        board.ClearBoard();

        foreach (var tile in saveData.SaveTiles)
        {
            board.CreateTile(tile);
        }
        
        board.enabled = true;
        
        CheckUndoButtonState();
    }

    private void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.Save2048.SaveData2048 = null;
            JsonHelper.Save2048(GameHelper.Save2048);
            return;
        }
        SaveData2048 data = new SaveData2048(SaveScores.IsWin, SaveScores.CurrentScore, board.Tiles);
        GameHelper.Save2048.SaveData2048 = data;
        JsonHelper.Save2048(GameHelper.Save2048);
    }

    public void ChangeMaximumNumber(int newMaximum)
    {
        if (newMaximum > GameHelper.Save2048.Maximum)
        {
            GameHelper.Save2048.Maximum = newMaximum;
            JsonHelper.Save2048(GameHelper.Save2048);
        }
        
        SaveScores.ChangeMaximum(newMaximum);
    }
    
    public void NewGame()
    {
        // reset score
        SaveScores.ChangeScore(0);
        ChangeMaximumNumber(GameHelper.Save2048.Maximum);
        
        // hide game over screen
        gameOver.ShowGameOverPanel(false);

        // update board state
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
        
        EventSteps.Clear();
        CheckUndoButtonState();
    }

    public void GameOver()
    {
        board.enabled = false;
        
        gameOver.ShowGameOverPanel(true, saveScores.IsWin);
    }
    
    public void CheckUndoButtonState()
    {
        if (EventSteps.Count > 0)
        {
            undoButton.interactable = true;
        }
        else
        {
            undoButton.interactable = false;
        }
    }
}