using System;
using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class BoardExitTests
    {
        private Board board;
        private Door redLeftDoor;

        [SetUp]
        public void SetUp()
        {
            board = new Board(6, 6);
            Assert.That(Door.TryCreate(BoardSide.Left, 0, 2, "red", out redLeftDoor, out var error), Is.True, error);
            Assert.That(board.TryAddDoor(redLeftDoor, out error), Is.True, error);
        }

        private Block AddBlock(ShapeDefinition shape, int x, int y, string color = "red")
        {
            Assert.That(board.TryAddBlock(shape, new GridPosition(x, y), color, out var block), Is.True);
            return block;
        }

        [Test]
        public void TryExit_BlockFits_RemovesIt()
        {
            var block = AddBlock(TestShapes.Single, 0, 0);

            Assert.That(board.TryExit(block, redLeftDoor), Is.True);
            Assert.That(board.Blocks, Is.Empty);
        }

        [Test]
        public void TryExit_Success_FreesAllItsCells()
        {
            var block = AddBlock(TestShapes.TwoByTwo, 0, 0);

            board.TryExit(block, redLeftDoor);

            Assert.That(board.GetBlockAt(new GridPosition(0, 0)), Is.Null);
            Assert.That(board.GetBlockAt(new GridPosition(1, 0)), Is.Null);
            Assert.That(board.GetBlockAt(new GridPosition(0, 1)), Is.Null);
            Assert.That(board.GetBlockAt(new GridPosition(1, 1)), Is.Null);
        }

        [Test]
        public void TryExit_Success_LeavesOtherBlocksAlone()
        {
            var leaving = AddBlock(TestShapes.Single, 0, 0);
            var staying = AddBlock(TestShapes.Single, 3, 3);

            board.TryExit(leaving, redLeftDoor);

            Assert.That(board.Blocks, Is.EqualTo(new[] { staying }));
            Assert.That(board.GetBlockAt(new GridPosition(3, 3)), Is.SameAs(staying));
        }

        [Test]
        public void TryExit_WrongColor_FailsAndChangesNothing()
        {
            var block = AddBlock(TestShapes.Single, 0, 0, "blue");

            Assert.That(board.TryExit(block, redLeftDoor), Is.False);
            Assert.That(board.Blocks, Has.Count.EqualTo(1));
            Assert.That(board.GetBlockAt(new GridPosition(0, 0)), Is.SameAs(block));
        }

        [Test]
        public void TryExit_NotTouchingDoor_FailsAndChangesNothing()
        {
            var block = AddBlock(TestShapes.Single, 1, 0);

            Assert.That(board.TryExit(block, redLeftDoor), Is.False);
            Assert.That(board.Blocks, Has.Count.EqualTo(1));
            Assert.That(board.GetBlockAt(new GridPosition(1, 0)), Is.SameAs(block));
        }

        [Test]
        public void IsCleared_NoBlocks_True()
        {
            Assert.That(board.IsCleared, Is.True);
        }

        [Test]
        public void IsCleared_BlockOnBoard_False()
        {
            AddBlock(TestShapes.Single, 3, 3);

            Assert.That(board.IsCleared, Is.False);
        }

        [Test]
        public void IsCleared_OneOfTwoExited_False()
        {
            var leaving = AddBlock(TestShapes.Single, 0, 0);
            AddBlock(TestShapes.Single, 3, 3);

            board.TryExit(leaving, redLeftDoor);

            Assert.That(board.IsCleared, Is.False);
        }

        [Test]
        public void IsCleared_LastBlockExited_True()
        {
            var block = AddBlock(TestShapes.Single, 0, 0);

            board.TryExit(block, redLeftDoor);

            Assert.That(board.IsCleared, Is.True);
        }

        [Test]
        public void TryMove_ExitedBlock_Throws()
        {
            var block = AddBlock(TestShapes.Single, 0, 0);
            board.TryExit(block, redLeftDoor);

            Assert.Throws<ArgumentException>(() => board.TryMove(block, Direction.Up));
        }

        [Test]
        public void TryExit_SameBlockTwice_Throws()
        {
            var block = AddBlock(TestShapes.Single, 0, 0);
            board.TryExit(block, redLeftDoor);

            Assert.Throws<ArgumentException>(() => board.TryExit(block, redLeftDoor));
        }

        [Test]
        public void TryAddBlock_AfterAnExit_NeverReusesAnId()
        {
            var first = AddBlock(TestShapes.Single, 0, 0);
            board.TryExit(first, redLeftDoor);

            var second = AddBlock(TestShapes.Single, 0, 0);

            Assert.That(second.Id, Is.Not.EqualTo(first.Id));
        }
    }
}
