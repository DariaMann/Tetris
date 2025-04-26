using Newtonsoft.Json;

[JsonObject]
public class SaveBlocks
{
    public int Record { get; set; }

    public SaveDataBlocks SaveDataBlocks { get; set; }
    
    public SaveBlocks() {}

    public SaveBlocks(int record, SaveDataBlocks saveDataBlocks)
    {
        Record = record;
        SaveDataBlocks = saveDataBlocks;
    }

    public override string ToString()
    {
        return "SaveBlocks: " + Record + " " + SaveDataBlocks;
    }
}