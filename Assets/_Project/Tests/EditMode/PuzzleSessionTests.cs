using System;
using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Game;

namespace StickerPuzzle.Tests
{
    public sealed class PuzzleSessionTests
    {
        private Board board;
        private Door redLeftDoor;
        private PuzzleSession session;

        [SetUp]
        public void SetUp()
        {
            board = new Board(6, 6);
            Assert.That(Door.TryCreate(BoardSide.Left, 0, 2, "red", out redLeftDoor, out var error), Is.True, error);
            Assert.That(board.TryAddDoor(redLeftDoor, out error), Is.True, error);
            session = new PuzzleSession(board);
        }

        private static GridPosition At(int x, int y)
        {
            return new GridPosition(x, y);
        }

        private Block AddBlock(int x, int y, string color = "red")
        {
            Assert.That(board.TryAddBlock(TestShapes.Single, At(x, y), color, out var block), Is.True);
            return block;
        }

        [Test]
        public void Constructor_NullBoard_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PuzzleSession(null));
        }

        [Test]
        public void TryPickUp_CellWithBlock_HoldsIt()
        {
            var block = AddBlock(2, 2);

            Assert.That(session.TryPickUp(At(2, 2)), Is.True);
            Assert.That(session.HeldBlock, Is.SameAs(block));
        }

        [Test]
        public void TryPickUp_EmptyCell_False()
        {
            Assert.That(session.TryPickUp(At(2, 2)), Is.False);
            Assert.That(session.HeldBlock, Is.Null);
        }

        [Test]
        public void TryPickUp_AlreadyHolding_FalseAndKeepsFirstBlock()
        {
            var first = AddBlock(2, 2);
            AddBlock(3, 3);
            session.TryPickUp(At(2, 2));

            Assert.That(session.TryPickUp(At(3, 3)), Is.False);
            Assert.That(session.HeldBlock, Is.SameAs(first));
        }

        [Test]
        public void TryStep_NothingHeld_False()
        {
            AddBlock(2, 2);

            Assert.That(session.TryStep(Direction.Up), Is.False);
        }

        [Test]
        public void TryStep_FreeCell_MovesAndReportsIt()
        {
            var block = AddBlock(2, 2);
            Block reported = null;
            session.BlockMoved += moved => reported = moved;
            session.TryPickUp(At(2, 2));

            Assert.That(session.TryStep(Direction.Up), Is.True);
            Assert.That(block.Position, Is.EqualTo(At(2, 3)));
            Assert.That(reported, Is.SameAs(block));
        }

        [Test]
        public void TryStep_Blocked_FalseAndReportsNothing()
        {
            AddBlock(2, 2);
            AddBlock(2, 3);
            var reports = 0;
            session.BlockMoved += _ => reports++;
            session.TryPickUp(At(2, 2));

            Assert.That(session.TryStep(Direction.Up), Is.False);
            Assert.That(reports, Is.EqualTo(0));
            Assert.That(session.HeldBlock, Is.Not.Null);
        }

        [Test]
        public void LetGo_NothingHeld_False()
        {
            Assert.That(session.LetGo(), Is.False);
        }

        [Test]
        public void LetGo_MovedToFittingDoor_ExitsAndReportsDoor()
        {
            var block = AddBlock(1, 0);
            Block reportedBlock = null;
            Door reportedDoor = null;
            session.BlockExited += (exited, door) =>
            {
                reportedBlock = exited;
                reportedDoor = door;
            };

            session.TryPickUp(At(1, 0));
            session.TryStep(Direction.Left);

            Assert.That(session.LetGo(), Is.True);
            Assert.That(session.HeldBlock, Is.Null);
            Assert.That(board.GetBlockAt(At(0, 0)), Is.Null);
            Assert.That(reportedBlock, Is.SameAs(block));
            Assert.That(reportedDoor, Is.SameAs(redLeftDoor));
        }

        [Test]
        public void LetGo_AtDoorWithoutMoving_StaysOnBoard()
        {
            var block = AddBlock(0, 0);

            session.TryPickUp(At(0, 0));
            session.LetGo();

            Assert.That(session.HeldBlock, Is.Null);
            Assert.That(board.GetBlockAt(At(0, 0)), Is.SameAs(block));
        }

        [Test]
        public void LetGo_AtDoorAfterOnlyABlockedStep_StaysOnBoard()
        {
            var block = AddBlock(0, 0);

            session.TryPickUp(At(0, 0));
            session.TryStep(Direction.Left);
            session.LetGo();

            Assert.That(board.GetBlockAt(At(0, 0)), Is.SameAs(block));
        }

        [Test]
        public void LetGo_MovedAwayAndBackInOneDrag_Exits()
        {
            AddBlock(0, 0);

            session.TryPickUp(At(0, 0));
            session.TryStep(Direction.Right);
            session.TryStep(Direction.Left);
            session.LetGo();

            Assert.That(board.IsCleared, Is.True);
        }

        [Test]
        public void LetGo_MovedAwayFromDoors_StaysInNewCell()
        {
            var block = AddBlock(2, 2);

            session.TryPickUp(At(2, 2));
            session.TryStep(Direction.Up);
            session.LetGo();

            Assert.That(session.HeldBlock, Is.Null);
            Assert.That(board.GetBlockAt(At(2, 3)), Is.SameAs(block));
        }

        [Test]
        public void LetGo_MovedToWrongColorDoor_StaysOnBoard()
        {
            var block = AddBlock(1, 0, "blue");

            session.TryPickUp(At(1, 0));
            session.TryStep(Direction.Left);
            session.LetGo();

            Assert.That(board.GetBlockAt(At(0, 0)), Is.SameAs(block));
        }

        [Test]
        public void LetGo_LastBlockExits_ReportsBoardCleared()
        {
            AddBlock(1, 0);
            var cleared = 0;
            session.BoardCleared += () => cleared++;

            session.TryPickUp(At(1, 0));
            session.TryStep(Direction.Left);
            session.LetGo();

            Assert.That(cleared, Is.EqualTo(1));
        }

        [Test]
        public void LetGo_BlocksRemain_DoesNotReportBoardCleared()
        {
            AddBlock(1, 0);
            AddBlock(3, 3);
            var cleared = 0;
            session.BoardCleared += () => cleared++;

            session.TryPickUp(At(1, 0));
            session.TryStep(Direction.Left);
            session.LetGo();

            Assert.That(cleared, Is.EqualTo(0));
        }
    }
}
