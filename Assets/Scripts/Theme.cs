using UnityEngine;

public class Theme : MonoBehaviour
{
    public Color ColorBgLight { get; set; }
    
    public Color ColorBgDark { get; set; }
    
    public Color ColorMiddleLight { get; set; }
    
    public Color ColorMiddleDark { get; set; }

    protected void Start()
    {
        GameHelper.OnThemeChanged += ApplyTheme;
        GameHelper.GetTheme();
        SetTheme(GameHelper.Theme);
    }
    
    public void OnDestroy()
    {
        GameHelper.OnThemeChanged -= ApplyTheme;
    }

    public void InitializeColor()
    {
        ColorBgLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color colorBgLight) ? colorBgLight : Color.white;
        ColorBgDark = ColorUtility.TryParseHtmlString("#2C2926", out Color colorBgDark) ? colorBgDark : Color.black;
        ColorMiddleLight = ColorUtility.TryParseHtmlString("#BBADA0", out Color colorMiddleLight) ? colorMiddleLight : Color.gray;
        ColorMiddleDark = ColorUtility.TryParseHtmlString("#897B6D", out Color colorMiddleDark) ? colorMiddleDark : Color.black;
    }
    
    public void ApplyTheme(Themes newTheme)
    {
        Debug.Log($"Theme changed to {newTheme.ToString()}");
        SetTheme(newTheme);
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
    
    public virtual void SetLight(){}
    public virtual void SetDark(){}
}