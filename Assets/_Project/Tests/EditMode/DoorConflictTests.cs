using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class DoorConflictTests
    {
        private Board board;

        [SetUp]
        public void SetUp()
        {
            board = new Board(6, 6);
        }

        private Door MakeDoor(BoardSide side, int position, int openingSize, string color)
        {
            Assert.That(Door.TryCreate(side, position, openingSize, color, out var door, out var error), Is.True, error);
            return door;
        }

        private Door AddDoor(BoardSide side, int position, int openingSize, string color)
        {
            var door = MakeDoor(side, position, openingSize, color);
            Assert.That(board.TryAddDoor(door, out var error), Is.True, error);
            return door;
        }

        [TestCase(BoardSide.Left, 0, 1, BoardSide.Bottom, 0, 1)]
        [TestCase(BoardSide.Left, 4, 2, BoardSide.Top, 0, 1)]
        [TestCase(BoardSide.Right, 0, 1, BoardSide.Bottom, 5, 1)]
        [TestCase(BoardSide.Right, 4, 2, BoardSide.Top, 5, 1)]
        public void TryAddDoor_SameColorSharingACorner_Fails(
            BoardSide firstSide, int firstPosition, int firstOpening,
            BoardSide secondSide, int secondPosition, int secondOpening)
        {
            AddDoor(firstSide, firstPosition, firstOpening, "red");

            var added = board.TryAddDoor(MakeDoor(secondSide, secondPosition, secondOpening, "red"), out var error);

            Assert.That(added, Is.False);
            Assert.That(error, Is.Not.Empty);
            Assert.That(board.Doors.Count, Is.EqualTo(1));
        }

        [Test]
        public void TryAddDoor_DifferentColorsSharingACorner_Succeeds()
        {
            AddDoor(BoardSide.Left, 0, 1, "red");

            Assert.That(board.TryAddDoor(MakeDoor(BoardSide.Bottom, 0, 1, "blue"), out _), Is.True);
        }

        [Test]
        public void TryAddDoor_SameColorNotReachingTheCorner_Succeeds()
        {
            AddDoor(BoardSide.Left, 1, 1, "red");

            Assert.That(board.TryAddDoor(MakeDoor(BoardSide.Bottom, 0, 1, "red"), out _), Is.True);
        }

        [Test]
        public void FindExitDoor_BlockFits_ReturnsThatDoor()
        {
            var door = AddDoor(BoardSide.Left, 0, 2, "red");
            board.TryAddBlock(TestShapes.Single, new GridPosition(0, 0), "red", out var block);

            Assert.That(board.FindExitDoor(block), Is.SameAs(door));
        }

        [Test]
        public void FindExitDoor_NothingFits_ReturnsNull()
        {
            AddDoor(BoardSide.Left, 0, 2, "red");
            board.TryAddBlock(TestShapes.Single, new GridPosition(3, 3), "red", out var block);

            Assert.That(board.FindExitDoor(block), Is.Null);
        }

        [Test]
        public void FindExitDoor_TwoColorsInACorner_ReturnsMatchingColor()
        {
            AddDoor(BoardSide.Left, 0, 1, "blue");
            var redDoor = AddDoor(BoardSide.Bottom, 0, 1, "red");
            board.TryAddBlock(TestShapes.Single, new GridPosition(0, 0), "red", out var block);

            Assert.That(board.FindExitDoor(block), Is.SameAs(redDoor));
        }
    }
}
