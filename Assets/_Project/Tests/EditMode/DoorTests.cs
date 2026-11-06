using System;
using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class DoorTests
    {
        private static Door MakeDoor(BoardSide side, int position, int openingSize)
        {
            Assert.That(Door.TryCreate(side, position, openingSize, "red", out var door, out var error), Is.True, error);
            return door;
        }

        [Test]
        public void TryCreate_ValidData_KeepsValues()
        {
            var created = Door.TryCreate(BoardSide.Left, 1, 2, "red", out var door, out var error);

            Assert.That(created, Is.True, error);
            Assert.That(door.Side, Is.EqualTo(BoardSide.Left));
            Assert.That(door.Position, Is.EqualTo(1));
            Assert.That(door.OpeningSize, Is.EqualTo(2));
            Assert.That(door.Color, Is.EqualTo("red"));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void TryCreate_OpeningTooSmall_Fails(int openingSize)
        {
            Assert.That(Door.TryCreate(BoardSide.Left, 0, openingSize, "red", out _, out var error), Is.False);
            Assert.That(error, Is.Not.Empty);
        }

        [Test]
        public void TryCreate_NegativePosition_Fails()
        {
            Assert.That(Door.TryCreate(BoardSide.Left, -1, 1, "red", out _, out var error), Is.False);
            Assert.That(error, Is.Not.Empty);
        }

        [Test]
        public void TryCreate_BlankColor_Fails()
        {
            Assert.That(Door.TryCreate(BoardSide.Left, 0, 1, " ", out _, out var error), Is.False);
            Assert.That(error, Is.Not.Empty);
        }

        [TestCase(BoardSide.Top, Direction.Up)]
        [TestCase(BoardSide.Bottom, Direction.Down)]
        [TestCase(BoardSide.Left, Direction.Left)]
        [TestCase(BoardSide.Right, Direction.Right)]
        public void ExitDirection_EachSide_PointsOutOfTheBoard(BoardSide side, Direction expected)
        {
            Assert.That(MakeDoor(side, 0, 1).ExitDirection, Is.EqualTo(expected));
        }

        // A 6 wide by 4 tall board: Top/Bottom edges have length 6, Left/Right have length 4.
        [TestCase(BoardSide.Left, 2, 2, true)]
        [TestCase(BoardSide.Left, 3, 2, false)]
        [TestCase(BoardSide.Right, 3, 2, false)]
        [TestCase(BoardSide.Top, 4, 2, true)]
        [TestCase(BoardSide.Top, 5, 2, false)]
        [TestCase(BoardSide.Bottom, 4, 2, true)]
        public void TryAddDoor_MeasuresAlongItsOwnEdge(BoardSide side, int position, int openingSize, bool expected)
        {
            var board = new Board(6, 4);

            Assert.That(board.TryAddDoor(MakeDoor(side, position, openingSize), out _), Is.EqualTo(expected));
            Assert.That(board.Doors.Count, Is.EqualTo(expected ? 1 : 0));
        }

        [Test]
        public void TryAddDoor_OverlapsDoorOnSameEdge_FailsAndChangesNothing()
        {
            var board = new Board(6, 6);
            board.TryAddDoor(MakeDoor(BoardSide.Left, 1, 2), out _);

            var added = board.TryAddDoor(MakeDoor(BoardSide.Left, 2, 2), out var error);

            Assert.That(added, Is.False);
            Assert.That(error, Is.Not.Empty);
            Assert.That(board.Doors.Count, Is.EqualTo(1));
        }

        [Test]
        public void TryAddDoor_NextToDoorOnSameEdge_Succeeds()
        {
            var board = new Board(6, 6);
            board.TryAddDoor(MakeDoor(BoardSide.Left, 1, 2), out _);

            Assert.That(board.TryAddDoor(MakeDoor(BoardSide.Left, 3, 2), out _), Is.True);
        }

        [Test]
        public void TryAddDoor_SamePositionOnDifferentEdge_Succeeds()
        {
            var board = new Board(6, 6);
            board.TryAddDoor(MakeDoor(BoardSide.Left, 1, 2), out _);

            Assert.That(board.TryAddDoor(MakeDoor(BoardSide.Right, 1, 2), out _), Is.True);
        }

        [Test]
        public void TryAddDoor_NullDoor_Throws()
        {
            var board = new Board(6, 6);

            Assert.Throws<ArgumentNullException>(() => board.TryAddDoor(null, out _));
        }

        // Every door covers columns or rows 1 and 2, on a 6 wide by 4 tall board.
        [TestCase(BoardSide.Left, 0, 1, true)]
        [TestCase(BoardSide.Left, 0, 2, true)]
        [TestCase(BoardSide.Left, 0, 0, false)]
        [TestCase(BoardSide.Left, 0, 3, false)]
        [TestCase(BoardSide.Left, 1, 1, false)]
        [TestCase(BoardSide.Left, -1, 1, false)]
        [TestCase(BoardSide.Right, 5, 1, true)]
        [TestCase(BoardSide.Right, 4, 1, false)]
        [TestCase(BoardSide.Top, 1, 3, true)]
        [TestCase(BoardSide.Top, 2, 3, true)]
        [TestCase(BoardSide.Top, 3, 3, false)]
        [TestCase(BoardSide.Top, 1, 2, false)]
        [TestCase(BoardSide.Bottom, 1, 0, true)]
        [TestCase(BoardSide.Bottom, 1, 1, false)]
        public void IsTouchingDoor_Cell_MustBeOnEdgeAndInsideOpening(BoardSide side, int x, int y, bool expected)
        {
            var board = new Board(6, 4);
            var door = MakeDoor(side, 1, 2);

            Assert.That(board.IsTouchingDoor(door, new GridPosition(x, y)), Is.EqualTo(expected));
        }
    }
}
