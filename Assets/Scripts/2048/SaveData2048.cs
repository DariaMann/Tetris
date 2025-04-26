using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject]
public class SaveData2048
{
    public bool IsWin { get; set; }
    
    public int Score { get; set; }

    public List<SaveTile2024> SaveTiles { get; set; } = new List<SaveTile2024>();
    
    public SaveData2048() {}

    public SaveData2048(bool isWin, int score, List<Tile2024> tiles)
    {
        IsWin = isWin;
        Score = score;
        foreach (var tile in tiles)
        {
            SaveTile2024 saveTile = new SaveTile2024(tile.State.index, tile.Cell.Coordinates.x, tile.Cell.Coordinates.y);
            SaveTiles.Add(saveTile);
        }
    }
    
    public override string ToString()
    {
        return "SaveData2048: " + IsWin + " " + Score + " " +
               "SaveTiles[" + string.Join(" ", SaveTiles) + "]";
    }
}