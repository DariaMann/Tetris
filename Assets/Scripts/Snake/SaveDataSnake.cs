using Newtonsoft.Json;
using UnityEngine;

[JsonObject]
public class SaveDataSnake
{
    public int Score { get; set; }

    public int HeadX { get; set; }
    
    public int HeadY { get; set; }

    public int FoodX { get; set; }
    
    public int FoodY { get; set; }

    public int DirectionX { get; set; }
    
    public int DirectionY { get; set; }
    
    public SaveDataSnake() {}
    
    public SaveDataSnake(Vector3 headPos, Vector2Int foodPos, Vector2Int direction, int score)
    {
        Score = score;
        HeadX = Mathf.RoundToInt(headPos.x);
        HeadY = Mathf.RoundToInt(headPos.y);
        FoodX = Mathf.RoundToInt(foodPos.x);
        FoodY = Mathf.RoundToInt(foodPos.y); 
        DirectionX = Mathf.RoundToInt(direction.x);
        DirectionY = Mathf.RoundToInt(direction.y);
    }
    
    public override string ToString()
    {
        return "SaveDataSnake: " + Score + " " + HeadX + " " + HeadY + " " + FoodX + " " + FoodY + " " + DirectionX + " " + DirectionY;
    }
}