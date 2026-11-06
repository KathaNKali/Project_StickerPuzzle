using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class ShapeDefinitionTests
    {
        private static GridPosition[] Cells(params int[] xy)
        {
            var cells = new GridPosition[xy.Length / 2];
            for (var i = 0; i < cells.Length; i++)
            {
                cells[i] = new GridPosition(xy[i * 2], xy[(i * 2) + 1]);
            }

            return cells;
        }

        private static void AssertInvalid(string id, string family, GridPosition[] cells)
        {
            var created = ShapeDefinition.TryCreate(id, family, cells, out var shape, out var error);

            Assert.That(created, Is.False);
            Assert.That(shape, Is.Null);
            Assert.That(error, Is.Not.Empty);
        }

        [Test]
        public void TryCreate_LShape_ReportsSizeAndCells()
        {
            var created = ShapeDefinition.TryCreate("L_A", "L", Cells(0, 0, 1, 0, 0, 1), out var shape, out var error);

            Assert.That(created, Is.True, error);
            Assert.That(shape.Id, Is.EqualTo("L_A"));
            Assert.That(shape.Family, Is.EqualTo("L"));
            Assert.That(shape.Width, Is.EqualTo(2));
            Assert.That(shape.Height, Is.EqualTo(2));
            Assert.That(shape.Cells.Count, Is.EqualTo(3));
        }

        [Test]
        public void TryCreate_Full3x3_IsValid()
        {
            var cells = Cells(0, 0, 1, 0, 2, 0, 0, 1, 1, 1, 2, 1, 0, 2, 1, 2, 2, 2);

            var created = ShapeDefinition.TryCreate("3x3", "Rect", cells, out var shape, out var error);

            Assert.That(created, Is.True, error);
            Assert.That(shape.Width, Is.EqualTo(3));
            Assert.That(shape.Height, Is.EqualTo(3));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void TryCreate_BlankId_Fails(string id)
        {
            AssertInvalid(id, "Rect", Cells(0, 0));
        }

        [Test]
        public void TryCreate_BlankFamily_Fails()
        {
            AssertInvalid("1x1", "", Cells(0, 0));
        }

        [Test]
        public void TryCreate_NoCells_Fails()
        {
            AssertInvalid("empty", "Rect", Cells());
        }

        [Test]
        public void TryCreate_NullCells_Fails()
        {
            AssertInvalid("null", "Rect", null);
        }

        [Test]
        public void TryCreate_DuplicateCell_Fails()
        {
            AssertInvalid("dup", "Rect", Cells(0, 0, 0, 0));
        }

        [TestCase(1, 0)]
        [TestCase(0, 1)]
        [TestCase(-1, 0)]
        public void TryCreate_NotStartingAtOrigin_Fails(int x, int y)
        {
            AssertInvalid("shifted", "Rect", Cells(x, y));
        }

        [Test]
        public void TryCreate_TooWide_Fails()
        {
            AssertInvalid("4x1", "Rect", Cells(0, 0, 1, 0, 2, 0, 3, 0));
        }

        [Test]
        public void TryCreate_TooTall_Fails()
        {
            AssertInvalid("1x4", "Rect", Cells(0, 0, 0, 1, 0, 2, 0, 3));
        }

        [Test]
        public void TryCreate_SeparatePieces_Fails()
        {
            AssertInvalid("gap", "Rect", Cells(0, 0, 2, 0));
        }

        [Test]
        public void TryCreate_TouchingOnlyDiagonally_Fails()
        {
            AssertInvalid("diagonal", "Rect", Cells(0, 0, 1, 1));
        }

        [Test]
        public void TryCreate_InputChangedAfterwards_ShapeIsUnaffected()
        {
            var cells = Cells(0, 0, 1, 0);
            ShapeDefinition.TryCreate("1x2", "Rect", cells, out var shape, out _);

            cells[1] = new GridPosition(0, 1);

            Assert.That(shape.Cells[1], Is.EqualTo(new GridPosition(1, 0)));
        }
    }
}
