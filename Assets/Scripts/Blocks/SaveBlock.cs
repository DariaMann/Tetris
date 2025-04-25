public class SaveBlock
{
    public BlockShape BlockShape { get; set; }
    
    public bool IsEnable { get; set; }
    
    public SaveBlock() {}
    
    public SaveBlock(BlockShape blockShape, bool isEnable)
    {
        BlockShape = blockShape;
        IsEnable = isEnable;
    }

    public override string ToString()
    {
        return "SaveBlock: " + BlockShape + " " + IsEnable;
    }
}