using System;
using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class BoardMoveTests
    {
        private static GridPosition At(int x, int y)
        {
            return new GridPosition(x, y);
        }

        private static Block Add(Board board, ShapeDefinition shape, int x, int y)
        {
            Assert.That(board.TryAddBlock(shape, At(x, y), "red", out var block), Is.True);
            return block;
        }

        [TestCase(Direction.Up, 2, 3)]
        [TestCase(Direction.Down, 2, 1)]
        [TestCase(Direction.Left, 1, 2)]
        [TestCase(Direction.Right, 3, 2)]
        public void TryMove_FreeCell_MovesOneStep(Direction direction, int expectedX, int expectedY)
        {
            var board = new Board(6, 6);
            var block = Add(board, TestShapes.Single, 2, 2);

            Assert.That(board.TryMove(block, direction), Is.True);
            Assert.That(block.Position, Is.EqualTo(At(expectedX, expectedY)));
        }

        [Test]
        public void TryMove_Success_FreesOldCellsAndTakesNewOnes()
        {
            var board = new Board(6, 6);
            var block = Add(board, TestShapes.TwoWide, 1, 1);

            board.TryMove(block, Direction.Up);

            Assert.That(board.GetBlockAt(At(1, 1)), Is.Null);
            Assert.That(board.GetBlockAt(At(2, 1)), Is.Null);
            Assert.That(board.GetBlockAt(At(1, 2)), Is.SameAs(block));
            Assert.That(board.GetBlockAt(At(2, 2)), Is.SameAs(block));
        }

        [TestCase(Direction.Up, 0, 5)]
        [TestCase(Direction.Down, 0, 0)]
        [TestCase(Direction.Left, 0, 0)]
        [TestCase(Direction.Right, 5, 0)]
        public void TryMove_IntoWall_FailsAndStaysPut(Direction direction, int x, int y)
        {
            var board = new Board(6, 6);
            var block = Add(board, TestShapes.Single, x, y);

            Assert.That(board.TryMove(block, direction), Is.False);
            Assert.That(block.Position, Is.EqualTo(At(x, y)));
            Assert.That(board.GetBlockAt(At(x, y)), Is.SameAs(block));
        }

        [Test]
        public void TryMove_IntoAnotherBlock_FailsAndChangesNothing()
        {
            var board = new Board(6, 6);
            var mover = Add(board, TestShapes.TwoWide, 0, 0);
            var obstacle = Add(board, TestShapes.Single, 2, 0);

            Assert.That(board.TryMove(mover, Direction.Right), Is.False);
            Assert.That(mover.Position, Is.EqualTo(At(0, 0)));
            Assert.That(board.GetBlockAt(At(1, 0)), Is.SameAs(mover));
            Assert.That(board.GetBlockAt(At(2, 0)), Is.SameAs(obstacle));
        }

        [Test]
        public void TryMove_OverItsOwnCells_Succeeds()
        {
            var board = new Board(6, 6);
            var block = Add(board, TestShapes.TwoWide, 0, 0);

            Assert.That(board.TryMove(block, Direction.Right), Is.True);
            Assert.That(board.GetBlockAt(At(0, 0)), Is.Null);
            Assert.That(board.GetBlockAt(At(1, 0)), Is.SameAs(block));
            Assert.That(board.GetBlockAt(At(2, 0)), Is.SameAs(block));
        }

        [Test]
        public void TryMove_LShapeEmptyCorner_DoesNotBlock()
        {
            var board = new Board(6, 6);
            var lShape = Add(board, TestShapes.LShape, 0, 0);
            Add(board, TestShapes.Single, 2, 1);

            Assert.That(board.TryMove(lShape, Direction.Right), Is.True);
            Assert.That(lShape.Position, Is.EqualTo(At(1, 0)));
        }

        [Test]
        public void TryMove_LShapeCellHitsBlock_Fails()
        {
            var board = new Board(6, 6);
            var lShape = Add(board, TestShapes.LShape, 0, 0);
            Add(board, TestShapes.Single, 1, 1);

            Assert.That(board.TryMove(lShape, Direction.Up), Is.False);
            Assert.That(lShape.Position, Is.EqualTo(At(0, 0)));
        }

        [Test]
        public void CanMove_FreeCell_DoesNotMoveTheBlock()
        {
            var board = new Board(6, 6);
            var block = Add(board, TestShapes.Single, 2, 2);

            Assert.That(board.CanMove(block, Direction.Up), Is.True);
            Assert.That(block.Position, Is.EqualTo(At(2, 2)));
        }

        [Test]
        public void TryMove_BlockFromAnotherBoard_Throws()
        {
            var board = new Board(6, 6);
            var other = Add(new Board(6, 6), TestShapes.Single, 2, 2);

            Assert.Throws<ArgumentException>(() => board.TryMove(other, Direction.Up));
        }

        [Test]
        public void TryMove_NullBlock_Throws()
        {
            var board = new Board(6, 6);

            Assert.Throws<ArgumentNullException>(() => board.TryMove(null, Direction.Up));
        }
    }
}
