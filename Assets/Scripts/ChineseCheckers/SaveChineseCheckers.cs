using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject]
public class SaveChineseCheckers
{
    public int Record { get; set; } = 1000;

    public bool ShowHint { get; set; } = true;
    
    public int BlueColorIndex { get; set; }
    
    public int CountGames { get; set; }
    
    public List<PlayerState> PlayerStates { get; set; }

    public SaveDataChineseCheckers SaveDataChineseCheckers { get; set; }
    
    public SaveChineseCheckers() {}

    public SaveChineseCheckers(int record, SaveDataChineseCheckers saveDataChineseCheckers, List<PlayerState> playerStates)
    {
        Record = record;
        SaveDataChineseCheckers = saveDataChineseCheckers;
        PlayerStates = playerStates;
    }

    public override string ToString()
    {
        return "SaveChineseCheckers: " + Record + " " + ShowHint + " " + BlueColorIndex + " " + CountGames + " " + SaveDataChineseCheckers;
    }
}