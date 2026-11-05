using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace StickerPuzzle.EditorTools
{
    /// <summary>Batch-mode entry points for scripts/build-*.sh.</summary>
    public static class BuildCommands
    {
        public static void BuildWebGL() => Build(BuildTarget.WebGL, "Builds/WebGL");

        public static void BuildAndroid() => Build(BuildTarget.Android, "Builds/Android/StickerPuzzle.apk");

        private static void Build(BuildTarget target, string outputPath)
        {
            ProjectSetup.Apply();

            var parent = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(parent))
            {
                Directory.CreateDirectory(parent);
            }

            EditorUserBuildSettings.buildAppBundle = false;

            var options = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                locationPathName = outputPath,
                target = target,
                options = BuildOptions.None,
            };

            var summary = BuildPipeline.BuildPlayer(options).summary;
            var megabytes = summary.totalSize / (1024f * 1024f);
            var seconds = summary.totalTime.TotalSeconds;
            Debug.Log($"[BUILD] {target} result={summary.result} size={megabytes:0.0}MB time={seconds:0}s errors={summary.totalErrors} warnings={summary.totalWarnings}");

            EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 1);
        }
    }
}
