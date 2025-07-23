using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeMenu : Theme
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private List<Image> buttonsBg;
    [SerializeField] private List<Image> buttonsIcons;
    [SerializeField] private List<TextMeshProUGUI> textsBg;
    
    private void Awake()
    {
        GameHelper.GameType = MiniGameType.None;
        GameHelper.IsRevived = false;
        InitializeColor();
    }

    public override void SetLight()
    {
        bgColor.backgroundColor = ColorBgLight;
        foreach (var button in buttonsBg)
        {
            button.color = ColorMiddleDark;
        }
        foreach (var button in buttonsIcons)
        {
            button.color = ColorBgLight;
        }
        foreach (var text in textsBg)
        {
            text.color = ColorBgLight;
        }
    } 
    
    public override void SetDark()
    {
        bgColor.backgroundColor = ColorBgDark;
        foreach (var button in buttonsBg)
        {
            button.color = ColorBgLight;
        }
        foreach (var button in buttonsIcons)
        {
            button.color = ColorBgDark;
        }
        foreach (var text in textsBg)
        {
            text.color = ColorBgDark;
        }
    }
}