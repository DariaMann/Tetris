using System.Collections.Generic;

public class SaveDataLines98
{
    public bool ShowFuture { get; set; }
    
    public int Score { get; set; }
    
    public List<SaveBall> SaveBalls { get; set; } = new List<SaveBall>();
    
    public List<SaveBall> SaveFutureBalls { get; set; } = new List<SaveBall>();
    
    public SaveDataLines98() {}

    public SaveDataLines98(bool showFuture, int score, List<Ball> balls, List<Ball> futureBalls)
    {
        ShowFuture = showFuture;
        Score = score;
        foreach (var ball in balls)
        {
            SaveBall saveBall = new SaveBall(ball.IndexSprite, ball.Tile.GridPosition.x, ball.Tile.GridPosition.y);
            SaveBalls.Add(saveBall);
        } 
        foreach (var ball in futureBalls)
        {
            SaveBall saveBall = new SaveBall(ball.IndexSprite, ball.Tile.GridPosition.x, ball.Tile.GridPosition.y);
            SaveFutureBalls.Add(saveBall);
        }
    }
    
    public override string ToString()
    {
        return "SaveDataLines98: " + ShowFuture + " " + Score + " " +
               "SaveBalls[" + string.Join(" ", SaveBalls) + "]" + " " +
               "SaveFutureBalls[" + string.Join(" ", SaveFutureBalls) + "]";
    }
}