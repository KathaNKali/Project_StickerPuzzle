using System;
using System.Collections.Generic;

namespace StickerPuzzle.Core
{
    /// <summary>
    /// Builds a board and its solution from settings and a seed, by reverse construction
    /// (decision 003): blocks are placed in reverse removal order, and each one must have a
    /// route to a door of its color among the blocks placed before it. Those are exactly the
    /// blocks still on the board when it leaves, so every board comes with a proven solution.
    /// The same settings and seed always give the same board, on every platform.
    /// </summary>
    public sealed class Generator
    {
        /// <summary>
        /// How many fresh placement runs one Create may try. A run can get stuck when an early block
        /// sits in front of another color's door; the next run continues the same seeded sequence,
        /// so the result is still the same for the same seed.
        /// </summary>
        public const int MaxRuns = 100;

        private readonly Pathfinder pathfinder = new Pathfinder();
        private readonly List<Direction> route = new List<Direction>();
        private readonly List<GridPosition> spots = new List<GridPosition>();
        private readonly List<ShapeWeight> shapesLeft = new List<ShapeWeight>();

        private sealed class Placement
        {
            public ShapeDefinition Shape;
            public GridPosition Position;
            public string Color;
            public Direction[] Moves;
            public int DoorIndex;
        }

        /// <summary>
        /// Creates a board and its solution. If the settings cannot work, or the blocks cannot all be
        /// placed with this seed, returns false and says why in plain language.
        /// </summary>
        public bool TryCreate(GeneratorSettings settings, int seed, out Board board, out Solution solution, out string problem)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            board = null;
            solution = null;

            var problems = settings.FindProblems();
            if (problems.Count > 0)
            {
                problem = string.Join("\n", problems);
                return false;
            }

            var random = new SeededRandom(seed);
            var placements = new List<Placement>(settings.BlockCount);
            var mostPlaced = 0;

            for (var run = 0; run < MaxRuns; run++)
            {
                if (TryRun(settings, random, placements))
                {
                    board = BuildFinalBoard(settings, placements, out solution);
                    problem = null;
                    return true;
                }

                mostPlaced = Math.Max(mostPlaced, placements.Count);
            }

            problem = $"Could not place all {settings.BlockCount} blocks in {MaxRuns} tries (at best {mostPlaced}): the board gets too crowded for every block to have a way out. Try another seed, fewer blocks, smaller shapes or a bigger board.";
            return false;
        }

        /// <summary>One placement run on a fresh scratch board. Fills the placements list; false if a block found no spot.</summary>
        private bool TryRun(GeneratorSettings settings, SeededRandom random, List<Placement> placements)
        {
            placements.Clear();
            var scratch = settings.CreateEmptyBoard();
            var colors = ShuffledBlockColors(settings, random);

            for (var i = 0; i < colors.Count; i++)
            {
                var placement = TryPlaceBlock(settings, scratch, colors[i], random);
                if (placement == null)
                {
                    return false;
                }

                placements.Add(placement);
            }

            return true;
        }

        /// <summary>One entry per block, e.g. red, red, blue, in a seeded random order.</summary>
        private static List<string> ShuffledBlockColors(GeneratorSettings settings, SeededRandom random)
        {
            var colors = new List<string>(settings.BlockCount);
            var rows = settings.ColorCounts;
            for (var r = 0; r < rows.Count; r++)
            {
                for (var n = 0; n < rows[r].Count; n++)
                {
                    colors.Add(rows[r].Color);
                }
            }

            Shuffle(colors, random);
            return colors;
        }

