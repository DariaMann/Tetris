using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManagerLines98 : MonoBehaviour
{
    [SerializeField] private LineBoard board;
    [SerializeField] private LineBoard boardEdu;
    [SerializeField] private EducationLines98 education;
    [SerializeField] private GameObject scorePlusPrefab;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Revive revivePanel;
    [SerializeField] private List<Button> undoButtons;
    [SerializeField] private ThemeLines98 theme;
    [SerializeField] private bool showFuture = true;

    [SerializeField] private Image hint;
    [SerializeField] private Sprite hintAvailable;
    [SerializeField] private Sprite hintUnavailable;
    
    public static GameManagerLines98 Instance { get; private set; }

    public Stack<SaveDataLines98> EventSteps { get; set; } = new Stack<SaveDataLines98>();
    
    public SaveScores SaveScores
    {
        get => saveScores;
        set => saveScores = value;
    }
    
    public ThemeLines98 Theme
    {
        get => theme;
        set => theme = value;
    }
    
    public bool ShowFuture
    {
        get => showFuture;
        set => showFuture = value;
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
        SetHintState(ShowFuture);

        if (!GameHelper.GetEducationState(MiniGameType.Lines98))
        {
            education.ShowEducation(true);
            GameHelper.SetEducationState(MiniGameType.Lines98, true);
        }
        else
        {
            AppodealManager.Instance.ShowBottomBanner();
        }

        AppodealManager.Instance.OnRewardedVideoFinishedAction += GiveReward;
        AppodealManager.Instance.OnInterstitialFinished += ShowGameOverPanel;
        AnalyticsManager.Instance.LogEvent(AnalyticType.game_start.ToString(), (float) GameHelper.GameType);
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
        AppodealManager.Instance.OnRewardedVideoFinishedAction -= GiveReward;
        AppodealManager.Instance.OnInterstitialFinished -= ShowGameOverPanel;
    }
    
    public void LoadLastPlay()
    {
        SaveDataLines98 saveData = GameHelper.SaveLines98.SaveDataLines98;
        if (saveData == null)
        {
            NewGame();
            return;
        }

        GameHelper.IsRevived = saveData.IsRevived;
        ShowFuture = saveData.ShowFuture;
        saveScores.ChangeScore(saveData.Score, false);
        saveScores.IsWin = saveData.IsWin;
        board.GenerateGrid();
        board.SpawnLoadedBalls(saveData.SaveBalls);
        board.SpawnLoadedBalls(saveData.SaveFutureBalls, true);
        CheckUndoButtonState();
    }
    
    public void LoadEducation(SaveDataLines98 saveData)
    {
        boardEdu.GenerateGrid();
        boardEdu.SpawnLoadedBalls(saveData.SaveBalls);
    }
    
    public void ReloadEducation(SaveDataLines98 saveData)
    {
        foreach (var ball in boardEdu.Balls)
        {
            Destroy(ball.gameObject);
        }
        boardEdu.Balls.Clear();
        boardEdu.SpawnLoadedBalls(saveData.SaveBalls);
    }

    public void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.SaveLines98.SaveDataLines98 = null;
            MyJsonHelper.SaveLines98(GameHelper.SaveLines98);
            return;
        }
        SaveDataLines98 data = new SaveDataLines98(saveScores.IsWin, GameHelper.IsRevived, ShowFuture, saveScores.CurrentScore, board.Balls, board.FutureBalls);
        
        GameHelper.SaveLines98.SaveDataLines98 = data;
        MyJsonHelper.SaveLines98(GameHelper.SaveLines98);
    }

    private void NewGame()
    {
        saveScores.ChangeScore(0, false);
        board.GenerateGrid();
        board.SpawnRandomBalls(board.GenerateCount);
        board.SpawnRandomBalls(board.GenerateCount, true);
    }
    
    public void OnChangeHintStateClick()
    {
        SetHintState(!ShowFuture);
        SaveLastPlay();
    }
    
    private void SetHintState(bool state)
    {
        ShowFuture = state;
        hint.sprite = ShowFuture ? hintAvailable : hintUnavailable;
        foreach (var future in board.FutureBalls)
        {
            future.DisabledBall();
        }
    }
    
    public void CheckUndoButtonState()
    {
        if (EventSteps.Count > 0)
        {
            foreach (var undoButton in undoButtons)
            {
                undoButton.interactable = true;
            }
        }
        else
        {
            foreach (var undoButton in undoButtons)
            {
                undoButton.interactable = false;
            }
        }
    }
    
    public void OnUndo()
    {
        if (EventSteps.Count > 0)
        {
            SaveDataLines98 saveData = EventSteps.Pop();
            
            ResetUndo();
            saveScores.ChangeScore(saveData.Score, false);
            board.SpawnLoadedBalls(saveData.SaveBalls);
            board.SpawnLoadedBalls(saveData.SaveFutureBalls, true);
            CheckUndoButtonState();
        }
    }
    
    public void AddStepEventObject()
    {
        SaveDataLines98 data = new SaveDataLines98(saveScores.IsWin, GameHelper.IsRevived, ShowFuture, saveScores.CurrentScore, board.Balls, board.FutureBalls);
        EventSteps.Push(data);
        CheckUndoButtonState();
    }
    
    public void ShowScorePlusAnimationFromUI(Vector3 worldCenter, int score)
    {
        // Переводим в локальную позицию канваса
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform,
            Camera.main.WorldToScreenPoint(worldCenter),
            Camera.main,
            out Vector2 anchoredPos
        );

        GameObject go = Instantiate(scorePlusPrefab, mainCanvas.transform);
        go.transform.SetSiblingIndex(1);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;

        Debug.Log($"[ShowScore] anchoredPosition: {anchoredPos}");

        go.GetComponent<ScorePlusAnimation>().Play(score);
    }
    
    public int CalculateScore(int ballsRemoved)
    {
        if (ballsRemoved >= 5)
        {
            int score = 10 + (ballsRemoved - 5) * 2;
            return score;
        }
        return ballsRemoved;
    }
    
    public void ResetAllBoardEducation()
    {
        foreach (var tile in boardEdu.Tiles)
        {
            Destroy(tile.gameObject);
        }
        boardEdu.Tiles.Clear();
        boardEdu.Balls.Clear();
    }
    
    public void ResetAll()
    {
        ResetUndo();
        EventSteps.Clear();
    }
    
    public void ResetUndo()
    {
        board.SetSelection(null);
        foreach (var tile in board.Tiles)
        {
            tile.RemoveBall();
        }
        foreach (var ball in board.Balls)
        {
            Destroy(ball.gameObject);
        }
        board.Balls.Clear();
        foreach (var ball in board.FutureBalls)
        {
            Destroy(ball.gameObject);
        }
        board.FutureBalls.Clear();
    }

    public void Again()
    {
        GameHelper.IsRevived = false;
        gameOver.ShowGameOverPanel(false);
        revivePanel.ShowMainRevivePanel(false);
        
        saveScores.ChangeScore(0, false);
        ResetAll();
        board.SpawnRandomBalls(board.GenerateCount);
        board.SpawnRandomBalls(board.GenerateCount, true);
        CheckUndoButtonState();
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
        if (!GameHelper.IsRevived)
        {
            revivePanel.ShowMainRevivePanel(true);
            return;
        }
        gameOver.ShowGameOverPanel(true, saveScores, saveScores.IsWin);
    }

    public void DeleteBallsByColor(int color)
    {
        board.DeleteBallsByColor(color);
    }
    
    public void OnRevive()
    {
        AnalyticsManager.Instance.LogEvent(AnalyticType.revive_lines_click.ToString(), saveScores.CurrentScore);
        
        if (GameHelper.HaveAds)
        {

#if UNITY_EDITOR
            GiveReward();
            return;
#endif
            AppodealManager.Instance.ShowRewardedVideo();
            return;
        }
        GiveReward();
    }
    
    private void GiveReward()
    {
        GameHelper.IsRevived = true;
        revivePanel.ShowLinesRevivePanel(true);
    }
    
    public void OnCancelRevive()
    {
        revivePanel.ShowMainRevivePanel(false);
        gameOver.ShowGameOverPanel(true, saveScores, saveScores.IsWin);
    }
}