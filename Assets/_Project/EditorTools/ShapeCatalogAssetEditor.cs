using StickerPuzzle.Game;
using UnityEditor;
using UnityEngine;

namespace StickerPuzzle.EditorTools
{
    /// <summary>Adds the Fill and Check buttons to the shape catalog's Inspector.</summary>
    [CustomEditor(typeof(ShapeCatalogAsset))]
    public sealed class ShapeCatalogAssetEditor : Editor
    {
        private string checkMessage;
        private MessageType checkMessageType;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var catalog = (ShapeCatalogAsset)target;
            EditorGUILayout.Space();

            if (GUILayout.Button("Fill Starting Shapes") && ConfirmFill(catalog))
            {
                Undo.RecordObject(catalog, "Fill Starting Shapes");
                catalog.FillStartingShapes();
                EditorUtility.SetDirty(catalog);
                AssetDatabase.SaveAssetIfDirty(catalog);
                checkMessage = null;
            }

            if (GUILayout.Button("Check Catalog"))
            {
                Check(catalog);
            }

            if (checkMessage != null)
            {
                EditorGUILayout.HelpBox(checkMessage, checkMessageType);
            }
        }

        private static bool ConfirmFill(ShapeCatalogAsset catalog)
        {
            return catalog.Count == 0 || EditorUtility.DisplayDialog(
                "Replace all shapes?",
                $"This replaces all {catalog.Count} shapes with the 13 starting shapes.",
                "Replace",
                "Cancel");
        }

        private void Check(ShapeCatalogAsset catalog)
        {
            if (catalog.TryBuild(out var built, out var errors))
            {
                checkMessage = $"{built.Shapes.Count} shapes, all valid.";
                checkMessageType = MessageType.Info;
            }
            else
            {
                checkMessage = string.Join("\n", errors);
                checkMessageType = MessageType.Error;
            }
        }
    }
}
