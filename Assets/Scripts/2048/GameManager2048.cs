using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager2048 : MonoBehaviour
{
    [SerializeField] private Education2048 education;
    [SerializeField] private TileBoard educationBoard;
    [SerializeField] private TileBoard board;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private List<Button> undoButtons;

    public static GameManager2048 Instance { get; private set; }
    
    public Education2048 Education
    {
        get => education;
        set => education = value;
    }

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
        
        if (!GameHelper.GetEducationState(MiniGameType.G2048))
        {
            education.ShowEducation(true);
            GameHelper.SetEducationState(MiniGameType.G2048, true);
        }
        else
        {
            AppodealManager.Instance.ShowBottomBanner();
        }
        AppodealManager.Instance.OnInterstitialFinished += ShowGameOverPanel;
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
        AppodealManager.Instance.OnInterstitialFinished -= ShowGameOverPanel;
    }

    public void LoadLastPlay()
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

        // update board state
        board.ClearBoard();

        foreach (var tile in saveData.SaveTiles)
        {
            board.CreateTile(tile);
        }
        
        board.enabled = true;
        
        CheckUndoButtonState();
    }
    
    public void LoadEducation(SaveData2048 saveData)
    {
        educationBoard.ClearBoard();

        foreach (var tile in saveData.SaveTiles)
        {
            educationBoard.CreateTile(tile);
        }
        
        educationBoard.enabled = true;
    }
    
    public void ResetAllBoardEducation()
    {
        educationBoard.ClearBoard();
    }

    public void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.Save2048.SaveData2048 = null;
            MyJsonHelper.Save2048(GameHelper.Save2048);
            return;
        }
        SaveData2048 data = new SaveData2048(SaveScores.IsWin, SaveScores.CurrentScore, board.Tiles);
        GameHelper.Save2048.SaveData2048 = data;
        MyJsonHelper.Save2048(GameHelper.Save2048);
    }

    public void ChangeMaximumNumber(int newMaximum)
    {
        if (newMaximum > GameHelper.Save2048.Maximum)
        {
            GameHelper.Save2048.Maximum = newMaximum;
            MyJsonHelper.Save2048(GameHelper.Save2048);
        }
        
        SaveScores.ChangeMaximum(newMaximum);
    }

    public void Again()
    {
        gameOver.ShowGameOverPanel(false);
        NewGame();
    }
    
    public void NewGame()
    {
        // reset score
        SaveScores.ChangeScore(0);
        ChangeMaximumNumber(GameHelper.Save2048.Maximum);

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
        if (AppodealManager.Instance.IsShowInterstitial())
        {
            AppodealManager.Instance.TryShowInterstitial();
        }
        else
        {
            ShowGameOverPanel();
        }
    }

    public void ShowGameOverPanel()
    {
        board.enabled = false;
        gameOver.ShowGameOverPanel(true, saveScores.IsWin);
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
            RestoreStepEvent(EventSteps.Pop());
        }
    }
    
    public Step2048 CreateStepEvent()
    {
        Step2048 step = new Step2048();
    
        foreach (var tile in board.Tiles)
        {
            TileEvent tileSnap = new TileEvent()
            {
                X = tile.Cell.Coordinates.x,
                Y = tile.Cell.Coordinates.y,
                StateIndex = tile.State.index
            };
            step.Tiles.Add(tileSnap);
        }

        step.Steps = SaveScores.CurrentScore;

        return step;
    }
    
    private void RestoreStepEvent(Step2048 step)
    {
        board.ClearBoard();

        foreach (var tileSnap in step.Tiles)
        {
            board.CreateTile(tileSnap);
        }

        SaveScores.ChangeScore(step.Steps, false);
        CheckUndoButtonState();
    }
}