﻿using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject]
public class SaveData2048
{
    public int Score { get; set; }

    public int Maximum { get; set; } = 2;
    
    public List<SaveTile2024> SaveTiles { get; set; } = new List<SaveTile2024>();
    
    public SaveData2048() {}

    public SaveData2048(int score, int maximum, List<Tile2024> tiles)
    {
        Score = score;
        if (maximum > Maximum)
        {
            Maximum = maximum;
        }
        foreach (var tile in tiles)
        {
            SaveTile2024 saveTile = new SaveTile2024(tile.State.index, tile.Cell.Coordinates.x, tile.Cell.Coordinates.y);
            SaveTiles.Add(saveTile);
        }
    }
    
    public override string ToString()
    {
        return "SaveData2048: " + Score + " " + Maximum + " " +
               "SaveTiles[" + string.Join(" ", SaveTiles) + "]";
    }
}