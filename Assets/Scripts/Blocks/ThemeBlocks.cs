using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeBlocks: MonoBehaviour
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

    private Themes _theme;
    private Color _colorLight;
    private Color _colorGrey;
    private Color _colorDark;

    private void Awake()
    {
        GameHelper.GameType = MiniGameType.Blocks;
    }

    private void Start()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color6) ? color6 : Color.white;
        _colorGrey = ColorUtility.TryParseHtmlString("#BBADA0", out Color color7) ? color7 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#2C2926", out Color color8) ? color8 : Color.black;
        
        SetTheme(GameHelper.Theme);
        GameHelper.OnThemeChanged += ApplyTheme;
    }
    
    private void ApplyTheme(Themes newTheme)
    {
        Console.WriteLine($"Theme changed to {newTheme.ToString()}");
        // Здесь можно добавить код смены оформления игры
        SetTheme(newTheme);
    }
    
    public void OnDestroy()
    {
        GameHelper.OnThemeChanged -= ApplyTheme;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            SetTheme(Themes.Light);
        }
        if (Input.GetKey(KeyCode.N))
        {
            SetTheme(Themes.Night);
        }
        
        if (Input.GetKey(KeyCode.C))
        {
            GameHelper.AdjustBoardSize(bgColor);
        }
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

    public void SetTheme(Themes theme)
    {
        switch (theme)
        {
            case Themes.Auto: SetAuto(); break;
            case Themes.Light: SetLight(); break;
            case Themes.Night: SetDark(); break;
        }
    }

    public void SetAuto()
    {
        bool isDark = ThemeManager.IsSystemDarkTheme();
        if (isDark)
        {
            SetDark();
        }
        else
        {
            SetLight();
        }
    }
    
    public void SetLight()
    {
        bgColor.backgroundColor = _colorLight;
        eduBgColor.color = _colorLight;
        foreach (var text in lightText)
        {
            text.color = _colorLight;
        }
        
        finger.sprite = lightFingerSprite;
        undoButton.image.color = _colorDark;
        
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
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorDark;
        eduBgColor.color = _colorDark;
        foreach (var text in lightText)
        {
            text.color = _colorDark;
        }
        
        finger.sprite = darkFingerSprite;
        undoButton.image.color = _colorGrey;
        
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