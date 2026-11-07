using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class CheckerTests
    {
        private const Direction U = Direction.Up, D = Direction.Down, L = Direction.Left, R = Direction.Right;
        private const int Red = 0, Blue = 1;
        private const int RedDoor = 0, BlueDoor = 1;

        private Board board;

        // 4x3 board. Red door on the left, blue door on the right, both in the middle row.
        // Blue (ID 1) sits left of red (ID 0), so blue must go around red.
        [SetUp]
        public void SetUp()
        {
            board = new Board(4, 3);
            AddDoor(BoardSide.Left, "red");
            AddDoor(BoardSide.Right, "blue");
            Assert.That(board.TryAddBlock(TestShapes.Single, new GridPosition(2, 1), "red", out _), Is.True);
            Assert.That(board.TryAddBlock(TestShapes.Single, new GridPosition(1, 1), "blue", out _), Is.True);
        }

        private void AddDoor(BoardSide side, string color)
        {
            Assert.That(Door.TryCreate(side, 1, 1, color, out var door, out var error), Is.True, error);
            Assert.That(board.TryAddDoor(door, out error), Is.True, error);
        }

        private static SolutionStep Step(int blockId, int doorIndex, params Direction[] moves)
        {
            return new SolutionStep(blockId, moves, doorIndex);
        }

        private static Solution Solve(params SolutionStep[] steps)
        {
            return new Solution(steps);
        }

        private static Solution Valid()
        {
            return Solve(Step(Blue, BlueDoor, U, R, R, D), Step(Red, RedDoor, L, L));
        }

        [Test]
        public void IsSolvable_ValidSolution_ReturnsTrue()
        {
            Assert.That(Checker.IsSolvable(board, Valid(), out var problem), Is.True, problem);
            Assert.That(problem, Is.Null);
        }

        [Test]
        public void IsSolvable_ValidSolution_LeavesBoardUnchanged()
        {
            Checker.IsSolvable(board, Valid(), out _);

            Assert.That(board.Blocks.Count, Is.EqualTo(2));
            Assert.That(board.GetBlockById(Red).Position, Is.EqualTo(new GridPosition(2, 1)));
            Assert.That(board.GetBlockById(Blue).Position, Is.EqualTo(new GridPosition(1, 1)));
        }

        [Test]
        public void IsSolvable_NoSolution_SaysSo()
        {
            Assert.That(Checker.IsSolvable(board, null, out var problem), Is.False);
            Assert.That(problem, Does.Contain("no saved solution"));

            Assert.That(Checker.IsSolvable(board, Solve(), out problem), Is.False);
            Assert.That(problem, Does.Contain("no saved solution"));
        }

        [Test]
        public void IsSolvable_BlockedMove_NamesStepAndMove()
        {
            var solution = Solve(Step(Red, RedDoor, L, L), Step(Blue, BlueDoor, R, R));

            Assert.That(Checker.IsSolvable(board, solution, out var problem), Is.False);
            Assert.That(problem, Does.StartWith("Step 1:").And.Contain("cannot move Left (move 1)"));
        }

        [Test]
        public void IsSolvable_UnknownBlock_SaysNotOnBoard()
        {
            Assert.That(Checker.IsSolvable(board, Solve(Step(9, RedDoor, L)), out var problem), Is.False);
            Assert.That(problem, Does.Contain("block 9 is not on the board"));
        }

        [Test]
        public void IsSolvable_SameBlockTwice_SecondStepFails()
        {
            var solution = Solve(Step(Blue, BlueDoor, U, R, R, D), Step(Blue, BlueDoor, L));

            Assert.That(Checker.IsSolvable(board, solution, out var problem), Is.False);
            Assert.That(problem, Does.StartWith("Step 2:").And.Contain("not on the board"));
        }

        [TestCase(-1)]
        [TestCase(2)]
        public void IsSolvable_DoorOutOfRange_SaysDoorMissing(int doorIndex)
        {
            Assert.That(Checker.IsSolvable(board, Solve(Step(Blue, doorIndex, U, R, R, D)), out var problem), Is.False);
            Assert.That(problem, Does.Contain($"door {doorIndex} does not exist"));
        }

        [Test]
        public void IsSolvable_NoMoves_SaysBlockMustMove()
        {
            Assert.That(Checker.IsSolvable(board, Solve(Step(Blue, BlueDoor)), out var problem), Is.False);
            Assert.That(problem, Does.Contain("does not move"));
        }

        [Test]
        public void IsSolvable_EndsAwayFromDoor_SaysNoDoorFits()
        {
            Assert.That(Checker.IsSolvable(board, Solve(Step(Blue, BlueDoor, U)), out var problem), Is.False);
            Assert.That(problem, Does.Contain("does not fit a door"));
        }

        [Test]
        public void IsSolvable_WrongDoorIndex_NamesBothDoors()
        {
            var solution = Solve(Step(Blue, RedDoor, U, R, R, D));

            Assert.That(Checker.IsSolvable(board, solution, out var problem), Is.False);
            Assert.That(problem, Does.Contain("would leave through door 1, not door 0"));
        }

        [Test]
        public void IsSolvable_BlocksLeftOver_CountsThem()
        {
            Assert.That(Checker.IsSolvable(board, Solve(Step(Blue, BlueDoor, U, R, R, D)), out var problem), Is.False);
            Assert.That(problem, Does.Contain("1 block(s) are still on the board"));
        }

        [Test]
        public void Clone_CopiesSizeDoorsBlocksAndIds()
        {
            var copy = board.Clone();

            Assert.That(copy.Width, Is.EqualTo(4));
            Assert.That(copy.Height, Is.EqualTo(3));
            Assert.That(copy.Doors, Is.EqualTo(board.Doors));
            Assert.That(copy.GetBlockById(Red).Position, Is.EqualTo(new GridPosition(2, 1)));
            Assert.That(copy.GetBlockById(Blue).Color, Is.EqualTo("blue"));
            Assert.That(copy.GetBlockAt(new GridPosition(1, 1)), Is.SameAs(copy.GetBlockById(Blue)));
        }

        [Test]
        public void Clone_ChangingCopy_LeavesOriginal()
        {
            var copy = board.Clone();

            Assert.That(copy.TryMove(copy.GetBlockById(Red), R), Is.True);

            Assert.That(board.GetBlockById(Red).Position, Is.EqualTo(new GridPosition(2, 1)));
            Assert.That(board.GetBlockAt(new GridPosition(3, 1)), Is.Null);
        }

        [Test]
        public void Clone_NewBlock_GetsNextFreeId()
        {
            var copy = board.Clone();

            Assert.That(copy.TryAddBlock(TestShapes.Single, new GridPosition(0, 0), "red", out var block), Is.True);
            Assert.That(block.Id, Is.EqualTo(2));
        }

        [Test]
        public void GetBlockById_Unknown_ReturnsNull()
        {
            Assert.That(board.GetBlockById(42), Is.Null);
        }
    }
}
