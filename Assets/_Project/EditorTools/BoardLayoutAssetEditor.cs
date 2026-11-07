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

        private void OnEnable()
        {
            var layout = (BoardLayoutAsset)target;
            if (layout.EnsureLevelId())
            {
                EditorUtility.SetDirty(layout);
                AssetDatabase.SaveAssetIfDirty(layout);
            }
        }

        public override void OnInspectorGUI()
        {
            var layout = (BoardLayoutAsset)target;
            EditorGUILayout.LabelField("Level ID (permanent)", layout.LevelId);

            DrawDefaultInspector();

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
            if (!ShapeCatalogLoader.TryLoad(out var catalog, out var catalogProblem))
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

    }
}
