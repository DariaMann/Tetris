public class SaveBlocksTile
{
    public int X { get; set; }
    
    public int Y { get; set; }
    
    public bool IsFull { get; set; }
    
    public SaveBlocksTile() {}

    public SaveBlocksTile(int x, int y, bool isFull)
    {
        X = x;
        Y = y;
        IsFull = isFull;
    }

    public override string ToString()
    {
        return "SaveBlocksTile: " + X + " " + Y + " " + IsFull;
    }
}