using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManagerSnake: MonoBehaviour
{
    [SerializeField] private Snake snake;
    [SerializeField] private FoodController foodController;
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Buttons pausePanel;
    [SerializeField] private EducationSnake education;
    
    public static GameManagerSnake Instance { get; private set; }
    
    public GameOver GameOverPanel
    {
        get => gameOver;
        set => gameOver = value;
    }
    
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
        
        if (!GameHelper.GetEducationState(MiniGameType.Snake))
        {
            education.ShowEducation(true);
            GameHelper.SetEducationState(MiniGameType.Snake, true);
        }
        else
        {
            if (GameHelper.SaveSnake.SaveDataSnake == null)
            {
                AppodealManager.Instance.ShowBottomBanner();
            }
        }

        AppodealManager.Instance.OnInterstitialFinished += ShowGameOverPanel;
        AnalyticsManager.Instance.LogEvent(AnalyticType.game_start.ToString(), new Dictionary<string, object>
        {
            { AnalyticType.game.ToString(), GameHelper.GameType.ToString() },
            { AnalyticType.timestamp.ToString(), DateTime.UtcNow.ToString("o") }
        });
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
        SaveLastPlay();
        AppodealManager.Instance.OnInterstitialFinished -= ShowGameOverPanel;
    }

    public void LoadLastPlay()
    {
        SaveDataSnake saveData = GameHelper.SaveSnake.SaveDataSnake;
        if (saveData == null)
        {
            snake.ResetState();
            return;
        }

        pausePanel.OnPauseClick();
        snake.LoadSave(saveData);
    }
    
    public void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.SaveSnake.SaveDataSnake = null;
            MyJsonHelper.SaveSnake(GameHelper.SaveSnake);
            return;
        }

        SaveDataSnake data = new SaveDataSnake(saveScores.IsWin, foodController.Foods, snake.Segments, snake.Direction, saveScores.CurrentScore);
        GameHelper.SaveSnake.SaveDataSnake = data;
        MyJsonHelper.SaveSnake(GameHelper.SaveSnake);
    }

    public void GameOver()
    {
        GameHelper.IsGameOver = true;
        if (AppodealManager.Instance.IsShowInterstitial())
        {
            AppodealManager.Instance.ShowInterstitial();
        }
        else
        {
            ShowGameOverPanel();
        }
    }

    public void ShowGameOverPanel()
    {
        gameOver.ShowGameOverPanel(true, saveScores, saveScores.IsWin);
    }
    
    public void Again()
    {
        gameOver.ShowGameOverPanel(false);
        
        snake.ResetState();
    }
}