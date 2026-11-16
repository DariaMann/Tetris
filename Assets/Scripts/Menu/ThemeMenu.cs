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
    [SerializeField] private List<TextMeshProUGUI> textsStartDarkBg;
    
    private void Awake()
    {
        GameHelper.GameType = MiniGameType.None;
        GameHelper.IsRevived = false;
        InitializeColor();
    }

    public override void SetLight()
    {
        if (bgColor != null) bgColor.backgroundColor = ColorBgLight;
        SetImageColors(buttonsBg, ColorMiddleDark);
        SetImageColors(buttonsIcons, ColorBgLight);
        SetTextColors(textsBg, ColorBgLight);
        SetTextColors(textsStartDarkBg, ColorMiddleDark);
    } 
    
    public override void SetDark()
    {
        if (bgColor != null) bgColor.backgroundColor = ColorBgDark;
        SetImageColors(buttonsBg, ColorBgLight);
        SetImageColors(buttonsIcons, ColorBgDark);
        SetTextColors(textsBg, ColorBgDark);
        SetTextColors(textsStartDarkBg, ColorBgLight);
    }

    private static void SetImageColors(List<Image> images, Color color)
    {
        if (images == null) return;
        foreach (var image in images)
            if (image != null) image.color = color;
    }

    private static void SetTextColors(List<TextMeshProUGUI> texts, Color color)
    {
        if (texts == null) return;
        foreach (var text in texts)
            if (text != null) text.color = color;
    }
}
