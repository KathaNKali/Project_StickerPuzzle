using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Presentation;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class BoardGridTests
    {
        [TestCase(6, 6, 0, 0, -2.5f, -2.5f)]
        [TestCase(6, 6, 5, 5, 2.5f, 2.5f)]
        [TestCase(6, 4, 5, 0, 2.5f, -1.5f)]
        [TestCase(5, 3, 2, 1, 0f, 0f)]
        public void CellToWorld_CentersBoardOnOrigin(int width, int height, int x, int y, float expectedX, float expectedZ)
        {
            var world = new BoardGrid(width, height).CellToWorld(new GridPosition(x, y));

            Assert.That(world, Is.EqualTo(new Vector3(expectedX, 0f, expectedZ)));
        }

        [Test]
        public void CellToWorld_WithHeight_SetsWorldY()
        {
            var world = new BoardGrid(6, 6).CellToWorld(new GridPosition(0, 0), 0.75f);

            Assert.That(world.y, Is.EqualTo(0.75f));
        }
    }
}
