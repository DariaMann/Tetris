using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeSettings : Theme
{
    [SerializeField] private Image settingsBg;
    [SerializeField] private Image settingsBackBg;
    
    [SerializeField] private List<Image> lightImages = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> lightTexts = new List<TextMeshProUGUI>();
    
    [SerializeField] private List<Image> middleImages = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> middleTexts = new List<TextMeshProUGUI>();

    private void Awake()
    {
        InitializeColor();
    }

    public override void SetLight()
    {
        if (settingsBg != null) settingsBg.color = ColorBgLight;
        if (settingsBackBg != null) settingsBackBg.color = ColorBgLight;
        SetImageColors(lightImages, ColorBgLight);
        SetTextColors(lightTexts, ColorBgLight);
        SetImageColors(middleImages, ColorMiddleDark);
        SetTextColors(middleTexts, ColorMiddleDark);
    } 
    
    public override void SetDark()
    {
        if (settingsBg != null) settingsBg.color = ColorBgDark;
        if (settingsBackBg != null) settingsBackBg.color = ColorBgDark;
        SetImageColors(lightImages, ColorBgDark);
        SetTextColors(lightTexts, ColorBgDark);
        SetImageColors(middleImages, ColorBgLight);
        SetTextColors(middleTexts, ColorBgLight);
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
