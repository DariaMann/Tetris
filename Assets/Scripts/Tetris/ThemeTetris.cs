using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeTetris : Theme
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image nextFigureBg;
    [SerializeField] private Image countLinesBg;
    [SerializeField] private Image recordBg;
    [SerializeField] private Image buttonDownLeft;
    [SerializeField] private Image buttonDownUp;
    [SerializeField] private SpriteRenderer gridTile;
    [SerializeField] private Image backButton;
    [SerializeField] private Ghost ghost;
    
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private TextMeshProUGUI scoreNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI recordNameText;
    [SerializeField] private TextMeshProUGUI recordText;
    
    [SerializeField] private Image nextFigureBgRight;
    [SerializeField] private Image countLinesBgRight;
    [SerializeField] private Image recordBgRight;
    [SerializeField] private Image buttonDownRight;
    [SerializeField] private TextMeshProUGUI nextTextRight;
    [SerializeField] private TextMeshProUGUI scoreNameTextRight;
    [SerializeField] private TextMeshProUGUI scoreTextRight;
    [SerializeField] private TextMeshProUGUI recordNameTextRight;
    [SerializeField] private TextMeshProUGUI recordTextRight;
    
    [SerializeField] private TextMeshProUGUI scoreAndRecordTextTop;
    
    [SerializeField] private List<TextMeshProUGUI> darkTexts;
    [SerializeField] private List<Image> lightImages;
    [SerializeField] private Image finger;
    [SerializeField] private Sprite lightFingerSprite;
    [SerializeField] private Sprite darkFingerSprite;
    
    private Color _colorLight;
    private Color _colorGrey;
    private Color _colorDark;
    
    private Color _colorImageLight;
    private Color _colorBgDark;
    
    private Color _colorGrid;
    
    private void Awake()
    {
        GameHelper.GameType = MiniGameType.Tetris;
        InitializeColor();
        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white;
        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color2) ? color2 : Color.black;
        
        _colorImageLight = ColorUtility.TryParseHtmlString("#CDC1B4", out Color color8) ? color8 : Color.black;
        
        _colorBgDark = ColorUtility.TryParseHtmlString("#9A8C7F", out Color colorBgDark) ? colorBgDark : Color.black;
        
        _colorGrid = ColorUtility.TryParseHtmlString("#D1CBC4", out Color colorWallDark) ? colorWallDark : Color.black;
    }

    public override void SetLight()
    {
        bgColor.backgroundColor = ColorBgLight;
        backButton.color = _colorImageLight;
        nextFigureBg.color = _colorImageLight;
        countLinesBg.color = _colorImageLight;
        recordBg.color = _colorImageLight;
        buttonDownUp.color = _colorImageLight;
        buttonDownLeft.color = _colorImageLight;
        buttonDownRight.color = _colorImageLight;
        gridTile.color = _colorGrid;
        ghost.Tile.color = ColorBgDark;
        
        nextText.color = ColorBgLight;
        scoreNameText.color = ColorBgLight;
        scoreText.color = ColorBgLight;
        recordNameText.color = ColorBgLight;
        recordText.color = ColorBgLight;
        
        nextFigureBgRight.color = _colorImageLight;
        countLinesBgRight.color = _colorImageLight;
        recordBgRight.color = _colorImageLight;
        nextTextRight.color = ColorBgLight;
        scoreNameTextRight.color = ColorBgLight;
        scoreTextRight.color = ColorBgLight;
        recordNameTextRight.color = ColorBgLight;
        recordTextRight.color = ColorBgLight;
        
        scoreAndRecordTextTop.color = _colorImageLight;
        
        finger.sprite = lightFingerSprite;
        
        foreach (var text in darkTexts)
        {
            text.color = ColorBgLight;
        }
        
        foreach (var img in lightImages)
        {
            img.color = _colorBgDark;
        }
    } 
    
    public override void SetDark()
    {
        bgColor.backgroundColor = _colorDark;
        backButton.color = _colorLight;
        nextFigureBg.color = _colorGrey;
        countLinesBg.color = _colorGrey;
        buttonDownUp.color = _colorGrey;
        buttonDownLeft.color = _colorGrey;
        buttonDownRight.color = _colorGrey;
        recordBg.color = _colorGrey;
        gridTile.color = _colorGrey;
        ghost.Tile.color = _colorLight;
        
        nextText.color = _colorLight;
        scoreNameText.color = _colorLight;
        scoreText.color = _colorLight;
        recordNameText.color = _colorLight;
        recordText.color = _colorLight;
        
        nextFigureBgRight.color = _colorGrey;
        countLinesBgRight.color = _colorGrey;
        recordBgRight.color = _colorGrey;
        nextTextRight.color = _colorLight;
        scoreNameTextRight.color = _colorLight;
        scoreTextRight.color = _colorLight;
        recordNameTextRight.color = _colorLight;
        recordTextRight.color = _colorLight;
        
        scoreAndRecordTextTop.color = _colorLight;
        
        finger.sprite = darkFingerSprite;
        
        foreach (var text in darkTexts)
        {
            text.color = ColorBgDark;
        }
        
        foreach (var img in lightImages)
        {
            img.color = ColorBgLight;
        }
    }
}