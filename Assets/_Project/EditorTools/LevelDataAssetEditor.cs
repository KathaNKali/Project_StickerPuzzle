using StickerPuzzle.Game;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace StickerPuzzle.EditorTools
{
    /// <summary>Shows Level Data as a drag-to-reorder list with editable names, plus a Check button.</summary>
    [CustomEditor(typeof(LevelDataAsset))]
    public sealed class LevelDataAssetEditor : Editor
    {
        private const float RowNumberWidth = 24f;
        private const float Gap = 4f;

        private SerializedProperty levelsProperty;
        private ReorderableList list;
        private string checkMessage;
        private MessageType checkMessageType;

        private void OnEnable()
        {
            levelsProperty = serializedObject.FindProperty("levels");
            list = new ReorderableList(serializedObject, levelsProperty, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Levels, in play order"),
                drawElementCallback = DrawRow,
                onAddCallback = AddEmptyRow,
                onChangedCallback = _ => checkMessage = null,
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            list.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                checkMessage = null;
            }

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Check Levels"))
            {
                Check((LevelDataAsset)target);
            }

            if (checkMessage != null)
            {
                EditorGUILayout.HelpBox(checkMessage, checkMessageType);
            }
        }

        private void DrawRow(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;

            var fieldWidth = (rect.width - RowNumberWidth - Gap * 2f) / 2f;
            var numberRect = new Rect(rect.x, rect.y, RowNumberWidth, rect.height);
            var nameRect = new Rect(numberRect.xMax + Gap, rect.y, fieldWidth, rect.height);
            var assetRect = new Rect(nameRect.xMax + Gap, rect.y, fieldWidth, rect.height);

            EditorGUI.LabelField(numberRect, (index + 1).ToString());

            var element = levelsProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(assetRect, element, GUIContent.none);
            DrawNameField(nameRect, element.objectReferenceValue as BoardLayoutAsset);
        }

        /// <summary>The name lives on the level itself. Delayed, so it saves once when typing ends.</summary>
        private static void DrawNameField(Rect rect, BoardLayoutAsset level)
        {
            if (level == null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.TextField(rect, "(empty)");
                }

                return;
            }

            var newName = EditorGUI.DelayedTextField(rect, level.DisplayName);
            if (newName != level.DisplayName)
            {
                Undo.RecordObject(level, "Rename Level");
                level.SetLevelName(newName);
                EditorUtility.SetDirty(level);
                AssetDatabase.SaveAssetIfDirty(level);
            }
        }

        /// <summary>Unity's default copies the last row, which would list the same level twice.</summary>
        private void AddEmptyRow(ReorderableList target)
        {
            var newIndex = levelsProperty.arraySize;
            levelsProperty.arraySize = newIndex + 1;
            levelsProperty.GetArrayElementAtIndex(newIndex).objectReferenceValue = null;
            target.index = newIndex;
        }

        private void Check(LevelDataAsset levelData)
        {
            if (!ShapeCatalogLoader.TryLoad(out var catalog, out var catalogProblem))
            {
                checkMessage = catalogProblem;
                checkMessageType = MessageType.Error;
                return;
            }

            var problems = levelData.FindProblems(catalog);
            if (problems.Count == 0)
            {
                checkMessage = $"Valid: {levelData.LevelCount} levels.";
                checkMessageType = MessageType.Info;
            }
            else
            {
                checkMessage = string.Join("\n", problems);
                checkMessageType = MessageType.Error;
            }
        }
    }
}
