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
        settingsBg.color = ColorBgLight;
        settingsBackBg.color = ColorBgLight;

        foreach (var light in lightImages)
        {
            light.color = ColorBgLight;
        }
        foreach (var light in lightTexts)
        {
            light.color = ColorBgLight;
        }
        
        foreach (var light in middleImages)
        {
            light.color = ColorMiddleDark;
        }
        foreach (var light in middleTexts)
        {
            light.color = ColorMiddleDark;
        }
    } 
    
    public override void SetDark()
    {
        settingsBg.color = ColorBgDark;
        settingsBackBg.color = ColorBgDark;

        foreach (var light in lightImages)
        {
            light.color = ColorBgDark;
        }
        foreach (var light in lightTexts)
        {
            light.color = ColorBgDark;
        }
        
        foreach (var light in middleImages)
        {
            light.color = ColorBgLight;
        }
        foreach (var light in middleTexts)
        {
            light.color = ColorBgLight;
        }
    }
}