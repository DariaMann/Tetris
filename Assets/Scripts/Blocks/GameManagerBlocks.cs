using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManagerBlocks : MonoBehaviour
{
    [SerializeField] private EducationBlocks education;
    [SerializeField] private BlocksBoard board;
    [SerializeField] private BlocksBoard boardEdu;
    [SerializeField] private GameObject scorePlusPrefab;
    [SerializeField] private Canvas mainCanvas;

    [SerializeField] private ThemeBlocks themeBlocks;
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Button undoButton;
    
    [SerializeField] private CanvasGroup changeBlocksButton;
    [SerializeField] private GameObject changeBlocksPanel;
    [SerializeField] private GameObject changeBlocksImage;
    [SerializeField] private TextMeshProUGUI changeBlocksText;

    public static GameManagerBlocks Instance { get; private set; }

    public static int CountChangeBlocks { get; private set; }

    public Stack<SaveDataBlocks> EventSteps { get; set; } = new Stack<SaveDataBlocks>();

    public EducationBlocks Education
    {
        get => education;
        set => education = value;
    }

    public SaveScores SaveScores
    {
        get => saveScores;
        set => saveScores = value;
    }
    
    public ThemeBlocks ThemeBlocks
    {
        get => themeBlocks;
        set => themeBlocks = value;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        LoadLastPlay();
        CheckUndoButtonState();

        if (!GameHelper.GetEducationState(MiniGameType.Blocks))
        {
            education.ShowEducation(true);
            GameHelper.SetEducationState(MiniGameType.Blocks, true);
        }
        else
        {
            AppodealManager.Instance.ShowBottomBanner();
        }
        
        AppodealManager.Instance.OnRewardedVideoFinishedAction += GiveReward;
        AppodealManager.Instance.OnRewardedVideoLoadedAction += CheckStateChangeBlocksButton;
        AppodealManager.Instance.OnInterstitialFinished += ShowGameOverPanel;
        
        CheckDailyHints();
        GameHelper.GetHaveAds();
        ApplyHaveAds(GameHelper.HaveAds);
        GameHelper.OnHaveAdsChanged += ApplyHaveAds;
    }
    
    private void CheckDailyHints()
    {
        // Загружаем текущее количество подсказок
        CountChangeBlocks = PlayerPrefs.GetInt("CountChangeBlocks");

        // Загружаем последнюю дату начисления
        string lastDateStr = PlayerPrefs.GetString("ChangeBlocksData");
        DateTime now = DateTime.Now;
        DateTime lastCheck = DateTime.Parse(lastDateStr);

        // Если текущая дата уже на следующий день и время 00:00 или позже
        if (now.Date > lastCheck.Date)
        {
            CountChangeBlocks = 3;
            PlayerPrefs.SetInt("CountChangeBlocks", CountChangeBlocks);
            // Обновляем дату последней проверки
            PlayerPrefs.SetString("ChangeBlocksData", now.ToString());
        }
        CheckStateChangeBlocksButton();
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
        if (AppodealManager.Instance != null)
        {
            AppodealManager.Instance.OnRewardedVideoFinishedAction -= GiveReward;
            AppodealManager.Instance.OnRewardedVideoLoadedAction -= CheckStateChangeBlocksButton;
            AppodealManager.Instance.OnInterstitialFinished -= ShowGameOverPanel;
        }
        GameHelper.OnHaveAdsChanged -= ApplyHaveAds;
    }

    public void LoadLastPlay()
    {
        SaveDataBlocks saveData = GameHelper.SaveBlocks.SaveDataBlocks;
        if (saveData == null)
        {
            NewGame();
            return;
        }

        saveScores.ChangeScore(saveData.Score, false);
        saveScores.IsWin = saveData.IsWin;
        board.GenerateGrid();
        board.FullLoadGrid(saveData.SaveBlocksTile);
        board.CreateBlocks(saveData.Blocks);
        board.CheckInteractableBlocks();
        CheckUndoButtonState();
    }

    public void LoadStartEducation()
    {
        boardEdu.GenerateGrid();
    }

    public void LoadEducation(SaveDataBlocks saveData)
    {
        boardEdu.FullLoadGrid(saveData.SaveBlocksTile);
        boardEdu.CreateBlocks(saveData.Blocks);

        themeBlocks.SetTheme(GameHelper.Theme);
    }

    public void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.SaveBlocks.SaveDataBlocks = null;
            MyJsonHelper.SaveBlocks(GameHelper.SaveBlocks);
            return;
        }

        SaveDataBlocks data = new SaveDataBlocks(saveScores.IsWin, saveScores.CurrentScore, board.Tiles, board.Blocks);

        GameHelper.SaveBlocks.SaveDataBlocks = data;
        MyJsonHelper.SaveBlocks(GameHelper.SaveBlocks);
    }

    private void NewGame()
    {
        saveScores.ChangeScore(0, false);
        board.GenerateGrid();
        board.CreateBlocks();
    }

    public void ApplyHaveAds(bool stateAds)
    {
        CheckStateChangeBlocksButton();
    }
    
    public void CheckStateChangeBlocksButton()
    {
        if (!GameHelper.HaveAds)
        {
            changeBlocksPanel.SetActive(false);
            changeBlocksButton.interactable = true;
            changeBlocksButton.alpha = 1;
            return;
        }
        
        if (CountChangeBlocks <= 0)
        {
            changeBlocksPanel.SetActive(true);
            changeBlocksImage.SetActive(true);
            changeBlocksText.gameObject.SetActive(false);
            
            if (AppodealManager.Instance.IsRewardedVideoReady())
            {
                changeBlocksButton.interactable = true;
                changeBlocksButton.alpha = 1;
            }
            else
            {
                changeBlocksButton.interactable = false;
                changeBlocksButton.alpha = 0.5f;
            }
        }
        else
        {
            changeBlocksPanel.SetActive(true);
            changeBlocksImage.SetActive(false);
            changeBlocksText.gameObject.SetActive(true);
            changeBlocksText.text = CountChangeBlocks.ToString();
            changeBlocksButton.interactable = true;
            changeBlocksButton.alpha = 1;
        }
    }
    
    private void GiveReward()
    {
        Debug.Log("Награда выдана!");
        // Например, добавим монеты игроку
        CountChangeBlocks += 1;
        PlayerPrefs.SetInt("CountChangeBlocks", CountChangeBlocks);

        CheckStateChangeBlocksButton();
    }

    public void OnChangeBlocks()
    {
        if (GameHelper.HaveAds)
        {

            if (CountChangeBlocks <= 0)
            {
#if UNITY_EDITOR
                GiveReward();
                return;
#endif
                AppodealManager.Instance.ShowRewardedVideo();
                return;
            }
            
            CountChangeBlocks -= 1;
            PlayerPrefs.SetInt("CountChangeBlocks", CountChangeBlocks);
        }

        board.CreateBlocks();
        CheckStateChangeBlocksButton();
    }

    public void Again()
    {
        gameOver.ShowGameOverPanel(false);

        saveScores.ChangeScore(0, false);
        ResetAll();
        board.CreateBlocks();
        board.CheckInteractableBlocks();
        CheckUndoButtonState();
    }

    public void GameOver()
    {
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
        gameOver.ShowGameOverPanel(true, saveScores.IsWin);
    }

    public void ResetAll()
    {
        ResetGrid();
        EventSteps.Clear();
    }

    public void ResetAllBoardEducation()
    {
        foreach (var tile in boardEdu.Tiles)
        {
            Destroy(tile.gameObject);
        }

        boardEdu.Tiles.Clear();
    }

    public void ResetGrid()
    {
        foreach (var tile in board.Tiles)
        {
            tile.Deactivate();
        }
    }
    
    public void ResetGridEdu()
    {
        foreach (var tile in boardEdu.Tiles)
        {
            tile.Deactivate();
        }
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

    public void OnUndo()
    {
        if (EventSteps.Count > 0)
        {
            SaveDataBlocks saveData = EventSteps.Pop();

            saveScores.ChangeScore(saveData.Score, false);
            ResetGrid();
            board.FullLoadGrid(saveData.SaveBlocksTile);
            board.CreateBlocks(saveData.Blocks);
            board.CheckInteractableBlocks();
            CheckUndoButtonState();
        }
    }

    public void AddStepEventObject()
    {
        SaveDataBlocks data = new SaveDataBlocks(saveScores.IsWin, saveScores.CurrentScore, board.Tiles, board.Blocks);
        EventSteps.Push(data);
        CheckUndoButtonState();
    }

    public void ShowScorePlusAnimationFromUI(List<BlockTile> deletedBlocks, int score)
    {
        if (deletedBlocks == null || deletedBlocks.Count == 0) return;

        // Получаем среднюю мировую позицию
        Vector3 worldCenter = GetCenterWorldPosition(deletedBlocks);

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
    
    private Vector3 GetCenterWorldPosition(List<BlockTile> blocks)
    {
        Vector3 sum = Vector3.zero;
        foreach (var b in blocks)
            sum += b.GetComponent<RectTransform>().position; // глобальная позиция
        return sum / blocks.Count;
    }

    public (List<BlockTile> tilesToClear, int score) CheckLinesAndGetScore(BlocksBoard currBoard)
    {
        List<BlockTile> tilesToClear = new List<BlockTile>();
        HashSet<BlockTile> uniqueTiles = new HashSet<BlockTile>();
        int score = 0;
        int comboCount = 0;

        // Проверка по горизонтали
        for (int y = 0; y < currBoard.GridSize; y++)
        {
            bool fullRow = true;
            for (int x = 0; x < currBoard.GridSize; x++)
            {
                if (!currBoard.GetTile(x, y).IsOccupied)
                {
                    fullRow = false;
                    break;
                }
            }

            if (fullRow)
            {
                for (int x = 0; x < currBoard.GridSize; x++)
                {
                    uniqueTiles.Add(currBoard.GetTile(x, y));
                }

                score += 10;
                comboCount++;
            }
        }

        // Проверка по вертикали
        for (int x = 0; x < currBoard.GridSize; x++)
        {
            bool fullColumn = true;
            for (int y = 0; y < currBoard.GridSize; y++)
            {
                if (!currBoard.GetTile(x, y).IsOccupied)
                {
                    fullColumn = false;
                    break;
                }
            }

            if (fullColumn)
            {
                for (int y = 0; y < currBoard.GridSize; y++)
                {
                    uniqueTiles.Add(currBoard.GetTile(x, y));
                }

                score += 10;
                comboCount++;
            }
        }

        // Проверка 3x3 блоков
        for (int blockY = 0; blockY < 3; blockY++)
        {
            for (int blockX = 0; blockX < 3; blockX++)
            {
                bool fullBlock = true;
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        int gx = blockX * 3 + x;
                        int gy = blockY * 3 + y;

                        if (!currBoard.GetTile(gx, gy).IsOccupied)
                        {
                            fullBlock = false;
                            break;
                        }
                    }

                    if (!fullBlock) break;
                }

                if (fullBlock)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            int gx = blockX * 3 + x;
                            int gy = blockY * 3 + y;

                            uniqueTiles.Add(currBoard.GetTile(gx, gy));
                        }
                    }

                    score += 20;
                    comboCount++;
                }
            }
        }

        // ✅ Добавляем 20% за каждую дополнительную комбинацию
        if (comboCount > 1)
        {
            float bonusMultiplier = 1 + 0.2f * (comboCount - 1); // например, при 3 комбинациях: 1 + 0.2 * 2 = 1.4
            score = Mathf.RoundToInt(score * bonusMultiplier);
        }

        tilesToClear = new List<BlockTile>(uniqueTiles);
        return (tilesToClear, score);
    }
}