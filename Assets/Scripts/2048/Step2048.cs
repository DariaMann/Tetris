using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[JsonObject]
public class Step2048
{
    public int Steps { get; set; }
    
    public List<TileEvent> Tiles { get; set; } = new List<TileEvent>();
}