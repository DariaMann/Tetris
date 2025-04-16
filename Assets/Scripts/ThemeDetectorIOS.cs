#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;

public class ThemeDetectorIOS
{
    [DllImport("__Internal")]
    private static extern bool _IsDarkTheme();

    public static bool IsDarkTheme()
    {
        return _IsDarkTheme();
    }
}
#endif