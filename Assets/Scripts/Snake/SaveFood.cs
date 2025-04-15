using UnityEngine;

public class SaveFood
{
    public int X { get; set; }
    
    public int Y { get; set; }
    
    public SaveFood() {}
    
    public SaveFood(Vector2Int position)
    {
        X = position.x;
        Y = position.y;
    }
    
    public override string ToString()
    {
        return "SaveFood: " + X + " " + Y;
    }
}