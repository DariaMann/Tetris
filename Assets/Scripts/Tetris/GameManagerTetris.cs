﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-1)]
public class GameManagerTetris : MonoBehaviour
{
    [SerializeField] private EducationTetris education;
    [SerializeField] private Board board;
    [SerializeField] private Board boardEdu;
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Buttons pausePanel;
    [SerializeField] private List<Image> nextTetrominoImage;

    public TetrominoData Next { get; set; }

    public static GameManagerTetris Instance { get; private set; }

    public EducationTetris Education
    {
        get => education;
        set => education = value;
    }

    public SaveScores SaveScores
    {
        get => saveScores;
        set => saveScores = value;
    }
    
    public GameOver GameOverPanel
    {
        get => gameOver;
        set => gameOver = value;
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
        if (!GameHelper.GetEducationState(MiniGameType.Tetris))
        {
            education.ShowEducation(true);
            GameHelper.SetEducationState(MiniGameType.Tetris, true);
        }
        else
        {
            if (GameHelper.SaveTetris.SaveDataTetris == null)
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
        AppodealManager.Instance.OnInterstitialFinished -= ShowGameOverPanel;
    }
//    private void OnDisable()
//    {
//        SaveLastPlay();
//    }

    public void LoadLastPlay()
    {
        SaveDataTetris saveData = GameHelper.SaveTetris.SaveDataTetris;
        if (saveData == null)
        {
            NextRandomTetromino();
            board.SpawnPiece(true);
            return;
        }
        
        saveScores.ChangeScore(saveData.Score);
        saveScores.IsWin = saveData.IsWin;
        LoadTilemap(saveData.SaveTetrominos, board);
        NextRandomTetromino(GetTetrominoDataByType(saveData.NextTetromino));
        board.SpawnPiece(GetTetrominoDataByType(saveData.CurrentTetromino));
        pausePanel.OnPauseClick();
    }
    
    public void LoadTilemap(List<SaveTetramino> saveTetrominos, Board curBoard)
    {
        curBoard.Tilemap.ClearAllTiles();

        foreach (SaveTetramino saved in saveTetrominos)
        {
            Vector3Int pos = new Vector3Int(saved.X, saved.Y, saved.Z);
            TetrominoData tetromino = curBoard.Tetrominoes.ToList().Find(t => t.tetromino == saved.Tetromino);
            curBoard.Tilemap.SetTile(pos, tetromino.tile);
        }
    }

    public void LoadEducation(SaveDataTetris saveData)
    {
        LoadTilemap(saveData.SaveTetrominos, boardEdu);
        boardEdu.SpawnPiece(GetTetrominoDataByType(saveData.CurrentTetromino));
    }
    
    public void ResetAllBoardEducation()
    {
        boardEdu.Tilemap.ClearAllTiles();
    }
    
    public void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.SaveTetris.SaveDataTetris = null;
            MyJsonHelper.SaveTetris(GameHelper.SaveTetris);
            return;
        }

        List<SaveTetramino> tetrominos = new List<SaveTetramino>();
        BoundsInt bounds = board.Tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = board.Tilemap.GetTile(pos);
            if (tile != null)
            {
                bool isCurrentTetramino = false;
                for (int i = 0; i < board.ActivePiece.Data.cells.Length; i++)
                {
                    Vector3Int globalCellPos = board.ActivePiece.Position + board.ActivePiece.Cells[i];
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

                TetrominoData tetromino = board.Tetrominoes.ToList().Find(t => t.tile == tile);
            
                tetrominos.Add(new SaveTetramino
                {
                    Tetromino = tetromino.tetromino,
                    X = pos.x,
                    Y = pos.y,
                    Z = pos.z
                });
            }
        }
        
        SaveDataTetris data = new SaveDataTetris(saveScores.IsWin, saveScores.CurrentScore, board.ActivePiece.Data.tetromino, Next.tetromino, tetrominos);
        GameHelper.SaveTetris.SaveDataTetris = data;
        MyJsonHelper.SaveTetris(GameHelper.SaveTetris);
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
        
        board.Tilemap.ClearAllTiles();
        saveScores.ChangeScore(0);
    }
    
    public void NextRandomTetromino()
    {
        int random = Random.Range(0, board.Tetrominoes.Length);
        Next = board.Tetrominoes[random];
        foreach (var next in nextTetrominoImage)
        {
            next.sprite = Next.sprite;
        }
        Debug.Log("Следующая : " +  Enum.GetName(typeof(Tetromino), Next.tetromino));
    }
    
    public void NextRandomTetromino(TetrominoData data)
    {
        Next = data;
        foreach (var next in nextTetrominoImage)
        {
            next.sprite = Next.sprite;
        }
        Debug.Log("Следующая : " +  Enum.GetName(typeof(Tetromino), Next.tetromino));
    }

    private TetrominoData GetTetrominoDataByType(Tetromino type)
    {
        TetrominoData data = board.Tetrominoes.ToList().Find(t => t.tetromino == type);
        return data;
    }
}