using Newtonsoft.Json;

[JsonObject]
public class SaveTile2024
{
    public int StateNumber { get; set; }
    
    public int X { get; set; }
    
    public int Y { get; set; }
    
    public SaveTile2024() {}
    
    public SaveTile2024(int stateNumber, int x, int y)
    {
        StateNumber = stateNumber;
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return "SaveTile2024: " + StateNumber + " " + X + " " + Y;
    }
}