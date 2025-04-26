using System.Collections.Generic;

public class SaveDataBlocks
{
    public bool IsWin { get; set; }
    
    public int Score { get; set; }
    
    public List<SaveBlocksTile> SaveBlocksTile { get; set; } = new List<SaveBlocksTile>();
    
    public List<SaveBlock> Blocks { get; set; } = new List<SaveBlock>();

    public SaveDataBlocks() {}

    public SaveDataBlocks(bool isWin, int score, List<BlockTile> blockTiles, List<Block> blocks)
    {
        IsWin = isWin;
        Score = score;
        foreach (var blockTile in blockTiles)
        {
            bool isFull = blockTile.IsOccupied;
            SaveBlocksTile saveBlock = new SaveBlocksTile(blockTile.GridPosition.x, blockTile.GridPosition.y, isFull);
            SaveBlocksTile.Add(saveBlock);
        }
        
        foreach (var block in blocks)
        {
            bool isEnabled = block.IsActive;
            SaveBlock saveBlock = new SaveBlock(block.BlockShape, isEnabled);
            Blocks.Add(saveBlock);
        }
    }
    
    public override string ToString()
    {
        return "SaveDataBlocks: " + IsWin + " " + Score + " " +
               "SaveBlocksTile[" + string.Join(" ", SaveBlocksTile) + "]" + " " +
               "Blocks[" + string.Join(" ", Blocks) + "]";
    }
}