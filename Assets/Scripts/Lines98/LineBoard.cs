using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LineBoard : MonoBehaviour
{
    [SerializeField] private bool isEducation;

    [SerializeField] private int gridSize = 9;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private List<Sprite> ballTypes;

    public int GenerateCount { get; set; } = 3;

    public List<LineTile> Tiles { get; set; } = new List<LineTile>();
    
    public List<Ball> Balls { get; set; } = new List<Ball>();
    
    public List<Ball> FutureBalls { get; set; } = new List<Ball>();
    
    public Ball SelectedBall { get; set; }
    
    public bool IsEducation
    {
        get => isEducation;
        set => isEducation = value;
    }
    
    public void GenerateGrid()
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject cellGo = Instantiate(cellPrefab, gridParent);
                LineTile cell = cellGo.GetComponent<LineTile>();

                cell.SetData(new Vector2Int(x, y), this);
                Tiles.Add(cell);
            }
        }
        
        GameManagerLines98.Instance.Theme.SetTheme(GameHelper.Theme);
    }

    public void DeleteBallsByColor(int indexColor)
    {
        List<Ball> ballsToRemove = new List<Ball>();
        foreach (var ball in Balls)
        {
            if (ball.IndexSprite == indexColor)
            {
                ballsToRemove.Add(ball);
            }
        }
        
        ballsToRemove = new HashSet<Ball>(ballsToRemove).ToList();
        
        foreach (Ball ball in ballsToRemove)
        {
            ball.Tile.RemoveBall();
            Balls.Remove(ball);
            ball.ExplodeAnimation();
        }
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

            if (isEducation)
            {
                return true;
            }

            int score = GameManagerLines98.Instance.CalculateScore(ballsToRemove.Count); // подсчет очков
            GameManagerLines98.Instance.SaveScores.ChangeScore(score);
            GameManagerLines98.Instance.ShowScorePlusAnimationFromUI(worldCenter, score);
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

    public void EnabledFutureBalls()
    {
        if (isEducation)
        {
            return;
        }
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

    public void SetSelection(Ball ball)
    {
        if (SelectedBall != null && SelectedBall != ball)
        {
            SelectedBall.SetSelection(false); 
        }

        SelectedBall = ball;
    }

    public void CheckGameOver()
    {
        List<LineTile> emptyCells = GetEmptyCells();
        
        if (emptyCells.Count == 0)
        {
            Debug.Log("КОНЕЦ ИГРЫ");
            GameManagerLines98.Instance.GameOver();
        }
    }

    public void SpawnRandomBalls(int count, bool isFuture = false)
    {
        if (isEducation)
        {
            return;
        }
        List<LineTile> emptyCells = GetEmptyCells();

        for (int i = 0; i < count; i++)
        {
            if (emptyCells.Count == 0 && isFuture)
            {
//                Debug.Log("КОНЕЦ ИГРЫ");
//                GameManagerLines98.Instance.GameOver();
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
            LineTile targetCell = GetLineTileByPos(new Vector2Int(saveBall.X, saveBall.Y));

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

    public LineTile GetLineTileByPos(Vector2Int pos)
    {
        foreach (var tile in Tiles)
        {
            if (pos == tile.GridPosition)
            {
                return tile;
            }
        }

        return null;
    }
    
    public Ball GetBallByPos(Vector2Int pos)
    {
        foreach (var ball in Balls)
        {
            if (pos == ball.Tile.GridPosition)
            {
                return ball;
            }
        }

        return null;
    }

}