using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Presentation;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class ExitAnimationTests
    {
        private const float Tolerance = 0.0001f;

        private static Block MakeBlock(ShapeDefinition shape, int x, int y)
        {
            new Board(6, 6).TryAddBlock(shape, new GridPosition(x, y), "red", out var block);
            return block;
        }

        private static Door MakeDoor(BoardSide side)
        {
            Door.TryCreate(side, 0, 1, "red", out var door, out _);
            return door;
        }

        // A 6x6 board, where the block's start is its bottom-left cell's world position.
        [TestCase(BoardSide.Top, 0f, 1.35f)]
        [TestCase(BoardSide.Bottom, 0f, -1.35f)]
        [TestCase(BoardSide.Left, -1.35f, 0f)]
        [TestCase(BoardSide.Right, 1.35f, 0f)]
        public void GetExitPosition_SlidesStraightOutOfThatSide(BoardSide side, float expectedMoveX, float expectedMoveZ)
        {
            var grid = new BoardGrid(6, 6);
            var block = MakeBlock(TestShapes.Single, 2, 3);
            var start = grid.CellToWorld(block.Position);

            var end = grid.GetExitPosition(block, MakeDoor(side), 0.35f);

            Assert.That(end.x - start.x, Is.EqualTo(expectedMoveX).Within(Tolerance));
            Assert.That(end.y, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(end.z - start.z, Is.EqualTo(expectedMoveZ).Within(Tolerance));
        }

        [Test]
        public void GetExitPosition_WideBlockThroughRightDoor_TravelsItsFullWidth()
        {
            var grid = new BoardGrid(6, 6);
            var block = MakeBlock(TestShapes.TwoWide, 4, 0);

            var end = grid.GetExitPosition(block, MakeDoor(BoardSide.Right), 0.35f);

            // The block's left edge finishes 0.35 past the board's right edge, at world X 3.
            var leftEdge = end.x - 0.5f;
            Assert.That(leftEdge, Is.EqualTo(3.35f).Within(Tolerance));
        }

        [Test]
        public void PlayExit_RunsWithoutJumping()
        {
            var view = new GameObject("TestBlock").AddComponent<BlockView>();
            view.SetMotion(12f, 0.1f);
            view.PlaceAt(Vector3.zero);

            view.PlayExit(Vector3.zero, new Vector3(5f, 0f, 0f), 8f);

            Assert.That(view.enabled, Is.True);
            Assert.That(view.transform.position, Is.EqualTo(Vector3.zero));
            Object.DestroyImmediate(view.gameObject);
        }

        [Test]
        public void SetMotion_WhileExiting_KeepsTheExitRunning()
        {
            var view = new GameObject("TestBlock").AddComponent<BlockView>();
            view.PlayExit(Vector3.zero, new Vector3(5f, 0f, 0f), 8f);

            view.SetMotion(0f, 0f);

            Assert.That(view.enabled, Is.True);
            Object.DestroyImmediate(view.gameObject);
        }

        [Test]
        public void SetHeld_WhileExiting_DoesNotLiftTheBlock()
        {
            var view = new GameObject("TestBlock").AddComponent<BlockView>();
            view.PlayExit(Vector3.zero, new Vector3(5f, 0f, 0f), 8f);

            view.SetHeld(true);

            Assert.That(view.transform.position.y, Is.EqualTo(0f));
            Assert.That(view.enabled, Is.True);
            Object.DestroyImmediate(view.gameObject);
        }
    }
}
