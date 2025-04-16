#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine;

public class ThemeDetectorAndroid
{
    public static bool IsDarkTheme()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject resources = activity.Call<AndroidJavaObject>("getResources");
            AndroidJavaObject configuration = resources.Call<AndroidJavaObject>("getConfiguration");
            int uiMode = configuration.Get<int>("uiMode");

            const int UI_MODE_NIGHT_MASK = 0x30;
            const int UI_MODE_NIGHT_YES = 0x20;

            return (uiMode & UI_MODE_NIGHT_MASK) == UI_MODE_NIGHT_YES;
        }
    }
}
#endif