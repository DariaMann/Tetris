public class SaveSegment
{
    public bool IsHead { get; set; }
    
    public int X { get; set; }
    
    public int Y { get; set; }
    
    public SaveSegment() {}
    
    public SaveSegment(bool isHead, int x, int y)
    {
        IsHead = isHead;
        X = x;
        Y = y;
    }
    
    public override string ToString()
    {
        return "SegmentData: " + IsHead + " " + X + " " + Y;
    }
}