using System;
using System.Linq;
using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Game;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class BoardLayoutTests
    {
        private BoardLayoutAsset layout;
        private ShapeCatalogAsset catalogAsset;
        private ShapeCatalog catalog;

        [SetUp]
        public void SetUp()
        {
            layout = ScriptableObject.CreateInstance<BoardLayoutAsset>();
            catalogAsset = ScriptableObject.CreateInstance<ShapeCatalogAsset>();
            catalogAsset.FillStartingShapes();
            Assert.That(catalogAsset.TryBuild(out catalog, out _), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(layout);
            UnityEngine.Object.DestroyImmediate(catalogAsset);
        }

        /// <summary>6x6 with two valid doors: red on the left (rows 2-3), blue on the right (rows 2-3).</summary>
        private void AddTwoDoors()
        {
            layout.SetSize(6, 6);
            layout.AddDoor(BoardSide.Left, 2, 2, "red");
            layout.AddDoor(BoardSide.Right, 2, 2, "blue");
        }

        private void AssertBuildFails(string expectedInError)
        {
            Assert.That(layout.TryBuild(catalog, out var board, out var errors), Is.False);
            Assert.That(board, Is.Null);
            Assert.That(errors, Has.Some.Contains(expectedInError));
        }

        private static void Drag(PuzzleSession session, int x, int y, params Direction[] steps)
        {
            Assert.That(session.TryPickUp(new GridPosition(x, y)), Is.True, $"Nothing to pick up at ({x}, {y})");
            foreach (var step in steps)
            {
                Assert.That(session.TryStep(step), Is.True, $"Blocked moving {step} from ({x}, {y})");
            }

            session.LetGo();
        }

        [Test]
        public void TryBuild_TestBoard_HasExpectedContents()
        {
            layout.FillTestBoard();

            Assert.That(layout.TryBuild(catalog, out var board, out var errors), Is.True, string.Join("\n", errors));
            Assert.That(board.Width, Is.EqualTo(6));
            Assert.That(board.Height, Is.EqualTo(6));
            Assert.That(board.Doors.Count, Is.EqualTo(3));
            Assert.That(board.Blocks.Count, Is.EqualTo(6));
        }

        [Test]
        public void TryBuild_TestBoard_HasNoLShapes()
        {
            layout.FillTestBoard();
            layout.TryBuild(catalog, out var board, out _);

            Assert.That(board.Blocks.Any(block => block.Shape.Family == "L"), Is.False);
        }

        [Test]
        public void TestBoard_FullSolution_ClearsTheBoard()
        {
            layout.FillTestBoard();
            layout.TryBuild(catalog, out var board, out _);
            var session = new PuzzleSession(board);
            var cleared = false;
            session.BoardCleared += () => cleared = true;

            const Direction L = Direction.Left;
            const Direction R = Direction.Right;
            const Direction U = Direction.Up;

            Drag(session, 4, 5, L, L);                // yellow 2x1 out the top
            Drag(session, 2, 3, L, L);                // red 1x1 out the left
            Drag(session, 0, 4, R, R, R, R);          // blue 2x1 out the right
            Drag(session, 3, 0, U, U, L, L, L);       // red 1x2 out the left
            Drag(session, 5, 0, U, U, U, U, L, L);    // yellow 1x2 out the top
            Drag(session, 0, 0, R, R, R, R, U, U);    // blue 2x2 out the right

            Assert.That(board.IsCleared, Is.True);
            Assert.That(cleared, Is.True);
        }

        [Test]
        public void TryBuild_Twice_GivesSeparateBoards()
        {
            layout.FillTestBoard();
            layout.TryBuild(catalog, out var first, out _);
            layout.TryBuild(catalog, out var second, out _);

            var session = new PuzzleSession(first);
            Drag(session, 4, 5, Direction.Left);

            // The block moved off (5, 5) on the first board only.
            Assert.That(first.GetBlockAt(new GridPosition(5, 5)), Is.Null);
            Assert.That(second.GetBlockAt(new GridPosition(5, 5)), Is.Not.Null);
        }

        [Test]
        public void FillTestBoard_Twice_DoesNotDuplicate()
        {
            layout.FillTestBoard();
            layout.FillTestBoard();

            Assert.That(layout.DoorCount, Is.EqualTo(3));
            Assert.That(layout.BlockCount, Is.EqualTo(6));
        }

        [Test]
        public void TryBuild_NullCatalog_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => layout.TryBuild(null, out _, out _));
        }

        [TestCase(0, 6)]
        [TestCase(6, 0)]
        [TestCase(4, 6)]
        [TestCase(6, 4)]
        public void TryBuild_InvalidSize_Fails(int width, int height)
        {
            layout.SetSize(width, height);

            AssertBuildFails("size");
        }

        [Test]
        public void TryBuild_OneDoor_Fails()
        {
            layout.AddDoor(BoardSide.Left, 2, 2, "red");
            layout.AddBlock("1x1", 3, 3, "red");

            AssertBuildFails("doors");
        }

        [Test]
        public void TryBuild_FiveDoors_Fails()
        {
            AddTwoDoors();
            layout.AddDoor(BoardSide.Top, 2, 1, "yellow");
            layout.AddDoor(BoardSide.Top, 3, 1, "green");
            layout.AddDoor(BoardSide.Bottom, 2, 1, "purple");
            layout.AddBlock("1x1", 3, 3, "red");

            AssertBuildFails("doors");
        }

        [Test]
        public void TryBuild_NoBlocks_Fails()
        {
            AddTwoDoors();

            AssertBuildFails("no blocks");
        }

        [Test]
        public void TryBuild_DoorDoesNotFitEdge_Fails()
        {
            AddTwoDoors();
            layout.AddDoor(BoardSide.Top, 5, 2, "yellow");
            layout.AddBlock("1x1", 3, 3, "red");

            AssertBuildFails("does not fit");
        }

        [Test]
        public void TryBuild_InvalidDoorOpening_Fails()
        {
            AddTwoDoors();
            layout.AddDoor(BoardSide.Top, 2, 0, "yellow");
            layout.AddBlock("1x1", 3, 3, "red");

            AssertBuildFails("opening");
        }

        [Test]
        public void TryBuild_UnknownShape_Fails()
        {
            AddTwoDoors();
            layout.AddBlock("missing", 3, 3, "red");

            AssertBuildFails("unknown shape");
        }

        [Test]
        public void TryBuild_BlockWithoutColor_Fails()
        {
            AddTwoDoors();
            layout.AddBlock("1x1", 3, 3, "");

            AssertBuildFails("no color");
        }

        [Test]
        public void TryBuild_OverlappingBlocks_Fails()
        {
            AddTwoDoors();
            layout.AddBlock("2x1", 2, 0, "red");
            layout.AddBlock("1x1", 3, 0, "red");

            AssertBuildFails("overlaps");
        }

        [Test]
        public void TryBuild_BlockOffBoard_Fails()
        {
            AddTwoDoors();
            layout.AddBlock("2x1", 5, 0, "red");

            AssertBuildFails("off the board");
        }

        [Test]
        public void TryBuild_BlockStartsTouchingDoorItFits_Fails()
        {
            AddTwoDoors();
            layout.AddBlock("1x1", 0, 2, "red");

            AssertBuildFails("starts touching");
        }

        [Test]
        public void TryBuild_BlockStartsTouchingOtherColorDoor_Succeeds()
        {
            AddTwoDoors();
            layout.AddBlock("1x1", 0, 2, "blue");

            Assert.That(layout.TryBuild(catalog, out _, out var errors), Is.True, string.Join("\n", errors));
        }

        [Test]
        public void TryBuild_SeveralProblems_ReportsThemAll()
        {
            AddTwoDoors();
            layout.AddBlock("missing", 3, 3, "red");
            layout.AddBlock("1x1", 0, 2, "red");

            Assert.That(layout.TryBuild(catalog, out _, out var errors), Is.False);
            Assert.That(errors, Has.Count.EqualTo(2));
        }
    }
}
