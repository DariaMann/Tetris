using Newtonsoft.Json;

[JsonObject]
public class SaveSnake
{
    public int Record { get; set; }

    public SaveDataSnake SaveDataSnake { get; set; }
    
    public SaveSnake() {}

    public SaveSnake(int record, SaveDataSnake saveDataSnake)
    {
        Record = record;
        SaveDataSnake = saveDataSnake;
    }

    public override string ToString()
    {
        return "SaveSnake: " + Record + " " + SaveDataSnake;
    }
}