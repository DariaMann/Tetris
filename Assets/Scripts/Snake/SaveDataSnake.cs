using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject]
public class SaveDataSnake
{
    public bool IsWin { get; set; }
    
    public int Score { get; set; }

    public int HeadX { get; set; }
    
    public int HeadY { get; set; }

    public int DirectionX { get; set; }
    
    public int DirectionY { get; set; }
    
    public List<SaveFood> SaveFoods { get; set; } = new List<SaveFood>();
    
    public SaveDataSnake() {}
    
    public SaveDataSnake(bool isWin, Vector3 headPos, List<Food> foods, Vector2Int direction, int score)
    {
        IsWin = isWin;
        Score = score;
        HeadX = Mathf.RoundToInt(headPos.x);
        HeadY = Mathf.RoundToInt(headPos.y); 
        DirectionX = Mathf.RoundToInt(direction.x);
        DirectionY = Mathf.RoundToInt(direction.y);
        foreach (var food in foods)
        {
            SaveFoods.Add(new SaveFood(food.Position));
        }
    }
    
    public override string ToString()
    {
        return "SaveDataSnake: " + IsWin + " " + Score + " " + HeadX + " " + HeadY + " " + DirectionX + " " + DirectionY + " " +
               "SaveFoods[" + string.Join(" ", SaveFoods) + "]";
    }
}