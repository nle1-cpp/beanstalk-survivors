using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class TestAutomation
{
    private const string PlaytestDefine = "PLAYTEST_TOOLS";

    public static string GetProjectRoot()
    {
        return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
    }

    public static string GetArtifactsRoot()
    {
        return Path.Combine(GetProjectRoot(), "Artifacts");
    }

    public static string GetBuildRoot(BuildTarget target)
    {
        return Path.Combine(GetProjectRoot(), "Builds", "Playtest", target.ToString());
    }

    public static string GetEnabledScenePath()
    {
        string scenePath = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(scenePath))
        {
            throw new InvalidOperationException("No enabled scenes found in Build Settings.");
        }

        return scenePath;
    }

    public static string GetPlayerOutputPath(BuildTarget target)
    {
        string buildRoot = GetBuildRoot(target);
        string productName = string.IsNullOrWhiteSpace(PlayerSettings.productName) ? "BeanstalkSurvivors" : PlayerSettings.productName;

        return target switch
        {
            BuildTarget.StandaloneWindows64 => Path.Combine(buildRoot, productName + ".exe"),
            BuildTarget.StandaloneOSX => Path.Combine(buildRoot, productName + ".app"),
            BuildTarget.StandaloneLinux64 => Path.Combine(buildRoot, productName + ".x86_64"),
            _ => throw new NotSupportedException($"Unsupported build target: {target}")
        };
    }

    public static string[] GetEnabledScenePaths()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }

    public static void BuildDevPlayer()
    {
        BuildDevPlayer(false);
    }

    public static void BuildAndRunDevPlayer()
    {
        BuildDevPlayer(true);
    }

    public static void BuildDevPlayer(bool autoRunPlayer)
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        if (target != BuildTarget.StandaloneWindows64 && target != BuildTarget.StandaloneOSX && target != BuildTarget.StandaloneLinux64)
        {
            throw new NotSupportedException($"Unsupported active build target: {target}");
        }

        string outputPath = GetPlayerOutputPath(target);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? GetBuildRoot(target));
        Directory.CreateDirectory(GetArtifactsRoot());

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenePaths(),
            locationPathName = outputPath,
            target = target,
            options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler | (autoRunPlayer ? BuildOptions.AutoRunPlayer : BuildOptions.None),
            extraScriptingDefines = new[] { PlaytestDefine }
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Build failed: {report.summary.result}\nOutput: {outputPath}");
        }

        Debug.Log($"Build succeeded: {outputPath}");
    }
}
