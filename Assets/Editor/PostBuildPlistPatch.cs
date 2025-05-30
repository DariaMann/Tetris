#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PostBuildPlistPatch
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Добавим LSApplicationQueriesSchemes
        PlistElementDict rootDict = plist.root;

        PlistElementArray queriesSchemes;
        if (rootDict.values.ContainsKey("LSApplicationQueriesSchemes"))
        {
            queriesSchemes = rootDict["LSApplicationQueriesSchemes"].AsArray();
        }
        else
        {
            queriesSchemes = rootDict.CreateArray("LSApplicationQueriesSchemes");
        }

        if (!queriesSchemes.values.Exists(el => el.AsString() == "mailto"))
            queriesSchemes.AddString("mailto");
        if (!queriesSchemes.values.Exists(el => el.AsString() == "https"))
            queriesSchemes.AddString("https");

        plist.WriteToFile(plistPath);
    }
}
#endif