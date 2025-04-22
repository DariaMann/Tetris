using System.Collections;
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
    [SerializeField] private TextMeshProUGUI maximumText;

    public static GameManager Instance { get; private set; }

    public int MaxNumber { get; private set; } = 2;
    
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
        SaveData2048 saveData = JsonHelper.Load2048Data();
        if (saveData == null)
        {
            NewGame();
            return;
        }
        
        SaveScores.ChangeScore(saveData.Score);
        saveScores.IsWin = saveData.IsWin;
        ChangeMaximumNumber(saveData.Maximum);
        // hide game over screen
        gameOver.ShowGameOverPanel(false);

        // update board state
        board.ClearBoard();

        foreach (var tile in saveData.SaveTiles)
        {
            board.CreateTile(tile);
        }
        
        board.enabled = true;
    }

    private void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            JsonHelper.Save2048Data(null);
            return;
        }
        SaveData2048 data = new SaveData2048(SaveScores.IsWin, SaveScores.CurrentScore, MaxNumber, board.Tiles);
        
        JsonHelper.Save2048Data(data);
    }

    public void ChangeMaximumNumber(int newMaximum)
    {
        if (newMaximum > MaxNumber)
        {
            MaxNumber = newMaximum;
        }
        
        SaveScores.ChangeMaximum(newMaximum);
        
        maximumText.text = LocalizationManager.Localize("2048.maximum") + ": " + MaxNumber;
    }
    
    public void NewGame()
    {
        // reset score
        SaveScores.ChangeScore(0);
        ChangeMaximumNumber(MaxNumber);
        
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