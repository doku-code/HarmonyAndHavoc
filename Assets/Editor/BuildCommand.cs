using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class BuildCommand
{
    public static void PerformBuild()
    {
        BuildTarget target = GetBuildTarget();
        string outputPath = GetArgument("-buildOutput", GetDefaultOutputPath(target));
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            throw new InvalidOperationException("No enabled scenes were found in EditorBuildSettings.");
        }

        string outputDirectory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Build failed with result {summary.result}. Errors: {summary.totalErrors}");
        }

        UnityEngine.Debug.Log(
            $"Build succeeded: {summary.outputPath} ({summary.totalSize} bytes)");
    }

    private static BuildTarget GetBuildTarget()
    {
        string target = GetArgument("-buildTarget", "StandaloneOSX");

        if (Enum.TryParse(target, true, out BuildTarget parsedTarget))
        {
            return parsedTarget;
        }

        throw new ArgumentException($"Unsupported build target: {target}");
    }

    private static string GetDefaultOutputPath(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.StandaloneOSX:
                return "Builds/macOS/Harmony and Havoc.app";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Builds/Windows/HarmonyAndHavoc.exe";
            case BuildTarget.WebGL:
                return "Builds/WebGL";
            default:
                return $"Builds/{target}/HarmonyAndHavoc";
        }
    }

    private static string GetArgument(string name, string defaultValue)
    {
        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return args[i + 1];
            }
        }

        return defaultValue;
    }
}
