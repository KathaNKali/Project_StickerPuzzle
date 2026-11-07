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
        public const int MinDoors = Board.MinDoors;
        public const int MaxDoors = Board.MaxDoors;

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

        [Serializable]
        private sealed class SolutionStepEntry
        {
            [Tooltip("The block's place in the Blocks list, counting from 0.")]
            public int blockId;
            public List<Direction> moves = new List<Direction>();
            [Tooltip("The door's place in the Doors list, counting from 0.")]
            public int doorIndex;
        }

        [Tooltip("The name shown in tools. Free to change.")]
        [SerializeField] private string levelName;

        // Permanent: saves will refer to it, so renaming or reordering never breaks a save.
        [SerializeField, HideInInspector] private string levelId;

        [SerializeField] private int width = 6;
        [SerializeField] private int height = 6;
        [SerializeField] private List<DoorEntry> doors = new List<DoorEntry>();
        [SerializeField] private List<BlockEntry> blocks = new List<BlockEntry>();

        [Tooltip("The generator seed this level was made from. Hand-made levels ignore it.")]
        [SerializeField] private int seed;

        [Tooltip("One drag per step, in order. The Checker replays it with the real rules.")]
        [SerializeField] private List<SolutionStepEntry> solution = new List<SolutionStepEntry>();

        public string LevelId => levelId;

        /// <summary>The level's name, or the asset's file name if none is set.</summary>
        public string DisplayName => string.IsNullOrWhiteSpace(levelName) ? name : levelName;

        public int DoorCount => doors.Count;

        public int BlockCount => blocks.Count;

        public int Seed => seed;

        public bool HasSolution => solution.Count > 0;

        /// <summary>The saved solution as a fresh Core record, or null if none is saved.</summary>
        public Solution GetSolution()
        {
            if (solution.Count == 0)
            {
                return null;
            }

            var steps = new List<SolutionStep>(solution.Count);
            foreach (var entry in solution)
            {
                steps.Add(new SolutionStep(entry.blockId, entry.moves, entry.doorIndex));
            }

            return new Solution(steps);
        }

        /// <summary>Replaces the saved solution and seed.</summary>
        public void SetSolution(Solution newSolution, int newSeed)
        {
            if (newSolution == null)
            {
                throw new ArgumentNullException(nameof(newSolution));
            }

            seed = newSeed;
            solution.Clear();
            foreach (var step in newSolution.Steps)
            {
                solution.Add(new SolutionStepEntry
                {
                    blockId = step.BlockId,
                    moves = new List<Direction>(step.Moves),
                    doorIndex = step.DoorIndex,
                });
            }
        }

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
        /// Replaces everything with the temporary Phase 1 test board and its solution: 6x6, 3 doors, 6 blocks, no
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

            const Direction U = Direction.Up, L = Direction.Left, R = Direction.Right;
            SetSolution(new Solution(new[]
            {
                new SolutionStep(5, new[] { L, L }, 2),                // yellow 2x1 out the top
                new SolutionStep(1, new[] { L, L }, 0),                // red 1x1 out the left
                new SolutionStep(2, new[] { R, R, R, R }, 1),          // blue 2x1 out the right
                new SolutionStep(0, new[] { U, U, L, L, L }, 0),       // red 1x2 out the left
                new SolutionStep(4, new[] { U, U, U, U, L, L }, 2),    // yellow 1x2 out the top
                new SolutionStep(3, new[] { R, R, R, R, U, U }, 1),    // blue 2x2 out the right
            }), 0);
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

            if (width < Board.MinPlayableSize || height < Board.MinPlayableSize)
            {
                errors.Add($"Board size must be at least {Board.MinPlayableSize}x{Board.MinPlayableSize}, got {width}x{height}.");
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
