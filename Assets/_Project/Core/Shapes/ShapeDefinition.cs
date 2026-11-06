using System;
using System.Collections.Generic;

namespace StickerPuzzle.Core
{
    /// <summary>
    /// One entry in the shape catalog. Immutable: a shape can only be made through
    /// <see cref="TryCreate"/>, so every shape in the game is known to be valid.
    /// </summary>
    public sealed class ShapeDefinition
    {
        public const int MaxSize = 3;

        private readonly GridPosition[] cells;

        private ShapeDefinition(string id, string family, GridPosition[] cells, int width, int height)
        {
            Id = id;
            Family = family;
            this.cells = cells;
            Width = width;
            Height = height;
        }

        /// <summary>Stable ID, used by data and saves.</summary>
        public string Id { get; }

        public string Family { get; }

        /// <summary>Cell offsets from the shape's bottom-left corner.</summary>
        public IReadOnlyList<GridPosition> Cells => cells;

        public int Width { get; }

        public int Height { get; }

        /// <summary>
        /// Builds a shape if the data is valid. If not, returns false and says why in
        /// <paramref name="error"/>, in words the designer can act on.
        /// </summary>
        public static bool TryCreate(
            string id,
            string family,
            IEnumerable<GridPosition> cellOffsets,
            out ShapeDefinition shape,
            out string error)
        {
            shape = null;

            if (string.IsNullOrWhiteSpace(id))
            {
                error = "Shape has no ID.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(family))
            {
                error = $"Shape '{id}' has no family.";
                return false;
            }

            if (cellOffsets == null)
            {
                error = $"Shape '{id}' has no cells.";
                return false;
            }

            var unique = new HashSet<GridPosition>();
            var cellList = new List<GridPosition>();
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;

            foreach (var cell in cellOffsets)
            {
                if (!unique.Add(cell))
                {
                    error = $"Shape '{id}' lists cell {cell} twice.";
                    return false;
                }

                cellList.Add(cell);
                minX = Math.Min(minX, cell.X);
                minY = Math.Min(minY, cell.Y);
                maxX = Math.Max(maxX, cell.X);
                maxY = Math.Max(maxY, cell.Y);
            }

            if (cellList.Count == 0)
            {
                error = $"Shape '{id}' has no cells.";
                return false;
            }

            if (minX != 0 || minY != 0)
            {
                error = $"Shape '{id}' must start at column 0 and row 0.";
                return false;
            }

            var width = maxX + 1;
            var height = maxY + 1;
            if (width > MaxSize || height > MaxSize)
            {
                error = $"Shape '{id}' is {width}x{height}; the maximum is {MaxSize}x{MaxSize}.";
                return false;
            }

            if (!IsConnected(cellList[0], unique))
            {
                error = $"Shape '{id}' has separate pieces; all cells must touch side by side.";
                return false;
            }

            shape = new ShapeDefinition(id, family, cellList.ToArray(), width, height);
            error = null;
            return true;
        }

        private static bool IsConnected(GridPosition start, HashSet<GridPosition> allCells)
        {
            var visited = new HashSet<GridPosition> { start };
            var toVisit = new Stack<GridPosition>();
            toVisit.Push(start);

            while (toVisit.Count > 0)
            {
                var current = toVisit.Pop();
                Visit(current.Step(Direction.Up));
                Visit(current.Step(Direction.Down));
                Visit(current.Step(Direction.Left));
                Visit(current.Step(Direction.Right));
            }

            return visited.Count == allCells.Count;

            void Visit(GridPosition neighbour)
            {
                if (allCells.Contains(neighbour) && visited.Add(neighbour))
                {
                    toVisit.Push(neighbour);
                }
            }
        }
    }
}
