using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject]
public class SaveDataSnake
{
    public bool IsWin { get; set; }
    
    public int Score { get; set; }

    public int DirectionX { get; set; }
    
    public int DirectionY { get; set; }
    
    public List<SaveSegment> SaveSegments { get; set; } = new List<SaveSegment>();
    
    public List<SaveFood> SaveFoods { get; set; } = new List<SaveFood>();
    
    public SaveDataSnake() {}
    
    public SaveDataSnake(bool isWin, List<Food> foods, List<Segment> segments, Vector2Int direction, int score)
    {
        IsWin = isWin;
        Score = score;
        DirectionX = Mathf.RoundToInt(direction.x);
        DirectionY = Mathf.RoundToInt(direction.y);
        for (int i = 0; i < segments.Count; i++)
        {
            SaveSegments.Add(new SaveSegment(i == 0, segments[i].CurrentCell.x, segments[i].CurrentCell.y));
        }
        foreach (var food in foods)
        {
            SaveFoods.Add(new SaveFood(food.Position));
        }
    }
    
    public override string ToString()
    {
        return "SaveDataSnake: " + IsWin + " " + Score + " " + DirectionX + " " + DirectionY + " " +
               "SaveFoods[" + string.Join(" ", SaveFoods) + "]";
    }
}