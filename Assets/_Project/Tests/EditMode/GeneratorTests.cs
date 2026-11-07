using System.Collections.Generic;
using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class GeneratorTests
    {
        private Generator generator;

        [SetUp]
        public void SetUp()
        {
            generator = new Generator();
        }

        private static Door MakeDoor(BoardSide side, int position, int openingSize, string color)
        {
            Assert.That(Door.TryCreate(side, position, openingSize, color, out var door, out var error), Is.True, error);
            return door;
        }

        /// <summary>6x6, red door on the left, blue on the right (rows 2-3), 3 red and 3 blue blocks of mixed shapes.</summary>
        private static GeneratorSettings MakeSettings(int red = 3, int blue = 3, List<ShapeWeight> weights = null, List<Door> doors = null, int size = 6)
        {
            return new GeneratorSettings(
                size,
                size,
                doors ?? new List<Door>
                {
                    MakeDoor(BoardSide.Left, 2, 2, "red"),
                    MakeDoor(BoardSide.Right, 2, 2, "blue"),
                },
                new List<ColorCount> { new ColorCount("red", red), new ColorCount("blue", blue) },
                weights ?? new List<ShapeWeight>
                {
                    new ShapeWeight(TestShapes.Single, 2),
                    new ShapeWeight(TestShapes.TwoWide, 1),
                    new ShapeWeight(TestShapes.TwoTall, 1),
                    new ShapeWeight(TestShapes.TwoByTwo, 1),
                });
        }

        private Board Create(GeneratorSettings settings, int seed, out Solution solution)
        {
            Assert.That(generator.TryCreate(settings, seed, out var board, out solution, out var problem), Is.True, problem);
            return board;
        }

        private static string Describe(Board board, Solution solution)
        {
            var parts = new List<string>();
            foreach (var block in board.Blocks)
            {
                parts.Add($"{block.Id}:{block.Shape.Id}@{block.Position}:{block.Color}");
            }

            foreach (var step in solution.Steps)
            {
                parts.Add($"{step.BlockId}>{step.DoorIndex}:{string.Join(",", step.Moves)}");
            }

            return string.Join(" ", parts);
        }

        [Test]
        public void TryCreate_GoodSettings_BoardPassesTheChecker()
        {
            var board = Create(MakeSettings(), 1, out var solution);

            Assert.That(Checker.IsSolvable(board, solution, out var problem), Is.True, problem);
        }

        [Test]
        public void TryCreate_GoodSettings_PlacesExactCountPerColor()
        {
            var board = Create(MakeSettings(red: 4, blue: 2), 7, out _);

            var red = 0;
            var blue = 0;
            foreach (var block in board.Blocks)
            {
                if (block.Color == "red") red++;
                if (block.Color == "blue") blue++;
            }

            Assert.That(red, Is.EqualTo(4));
            Assert.That(blue, Is.EqualTo(2));
        }

        [Test]
        public void TryCreate_GoodSettings_IdsRunFromZeroAndSolutionUsesEachOnce()
        {
            var board = Create(MakeSettings(), 3, out var solution);

            var ids = new List<int>();
            foreach (var step in solution.Steps)
            {
                ids.Add(step.BlockId);
            }

            ids.Sort();
            Assert.That(ids, Is.EqualTo(new[] { 0, 1, 2, 3, 4, 5 }));
            Assert.That(board.Blocks, Has.Count.EqualTo(6));
        }

        [Test]
        public void TryCreate_GoodSettings_NoBlockStartsAtADoorItFits()
        {
            for (var seed = 0; seed < 50; seed++)
            {
                var board = Create(MakeSettings(), seed, out _);
                foreach (var block in board.Blocks)
                {
                    Assert.That(board.FindExitDoor(block), Is.Null, $"Seed {seed}, block {block.Id}");
                }
            }
        }

        [Test]
        public void TryCreate_SameSeed_GivesTheSameBoardAndSolution()
        {
            var first = Create(MakeSettings(), 42, out var firstSolution);
            var second = Create(MakeSettings(), 42, out var secondSolution);
            var fresh = new Generator();
            Assert.That(fresh.TryCreate(MakeSettings(), 42, out var third, out var thirdSolution, out _), Is.True);

            Assert.That(Describe(second, secondSolution), Is.EqualTo(Describe(first, firstSolution)));
            Assert.That(Describe(third, thirdSolution), Is.EqualTo(Describe(first, firstSolution)));
        }

        [Test]
        public void TryCreate_DifferentSeeds_GiveDifferentBoards()
        {
            var boards = new HashSet<string>();
            for (var seed = 0; seed < 10; seed++)
            {
                boards.Add(Describe(Create(MakeSettings(), seed, out var solution), solution));
            }

            Assert.That(boards.Count, Is.GreaterThan(1));
        }

        [Test]
        public void TryCreate_ManySeeds_AllPassTheChecker()
        {
            for (var seed = 0; seed < 200; seed++)
            {
                var board = Create(MakeSettings(red: 5, blue: 5), seed, out var solution);
                Assert.That(Checker.IsSolvable(board, solution, out var problem), Is.True, $"Seed {seed}: {problem}");
            }
        }

        [Test]
        public void TryCreate_BadSettings_ReturnsSettingsProblem()
        {
            Assert.That(generator.TryCreate(MakeSettings(size: 4), 1, out var board, out var solution, out var problem), Is.False);
            Assert.That(problem, Does.Contain("at least 5x5"));
            Assert.That(board, Is.Null);
            Assert.That(solution, Is.Null);
        }

        [Test]
        public void TryCreate_BoardFullOfBlocks_SaysCouldNotPlace()
        {
            var weights = new List<ShapeWeight> { new ShapeWeight(TestShapes.Single, 1) };

            Assert.That(generator.TryCreate(MakeSettings(red: 13, blue: 12, weights: weights, size: 5), 1, out var board, out _, out var problem), Is.False);
            Assert.That(problem, Does.Contain("Could not place"));
            Assert.That(board, Is.Null);
        }

        [Test]
        public void TryCreate_ZeroWeightShape_IsNeverUsed()
        {
            var weights = new List<ShapeWeight>
            {
                new ShapeWeight(TestShapes.Single, 1),
                new ShapeWeight(TestShapes.TwoByTwo, 0),
            };

            for (var seed = 0; seed < 30; seed++)
            {
                foreach (var block in Create(MakeSettings(weights: weights), seed, out _).Blocks)
                {
                    Assert.That(block.Shape, Is.Not.SameAs(TestShapes.TwoByTwo), $"Seed {seed}");
                }
            }
        }

        [Test]
        public void TryCreate_ShapeTooWideForAColorsDoor_IsNotUsedForThatColor()
        {
            // Red door 1 wide, blue door 2 wide, on top and bottom: a 2-wide block can only be blue.
            var doors = new List<Door>
            {
                MakeDoor(BoardSide.Top, 2, 1, "red"),
                MakeDoor(BoardSide.Bottom, 2, 2, "blue"),
            };
            var weights = new List<ShapeWeight>
            {
                new ShapeWeight(TestShapes.Single, 1),
                new ShapeWeight(TestShapes.TwoWide, 3),
            };

            for (var seed = 0; seed < 30; seed++)
            {
                foreach (var block in Create(MakeSettings(weights: weights, doors: doors), seed, out _).Blocks)
                {
                    if (block.Color == "red")
                    {
                        Assert.That(block.Shape, Is.SameAs(TestShapes.Single), $"Seed {seed}");
                    }
                }
            }
        }
    }
}
