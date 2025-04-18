public class SaveBall
{
    public int Index { get; set; }
    
    public int X { get; set; }
    
    public int Y { get; set; }
    
    public SaveBall() {}

    public SaveBall(int index, int x, int y)
    {
        Index = index;
        X = x;
        Y = y;
    }
    
    public override string ToString()
    {
        return "SaveBall: " + Index + " " + X + " " + Y;
    }
}