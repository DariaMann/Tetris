﻿using Newtonsoft.Json;

[JsonObject]
public class SnakeSettings
{
    public bool ManyFood { get; set; } = true;
    
    public bool MoveThroughWalls { get; set; } = true;
    
    public bool Acceleration { get; set; } = false;

    public float Speed { get; set; } = 6;
}