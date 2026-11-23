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
            // Keep existing post_install fixes and add the deployment floor to the same hook.
            string iosPodfilePath = System.IO.Path.Combine(pathToBuiltProject, "Podfile");
            if (!System.IO.File.Exists(iosPodfilePath))
                throw new System.Exception("Podfile is missing; resolve iOS dependencies first.");

            string iosPodfile = System.IO.File.ReadAllText(iosPodfilePath);
            if (!iosPodfile.Contains("# iOS 15 minimum for Xcode 27"))
            {
                const string iosDeploymentFix = @"# iOS 15 minimum for Xcode 27
            minimum_ios = Gem::Version.new('15.0')
            configs = installer.pods_project.build_configurations + installer.pods_project.targets.flat_map(&:build_configurations)
            configs.each do |config|
              current_ios = config.build_settings['IPHONEOS_DEPLOYMENT_TARGET']
              if current_ios.nil? || !Gem::Version.correct?(current_ios.to_s) || Gem::Version.new(current_ios.to_s) < minimum_ios
                config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = minimum_ios.to_s
              end
            end
            ";
                const string installHook = "post_install do |installer|";
                if (iosPodfile.Contains(installHook))
                    iosPodfile = iosPodfile.Replace(installHook, installHook + "\n" + iosDeploymentFix);
                else
                    iosPodfile += "\n" + installHook + "\n" + iosDeploymentFix + "end\n";
                System.IO.File.WriteAllText(iosPodfilePath, iosPodfile);
            }

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