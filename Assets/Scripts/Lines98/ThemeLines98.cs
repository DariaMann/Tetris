using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeLines98: Theme
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image educationBgColor;
    [SerializeField] private List<Button> undoButtons;
    [SerializeField] private List<TextMeshProUGUI> lightText;
    
    [SerializeField] private LineBoard educationLineBoard;
    [SerializeField] private LineBoard lineBoard;
    [SerializeField] private Sprite lightTileSprite;
    [SerializeField] private Sprite darkTileSprite;
    
    [SerializeField] private Image finger;
    [SerializeField] private Sprite lightFingerSprite;
    [SerializeField] private Sprite darkFingerSprite;

    private void Awake()
    {
        GameHelper.GameType = MiniGameType.Lines98;
        GameplayTimeTracker.Instance.RestartTimer();
        InitializeColor();
    }

    public override void SetLight()
    {
        bgColor.backgroundColor = ColorBgLight;
        educationBgColor.color = ColorBgLight;
        foreach (var text in lightText)
        {
            text.color = ColorBgLight;
        }

        finger.sprite = lightFingerSprite;
        foreach (var undoButton in undoButtons)
        {
            undoButton.image.color = ColorBgDark;

            ColorBlock colors = undoButton.colors;

            // Изменяем только альфу отключенного цвета
            Color disabled = colors.disabledColor;
            disabled.a = 0.2f; // нужная альфа, например, 30%
            colors.disabledColor = disabled;

            undoButton.colors = colors;
        }

        foreach (var tile in lineBoard.Tiles)
        {
            tile.SetTheme(lightTileSprite);
        }   
        foreach (var tile in educationLineBoard.Tiles)
        {
            tile.SetTheme(lightTileSprite);
        }
    } 
    
    public override void SetDark()
    {
        bgColor.backgroundColor = ColorBgDark;
        educationBgColor.color = ColorBgDark;
        foreach (var text in lightText)
        {
            text.color = ColorBgDark;
        }
        
        finger.sprite = darkFingerSprite;

        foreach (var undoButton in undoButtons)
        {
            undoButton.image.color = ColorMiddleLight;

            ColorBlock colors = undoButton.colors;

            // Изменяем только альфу отключенного цвета
            Color disabled = colors.disabledColor;
            disabled.a = 0.3f; // нужная альфа, например, 30%
            colors.disabledColor = disabled;

            undoButton.colors = colors;
        }

        foreach (var tile in lineBoard.Tiles)
        {
            tile.SetTheme(darkTileSprite);
        }
        foreach (var tile in educationLineBoard.Tiles)
        {
            tile.SetTheme(darkTileSprite);
        }
    }
}