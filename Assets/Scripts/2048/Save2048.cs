using Newtonsoft.Json;

[JsonObject]
public class Save2048
{
    public int Record { get; set; }
    
    public int Maximum { get; set; }
    
    public SaveData2048 SaveData2048 { get; set; }
    
    public Save2048() {}

    public Save2048(int record, int maximum, SaveData2048 saveData2048)
    {
        Record = record;
        Maximum = maximum;
        SaveData2048 = saveData2048;
    }

    public override string ToString()
    {
        return "SaveData2048: " + Record + " " + Maximum + " " + SaveData2048;
    }
    
}