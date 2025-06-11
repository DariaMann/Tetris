#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;

public class PodInstaller
{
    [PostProcessBuild(45)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.iOS)
        {
            var process = new Process();
            process.StartInfo.FileName = "/usr/bin/env";
            process.StartInfo.Arguments = $"pod install";
            process.StartInfo.WorkingDirectory = pathToBuiltProject;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.WaitForExit();
        }
    }
}
#endif