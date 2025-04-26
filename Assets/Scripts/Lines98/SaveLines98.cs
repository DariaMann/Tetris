using Newtonsoft.Json;

[JsonObject]
public class SaveLines98
{
    public int Record { get; set; }

    public SaveDataLines98 SaveDataLines98 { get; set; }
    
    public SaveLines98() {}

    public SaveLines98(int record, SaveDataLines98 saveDataLines98)
    {
        Record = record;
        SaveDataLines98 = saveDataLines98;
    }

    public override string ToString()
    {
        return "SaveLines98: " + Record + " " + SaveDataLines98;
    }
}