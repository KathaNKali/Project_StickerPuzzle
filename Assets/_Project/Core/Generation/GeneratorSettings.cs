using System;
using System.Collections.Generic;

namespace StickerPuzzle.Core
{
    /// <summary>How often the generator picks a shape. Weight 0 means never.</summary>
    public readonly struct ShapeWeight
    {
        public ShapeWeight(ShapeDefinition shape, int weight)
        {
            Shape = shape;
            Weight = weight;
        }

        public ShapeDefinition Shape { get; }

        public int Weight { get; }
    }

    /// <summary>How many blocks of one color the generator places. Exact, not a weight (designer, 2026-11-07).</summary>
    public readonly struct ColorCount
    {
        public ColorCount(string color, int count)
        {
            Color = color;
            Count = count;
        }

        /// <summary>Must match a door's color, since a block can only leave through a door of its color.</summary>
        public string Color { get; }

        public int Count { get; }
    }

    /// <summary>
    /// What the designer asks the generator for: board size, doors, door limits, blocks per color and shape weights.
    /// <see cref="FindProblems"/> says in plain language why settings cannot work.
    /// </summary>
    public sealed class GeneratorSettings
    {
        private readonly Door[] doors;
        private readonly ShapeWeight[] shapeWeights;
        private readonly ColorCount[] colorCounts;

        public GeneratorSettings(
            int width,
            int height,
            IEnumerable<Door> doors,
            IEnumerable<ColorCount> colorCounts,
            IEnumerable<ShapeWeight> shapeWeights,
            int minDoors = Board.MinDoors,
            int maxDoors = Board.MaxDoors)
        {
            MinDoors = minDoors;
            MaxDoors = maxDoors;
            Width = width;
            Height = height;
            this.doors = new List<Door>(doors ?? throw new ArgumentNullException(nameof(doors))).ToArray();
            this.colorCounts = new List<ColorCount>(colorCounts ?? throw new ArgumentNullException(nameof(colorCounts))).ToArray();
            for (var i = 0; i < this.colorCounts.Length; i++)
            {
                BlockCount += Math.Max(0, this.colorCounts[i].Count);
            }

            this.shapeWeights = new List<ShapeWeight>(shapeWeights ?? throw new ArgumentNullException(nameof(shapeWeights))).ToArray();
        }

        public int Width { get; }

        public int Height { get; }

        public IReadOnlyList<Door> Doors => doors;

        /// <summary>Fewest doors allowed. At least 1, since blocks need a way out.</summary>
        public int MinDoors { get; }

        /// <summary>Most doors allowed.</summary>
        public int MaxDoors { get; }

        /// <summary>Blocks of each color, in the designer's order.</summary>
        public IReadOnlyList<ColorCount> ColorCounts => colorCounts;

        /// <summary>Total blocks: the sum of the color counts.</summary>
        public int BlockCount { get; }

        public IReadOnlyList<ShapeWeight> ShapeWeights => shapeWeights;

        /// <summary>Every reason these settings cannot work. Empty means they are fine.</summary>
        public List<string> FindProblems()
        {
            var problems = new List<string>();

            if (Width < Board.MinPlayableSize || Height < Board.MinPlayableSize)
            {
                problems.Add($"The board must be at least {Board.MinPlayableSize}x{Board.MinPlayableSize}, but it is {Width}x{Height}.");
                return problems;
            }

            var board = new Board(Width, Height);
            CheckDoors(board, problems);

            var usable = CheckShapes(board, problems);

            // No check for a block touching two same-color doors on opposite sides: shapes are at most
            // 3 cells across and boards at least 5, so a block can never span the board.
            CheckColors(board, usable, problems);
            return problems;
        }

        /// <summary>An empty board with these doors. Only call it when <see cref="FindProblems"/> finds none.</summary>
        public Board CreateEmptyBoard()
        {
            var board = new Board(Width, Height);
            for (var i = 0; i < doors.Length; i++)
            {
                if (!board.TryAddDoor(doors[i], out var error))
                {
                    throw new InvalidOperationException(error);
                }
            }

            return board;
        }

        /// <summary>True if a block of this shape can leave through this door: its width along the edge fits the opening.</summary>
        public static bool ShapeFitsDoor(ShapeDefinition shape, Door door)
        {
            var alongEdge = door.Side == BoardSide.Top || door.Side == BoardSide.Bottom ? shape.Width : shape.Height;
            return alongEdge <= door.OpeningSize;
        }

        private void CheckDoors(Board board, List<string> problems)
        {
            if (MinDoors < 1)
            {
                problems.Add($"Min Doors must be at least 1, since blocks need a way out. It is {MinDoors}.");
            }
            else if (MaxDoors < MinDoors)
            {
                problems.Add($"Max Doors ({MaxDoors}) is less than Min Doors ({MinDoors}).");
            }
            else if (doors.Length < MinDoors || doors.Length > MaxDoors)
            {
                problems.Add($"A board needs {MinDoors} to {MaxDoors} doors, but these settings have {doors.Length}.");
            }

            for (var i = 0; i < doors.Length; i++)
            {
                if (doors[i] == null)
                {
                    problems.Add($"Door {i + 1} is empty.");
                }
                else if (!board.TryAddDoor(doors[i], out var error))
                {
                    problems.Add($"Door {i + 1}: {error}");
                }
            }
        }

