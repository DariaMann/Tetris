using Newtonsoft.Json;

[JsonObject]
public class SaveTetramino
{
    public Tetromino Tetromino { get; set; }
    
    public int X { get; set; }
    
    public int Y { get; set; }
    
    public int Z { get; set; }
    
    public SaveTetramino(){}
    
    public SaveTetramino(Tetromino tetromino, int x, int y, int z)
    {
        Tetromino = tetromino;
        X = x;
        Y = y;
        Z = z;
    }
    
}