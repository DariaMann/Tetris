using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    [SerializeField] private GameOver gameOver;

    [SerializeField] private SaveScores saveScores;
    [SerializeField] private List<Image> nextTetrominoImage;

    [SerializeField] private TetrominoData[] tetrominoes;
    [SerializeField] private Vector2Int boardSize = new Vector2Int(10, 20);

    [SerializeField] private Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    private TetrominoData _next;
    
    public bool IsPaused { get; private set; } = false;
    
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

    public Tilemap Tilemap { get; private set; }
    
    public Piece ActivePiece { get; private set; }

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    public Vector2Int BoardSize
    {
        get => boardSize;
        set => boardSize = value;
    }
    
    private void Awake()
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        ActivePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        LoadLastPlay();
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
        SaveDataTetris saveData = GameHelper.SaveTetris.SaveDataTetris;
        if (saveData == null)
        {
            NextRandomTetromino();
            SpawnPiece(true);
            return;
        }
        
        saveScores.ChangeScore(saveData.Score);
        saveScores.IsWin = saveData.IsWin;
        LoadTilemap(saveData.SaveTetrominos);
        NextRandomTetromino(GetTetrominoDataByType(saveData.NextTetromino));
        SpawnPiece(GetTetrominoDataByType(saveData.CurrentTetromino));
    }
    
    private void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.SaveTetris.SaveDataTetris = null;
            JsonHelper.SaveTetris(GameHelper.SaveTetris);
            return;
        }

        List<SaveTetramino> tetrominos = new List<SaveTetramino>();
        BoundsInt bounds = Tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = Tilemap.GetTile(pos);
            if (tile != null)
            {
                bool isCurrentTetramino = false;
                for (int i = 0; i < ActivePiece.Data.cells.Length; i++)
                {
                    Vector3Int globalCellPos = ActivePiece.Position + ActivePiece.Cells[i];
                    if (pos == globalCellPos)
                    {
                        isCurrentTetramino = true;
                        break;
                    }
                }

                if (isCurrentTetramino)
                {
                    continue;
                }

                TetrominoData tetromino = tetrominoes.ToList().Find(t => t.tile == tile);
            
                tetrominos.Add(new SaveTetramino
                {
                    Tetromino = tetromino.tetromino,
                    X = pos.x,
                    Y = pos.y,
                    Z = pos.z
                });
            }
        }
        
        SaveDataTetris data = new SaveDataTetris(saveScores.IsWin, saveScores.CurrentScore, ActivePiece.Data.tetromino, _next.tetromino, tetrominos);
        GameHelper.SaveTetris.SaveDataTetris = data;
        JsonHelper.SaveTetris(GameHelper.SaveTetris);
    }
    
    public void LoadTilemap(List<SaveTetramino> saveTetrominos)
    {
        Tilemap.ClearAllTiles();

        foreach (SaveTetramino saved in saveTetrominos)
        {
            Vector3Int pos = new Vector3Int(saved.X, saved.Y, saved.Z);
            TetrominoData tetromino = tetrominoes.ToList().Find(t => t.tetromino == saved.Tetromino);
            Tilemap.SetTile(pos, tetromino.tile);
        }
    }

    
    public void NextRandomTetromino()
    {
        int random = Random.Range(0, tetrominoes.Length);
        _next = tetrominoes[random];
        foreach (var next in nextTetrominoImage)
        {
            next.sprite = _next.sprite;
        }
        Debug.Log("Следующая : " +  Enum.GetName(typeof(Tetromino), _next.tetromino));
    }

    public void SpawnPiece(bool first = false)
    {
        if (gameOver.IsGameOver)
        {
            return;
        }
        TetrominoData data = _next;
        if (first)
        {
            int random = Random.Range(0, tetrominoes.Length);
            data = tetrominoes[random];
        }

        ActivePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(ActivePiece, spawnPosition)) {
            Set(ActivePiece);
        } else {
            GameOver();
        }
    }
    
    public void SpawnPiece(TetrominoData data)
    {
        ActivePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(ActivePiece, spawnPosition)) {
            Set(ActivePiece);
        } else {
            GameOver();
        }
    }
    
    public void NextRandomTetromino(TetrominoData data)
    {
        _next = data;
        foreach (var next in nextTetrominoImage)
        {
            next.sprite = _next.sprite;
        }
        Debug.Log("Следующая : " +  Enum.GetName(typeof(Tetromino), _next.tetromino));
    }

    private TetrominoData GetTetrominoDataByType(Tetromino type)
    {
        TetrominoData data = tetrominoes.ToList().Find(t => t.tetromino == type);
        return data;
    }

    public void GameOver()
    {
        gameOver.ShowGameOverPanel(true, saveScores.IsWin);
    }
    
    public void PausedGame(bool isPause)
    {
        IsPaused = isPause;
    }
    
    public void Again()
    {
        gameOver.ShowGameOverPanel(false);
        
        Tilemap.ClearAllTiles();
        saveScores.ChangeScore(0);
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, piece.Data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (Tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) {
                LineClear(row);
            } else {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!Tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        GameServicesManager.UnlockAchieve(AchivementServices.FirstLine);
        GameHelper.VibrationStart();
        
        RectInt bounds = Bounds;
        saveScores.ChangeScore(1);

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            Tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = Tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                Tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}