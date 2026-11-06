using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Presentation;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class BoardVisualsTests
    {
        private const float Tolerance = 0.0001f;

        private static Door MakeDoor(BoardSide side, int position, int openingSize)
        {
            Door.TryCreate(side, position, openingSize, "red", out var door, out _);
            return door;
        }

        // A 6 wide by 4 tall board, with bars 0.15 outside the edge.
        [TestCase(BoardSide.Left, 1, 2, -3.15f, 0f)]
        [TestCase(BoardSide.Right, 0, 1, 3.15f, -1.5f)]
        [TestCase(BoardSide.Top, 2, 2, 0f, 2.15f)]
        [TestCase(BoardSide.Bottom, 5, 1, 2.5f, -2.15f)]
        public void GetDoorCenter_SitsOutsideEdgeAtMiddleOfOpening(BoardSide side, int position, int openingSize, float expectedX, float expectedZ)
        {
            var center = new BoardGrid(6, 4).GetDoorCenter(MakeDoor(side, position, openingSize), 0.15f, 0.1f);

            Assert.That(center.x, Is.EqualTo(expectedX).Within(Tolerance));
            Assert.That(center.y, Is.EqualTo(0.1f).Within(Tolerance));
            Assert.That(center.z, Is.EqualTo(expectedZ).Within(Tolerance));
        }

        [Test]
        public void GetDoorSize_TopDoor_RunsAlongX()
        {
            var size = BoardGrid.GetDoorSize(MakeDoor(BoardSide.Top, 0, 3), 0.2f, 0.4f);

            Assert.That(size, Is.EqualTo(new Vector3(3f, 0.4f, 0.2f)));
        }

        [Test]
        public void GetDoorSize_LeftDoor_RunsAlongZ()
        {
            var size = BoardGrid.GetDoorSize(MakeDoor(BoardSide.Left, 0, 3), 0.2f, 0.4f);

            Assert.That(size, Is.EqualTo(new Vector3(0.2f, 0.4f, 3f)));
        }

        [Test]
        public void GetPart_SingleCell_HasGapOnEverySide()
        {
            BlockPartLayout.GetPart(TestShapes.Single, 0, 0.05f, out var center, out var size);

            Assert.That(center.x, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(center.y, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(size.x, Is.EqualTo(0.9f).Within(Tolerance));
            Assert.That(size.y, Is.EqualTo(0.9f).Within(Tolerance));
        }

        [Test]
        public void GetPart_TwoWide_HasNoSeamBetweenCells()
        {
            BlockPartLayout.GetPart(TestShapes.TwoWide, 0, 0.05f, out var leftCenter, out var leftSize);
            BlockPartLayout.GetPart(TestShapes.TwoWide, 1, 0.05f, out var rightCenter, out var rightSize);

            var leftPartRightEdge = leftCenter.x + (leftSize.x * 0.5f);
            var rightPartLeftEdge = rightCenter.x - (rightSize.x * 0.5f);

            Assert.That(leftPartRightEdge, Is.EqualTo(rightPartLeftEdge).Within(Tolerance));
            Assert.That(leftCenter.x - (leftSize.x * 0.5f), Is.EqualTo(-0.45f).Within(Tolerance));
            Assert.That(rightCenter.x + (rightSize.x * 0.5f), Is.EqualTo(1.45f).Within(Tolerance));
        }

        [Test]
        public void GetPart_TwoByTwo_CoversTheFullBlockExceptOuterGap()
        {
            for (var i = 0; i < TestShapes.TwoByTwo.Cells.Count; i++)
            {
                BlockPartLayout.GetPart(TestShapes.TwoByTwo, i, 0.05f, out var center, out var size);

                Assert.That(center.x - (size.x * 0.5f), Is.GreaterThanOrEqualTo(-0.45f - Tolerance));
                Assert.That(center.x + (size.x * 0.5f), Is.LessThanOrEqualTo(1.45f + Tolerance));
                Assert.That(center.y - (size.y * 0.5f), Is.GreaterThanOrEqualTo(-0.45f - Tolerance));
                Assert.That(center.y + (size.y * 0.5f), Is.LessThanOrEqualTo(1.45f + Tolerance));
            }
        }

        [Test]
        public void GetPart_LShape_KeepsGapAtItsInnerCorner()
        {
            // Cell 1 is bottom-right; the empty top-right corner is above it.
            BlockPartLayout.GetPart(TestShapes.LShape, 1, 0.05f, out var center, out var size);

            Assert.That(center.y + (size.y * 0.5f), Is.EqualTo(0.45f).Within(Tolerance));
        }

        [Test]
        public void TryGetMaterial_KnownColor_ReturnsIt()
        {
            var palette = ScriptableObject.CreateInstance<ColorPaletteAsset>();
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            palette.AddColor("red", material);

            Assert.That(palette.TryGetMaterial("red", out var found), Is.True);
            Assert.That(found, Is.SameAs(material));

            Object.DestroyImmediate(material);
            Object.DestroyImmediate(palette);
        }

        [Test]
        public void TryGetMaterial_UnknownColor_ReturnsFalse()
        {
            var palette = ScriptableObject.CreateInstance<ColorPaletteAsset>();

            Assert.That(palette.TryGetMaterial("red", out _), Is.False);

            Object.DestroyImmediate(palette);
        }
    }
}
