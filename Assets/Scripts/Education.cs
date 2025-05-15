using UnityEngine;

public abstract class Education : MonoBehaviour
{
    public abstract void ShowEducation(bool isFirstEducation);
    public abstract void ShowEducation();
    public abstract void ShowView(bool isFirstEducation);
//    public abstract void HideFinishEducation();
//    public abstract void RepeatEducation();
    
//    public abstract void HideEducation();
    public abstract void Restart();
    
//    public abstract void StartPlay();
    public abstract void StopTutorial();
}