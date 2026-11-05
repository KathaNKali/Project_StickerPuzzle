using System;
using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class BoardTests
    {
        private static GridPosition At(int x, int y)
        {
            return new GridPosition(x, y);
        }

        [TestCase(0, 5)]
        [TestCase(5, 0)]
        [TestCase(-1, 5)]
        public void Constructor_InvalidSize_Throws(int width, int height)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Board(width, height));
        }

        [Test]
        public void Constructor_ValidSize_StartsEmpty()
        {
            var board = new Board(6, 4);

            Assert.That(board.Width, Is.EqualTo(6));
            Assert.That(board.Height, Is.EqualTo(4));
            Assert.That(board.Blocks, Is.Empty);
        }

        [TestCase(0, 0, true)]
        [TestCase(5, 3, true)]
        [TestCase(6, 0, false)]
        [TestCase(0, 4, false)]
        [TestCase(-1, 0, false)]
        [TestCase(0, -1, false)]
        public void IsInside_Cell_MatchesBoardSize(int x, int y, bool expected)
        {
            var board = new Board(6, 4);

            Assert.That(board.IsInside(At(x, y)), Is.EqualTo(expected));
        }

        [Test]
        public void TryAddBlock_FreeCells_AddsBlock()
        {
            var board = new Board(6, 6);

            var added = board.TryAddBlock(TestShapes.TwoWide, At(1, 2), "red", out var block);

            Assert.That(added, Is.True);
            Assert.That(block.Position, Is.EqualTo(At(1, 2)));
            Assert.That(block.Color, Is.EqualTo("red"));
            Assert.That(block.Shape, Is.SameAs(TestShapes.TwoWide));
            Assert.That(board.Blocks, Has.Count.EqualTo(1));
        }

        [Test]
        public void TryAddBlock_TwoByTwo_TakesExactlyItsCells()
        {
            var board = new Board(6, 6);
            board.TryAddBlock(TestShapes.TwoByTwo, At(1, 1), "red", out var block);

            Assert.That(board.GetBlockAt(At(1, 1)), Is.SameAs(block));
            Assert.That(board.GetBlockAt(At(2, 1)), Is.SameAs(block));
            Assert.That(board.GetBlockAt(At(1, 2)), Is.SameAs(block));
            Assert.That(board.GetBlockAt(At(2, 2)), Is.SameAs(block));
            Assert.That(board.GetBlockAt(At(3, 1)), Is.Null);
            Assert.That(board.GetBlockAt(At(1, 3)), Is.Null);
        }

        [Test]
        public void TryAddBlock_LShape_LeavesItsEmptyCornerFree()
        {
            var board = new Board(6, 6);
            board.TryAddBlock(TestShapes.LShape, At(0, 0), "red", out _);

            Assert.That(board.GetBlockAt(At(1, 1)), Is.Null);
            Assert.That(board.TryAddBlock(TestShapes.Single, At(1, 1), "blue", out _), Is.True);
        }

        [Test]
        public void TryAddBlock_Overlapping_FailsAndChangesNothing()
        {
            var board = new Board(6, 6);
            board.TryAddBlock(TestShapes.Single, At(2, 0), "red", out var first);

            var added = board.TryAddBlock(TestShapes.TwoWide, At(1, 0), "blue", out var second);

            Assert.That(added, Is.False);
            Assert.That(second, Is.Null);
            Assert.That(board.Blocks, Has.Count.EqualTo(1));
            Assert.That(board.GetBlockAt(At(1, 0)), Is.Null);
            Assert.That(board.GetBlockAt(At(2, 0)), Is.SameAs(first));
        }

        [Test]
        public void TryAddBlock_PartlyOffBoard_FailsAndChangesNothing()
        {
            var board = new Board(6, 6);

            var added = board.TryAddBlock(TestShapes.TwoWide, At(5, 0), "red", out _);

            Assert.That(added, Is.False);
            Assert.That(board.Blocks, Is.Empty);
            Assert.That(board.GetBlockAt(At(5, 0)), Is.Null);
        }

        [Test]
        public void GetBlockAt_OffBoard_ReturnsNull()
        {
            var board = new Board(6, 6);

            Assert.That(board.GetBlockAt(At(-1, 0)), Is.Null);
            Assert.That(board.GetBlockAt(At(6, 0)), Is.Null);
        }

        [Test]
        public void TryAddBlock_TwoBlocks_GetDifferentIds()
        {
            var board = new Board(6, 6);
            board.TryAddBlock(TestShapes.Single, At(0, 0), "red", out var first);
            board.TryAddBlock(TestShapes.Single, At(1, 0), "red", out var second);

            Assert.That(first.Id, Is.Not.EqualTo(second.Id));
        }

        [Test]
        public void TryAddBlock_NullShape_Throws()
        {
            var board = new Board(6, 6);

            Assert.Throws<ArgumentNullException>(() => board.TryAddBlock(null, At(0, 0), "red", out _));
        }

        [Test]
        public void TryAddBlock_BlankColor_Throws()
        {
            var board = new Board(6, 6);

            Assert.Throws<ArgumentException>(() => board.TryAddBlock(TestShapes.Single, At(0, 0), " ", out _));
        }
    }
}
