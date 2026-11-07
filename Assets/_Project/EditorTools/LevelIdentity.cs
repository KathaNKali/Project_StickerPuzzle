using System;
using System.Collections.Generic;
using System.IO;
using StickerPuzzle.Game;
using UnityEditor;

namespace StickerPuzzle.EditorTools
{
    /// <summary>
    /// Keeps every level's permanent ID present and unique. A copied level gets its own ID and
    /// falls back to its file name, instead of sharing the original's ID and name.
    /// </summary>
    public static class LevelIdentity
    {
        /// <summary>
        /// Fixes each incoming level whose ID is blank or already taken. Settled levels are never
        /// changed. Returns the levels that changed, so the caller can save them.
        /// </summary>
        public static List<BoardLayoutAsset> Repair(IEnumerable<BoardLayoutAsset> settled, IEnumerable<BoardLayoutAsset> incoming)
        {
            var takenIds = new HashSet<string>();
            foreach (var level in settled)
            {
                if (!string.IsNullOrEmpty(level.LevelId))
                {
                    takenIds.Add(level.LevelId);
                }
            }

            var changed = new List<BoardLayoutAsset>();
            foreach (var level in incoming)
            {
                if (level.EnsureLevelId())
                {
                    changed.Add(level);
                }
                else if (takenIds.Contains(level.LevelId))
                {
                    level.AssignNewLevelId();
                    level.SetLevelName(null);
                    changed.Add(level);
                }

                takenIds.Add(level.LevelId);
            }

            return changed;
        }

        /// <summary>
        /// Sorts asset paths so "Level" comes before its copy "Level 1". The extension is ignored
        /// because "Level 1.asset" would otherwise sort before "Level.asset".
        /// </summary>
        public static void SortOriginalsFirst(List<string> assetPaths)
        {
            assetPaths.Sort((a, b) => string.CompareOrdinal(Path.ChangeExtension(a, null), Path.ChangeExtension(b, null)));
        }
    }

    /// <summary>Runs LevelIdentity when levels are created or changed, and after scripts recompile.</summary>
    internal sealed class LevelIdentityPostprocessor : AssetPostprocessor
    {
        private const string AssetExtension = ".asset";

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload)
        {
            var incomingPaths = new HashSet<string>();
            foreach (var path in importedAssets)
            {
                if (path.EndsWith(AssetExtension, StringComparison.OrdinalIgnoreCase)
                    && AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(BoardLayoutAsset))
                {
                    incomingPaths.Add(path);
                }
            }

            // Skip the search unless a level arrived. A recompile also checks every level,
            // which covers ones made before this tool existed.
            if (incomingPaths.Count == 0 && !didDomainReload)
            {
                return;
            }

            var allPaths = new List<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:" + nameof(BoardLayoutAsset)))
            {
                allPaths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            LevelIdentity.SortOriginalsFirst(allPaths);

            var settled = new List<BoardLayoutAsset>();
            var incoming = new List<BoardLayoutAsset>();
            foreach (var path in allPaths)
            {
                var level = AssetDatabase.LoadAssetAtPath<BoardLayoutAsset>(path);
                if (level != null)
                {
                    (didDomainReload || incomingPaths.Contains(path) ? incoming : settled).Add(level);
                }
            }

            foreach (var level in LevelIdentity.Repair(settled, incoming))
            {
                EditorUtility.SetDirty(level);
                AssetDatabase.SaveAssetIfDirty(level);
            }
        }
    }
}
