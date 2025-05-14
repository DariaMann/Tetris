using System.Collections.Generic;
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

    public static GameManagerBlocks Instance { get; private set; }

    public Stack<SaveDataBlocks> EventSteps { get; set; } = new Stack<SaveDataBlocks>();
    
    public BlockTile EnableTile { get; set; }

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
    }

    private void LoadLastPlay()
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

    private void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.SaveBlocks.SaveDataBlocks = null;
            JsonHelper.SaveBlocks(GameHelper.SaveBlocks);
            return;
        }

        SaveDataBlocks data = new SaveDataBlocks(saveScores.IsWin, saveScores.CurrentScore, board.Tiles, board.Blocks);

        GameHelper.SaveBlocks.SaveDataBlocks = data;
        JsonHelper.SaveBlocks(GameHelper.SaveBlocks);
    }

    private void NewGame()
    {
        saveScores.ChangeScore(0, false);
        board.GenerateGrid();
        board.CreateBlocks();
    }

    public void OnChangeBlocks()
    {
        board.CreateBlocks();
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