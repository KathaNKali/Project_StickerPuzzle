using StickerPuzzle.Core;
using StickerPuzzle.Game;
using UnityEditor;

namespace StickerPuzzle.EditorTools
{
    /// <summary>Finds the project's Shape Catalog asset for the Inspector Check buttons.</summary>
    internal static class ShapeCatalogLoader
    {
        public static bool TryLoad(out ShapeCatalog catalog, out string problem)
        {
            catalog = null;

            var guids = AssetDatabase.FindAssets("t:" + nameof(ShapeCatalogAsset));
            if (guids.Length == 0)
            {
                problem = "No Shape Catalog asset found in the project.";
                return false;
            }

            var asset = AssetDatabase.LoadAssetAtPath<ShapeCatalogAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
            if (!asset.TryBuild(out catalog, out var errors))
            {
                problem = "The Shape Catalog has problems:\n" + string.Join("\n", errors);
                return false;
            }

            problem = null;
            return true;
        }
    }
}
