using Newtonsoft.Json;

[JsonObject]
public class SavePlayer
{
    public int Id { get; set; }

    public PlayerState State { get; set; }
    
    public SavePlayer() {}
    
    public SavePlayer(int id, PlayerState state)
    {
        Id = id;
        State = state;
    }
    
    public override string ToString()
    {
        return "SavePlayer: " + Id + " " + State;
    }
}