        /// <summary>
        /// Tries the color's shapes in weighted random order and, for each, every spot in random order.
        /// The first spot that is free, does not start at a door it fits, and has a route out wins.
        /// The block stays on the scratch board. Returns null if nothing works.
        /// </summary>
        private Placement TryPlaceBlock(GeneratorSettings settings, Board scratch, string color, SeededRandom random)
        {
            FillShapesFor(settings, scratch.Doors, color);

            while (shapesLeft.Count > 0)
            {
                var shape = TakeWeightedShape(random);

                spots.Clear();
                for (var y = 0; y + shape.Height <= scratch.Height; y++)
                {
                    for (var x = 0; x + shape.Width <= scratch.Width; x++)
                    {
                        spots.Add(new GridPosition(x, y));
                    }
                }

                Shuffle(spots, random);

                for (var s = 0; s < spots.Count; s++)
                {
                    if (!scratch.TryAddBlock(shape, spots[s], color, out var block))
                    {
                        continue;
                    }

                    if (scratch.FindExitDoor(block) == null && pathfinder.TryFindExit(scratch, block, route, out var door))
                    {
                        return new Placement
                        {
                            Shape = shape,
                            Position = spots[s],
                            Color = color,
                            Moves = route.ToArray(),
                            DoorIndex = IndexOfDoor(scratch.Doors, door),
                        };
                    }

                    scratch.RemoveBlock(block);
                }
            }

            return null;
        }

        /// <summary>Shapes this color may use: picked at all, rectangles, and able to fit a door of this color.</summary>
        private void FillShapesFor(GeneratorSettings settings, IReadOnlyList<Door> doors, string color)
        {
            shapesLeft.Clear();
            var weights = settings.ShapeWeights;
            for (var i = 0; i < weights.Count; i++)
            {
                var shape = weights[i].Shape;
                if (weights[i].Weight > 0
                    && shape.Cells.Count == shape.Width * shape.Height
                    && FitsDoorOfColor(shape, doors, color))
                {
                    shapesLeft.Add(weights[i]);
                }
            }
        }

        private static bool FitsDoorOfColor(ShapeDefinition shape, IReadOnlyList<Door> doors, string color)
        {
            for (var d = 0; d < doors.Count; d++)
            {
                if (string.Equals(doors[d].Color, color, StringComparison.Ordinal) && GeneratorSettings.ShapeFitsDoor(shape, doors[d]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Picks one remaining shape, more likely the higher its weight, and removes it from the list.</summary>
        private ShapeDefinition TakeWeightedShape(SeededRandom random)
        {
            var total = 0;
            for (var i = 0; i < shapesLeft.Count; i++)
            {
                total += shapesLeft[i].Weight;
            }

            var roll = random.Next(total);
            for (var i = 0; i < shapesLeft.Count; i++)
            {
                roll -= shapesLeft[i].Weight;
                if (roll < 0)
                {
                    var shape = shapesLeft[i].Shape;
                    shapesLeft.RemoveAt(i);
                    return shape;
                }
            }

            throw new InvalidOperationException("Weighted pick ran past the end of the list.");
        }

        /// <summary>
        /// Rebuilds the board cleanly in placement order, so block IDs run 0, 1, 2… with no gaps,
        /// and lists the solution in removal order: last placed leaves first.
        /// </summary>
        private static Board BuildFinalBoard(GeneratorSettings settings, List<Placement> placements, out Solution solution)
        {
            var board = settings.CreateEmptyBoard();
            var ids = new int[placements.Count];
            for (var i = 0; i < placements.Count; i++)
            {
                var p = placements[i];
                if (!board.TryAddBlock(p.Shape, p.Position, p.Color, out var block))
                {
                    throw new InvalidOperationException($"Placed block {i + 1} no longer fits when rebuilding the board.");
                }

                ids[i] = block.Id;
            }

            var steps = new List<SolutionStep>(placements.Count);
            for (var i = placements.Count - 1; i >= 0; i--)
            {
                steps.Add(new SolutionStep(ids[i], placements[i].Moves, placements[i].DoorIndex));
            }

            solution = new Solution(steps);
            return board;
        }

        private static int IndexOfDoor(IReadOnlyList<Door> doors, Door door)
        {
            for (var i = 0; i < doors.Count; i++)
            {
                if (doors[i] == door)
                {
                    return i;
                }
            }

            throw new InvalidOperationException("Door is not on the board.");
        }

        /// <summary>Fisher-Yates shuffle with the seeded random, so the order is the same on every platform.</summary>
        private static void Shuffle<T>(List<T> list, SeededRandom random)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
