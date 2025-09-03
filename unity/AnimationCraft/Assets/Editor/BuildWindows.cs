#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class BuildWindows
{
    [MenuItem("AnimationCraft/Build/Windows x86_64")] 
    public static void BuildWin64()
    {
        var buildPath = "Builds/Windows";
        Directory.CreateDirectory(buildPath);
        var scenes = new[] { "Assets/Scenes/Menu.unity", "Assets/Scenes/Main.unity" };
        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = Path.Combine(buildPath, "AnimationCraft.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        var report = BuildPipeline.BuildPlayer(options);
        EditorUtility.DisplayDialog("Build", report.summary.result.ToString(), "OK");
    }
}
#endif
