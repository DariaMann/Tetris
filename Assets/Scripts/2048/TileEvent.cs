using UnityEngine;
using Newtonsoft.Json;

[JsonObject]
public class TileEvent
{
    public int X { get; set; }
    public int Y { get; set; }
    public int StateIndex { get; set; }
}