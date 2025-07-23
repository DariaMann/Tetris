using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject]
public class SaveDataTetris
{
    public bool IsWin { get; set; }
    
    public bool IsRevived { get; set; }
    
    public int Score { get; set; }
    
    public Tetromino CurrentTetromino { get; set; }
    
    public Tetromino NextTetromino { get; set; }
    
    public List<SaveTetramino> SaveTetrominos { get; set; } = new List<SaveTetramino>();
    
    public SaveDataTetris() {}

    public SaveDataTetris(bool isWin, bool isRevived, int score, Tetromino currentTetromino, Tetromino nextTetromino, List<SaveTetramino> tetrominos)
    {
        IsWin = isWin;
        IsRevived = isRevived;
        Score = score;
        CurrentTetromino = currentTetromino;
        NextTetromino = nextTetromino;
        SaveTetrominos = tetrominos;
    }
    
    public override string ToString()
    {
        return "SaveDataTetris: " + IsWin + " " + IsRevived + " " + Score + " " + CurrentTetromino + " " + NextTetromino + " " +
               "SaveTetrominos[" + string.Join(" ", SaveTetrominos) + "]";
    }
}