        /// <summary>Reports bad shape entries and returns the shapes the generator can actually use.</summary>
        private List<ShapeDefinition> CheckShapes(Board board, List<string> problems)
        {
            var usable = new List<ShapeDefinition>();
            var seenIds = new HashSet<string>(StringComparer.Ordinal);
            var anyPositive = false;

            for (var i = 0; i < shapeWeights.Length; i++)
            {
                var entry = shapeWeights[i];
                if (entry.Shape == null)
                {
                    problems.Add($"Shape row {i + 1} has no shape.");
                    continue;
                }

                var id = entry.Shape.Id;
                if (!seenIds.Add(id))
                {
                    problems.Add($"Shape '{id}' is listed more than once. Keep one row and add the weights together.");
                }

                if (entry.Weight < 0)
                {
                    problems.Add($"Shape '{id}' has weight {entry.Weight}. Weights cannot be negative; use 0 for never.");
                    continue;
                }

                if (entry.Weight == 0)
                {
                    continue;
                }

                anyPositive = true;

                if (entry.Shape.Cells.Count != entry.Shape.Width * entry.Shape.Height)
                {
                    problems.Add($"Shape '{id}' is not a rectangle. The generator uses rectangles only for now; set its weight to 0.");
                    continue;
                }

                if (!FitsAnyDoor(entry.Shape, board.Doors))
                {
                    problems.Add($"Shape '{id}' fits through no door: every opening is too narrow. Widen a door or set its weight to 0.");
                    continue;
                }

                usable.Add(entry.Shape);
            }

            if (!anyPositive)
            {
                problems.Add("No shape can be picked. Give at least one shape a weight above 0.");
            }

            return usable;
        }

        private bool FitsAnyDoor(ShapeDefinition shape, IReadOnlyList<Door> boardDoors)
        {
            for (var i = 0; i < boardDoors.Count; i++)
            {
                if (ShapeFitsDoor(shape, boardDoors[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Reports bad color rows, colors no block can use, and boards too small for all the blocks.</summary>
        private void CheckColors(Board board, List<ShapeDefinition> usable, List<string> problems)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            long cellsNeeded = 0;

            for (var i = 0; i < colorCounts.Length; i++)
            {
                var row = colorCounts[i];
                if (string.IsNullOrWhiteSpace(row.Color))
                {
                    problems.Add($"Color row {i + 1} has no color.");
                    continue;
                }

                if (!seen.Add(row.Color))
                {
                    problems.Add($"Color '{row.Color}' is listed more than once. Keep one row and add the counts together.");
                    continue;
                }

                if (row.Count < 0)
                {
                    problems.Add($"Color '{row.Color}' has {row.Count} blocks. Counts cannot be negative; use 0 for none.");
                    continue;
                }

                if (row.Count == 0)
                {
                    continue;
                }

                if (!HasDoorOfColor(board.Doors, row.Color))
                {
                    problems.Add($"Color '{row.Color}' has blocks but no door. Add a {row.Color} door or set its count to 0.");
                    continue;
                }

                var smallest = SmallestShapeForColor(board.Doors, usable, row.Color);
                if (smallest == 0)
                {
                    if (usable.Count > 0)
                    {
                        problems.Add($"No shape fits through any {row.Color} door. Widen a {row.Color} door or allow smaller shapes.");
                    }

                    continue;
                }

                cellsNeeded += (long)smallest * row.Count;
            }

            if (BlockCount < 1)
            {
                problems.Add("No blocks to place. Give at least one color a count above 0.");
            }

            var cells = Width * Height;
            if (cellsNeeded > cells)
            {
                problems.Add($"{BlockCount} blocks need at least {cellsNeeded} cells, but the board has only {cells}. Use fewer blocks, smaller shapes or a bigger board.");
            }
        }

        private static bool HasDoorOfColor(IReadOnlyList<Door> boardDoors, string color)
        {
            for (var i = 0; i < boardDoors.Count; i++)
            {
                if (string.Equals(boardDoors[i].Color, color, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Cell count of the smallest usable shape that fits a door of this color, or 0 if none does.</summary>
        private static int SmallestShapeForColor(IReadOnlyList<Door> boardDoors, List<ShapeDefinition> usable, string color)
        {
            var smallest = 0;
            for (var d = 0; d < boardDoors.Count; d++)
            {
                if (!string.Equals(boardDoors[d].Color, color, StringComparison.Ordinal))
                {
                    continue;
                }

                for (var s = 0; s < usable.Count; s++)
                {
                    var size = usable[s].Cells.Count;
                    if (ShapeFitsDoor(usable[s], boardDoors[d]) && (smallest == 0 || size < smallest))
                    {
                        smallest = size;
                    }
                }
            }

            return smallest;
        }
    }
}
