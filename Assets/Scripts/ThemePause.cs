using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemePause: Theme
{
    [SerializeField] private List<Image> bgButtons;

    private void Awake()
    {
        InitializeColor();
    }

    public override void SetLight()
    {
        foreach (var button in bgButtons)
        {
            button.color = ColorMiddleLight;
        }
    } 
    
    public override void SetDark()
    {
        foreach (var button in bgButtons)
        {
            button.color = ColorMiddleDark;
        }
    }
}