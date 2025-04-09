using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject]
public class SaveTetramino
{
    public Tetromino Tetromino { get; set; }
    
    public int X { get; set; }
    
    public int Y { get; set; }
    
    public int Z { get; set; }
    
}