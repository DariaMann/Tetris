using UnityEngine;

public abstract class GameManager : MonoBehaviour
{
    public abstract void LoadLastPlay();
//    public abstract void LoadEducation();
    public abstract void SaveLastPlay();
    public abstract void ResetAllBoardEducation();
    public abstract void GameOver();
    public abstract void Again();
}