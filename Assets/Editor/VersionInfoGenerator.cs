using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class VersionInfoGenerator : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string version = PlayerSettings.bundleVersion;
        string build = "0";

#if UNITY_ANDROID
        build = PlayerSettings.Android.bundleVersionCode.ToString();
#elif UNITY_IOS
        build = PlayerSettings.iOS.buildNumber;
#endif

        string content =
            $@"// Auto-generated during build
public static class VersionInfo
{{
    public const string Version = ""{version}"";
    public const string Build = ""{build}"";
}}";

        string path = "Assets/Scripts/VersionInfo.cs";

        // Создаём директорию, если нужно
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        File.WriteAllText(path, content);
        AssetDatabase.Refresh();

        UnityEngine.Debug.Log("✅ VersionInfo.cs обновлён: " + version + " (Build: " + build + ")");
    }
}