using System;
using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    /// <summary>Rectangles only. L shapes are on hold; see docs/design/puzzle-rules.md.</summary>
    public sealed class ExitRuleTests
    {
        private Board board;

        [SetUp]
        public void SetUp()
        {
            board = new Board(6, 6);
        }

        private Door AddDoor(BoardSide side, int position, int openingSize, string color = "red")
        {
            Assert.That(Door.TryCreate(side, position, openingSize, color, out var door, out var error), Is.True, error);
            Assert.That(board.TryAddDoor(door, out error), Is.True, error);
            return door;
        }

        private Block AddBlock(ShapeDefinition shape, int x, int y, string color = "red")
        {
            Assert.That(board.TryAddBlock(shape, new GridPosition(x, y), color, out var block), Is.True);
            return block;
        }

        [Test]
        public void CanExit_ExactFitOnTop_True()
        {
            var door = AddDoor(BoardSide.Top, 2, 2);
            var block = AddBlock(TestShapes.TwoWide, 2, 5);

            Assert.That(board.CanExitThroughDoor(block, door), Is.True);
        }

        [Test]
        public void CanExit_ExactFitOnBottom_True()
        {
            var door = AddDoor(BoardSide.Bottom, 2, 2);
            var block = AddBlock(TestShapes.TwoWide, 2, 0);

            Assert.That(board.CanExitThroughDoor(block, door), Is.True);
        }

        [Test]
        public void CanExit_ExactFitOnLeft_True()
        {
            var door = AddDoor(BoardSide.Left, 2, 2);
            var block = AddBlock(TestShapes.TwoTall, 0, 2);

            Assert.That(board.CanExitThroughDoor(block, door), Is.True);
        }

        [Test]
        public void CanExit_ExactFitOnRight_True()
        {
            var door = AddDoor(BoardSide.Right, 2, 2);
            var block = AddBlock(TestShapes.TwoTall, 5, 2);

            Assert.That(board.CanExitThroughDoor(block, door), Is.True);
        }

        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void CanExit_NarrowerThanOpening_TrueAnywhereInside(int row)
        {
            var door = AddDoor(BoardSide.Left, 2, 3);
            var block = AddBlock(TestShapes.Single, 0, row);

            Assert.That(board.CanExitThroughDoor(block, door), Is.True);
        }

        [Test]
        public void CanExit_WiderThanOpening_False()
        {
            var door = AddDoor(BoardSide.Left, 2, 1);
            var block = AddBlock(TestShapes.TwoByTwo, 0, 2);

            Assert.That(board.CanExitThroughDoor(block, door), Is.False);
        }

        [TestCase(1)]
        [TestCase(3)]
        public void CanExit_PartialOverlap_False(int row)
        {
            var door = AddDoor(BoardSide.Left, 2, 2);
            var block = AddBlock(TestShapes.TwoTall, 0, row);

            Assert.That(board.CanExitThroughDoor(block, door), Is.False);
        }

        [Test]
        public void CanExit_WrongColor_False()
        {
            var door = AddDoor(BoardSide.Left, 2, 2, "blue");
            var block = AddBlock(TestShapes.TwoTall, 0, 2, "red");

            Assert.That(board.CanExitThroughDoor(block, door), Is.False);
        }

        [Test]
        public void CanExit_OneCellAwayFromDoor_False()
        {
            var door = AddDoor(BoardSide.Left, 2, 2);
            var block = AddBlock(TestShapes.TwoTall, 1, 2);

            Assert.That(board.CanExitThroughDoor(block, door), Is.False);
        }

        [Test]
        public void CanExit_AgainstOppositeEdge_False()
        {
            var door = AddDoor(BoardSide.Left, 2, 2);
            var block = AddBlock(TestShapes.TwoTall, 5, 2);

            Assert.That(board.CanExitThroughDoor(block, door), Is.False);
        }

        [Test]
        public void CanExit_TallBlockThroughTopDoor_OnlyWidthCounts()
        {
            var door = AddDoor(BoardSide.Top, 2, 1);
            var block = AddBlock(TestShapes.TwoTall, 2, 4);

            Assert.That(board.CanExitThroughDoor(block, door), Is.True);
        }

        [Test]
        public void CanExit_WideBlockThroughLeftDoor_OnlyHeightCounts()
        {
            var door = AddDoor(BoardSide.Left, 2, 1);
            var block = AddBlock(TestShapes.TwoWide, 0, 2);

            Assert.That(board.CanExitThroughDoor(block, door), Is.True);
        }

        [Test]
        public void CanExit_NullBlock_Throws()
        {
            var door = AddDoor(BoardSide.Left, 2, 2);

            Assert.Throws<ArgumentNullException>(() => board.CanExitThroughDoor(null, door));
        }

        [Test]
        public void CanExit_NullDoor_Throws()
        {
            var block = AddBlock(TestShapes.Single, 0, 2);

            Assert.Throws<ArgumentNullException>(() => board.CanExitThroughDoor(block, null));
        }

        [Test]
        public void CanExit_DoorNotOnThisBoard_Throws()
        {
            var block = AddBlock(TestShapes.Single, 0, 2);
            Door.TryCreate(BoardSide.Left, 2, 2, "red", out var looseDoor, out _);

            Assert.Throws<ArgumentException>(() => board.CanExitThroughDoor(block, looseDoor));
        }

        [Test]
        public void CanExit_BlockNotOnThisBoard_Throws()
        {
            var door = AddDoor(BoardSide.Left, 2, 2);
            new Board(6, 6).TryAddBlock(TestShapes.Single, new GridPosition(0, 2), "red", out var otherBlock);

            Assert.Throws<ArgumentException>(() => board.CanExitThroughDoor(otherBlock, door));
        }
    }
}
