public static class ThemeManager
{
    public static bool IsSystemDarkTheme()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return ThemeDetectorAndroid.IsDarkTheme();
#elif UNITY_IOS && !UNITY_EDITOR
        return ThemeDetectorIOS.IsDarkTheme();
#else
        // В редакторе и других платформах
        return IsNightTime();
#endif
    }
    
    private static bool IsNightTime()
    {
        int hour = System.DateTime.Now.Hour;
        return hour < 7 || hour >= 19; // Считаем ночью с 19:00 до 07:00
    }
    
    private static bool a()
    {
        int hour = System.DateTime.Now.Hour;
        return hour < 7 || hour >= 19; // Считаем ночью с 19:00 до 07:00
    }
}