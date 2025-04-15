using Newtonsoft.Json;

[JsonObject]
public class SnakeSettings
{
    public bool ManyFood { get; set; } = false;
    
    public bool MoveThroughWalls { get; set; } = true;
    
    public bool Acceleration { get; set; } = false;
}