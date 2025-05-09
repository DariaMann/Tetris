using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LineBoard : MonoBehaviour
{
    [SerializeField] private GameObject scorePlusPrefab;
    [SerializeField] private Canvas mainCanvas;
    
    [SerializeField] private bool showFuture = true;
    
    [SerializeField] private int gridSize = 9;
    [SerializeField] private ThemeLines98 theme;
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Button undoButton;
    
    [SerializeField] private Image hint;
    [SerializeField] private Sprite hintAvailable;
    [SerializeField] private Sprite hintUnavailable;
    
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private List<Sprite> ballTypes;

//    private LineTile[,] _cells;

    public int GenerateCount { get; set; } = 3;
    
    public bool ShowFuture
    {
        get => showFuture;
        set => showFuture = value;
    }
    
    public bool IsPaused { get; private set; } = false;
    
    public List<LineTile> Tiles { get; set; } = new List<LineTile>();
    
    public List<Ball> Balls { get; set; } = new List<Ball>();
    
    public List<Ball> FutureBalls { get; set; } = new List<Ball>();
    
    public Ball SelectedBall { get; set; }
    
    public Stack<SaveDataLines98> EventSteps { get; set; } = new Stack<SaveDataLines98>();

    private void Start()
    {
        LoadLastPlay();
        SetHintState(ShowFuture);
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
        SaveDataLines98 saveData = GameHelper.SaveLines98.SaveDataLines98;
        if (saveData == null)
        {
            NewGame();
            return;
        }

        ShowFuture = saveData.ShowFuture;
        saveScores.ChangeScore(saveData.Score, false);
        saveScores.IsWin = saveData.IsWin;
        GenerateGrid();
        SpawnLoadedBalls(saveData.SaveBalls);
        SpawnLoadedBalls(saveData.SaveFutureBalls, true);
        CheckUndoButtonState();
    }
    

    private void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.SaveLines98.SaveDataLines98 = null;
            JsonHelper.SaveLines98(GameHelper.SaveLines98);
            return;
        }
        SaveDataLines98 data = new SaveDataLines98(saveScores.IsWin, ShowFuture, saveScores.CurrentScore, Balls, FutureBalls);
        
        GameHelper.SaveLines98.SaveDataLines98 = data;
        JsonHelper.SaveLines98(GameHelper.SaveLines98);
    }

    private void NewGame()
    {
        saveScores.ChangeScore(0, false);
        GenerateGrid();
        SpawnRandomBalls(GenerateCount);
        SpawnRandomBalls(GenerateCount, true);
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
        foreach (var future in FutureBalls)
        {
            future.DisabledBall();
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
            SaveDataLines98 saveData = EventSteps.Pop();
            
            ResetUndo();
            saveScores.ChangeScore(saveData.Score, false);
            SpawnLoadedBalls(saveData.SaveBalls);
            SpawnLoadedBalls(saveData.SaveFutureBalls, true);
            CheckUndoButtonState();
        }
    }
      
    
    public void AddStepEventObject()
    {
        SaveDataLines98 data = new SaveDataLines98(saveScores.IsWin, ShowFuture, saveScores.CurrentScore, Balls, FutureBalls);
        EventSteps.Push(data);
        CheckUndoButtonState();
    }

    public bool CheckLines()
    {
        bool haveDeleted = false;
        List<Ball> ballsToRemove = new List<Ball>();

        foreach (LineTile tile in Tiles)
        {
            if (!tile.IsEmpty())
            {
                Ball ball = tile.Ball;

                ballsToRemove.AddRange(CheckDirection(tile, Vector2Int.right));       // Горизонталь
                ballsToRemove.AddRange(CheckDirection(tile, Vector2Int.up));          // Вертикаль
                ballsToRemove.AddRange(CheckDirection(tile, new Vector2Int(1, 1)));   // Диагональ /
                ballsToRemove.AddRange(CheckDirection(tile, new Vector2Int(-1, 1)));  // Диагональ \
            }
        }

        // Удаляем дубликаты
        ballsToRemove = new HashSet<Ball>(ballsToRemove).ToList();

        if (ballsToRemove.Count >= 5)
        {
            Vector3 worldCenter = GetCenterWorldPosition(ballsToRemove);
            haveDeleted = true;
            foreach (Ball ball in ballsToRemove)
            {
                ball.Tile.RemoveBall();
                Balls.Remove(ball);
                ball.ExplodeAnimation();
//                Destroy(ball.gameObject);
            }

            int score = CalculateScore(ballsToRemove.Count); // подсчет очков
            saveScores.ChangeScore(score);
            ShowScorePlusAnimationFromUI(worldCenter, score);
            AudioManager.Instance.PlaySuccessLineSound();
        }

        return haveDeleted;
    }
    
    private Vector3 GetCenterWorldPosition(List<Ball> blocks)
    {
        Vector3 sum = Vector3.zero;
        foreach (var b in blocks)
            sum += b.GetComponent<RectTransform>().position; // глобальная позиция
        return sum / blocks.Count;
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

    private List<Ball> CheckDirection(LineTile startTile, Vector2Int direction)
    {
        List<Ball> matchedBalls = new List<Ball> { startTile.Ball };
        int colorIndex = startTile.Ball.IndexSprite;

        for (int i = 1; i < 9; i++)
        {
            Vector2Int checkPos = startTile.GridPosition + direction * i;
            LineTile tile = Tiles.Find(t => t.GridPosition == checkPos);
            if (tile != null && !tile.IsEmpty() && tile.Ball.IndexSprite == colorIndex)
            {
                matchedBalls.Add(tile.Ball);
            }
            else break;
        }

        if (matchedBalls.Count >= 5)
            return matchedBalls;

        return new List<Ball>();
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

    private void GenerateGrid()
    {
//        _cells = new LineTile[gridSize, gridSize];

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject cellGo = Instantiate(cellPrefab, gridParent);
                LineTile cell = cellGo.GetComponent<LineTile>();

                cell.SetData(new Vector2Int(x, y), this);
//                _cells[x, y] = cell;
                Tiles.Add(cell);
            }
        }
        
        theme.SetTheme(GameHelper.Theme);
    }
    
    public void EnabledFutureBalls()
    {
        if (FutureBalls.Count < GenerateCount)
        {
            SpawnRandomBalls(GenerateCount - FutureBalls.Count, true);
        }
        foreach (var future in FutureBalls)
        {
            future.EnabledBall();
            Balls.Add(future);
        }
        FutureBalls.Clear();
    }

    public void ShakingBalls()
    {
        foreach (var ball in Balls)
        {
            if (ball.IsSelected)
            {
                continue;
            }
            ball.ShakingAnimation();
        }
    }
    
    public void ResetAll()
    {
        ResetUndo();
        EventSteps.Clear();
    }
    
    public void ResetUndo()
    {
        SetSelection(null);
        foreach (var tile in Tiles)
        {
            tile.RemoveBall();
        }
        foreach (var ball in Balls)
        {
            Destroy(ball.gameObject);
        }
        Balls.Clear();
        foreach (var ball in FutureBalls)
        {
            Destroy(ball.gameObject);
        }
        FutureBalls.Clear();
    }
    
    public void PausedGame(bool isPause)
    {
        IsPaused = isPause;
    }
    
    public void SetSelection(Ball ball)
    {
        if (SelectedBall != null && SelectedBall != ball)
        {
            SelectedBall.SetSelection(false); 
        }

        SelectedBall = ball;
    }

    public void Again()
    {
        gameOver.ShowGameOverPanel(false);
        
        saveScores.ChangeScore(0, false);
        ResetAll();
        SpawnRandomBalls(GenerateCount);
        SpawnRandomBalls(GenerateCount, true);
        CheckUndoButtonState();
    }

    public void GameOver()
    {
        gameOver.ShowGameOverPanel(true, saveScores.IsWin);
    }
    
    public void SpawnRandomBalls(int count, bool isFuture = false)
    {
        List<LineTile> emptyCells = GetEmptyCells();

        for (int i = 0; i < count; i++)
        {
            if (emptyCells.Count == 0 && isFuture)
            {
                Debug.Log("КОНЕЦ ИГРЫ");
                GameOver();
                break;
            }

            // выбираем случайную пустую клетку
            int randomIndex = Random.Range(0, emptyCells.Count);
            LineTile targetCell = emptyCells[randomIndex];
            emptyCells.RemoveAt(randomIndex);

            // создаем шарик
            Ball ball = CreateRandomBall(targetCell);

            if (isFuture)
            {
                ball.DisabledBall();
                FutureBalls.Add(ball);
            }
            else
            {
                ball.EnabledBall();
                Balls.Add(ball);
            }
        }
    }
    
    public void SpawnLoadedBalls(List<SaveBall> saveBalls, bool isFuture = false)
    {
        foreach (var saveBall in saveBalls)
        {
            LineTile targetCell =
                Tiles.Find(tile => tile.GridPosition.x == saveBall.X && tile.GridPosition.y == saveBall.Y);

            // создаем шарик
            Ball ball = CreateRandomBall(targetCell, saveBall.Index);
            
            if (isFuture)
            {
                ball.DisabledBall();
                FutureBalls.Add(ball);
            }
            else
            {
                ball.EnabledBall();
                Balls.Add(ball);
            }
        }
    }

    private Ball CreateRandomBall(LineTile targetCell, int index = -1)
    {
        // создаем шарик
        GameObject ballGO = Instantiate(ballPrefab, targetCell.transform);
        Ball ball = ballGO.GetComponent<Ball>();
        if (index == -1)
        {
            index = Random.Range(0, ballTypes.Count);
        }
        Sprite randomBall = ballTypes[index];
        ball.SetData(randomBall, index, targetCell, this);
        targetCell.SetBall(ball);
        return ball;
    }

    private List<LineTile> GetEmptyCells()
    {
        List<LineTile> result = new List<LineTile>();

        foreach (var cell in Tiles)
        {
            if (cell.IsEmpty())
            {
                result.Add(cell);
            }
        }

        return result;
    }
    
    public List<LineTile> GetPathToTarget(LineTile startTile, LineTile targetTile)
    {
        if (startTile == targetTile)
            return new List<LineTile>();

        Queue<List<LineTile>> pathsQueue = new Queue<List<LineTile>>();
        HashSet<LineTile> visited = new HashSet<LineTile>();

        pathsQueue.Enqueue(new List<LineTile> { startTile });

        while (pathsQueue.Count > 0)
        {
            List<LineTile> currentPath = pathsQueue.Dequeue();
            LineTile currentTile = currentPath[currentPath.Count - 1]; // Берем последнюю клетку в текущем пути

            if (currentTile == targetTile)
                return currentPath; // Нашли путь до цели

            foreach (LineTile neighbor in GetNeighbors(currentTile))
            {
                if (neighbor.IsEmpty() && !visited.Contains(neighbor))
                {
                    List<LineTile> newPath = new List<LineTile>(currentPath) { neighbor };
                    pathsQueue.Enqueue(newPath);
                    visited.Add(neighbor);
                }
            }
        }

        return new List<LineTile>(); // Если пути нет, возвращаем пустой список
    }
    
    private List<LineTile> GetNeighbors(LineTile tile)
    {
        List<LineTile> neighbors = new List<LineTile>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),   // вправо
            new Vector2Int(-1, 0),  // влево
            new Vector2Int(0, 1),   // вверх
            new Vector2Int(0, -1)   // вниз
        };

        foreach (var dir in directions)
        {
            Vector2Int checkPos = tile.GridPosition + dir;
            LineTile neighbor = Tiles.Find(t => t.GridPosition == checkPos);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

}