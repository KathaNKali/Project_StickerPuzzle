using System;
using System.Collections.Generic;
using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Game
{
    /// <summary>
    /// A hand-editable starting board: size, doors and blocks. Blocks refer to shapes by
    /// stable ID. Building a board from this creates a fresh copy, so playing never changes the asset.
    /// </summary>
    [CreateAssetMenu(fileName = "BoardLayout", menuName = "Sticker Puzzle/Board Layout")]
    public sealed class BoardLayoutAsset : ScriptableObject
    {
        public const int MinDoors = 2;
        public const int MaxDoors = 4;

        [Serializable]
        private sealed class DoorEntry
        {
            public BoardSide side;
            public int position;
            public int openingSize = 1;
            public string color;
        }

        [Serializable]
        private sealed class BlockEntry
        {
            public string shapeId;
            public Vector2Int position;
            public string color;
        }

        [Tooltip("The name shown in tools. Free to change.")]
        [SerializeField] private string levelName;

        // Permanent: saves will refer to it, so renaming or reordering never breaks a save.
        [SerializeField, HideInInspector] private string levelId;

        [SerializeField] private int width = 6;
        [SerializeField] private int height = 6;
        [SerializeField] private List<DoorEntry> doors = new List<DoorEntry>();
        [SerializeField] private List<BlockEntry> blocks = new List<BlockEntry>();

        public string LevelId => levelId;

        /// <summary>The level's name, or the asset's file name if none is set.</summary>
        public string DisplayName => string.IsNullOrWhiteSpace(levelName) ? name : levelName;

        public int DoorCount => doors.Count;

        public int BlockCount => blocks.Count;

        /// <summary>Gives the level a permanent ID if it has none. True if one was created.</summary>
        public bool EnsureLevelId()
        {
            if (!string.IsNullOrEmpty(levelId))
            {
                return false;
            }

            levelId = Guid.NewGuid().ToString("N");
            return true;
        }

        /// <summary>Editor tools only, for copied levels. Using this on a released level would break saves.</summary>
        public void AssignNewLevelId()
        {
            levelId = Guid.NewGuid().ToString("N");
        }

        public void SetLevelName(string newName)
        {
            levelName = newName;
        }

        public void SetSize(int newWidth, int newHeight)
        {
            width = newWidth;
            height = newHeight;
        }

        public void AddDoor(BoardSide side, int position, int openingSize, string color)
        {
            doors.Add(new DoorEntry { side = side, position = position, openingSize = openingSize, color = color });
        }

        public void AddBlock(string shapeId, int x, int y, string color)
        {
            blocks.Add(new BlockEntry { shapeId = shapeId, position = new Vector2Int(x, y), color = color });
        }

        /// <summary>
        /// Replaces everything with the temporary Phase 1 test board: 6x6, 3 doors, 6 blocks, no
        /// L shapes. BoardLayoutTests plays through a full solution, so it is proven clearable.
        /// </summary>
        public void FillTestBoard()
        {
            doors.Clear();
            blocks.Clear();
            SetSize(6, 6);

            AddDoor(BoardSide.Left, 2, 2, "red");
            AddDoor(BoardSide.Right, 2, 3, "blue");
            AddDoor(BoardSide.Top, 2, 2, "yellow");

            AddBlock("1x2", 3, 0, "red");
            AddBlock("1x1", 2, 3, "red");
            AddBlock("2x1", 0, 4, "blue");
            AddBlock("2x2", 0, 0, "blue");
            AddBlock("1x2", 5, 0, "yellow");
            AddBlock("2x1", 4, 5, "yellow");
        }

        /// <summary>
        /// Builds a fresh board. If anything is wrong, returns false and lists every problem,
        /// so the designer can fix them all in one go.
        /// </summary>
        public bool TryBuild(ShapeCatalog catalog, out Board board, out List<string> errors)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            board = null;
            errors = new List<string>();

            if (width < 1 || height < 1)
            {
                errors.Add($"Board size must be at least 1x1, got {width}x{height}.");
                return false;
            }

            if (doors.Count < MinDoors || doors.Count > MaxDoors)
            {
                errors.Add($"A board needs {MinDoors} to {MaxDoors} doors, this one has {doors.Count}.");
            }

            if (blocks.Count == 0)
            {
                errors.Add("The board has no blocks.");
            }

            var built = new Board(width, height);
            AddDoors(built, errors);
            AddBlocks(built, catalog, errors);

            if (errors.Count > 0)
            {
                return false;
            }

            board = built;
            return true;
        }

        private void AddDoors(Board target, List<string> errors)
        {
            foreach (var entry in doors)
            {
                if (!Door.TryCreate(entry.side, entry.position, entry.openingSize, entry.color, out var door, out var error)
                    || !target.TryAddDoor(door, out error))
                {
                    errors.Add(error);
                }
            }
        }

        private void AddBlocks(Board target, ShapeCatalog catalog, List<string> errors)
        {
            foreach (var entry in blocks)
            {
                var position = new GridPosition(entry.position.x, entry.position.y);

                if (!catalog.TryGet(entry.shapeId, out var shape))
                {
                    errors.Add($"Block at {position} uses unknown shape '{entry.shapeId}'.");
                }
                else if (string.IsNullOrWhiteSpace(entry.color))
                {
                    errors.Add($"Block '{entry.shapeId}' at {position} has no color.");
                }
                else if (!target.TryAddBlock(shape, position, entry.color, out var block))
                {
                    errors.Add($"Block '{entry.shapeId}' at {position} is off the board or overlaps another block.");
                }
                else if (target.FindExitDoor(block) != null)
                {
                    errors.Add($"Block '{entry.shapeId}' at {position} starts touching a door it fits. Levels cannot start this way.");
                }
            }
        }
    }
}
