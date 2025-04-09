using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject]
public class SaveDataChineseCheckers
{

    public int IdPlayingPlayer { get; set; }
    
    public int Steps { get; set; }
    
    public List<SavePlayer> SavePlayers { get; set; } = new List<SavePlayer>();
    
    public List<SaveChip> SaveChips { get; set; } = new List<SaveChip>();
    
    public SaveDataChineseCheckers() {}
    
    public SaveDataChineseCheckers(int idPlayingPlayer, int steps, List<Player> players, List<Chip> chips)
    {
        IdPlayingPlayer = idPlayingPlayer;
        Steps = steps;
        foreach (var player in players)
        {
            SavePlayer savePlayer = new SavePlayer(player.ID, player.State);
            
            SavePlayers.Add(savePlayer);
        }   
        
        foreach (var chip in chips)
        {
            SaveChip saveChip = new SaveChip(chip.Player.ID, chip.Tile.Row, chip.Tile.Col);
            
            SaveChips.Add(saveChip);
        }
    }
    
    public override string ToString()
    {
        return "LevelStructure: " + IdPlayingPlayer + " " + Steps + " " +
               "SavePlayers[" + string.Join(" ", SavePlayers) + "]" + " " +
               "SaveChips[" + string.Join(" ", SaveChips) + "]";
    }
}