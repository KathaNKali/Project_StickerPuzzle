using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Game;
using StickerPuzzle.Presentation;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class DragTests
    {
        private Board board;
        private PuzzleSession session;

        [SetUp]
        public void SetUp()
        {
            board = new Board(6, 6);
            session = new PuzzleSession(board);
        }

        private static GridPosition At(int x, int y)
        {
            return new GridPosition(x, y);
        }

        private Block AddAndHold(ShapeDefinition shape, int x, int y)
        {
            var block = AddBlock(shape, x, y);
            Assert.That(session.TryPickUp(At(x, y)), Is.True);
            return block;
        }

        private Block AddBlock(ShapeDefinition shape, int x, int y)
        {
            Assert.That(board.TryAddBlock(shape, At(x, y), "red", out var block), Is.True);
            return block;
        }

        [Test]
        public void Subtract_TwoPositions_SubtractsEachAxis()
        {
            Assert.That(At(4, 1) - At(1, 3), Is.EqualTo(At(3, -2)));
        }

        [Test]
        public void MoveTo_KeepsCurrentLift()
        {
            var view = new GameObject("TestBlock").AddComponent<BlockView>();
            view.SetHeld(true);

            view.MoveTo(new Vector3(1f, 0f, 2f));

            Assert.That(view.transform.position, Is.EqualTo(new Vector3(1f, BlockView.HeldLift, 2f)));
            Object.DestroyImmediate(view.gameObject);
        }

        [Test]
        public void DragHeldToward_NothingHeld_MovesNothing()
        {
            Assert.That(session.DragHeldToward(At(3, 3)), Is.EqualTo(0));
        }

        [Test]
        public void DragHeldToward_AlreadyThere_MovesNothing()
        {
            AddAndHold(TestShapes.Single, 2, 2);

            Assert.That(session.DragHeldToward(At(2, 2)), Is.EqualTo(0));
        }

        [Test]
        public void DragHeldToward_StraightLine_ReportsEveryStep()
        {
            var block = AddAndHold(TestShapes.Single, 0, 0);
            var reports = 0;
            session.BlockMoved += _ => reports++;

            Assert.That(session.DragHeldToward(At(3, 0)), Is.EqualTo(3));
            Assert.That(block.Position, Is.EqualTo(At(3, 0)));
            Assert.That(reports, Is.EqualTo(3));
        }

        [Test]
        public void DragHeldToward_Diagonal_ReachesTarget()
        {
            var block = AddAndHold(TestShapes.Single, 0, 0);

            Assert.That(session.DragHeldToward(At(3, 2)), Is.EqualTo(5));
            Assert.That(block.Position, Is.EqualTo(At(3, 2)));
        }

        [Test]
        public void DragHeldToward_LongerAxis_GoesFirst()
        {
            AddAndHold(TestShapes.Single, 0, 0);
            var firstMove = At(-1, -1);
            session.BlockMoved += moved =>
            {
                if (firstMove == At(-1, -1))
                {
                    firstMove = moved.Position;
                }
            };

            session.DragHeldToward(At(1, 3));

            Assert.That(firstMove, Is.EqualTo(At(0, 1)));
        }

        [Test]
        public void DragHeldToward_LongerAxisBlocked_GoesAroundOnTheOther()
        {
            var block = AddAndHold(TestShapes.Single, 2, 2);
            AddBlock(TestShapes.Single, 3, 2);

            Assert.That(session.DragHeldToward(At(4, 3)), Is.EqualTo(3));
            Assert.That(block.Position, Is.EqualTo(At(4, 3)));
        }

        [Test]
        public void DragHeldToward_BothWaysBlocked_StaysPut()
        {
            var block = AddAndHold(TestShapes.Single, 2, 2);
            AddBlock(TestShapes.Single, 3, 2);
            AddBlock(TestShapes.Single, 2, 3);

            Assert.That(session.DragHeldToward(At(3, 3)), Is.EqualTo(0));
            Assert.That(block.Position, Is.EqualTo(At(2, 2)));
        }

        [Test]
        public void DragHeldToward_BlockInTheWay_NeverJumpsOverIt()
        {
            var block = AddAndHold(TestShapes.Single, 0, 0);
            AddBlock(TestShapes.Single, 3, 0);

            Assert.That(session.DragHeldToward(At(5, 0)), Is.EqualTo(2));
            Assert.That(block.Position, Is.EqualTo(At(2, 0)));
        }

        [Test]
        public void DragHeldToward_TargetOffBoard_StopsAtTheWall()
        {
            var block = AddAndHold(TestShapes.Single, 2, 2);

            Assert.That(session.DragHeldToward(At(9, 2)), Is.EqualTo(3));
            Assert.That(block.Position, Is.EqualTo(At(5, 2)));
        }

        [Test]
        public void DragHeldToward_WideBlockCellBlocked_StaysPut()
        {
            var block = AddAndHold(TestShapes.TwoWide, 0, 0);
            AddBlock(TestShapes.Single, 1, 1);

            Assert.That(session.DragHeldToward(At(0, 2)), Is.EqualTo(0));
            Assert.That(block.Position, Is.EqualTo(At(0, 0)));
        }

        [Test]
        public void DragHeldToward_ThenLetGoAtFittingDoor_Exits()
        {
            Door.TryCreate(BoardSide.Left, 0, 2, "red", out var door, out _);
            board.TryAddDoor(door, out _);
            AddAndHold(TestShapes.Single, 2, 0);

            session.DragHeldToward(At(0, 0));
            session.LetGo();

            Assert.That(board.IsCleared, Is.True);
        }
    }
}
