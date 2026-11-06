using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace StickerPuzzle.EditorTools
{
    /// <summary>Applies project settings and regenerates scenes. Run by scripts/build-scenes.sh.</summary>
    public static class ProjectSetup
    {
        // Placeholders until the designer confirms them; see docs/STATUS.md.
        private const string CompanyName = "KathaNKali";
        private const string ProductName = "Sticker Puzzle";
        private const string AndroidId = "com.kathankali.stickerpuzzle";

        [MenuItem("StickerPuzzle/Apply Project Setup")]
        public static void Apply()
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
            VersionControlSettings.mode = "Visible Meta Files";

            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, AndroidId);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

            // See docs/decisions/004-build-settings.md.
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;

            SpikeSceneBuilder.Build();
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(SpikeSceneBuilder.ScenePath, true) };

            AssetDatabase.SaveAssets();
            Debug.Log("[SETUP] Project setup applied");
        }
    }
}
