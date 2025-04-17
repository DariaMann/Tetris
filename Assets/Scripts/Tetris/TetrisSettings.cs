using Newtonsoft.Json;

[JsonObject]
public class TetrisSettings
{
    public bool Acceleration { get; set; } = false;

    public float Speed { get; set; } = 1;
}