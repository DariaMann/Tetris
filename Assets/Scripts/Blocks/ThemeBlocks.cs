using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeBlocks: Theme
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Button undoButton;
    [SerializeField] private List<TextMeshProUGUI> lightText;
    
    [SerializeField] private BlocksBoard blocksBoard;
    [SerializeField] private BlocksBoard educationBlocksBoard;
    
    [SerializeField] private Sprite lightBlockTileLightSprite;
    [SerializeField] private Sprite lightBlockTileDarkSprite;
    [SerializeField] private Sprite darkBlockTileLightSprite;
    [SerializeField] private Sprite darkBlockTileDarkSprite;
    
    [SerializeField] private Sprite lightBlockShadowSprite;
    [SerializeField] private Sprite darkBlockShadowSprite;
    
    [SerializeField] private Sprite lightBlockSprite;
    [SerializeField] private Sprite darkBlockSprite;
    
    [SerializeField] private Image eduBgColor;
    [SerializeField] private Image finger;
    [SerializeField] private Sprite lightFingerSprite;
    [SerializeField] private Sprite darkFingerSprite;

    private void Awake()
    {
        GameHelper.GameType = MiniGameType.Blocks;
        InitializeColor();
    }

    public Sprite GetTileSprite(Themes theme)
    {
        switch (theme)
        {
            case Themes.Auto: return GetAutoTileSprite();
            case Themes.Light: return lightBlockSprite;
            case Themes.Night: return darkBlockSprite;
            default: return lightBlockSprite;
        }
    }
    
    public Sprite GetAutoTileSprite()
    {
        bool isDark = ThemeManager.IsSystemDarkTheme();
        if (isDark)
        {
            return darkBlockSprite;
        }
        else
        {
            return lightBlockSprite;
        }
    }

    public override void SetLight()
    {
        bgColor.backgroundColor = ColorBgLight;
        eduBgColor.color = ColorBgLight;
        foreach (var text in lightText)
        {
            text.color = ColorBgLight;
        }
        
        finger.sprite = lightFingerSprite;
        undoButton.image.color = ColorBgDark;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.2f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;

        foreach (var tile in blocksBoard.Tiles)
        {
            tile.SetTheme(lightBlockShadowSprite, lightBlockSprite, lightBlockTileLightSprite, lightBlockTileDarkSprite);
        }
        
        foreach (var tile in blocksBoard.Blocks)
        {
            tile.SetTheme(lightBlockSprite);
        }  
        
        foreach (var tile in educationBlocksBoard.Tiles)
        {
            tile.SetTheme(lightBlockShadowSprite, lightBlockSprite, lightBlockTileLightSprite, lightBlockTileDarkSprite);
        }
        
        foreach (var tile in educationBlocksBoard.Blocks)
        {
            tile.SetTheme(lightBlockSprite);
        }
    } 
    
    public override void SetDark()
    {
        bgColor.backgroundColor = ColorBgDark;
        eduBgColor.color = ColorBgDark;
        foreach (var text in lightText)
        {
            text.color = ColorBgDark;
        }
        
        finger.sprite = darkFingerSprite;
        undoButton.image.color = ColorMiddleLight;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.3f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;
        
        foreach (var tile in blocksBoard.Tiles)
        {
            tile.SetTheme(darkBlockShadowSprite, darkBlockSprite, darkBlockTileLightSprite, darkBlockTileDarkSprite);
        }
        
        foreach (var tile in blocksBoard.Blocks)
        {
            tile.SetTheme(darkBlockSprite);
        }   
        
        foreach (var tile in educationBlocksBoard.Tiles)
        {
            tile.SetTheme(darkBlockShadowSprite, darkBlockSprite, darkBlockTileLightSprite, darkBlockTileDarkSprite);
        }
        
        foreach (var tile in educationBlocksBoard.Blocks)
        {
            tile.SetTheme(darkBlockSprite);
        }
    }
}