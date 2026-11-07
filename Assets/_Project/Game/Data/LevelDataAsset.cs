using System;
using System.Collections.Generic;
using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Game
{
    /// <summary>
    /// The levels in play order. This points at the level assets so they can be dragged into order
    /// and are included in builds. Saves never use these references, only each level's permanent ID.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "Sticker Puzzle/Level Data")]
    public sealed class LevelDataAsset : ScriptableObject
    {
        [SerializeField] private List<BoardLayoutAsset> levels = new List<BoardLayoutAsset>();

        public int LevelCount => levels.Count;

        public BoardLayoutAsset GetLevel(int index)
        {
            return levels[index];
        }

        public void AddLevel(BoardLayoutAsset level)
        {
            levels.Add(level);
        }

        /// <summary>
        /// Checks the whole list and returns every problem, so the designer can fix them all in one
        /// go. An empty result means the list is ready to play. Rows are numbered from 1.
        /// </summary>
        public List<string> FindProblems(ShapeCatalog catalog)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            var problems = new List<string>();

            if (levels.Count == 0)
            {
                problems.Add("The list has no levels.");
                return problems;
            }

            var rowByLevel = new Dictionary<BoardLayoutAsset, int>();
            var rowById = new Dictionary<string, int>();

            for (var i = 0; i < levels.Count; i++)
            {
                var row = i + 1;
                var level = levels[i];

                if (level == null)
                {
                    problems.Add($"Row {row} is empty.");
                    continue;
                }

                if (rowByLevel.TryGetValue(level, out var firstRow))
                {
                    problems.Add($"Row {row} '{level.DisplayName}' is the same level as row {firstRow}.");
                    continue;
                }

                rowByLevel.Add(level, row);
                CheckId(level, row, rowById, problems);

                if (!level.TryBuild(catalog, out _, out var boardErrors))
                {
                    foreach (var error in boardErrors)
                    {
                        problems.Add($"Row {row} '{level.DisplayName}': {error}");
                    }
                }
            }

            return problems;
        }

        private static void CheckId(BoardLayoutAsset level, int row, Dictionary<string, int> rowById, List<string> problems)
        {
            if (string.IsNullOrEmpty(level.LevelId))
            {
                problems.Add($"Row {row} '{level.DisplayName}' has no permanent ID.");
            }
            else if (rowById.TryGetValue(level.LevelId, out var firstRow))
            {
                problems.Add($"Row {row} '{level.DisplayName}' has the same permanent ID as row {firstRow}.");
            }
            else
            {
                rowById.Add(level.LevelId, row);
            }
        }
    }
}
