using StickerPuzzle.Core;
using StickerPuzzle.Game;
using UnityEditor;
using UnityEngine;

namespace StickerPuzzle.EditorTools
{
    /// <summary>Adds the Fill and Check buttons to a board layout's Inspector.</summary>
    [CustomEditor(typeof(BoardLayoutAsset))]
    public sealed class BoardLayoutAssetEditor : Editor
    {
        private string checkMessage;
        private MessageType checkMessageType;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var layout = (BoardLayoutAsset)target;
            EditorGUILayout.Space();

            if (GUILayout.Button("Fill Test Board") && ConfirmFill(layout))
            {
                Undo.RecordObject(layout, "Fill Test Board");
                layout.FillTestBoard();
                EditorUtility.SetDirty(layout);
                AssetDatabase.SaveAssetIfDirty(layout);
                checkMessage = null;
            }

            if (GUILayout.Button("Check Layout"))
            {
                Check(layout);
            }

            if (checkMessage != null)
            {
                EditorGUILayout.HelpBox(checkMessage, checkMessageType);
            }
        }

        private static bool ConfirmFill(BoardLayoutAsset layout)
        {
            return layout.DoorCount + layout.BlockCount == 0 || EditorUtility.DisplayDialog(
                "Replace this layout?",
                "This replaces all doors and blocks with the test board.",
                "Replace",
                "Cancel");
        }

        private void Check(BoardLayoutAsset layout)
        {
            if (!TryLoadCatalog(out var catalog, out var catalogProblem))
            {
                checkMessage = catalogProblem;
                checkMessageType = MessageType.Error;
            }
            else if (layout.TryBuild(catalog, out var board, out var errors))
            {
                checkMessage = $"Valid: {board.Width}x{board.Height}, {board.Doors.Count} doors, {board.Blocks.Count} blocks.";
                checkMessageType = MessageType.Info;
            }
            else
            {
                checkMessage = string.Join("\n", errors);
                checkMessageType = MessageType.Error;
            }
        }

        private static bool TryLoadCatalog(out ShapeCatalog catalog, out string problem)
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
