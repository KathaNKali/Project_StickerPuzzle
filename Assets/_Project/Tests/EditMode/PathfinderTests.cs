using System.Collections.Generic;
using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class PathfinderTests
    {
        private const Direction U = Direction.Up, D = Direction.Down, L = Direction.Left, R = Direction.Right;

        private Board board;
        private Pathfinder pathfinder;
        private List<Direction> route;

        [SetUp]
        public void SetUp()
        {
            board = new Board(5, 5);
            pathfinder = new Pathfinder();
            route = new List<Direction>();
        }

        private Door AddDoor(BoardSide side, int position, int openingSize, string color)
        {
            Assert.That(Door.TryCreate(side, position, openingSize, color, out var door, out var error), Is.True, error);
            Assert.That(board.TryAddDoor(door, out error), Is.True, error);
            return door;
        }

        private Block AddBlock(ShapeDefinition shape, int x, int y, string color)
        {
            Assert.That(board.TryAddBlock(shape, new GridPosition(x, y), color, out var block), Is.True);
            return block;
        }

        /// <summary>Plays the route on a copy of the board with the real rules; the block must end able to leave.</summary>
        private void AssertRouteWorks(Block block, Door door)
        {
            var copy = board.Clone();
            var moving = copy.GetBlockById(block.Id);
            foreach (var step in route)
            {
                Assert.That(copy.TryMove(moving, step), Is.True, $"Step {step} was blocked.");
            }

            Assert.That(copy.CanExitThroughDoor(moving, door), Is.True);
        }

        [Test]
        public void TryFindRoute_ClearPath_GoesStraight()
        {
            var door = AddDoor(BoardSide.Left, 2, 1, "red");
            var block = AddBlock(TestShapes.Single, 3, 2, "red");

            Assert.That(pathfinder.TryFindRoute(board, block, door, route), Is.True);
            Assert.That(route, Is.EqualTo(new[] { L, L, L }));
            AssertRouteWorks(block, door);
        }

        [Test]
        public void TryFindRoute_BlockInTheWay_GoesAroundIt()
        {
            var door = AddDoor(BoardSide.Left, 2, 1, "red");
            var block = AddBlock(TestShapes.Single, 3, 2, "red");
            AddBlock(TestShapes.Single, 1, 2, "blue");

            Assert.That(pathfinder.TryFindRoute(board, block, door, route), Is.True);
            Assert.That(route, Has.Count.EqualTo(5));
            AssertRouteWorks(block, door);
        }

        [Test]
        public void TryFindRoute_BiggerShape_UsesItsWholeWidth()
        {
            var door = AddDoor(BoardSide.Top, 1, 2, "red");
            var block = AddBlock(TestShapes.TwoByTwo, 1, 0, "red");

            Assert.That(pathfinder.TryFindRoute(board, block, door, route), Is.True);
            Assert.That(route, Is.EqualTo(new[] { U, U, U }));
            AssertRouteWorks(block, door);
        }

        [Test]
        public void TryFindRoute_WalledIn_ReturnsFalseAndEmptyRoute()
        {
            var door = AddDoor(BoardSide.Left, 2, 1, "red");
            var block = AddBlock(TestShapes.Single, 2, 2, "red");
            AddBlock(TestShapes.Single, 1, 2, "blue");
            AddBlock(TestShapes.Single, 3, 2, "blue");
            AddBlock(TestShapes.Single, 2, 1, "blue");
            AddBlock(TestShapes.Single, 2, 3, "blue");
            route.Add(U);

            Assert.That(pathfinder.TryFindRoute(board, block, door, route), Is.False);
            Assert.That(route, Is.Empty);
        }

        [Test]
        public void TryFindRoute_DoorOfAnotherColor_ReturnsFalse()
        {
            var door = AddDoor(BoardSide.Left, 2, 1, "blue");
            var block = AddBlock(TestShapes.Single, 3, 2, "red");

            Assert.That(pathfinder.TryFindRoute(board, block, door, route), Is.False);
        }

        [Test]
        public void TryFindRoute_OpeningTooNarrow_ReturnsFalse()
        {
            var door = AddDoor(BoardSide.Left, 2, 1, "red");
            var block = AddBlock(TestShapes.TwoTall, 3, 2, "red");

            Assert.That(pathfinder.TryFindRoute(board, block, door, route), Is.False);
        }

        [Test]
        public void TryFindRoute_AlreadyAtExitAndBoxedIn_StepsAwayAndBack()
        {
            var door = AddDoor(BoardSide.Left, 2, 1, "red");
            var block = AddBlock(TestShapes.Single, 0, 2, "red");
            AddBlock(TestShapes.Single, 0, 1, "blue");
            AddBlock(TestShapes.Single, 0, 3, "blue");

            Assert.That(pathfinder.TryFindRoute(board, block, door, route), Is.True);
            Assert.That(route, Is.EqualTo(new[] { R, L }));
            AssertRouteWorks(block, door);
        }

        [Test]
        public void TryFindRoute_AlreadyAtWideExit_MovesOneCellAlongIt()
        {
            var door = AddDoor(BoardSide.Left, 1, 3, "red");
            var block = AddBlock(TestShapes.Single, 0, 2, "red");

            Assert.That(pathfinder.TryFindRoute(board, block, door, route), Is.True);
            Assert.That(route, Has.Count.EqualTo(1));
            AssertRouteWorks(block, door);
        }

        [Test]
        public void TryFindRoute_NeverMovesTheBlock()
        {
            var door = AddDoor(BoardSide.Left, 2, 1, "red");
            var block = AddBlock(TestShapes.Single, 3, 2, "red");

            pathfinder.TryFindRoute(board, block, door, route);

            Assert.That(block.Position, Is.EqualTo(new GridPosition(3, 2)));
            Assert.That(board.GetBlockAt(new GridPosition(3, 2)), Is.SameAs(block));
        }

        [Test]
        public void TryFindExit_TwoDoorsOfItsColor_PicksTheNearer()
        {
            AddDoor(BoardSide.Left, 2, 1, "red");
            var right = AddDoor(BoardSide.Right, 2, 1, "red");
            var block = AddBlock(TestShapes.Single, 3, 2, "red");

            Assert.That(pathfinder.TryFindExit(board, block, route, out var door), Is.True);
            Assert.That(door, Is.SameAs(right));
            Assert.That(route, Is.EqualTo(new[] { R }));
        }

        [Test]
        public void TryFindExit_IgnoresDoorsOfOtherColors()
        {
            AddDoor(BoardSide.Right, 2, 1, "blue");
            var left = AddDoor(BoardSide.Left, 2, 1, "red");
            var block = AddBlock(TestShapes.Single, 3, 2, "red");

            Assert.That(pathfinder.TryFindExit(board, block, route, out var door), Is.True);
            Assert.That(door, Is.SameAs(left));
            AssertRouteWorks(block, left);
        }

        [Test]
        public void TryFindExit_NoDoorOfItsColor_ReturnsFalseAndNoDoor()
        {
            AddDoor(BoardSide.Left, 2, 1, "blue");
            var block = AddBlock(TestShapes.Single, 3, 2, "red");

            Assert.That(pathfinder.TryFindExit(board, block, route, out var door), Is.False);
            Assert.That(door, Is.Null);
            Assert.That(route, Is.Empty);
        }

        [Test]
        public void TryFindRoute_SameInstanceOnBoardsOfDifferentSizes_StaysCorrect()
        {
            var sizes = new[] { 8, 5, 8 };
            foreach (var size in sizes)
            {
                board = new Board(size, size);
                var door = AddDoor(BoardSide.Left, 0, 1, "red");
                var block = AddBlock(TestShapes.Single, size - 1, size - 1, "red");

                Assert.That(pathfinder.TryFindRoute(board, block, door, route), Is.True, $"{size}x{size}");
                Assert.That(route, Has.Count.EqualTo((size - 1) * 2), $"{size}x{size}");
                AssertRouteWorks(block, door);
            }
        }

        [Test]
        public void CanOccupy_OwnCellsCountAsFree_OthersDoNot()
        {
            var block = AddBlock(TestShapes.TwoWide, 1, 1, "red");
            AddBlock(TestShapes.Single, 3, 1, "blue");

            Assert.That(board.CanOccupy(block, new GridPosition(0, 1)), Is.True);
            Assert.That(board.CanOccupy(block, new GridPosition(2, 1)), Is.False);
            Assert.That(board.CanOccupy(block, new GridPosition(4, 1)), Is.False);
        }

        [Test]
        public void CanExitThroughDoorAt_MatchesCanExitThroughDoorAfterMoving()
        {
            var door = AddDoor(BoardSide.Left, 2, 1, "red");
            var block = AddBlock(TestShapes.Single, 1, 2, "red");

            Assert.That(board.CanExitThroughDoorAt(block, door, new GridPosition(0, 2)), Is.True);
            Assert.That(board.CanExitThroughDoor(block, door), Is.False);
            Assert.That(board.TryMove(block, L), Is.True);
            Assert.That(board.CanExitThroughDoor(block, door), Is.True);
        }
    }
}
