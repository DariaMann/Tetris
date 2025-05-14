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

    [SerializeField] private Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

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
        if (GameManagerTetris.Instance.GameOverPanel.IsGameOver)
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
}