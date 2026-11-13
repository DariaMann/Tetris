using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Processes only the build's scene copy; source scenes and Android are unchanged.
public sealed class AppleDonationBuildProcessor : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        // A null report is also used when entering Play Mode: leave that scene alone.
        if (report == null) return;
        StripDonationUI(scene, report.summary.platform);
    }

    public static void StripDonationUI(Scene scene, BuildTarget target)
    {
        if (target != BuildTarget.iOS && target != BuildTarget.StandaloneOSX) return;

        var objectsToRemove = new HashSet<GameObject>();
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var panel in root.GetComponentsInChildren<GUIDonatPanel>(true))
                objectsToRemove.Add(panel.gameObject);

            // Follow the actual button callbacks, so renaming a button is safe.
            foreach (var button in root.GetComponentsInChildren<Button>(true))
            {
                for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                {
                    if (button.onClick.GetPersistentTarget(i) is GUIDonatPanel)
                    {
                        objectsToRemove.Add(button.gameObject);
                        break;
                    }
                }
            }
        }

        foreach (var item in objectsToRemove)
            if (item != null) Object.DestroyImmediate(item);
    }
}