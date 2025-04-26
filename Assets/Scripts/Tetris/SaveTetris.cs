using Newtonsoft.Json;

[JsonObject]
public class SaveTetris
{
    public int Record { get; set; }

    public SaveDataTetris SaveDataTetris { get; set; }
    
    public SaveTetris() {}

    public SaveTetris(int record, SaveDataTetris saveDataTetris)
    {
        Record = record;
        SaveDataTetris = saveDataTetris;
    }

    public override string ToString()
    {
        return "SaveTetris: " + Record + " " + SaveDataTetris;
    }
}