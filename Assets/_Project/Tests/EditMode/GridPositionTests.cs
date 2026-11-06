using System;
using System.Collections.Generic;
using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class GridPositionTests
    {
        [TestCase(Direction.Up, 2, 4)]
        [TestCase(Direction.Down, 2, 2)]
        [TestCase(Direction.Left, 1, 3)]
        [TestCase(Direction.Right, 3, 3)]
        public void Step_EachDirection_MovesOneCell(Direction direction, int expectedX, int expectedY)
        {
            var result = new GridPosition(2, 3).Step(direction);

            Assert.That(result, Is.EqualTo(new GridPosition(expectedX, expectedY)));
        }

        [Test]
        public void Step_UnknownDirection_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GridPosition(0, 0).Step((Direction)99));
        }

        [Test]
        public void Add_TwoPositions_AddsEachAxis()
        {
            var result = new GridPosition(1, 2) + new GridPosition(3, -1);

            Assert.That(result, Is.EqualTo(new GridPosition(4, 1)));
        }

        [Test]
        public void Equality_SameCell_IsEqual()
        {
            Assert.That(new GridPosition(1, 2) == new GridPosition(1, 2), Is.True);
            Assert.That(new GridPosition(1, 2) != new GridPosition(1, 2), Is.False);
        }

        [Test]
        public void Equality_SwappedAxes_IsNotEqual()
        {
            Assert.That(new GridPosition(1, 2) == new GridPosition(2, 1), Is.False);
        }

        [Test]
        public void HashSet_SameCellTwice_StoresOnce()
        {
            var cells = new HashSet<GridPosition> { new GridPosition(1, 2), new GridPosition(1, 2) };

            Assert.That(cells.Count, Is.EqualTo(1));
        }
    }
}
