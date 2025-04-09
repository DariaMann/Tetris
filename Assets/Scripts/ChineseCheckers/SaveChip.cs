using Newtonsoft.Json;

[JsonObject]
public class SaveChip
{
    public int IdPlayer { get; set; }
    
    public int Row { get; set; }
    
    public int Col { get; set; }
    
    public SaveChip() {}
    
    public SaveChip(int idPlayer, int row, int col)
    {
        IdPlayer = idPlayer;
        Row = row;
        Col = col;
    }

    public override string ToString()
    {
        return "SaveChip: " + IdPlayer + " " + Row + " " + Col;
    }
}