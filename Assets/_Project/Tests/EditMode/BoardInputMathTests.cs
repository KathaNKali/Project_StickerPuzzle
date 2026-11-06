using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Presentation;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class BoardInputMathTests
    {
        [TestCase(6, 6, -2.5f, -2.5f, 0, 0)]
        [TestCase(6, 6, 2.5f, 2.5f, 5, 5)]
        [TestCase(6, 4, 2.5f, -1.5f, 5, 0)]
        [TestCase(5, 3, 0f, 0f, 2, 1)]
        [TestCase(6, 6, -2.99f, 0.1f, 0, 3)]
        [TestCase(6, 6, -2.01f, 0.1f, 0, 3)]
        [TestCase(6, 6, -1.99f, 0.1f, 1, 3)]
        [TestCase(6, 6, -3.01f, 0.1f, -1, 3)]
        public void WorldToCell_FindsTheCellContainingThePoint(int width, int height, float worldX, float worldZ, int expectedX, int expectedY)
        {
            var cell = new BoardGrid(width, height).WorldToCell(new Vector3(worldX, 0.3f, worldZ));

            Assert.That(cell, Is.EqualTo(new GridPosition(expectedX, expectedY)));
        }

        [Test]
        public void WorldToCell_EveryCell_RoundTripsThroughCellToWorld()
        {
            var grid = new BoardGrid(7, 4);

            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 7; x++)
                {
                    var cell = new GridPosition(x, y);
                    Assert.That(grid.WorldToCell(grid.CellToWorld(cell)), Is.EqualTo(cell));
                }
            }
        }

        [Test]
        public void TryGetPointAtHeight_TiltedRay_LandsOnThatHeight()
        {
            var ray = new Ray(new Vector3(1f, 10f, -2f), new Vector3(0f, -1f, 0.5f));

            Assert.That(BoardGrid.TryGetPointAtHeight(ray, 0.3f, out var point), Is.True);
            Assert.That(point.x, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(point.y, Is.EqualTo(0.3f).Within(0.0001f));
            Assert.That(point.z, Is.EqualTo(2.85f).Within(0.0001f));
        }

        [Test]
        public void TryGetPointAtHeight_RayPointingAway_False()
        {
            var ray = new Ray(new Vector3(0f, 10f, 0f), Vector3.up);

            Assert.That(BoardGrid.TryGetPointAtHeight(ray, 0.3f, out _), Is.False);
        }

        [Test]
        public void TryGetPointAtHeight_RayParallelToPlane_False()
        {
            var ray = new Ray(new Vector3(0f, 10f, 0f), Vector3.forward);

            Assert.That(BoardGrid.TryGetPointAtHeight(ray, 0.3f, out _), Is.False);
        }

        [Test]
        public void SetHeld_True_LiftsAndKeepsCell()
        {
            var view = new GameObject("TestBlock").AddComponent<BlockView>();
            view.transform.position = new Vector3(1f, 0f, 2f);

            view.SetHeld(true);

            Assert.That(view.transform.position, Is.EqualTo(new Vector3(1f, BlockView.HeldLift, 2f)));
            Object.DestroyImmediate(view.gameObject);
        }

        [Test]
        public void SetHeld_TrueThenFalse_ReturnsToBoard()
        {
            var view = new GameObject("TestBlock").AddComponent<BlockView>();
            view.transform.position = new Vector3(1f, 0f, 2f);

            view.SetHeld(true);
            view.SetHeld(true);
            view.SetHeld(false);

            Assert.That(view.transform.position, Is.EqualTo(new Vector3(1f, 0f, 2f)));
            Object.DestroyImmediate(view.gameObject);
        }
    }
}
