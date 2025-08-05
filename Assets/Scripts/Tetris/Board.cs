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
    [SerializeField] private bool isEducation;

    [SerializeField] private TetrominoData[] tetrominoes;
    
    [SerializeField] private Vector2Int boardSize = new Vector2Int(10, 20);

    private Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    public TetrominoData[] Tetrominoes
    {
        get => tetrominoes;
        set => tetrominoes = value;
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
    
    private void OnDisable()
    {
        if (isEducation)
        {
            return;
        }
        GameManagerTetris.Instance.SaveLastPlay();
    }

    public void SpawnPiece(bool first = false)
    {
        if (GameHelper.IsGameOver)
        {
            return;
        }
        TetrominoData data = GameManagerTetris.Instance.Next;
        if (first)
        {
            int random = Random.Range(0, tetrominoes.Length);
            data = tetrominoes[random];
        }

        ActivePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(ActivePiece, spawnPosition)) {
            Set(ActivePiece);
        } else {
            GameManagerTetris.Instance.GameOver();
        }
    }
    
    public void SpawnPiece(TetrominoData data)
    {
        ActivePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(ActivePiece, spawnPosition)) {
            Set(ActivePiece);
        } else {
            GameManagerTetris.Instance.GameOver();
        }
    }

    public void Set(Piece piece)
    {
        if (GameHelper.IsEdication && GameManagerTetris.Instance.Education.EducationIsOver)
        {
            ClearAll();
            return;
        }
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
    
    private void ClearAll()
    {
        Tilemap.ClearAllTiles();
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

    public void ClearHalf()
    {
        RectInt bounds = Bounds;
        int yMin = bounds.yMin;
        int yMax = bounds.yMax;
        int middleRow = (yMin + yMax) / 2;
        int height = yMax - yMin;

        int rowsToClear = middleRow - yMin;

        // Шаг 1: очистить нижнюю половину без смещения
        for (int y = yMin; y < middleRow; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                Tilemap.SetTile(pos, null);
            }
        }

        // Шаг 2: сдвинуть оставшиеся строки вниз на количество очищенных строк
        for (int y = middleRow; y < yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int from = new Vector3Int(x, y, 0);
                Vector3Int to = new Vector3Int(x, y - rowsToClear, 0);
                TileBase tile = Tilemap.GetTile(from);
                Tilemap.SetTile(to, tile);
            }
        }

        // Шаг 3: очистить верхние строки, которые "спустились"
        for (int y = yMax - rowsToClear; y < yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                Tilemap.SetTile(pos, null);
            }
        }
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        bool clearLines = false;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row))
            {
                clearLines = true;
                LineClear(row);
            } else {
                row++;
            }
        }

        if (clearLines)
        {
            AudioManager.Instance.PlaySuccessLineSound();
            
            if (isEducation && GameHelper.IsEdication)
            {
                GameManagerTetris.Instance.Education.ChangeStep();
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
        if (!isEducation)
        {
            GameServicesManager.UnlockAchieve(AchivementServices.FirstLine);
            GameManagerTetris.Instance.SaveScores.ChangeScore(1);
        }
        GameHelper.VibrationStart();

        RectInt bounds = Bounds;

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
    
    private void ClearRowAndShiftDown(int row)
    {
        RectInt bounds = Bounds;

        // Удалить все тайлы в строке
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            Tilemap.SetTile(position, null);
        }

        // Сдвинуть все строки выше на одну вниз
        for (int y = row + 1; y < bounds.yMax; y++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int from = new Vector3Int(col, y, 0);
                Vector3Int to = new Vector3Int(col, y - 1, 0);
                TileBase tile = Tilemap.GetTile(from);
                Tilemap.SetTile(to, tile);
            }
        }

        // Очистить верхнюю строку (которая сдвинулась вниз)
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, bounds.yMax - 1, 0);
            Tilemap.SetTile(position, null);
        }
    }

}