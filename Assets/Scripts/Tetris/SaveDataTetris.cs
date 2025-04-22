using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject]
public class SaveDataTetris
{
    public bool IsWin { get; set; }
    
    public int Score { get; set; }
    
    public Tetromino CurrentTetromino { get; set; }
    
    public Tetromino NextTetromino { get; set; }
    
    public List<SaveTetramino> SaveTetrominos { get; set; } = new List<SaveTetramino>();
    
    public SaveDataTetris() {}

    public SaveDataTetris(bool isWin, int score, Tetromino currentTetromino, Tetromino nextTetromino, List<SaveTetramino> tetrominos)
    {
        IsWin = isWin;
        Score = score;
        CurrentTetromino = currentTetromino;
        NextTetromino = nextTetromino;
        SaveTetrominos = tetrominos;
    }
    
    public override string ToString()
    {
        return "SaveDataTetris: " + IsWin + " " + Score + " " + CurrentTetromino + " " + NextTetromino + " " +
               "SaveTetrominos[" + string.Join(" ", SaveTetrominos) + "]";
    }
}