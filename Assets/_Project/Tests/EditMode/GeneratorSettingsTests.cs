using System.Collections.Generic;
using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class GeneratorSettingsTests
    {
        private static Door MakeDoor(BoardSide side, int position, int openingSize, string color)
        {
            Assert.That(Door.TryCreate(side, position, openingSize, color, out var door, out var error), Is.True, error);
            return door;
        }

        private static List<Door> TwoDoors()
        {
            return new List<Door>
            {
                MakeDoor(BoardSide.Left, 2, 2, "red"),
                MakeDoor(BoardSide.Right, 2, 2, "blue"),
            };
        }

        private static List<ShapeWeight> Weights(params (ShapeDefinition shape, int weight)[] entries)
        {
            var list = new List<ShapeWeight>();
            foreach (var entry in entries)
            {
                list.Add(new ShapeWeight(entry.shape, entry.weight));
            }

            return list;
        }

        private static List<ColorCount> Colors(params (string color, int count)[] entries)
        {
            var list = new List<ColorCount>();
            foreach (var entry in entries)
            {
                list.Add(new ColorCount(entry.color, entry.count));
            }

            return list;
        }

        private static GeneratorSettings Make(
            int width = 6,
            int height = 6,
            List<Door> doors = null,
            List<ColorCount> colors = null,
            List<ShapeWeight> weights = null,
            int minDoors = Board.MinDoors,
            int maxDoors = Board.MaxDoors)
        {
            return new GeneratorSettings(
                width,
                height,
                doors ?? TwoDoors(),
                colors ?? Colors(("red", 2), ("blue", 2)),
                weights ?? Weights((TestShapes.Single, 1), (TestShapes.TwoWide, 2)),
                minDoors,
                maxDoors);
        }

        private static void AssertOneProblemContaining(GeneratorSettings settings, string text)
        {
            var problems = settings.FindProblems();
            Assert.That(problems, Has.Count.EqualTo(1), string.Join("\n", problems));
            Assert.That(problems[0], Does.Contain(text));
        }

        [Test]
        public void FindProblems_GoodSettings_ReturnsNone()
        {
            Assert.That(Make().FindProblems(), Is.Empty);
        }

        [TestCase(0, 6)]
        [TestCase(6, 0)]
        [TestCase(4, 6)]
        [TestCase(6, 4)]
        public void FindProblems_BoardSmallerThan5x5_SaysSize(int width, int height)
        {
            AssertOneProblemContaining(Make(width, height), "at least 5x5");
        }

        [Test]
        public void FindProblems_Board5x5_IsFine()
        {
            Assert.That(Make(5, 5).FindProblems(), Is.Empty);
        }

        [Test]
        public void FindProblems_DoorCountWithinCustomLimits_IsFine()
        {
            Assert.That(Make(doors: new List<Door> { MakeDoor(BoardSide.Left, 0, 6, "red") }, colors: Colors(("red", 4)), minDoors: 1, maxDoors: 1).FindProblems(), Is.Empty);
        }

        [Test]
        public void FindProblems_DoorCountOutsideCustomLimits_SaysLimits()
        {
            AssertOneProblemContaining(Make(minDoors: 3, maxDoors: 4), "3 to 4 doors");
        }

        [Test]
        public void FindProblems_MinDoorsZero_SaysWayOut()
        {
            AssertOneProblemContaining(Make(minDoors: 0), "Min Doors must be at least 1");
        }

        [Test]
        public void FindProblems_MaxBelowMin_SaysLimitsSwapped()
        {
            AssertOneProblemContaining(Make(minDoors: 3, maxDoors: 2), "less than Min Doors");
        }

        [Test]
        public void FindProblems_OneDoor_SaysDoorCount()
        {
            AssertOneProblemContaining(Make(doors: new List<Door> { MakeDoor(BoardSide.Left, 0, 6, "red") }, colors: Colors(("red", 4))), "2 to 4 doors");
        }

        [Test]
        public void FindProblems_FiveDoors_SaysDoorCount()
        {
            var doors = new List<Door>
            {
                MakeDoor(BoardSide.Left, 1, 1, "a"),
                MakeDoor(BoardSide.Right, 1, 1, "b"),
                MakeDoor(BoardSide.Top, 1, 1, "c"),
                MakeDoor(BoardSide.Bottom, 1, 1, "d"),
                MakeDoor(BoardSide.Left, 3, 1, "e"),
            };
            AssertOneProblemContaining(Make(doors: doors, colors: Colors(("a", 1)), weights: Weights((TestShapes.Single, 1))), "2 to 4 doors");
        }

        [Test]
        public void FindProblems_DoorOffTheEdge_NamesTheDoor()
        {
            var doors = TwoDoors();
            doors.Add(MakeDoor(BoardSide.Top, 5, 2, "green"));
            AssertOneProblemContaining(Make(doors: doors), "Door 3:");
        }

        [Test]
        public void FindProblems_EmptyDoor_SaysEmpty()
        {
            var doors = TwoDoors();
            doors.Add(null);
            AssertOneProblemContaining(Make(doors: doors), "Door 3 is empty");
        }

        [Test]
        public void FindProblems_SameColorDoorsShareCorner_SaysCorner()
        {
            var doors = new List<Door>
            {
                MakeDoor(BoardSide.Left, 0, 2, "red"),
                MakeDoor(BoardSide.Bottom, 0, 2, "red"),
            };
            AssertOneProblemContaining(Make(doors: doors, colors: Colors(("red", 4))), "shares a corner");
        }

        [Test]
        public void FindProblems_ZeroBlocks_SaysNoBlocks()
        {
            AssertOneProblemContaining(Make(colors: Colors(("red", 0), ("blue", 0))), "No blocks to place");
        }

        [Test]
        public void FindProblems_ColorWithoutDoor_SaysAddDoor()
        {
            AssertOneProblemContaining(Make(colors: Colors(("red", 2), ("green", 1))), "'green' has blocks but no door");
        }

        [Test]
        public void FindProblems_ColorWithoutDoorAndZeroCount_IsFine()
        {
            Assert.That(Make(colors: Colors(("red", 2), ("green", 0))).FindProblems(), Is.Empty);
        }

        [Test]
        public void FindProblems_ColorListedTwice_SaysDuplicate()
        {
            AssertOneProblemContaining(Make(colors: Colors(("red", 2), ("red", 1))), "Color 'red' is listed more than once");
        }

        [Test]
        public void FindProblems_NegativeColorCount_SaysNegative()
        {
            AssertOneProblemContaining(Make(colors: Colors(("red", 2), ("blue", -1))), "Counts cannot be negative");
        }

        [Test]
        public void FindProblems_EmptyColorName_SaysRow()
        {
            AssertOneProblemContaining(Make(colors: Colors(("red", 2), ("", 1))), "Color row 2 has no color");
        }

        [Test]
        public void FindProblems_NoShapeFitsOneColorsDoors_NamesTheColor()
        {
            var doors = new List<Door>
            {
                MakeDoor(BoardSide.Left, 2, 1, "red"),
                MakeDoor(BoardSide.Right, 2, 2, "blue"),
            };

            // 1 wide and 2 tall: on a side door it needs an opening of 2, so only blue fits.
            AssertOneProblemContaining(Make(doors: doors, weights: Weights((TestShapes.TwoTall, 1))), "No shape fits through any red door");
        }

        [Test]
        public void BlockCount_IsTheSumOfColorCounts()
        {
            Assert.That(Make(colors: Colors(("red", 3), ("blue", 2))).BlockCount, Is.EqualTo(5));
        }

        [Test]
        public void FindProblems_AllWeightsZero_SaysNoShape()
        {
            AssertOneProblemContaining(Make(weights: Weights((TestShapes.Single, 0))), "No shape can be picked");
        }

        [Test]
        public void FindProblems_NegativeWeight_SaysNegative()
        {
            AssertOneProblemContaining(Make(weights: Weights((TestShapes.Single, 1), (TestShapes.TwoWide, -1))), "cannot be negative");
        }

        [Test]
        public void FindProblems_ShapeListedTwice_SaysDuplicate()
        {
            AssertOneProblemContaining(Make(weights: Weights((TestShapes.Single, 1), (TestShapes.Single, 1))), "more than once");
        }

        [Test]
        public void FindProblems_MissingShape_SaysRow()
        {
            AssertOneProblemContaining(Make(weights: Weights((TestShapes.Single, 1), (null, 1))), "Shape row 2 has no shape");
        }

        [Test]
        public void FindProblems_LShapeWithWeight_SaysRectanglesOnly()
        {
            AssertOneProblemContaining(Make(weights: Weights((TestShapes.Single, 1), (TestShapes.LShape, 1))), "not a rectangle");
        }

        [Test]
        public void FindProblems_LShapeWithZeroWeight_IsFine()
        {
            Assert.That(Make(weights: Weights((TestShapes.Single, 1), (TestShapes.LShape, 0))).FindProblems(), Is.Empty);
        }

        [Test]
        public void FindProblems_ShapeWiderThanEveryOpening_SaysWidenDoor()
        {
            var doors = new List<Door>
            {
                MakeDoor(BoardSide.Left, 2, 1, "red"),
                MakeDoor(BoardSide.Right, 2, 1, "blue"),
            };
            AssertOneProblemContaining(Make(doors: doors, weights: Weights((TestShapes.Single, 1), (TestShapes.TwoByTwo, 1))), "fits through no door");
        }

        [Test]
        public void FindProblems_TooManyBlocksForBoard_SaysCells()
        {
            AssertOneProblemContaining(Make(colors: Colors(("red", 37)), weights: Weights((TestShapes.Single, 1))), "only 36");
        }

        [Test]
        public void FindProblems_BoardExactlyFull_IsFine()
        {
            Assert.That(Make(colors: Colors(("red", 18), ("blue", 18)), weights: Weights((TestShapes.Single, 1))).FindProblems(), Is.Empty);
        }

        [Test]
        public void FindProblems_SeveralProblems_ReportsAll()
        {
            Assert.That(Make(doors: new List<Door>(), colors: Colors(), weights: Weights()).FindProblems(), Has.Count.EqualTo(3));
        }

        [Test]
        public void CreateEmptyBoard_GoodSettings_HasSizeAndDoorsInOrder()
        {
            var settings = Make();
            var board = settings.CreateEmptyBoard();
            Assert.That(board.Width, Is.EqualTo(6));
            Assert.That(board.Height, Is.EqualTo(6));
            Assert.That(board.Doors, Is.EqualTo(settings.Doors));
            Assert.That(board.Blocks, Is.Empty);
        }

        [TestCase(BoardSide.Top, 2, true)]
        [TestCase(BoardSide.Top, 1, false)]
        [TestCase(BoardSide.Left, 1, true)]
        public void ShapeFitsDoor_MeasuresAlongTheEdge(BoardSide side, int opening, bool expected)
        {
            var door = MakeDoor(side, 0, opening, "red");
            Assert.That(GeneratorSettings.ShapeFitsDoor(TestShapes.TwoWide, door), Is.EqualTo(expected));
        }
    }
}
