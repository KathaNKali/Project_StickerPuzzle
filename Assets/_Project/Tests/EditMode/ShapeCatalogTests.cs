using System.Linq;
using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Game;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class ShapeCatalogTests
    {
        private ShapeCatalogAsset asset;

        [SetUp]
        public void SetUp()
        {
            asset = ScriptableObject.CreateInstance<ShapeCatalogAsset>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(asset);
        }

        [Test]
        public void TryCreate_ValidShapes_FindsThemById()
        {
            var created = ShapeCatalog.TryCreate(new[] { TestShapes.Single, TestShapes.TwoWide }, out var catalog, out var error);

            Assert.That(created, Is.True, error);
            Assert.That(catalog.Shapes, Has.Count.EqualTo(2));
            Assert.That(catalog.TryGet("twoWide", out var found), Is.True);
            Assert.That(found, Is.SameAs(TestShapes.TwoWide));
        }

        [Test]
        public void TryGet_UnknownOrNullId_ReturnsFalse()
        {
            ShapeCatalog.TryCreate(new[] { TestShapes.Single }, out var catalog, out _);

            Assert.That(catalog.TryGet("missing", out _), Is.False);
            Assert.That(catalog.TryGet(null, out _), Is.False);
        }

        [Test]
        public void TryCreate_DuplicateId_Fails()
        {
            var created = ShapeCatalog.TryCreate(new[] { TestShapes.Single, TestShapes.Single }, out _, out var error);

            Assert.That(created, Is.False);
            Assert.That(error, Does.Contain("single"));
        }

        [Test]
        public void TryCreate_NoShapes_Fails()
        {
            Assert.That(ShapeCatalog.TryCreate(new ShapeDefinition[0], out _, out _), Is.False);
        }

        [Test]
        public void TryCreate_NullList_Fails()
        {
            Assert.That(ShapeCatalog.TryCreate(null, out _, out _), Is.False);
        }

        [Test]
        public void FillStartingShapes_Builds13ValidShapes()
        {
            asset.FillStartingShapes();

            Assert.That(asset.TryBuild(out var catalog, out var errors), Is.True, string.Join("\n", errors));
            Assert.That(catalog.Shapes, Has.Count.EqualTo(13));
        }

        [Test]
        public void FillStartingShapes_Twice_DoesNotDuplicate()
        {
            asset.FillStartingShapes();
            asset.FillStartingShapes();

            Assert.That(asset.Count, Is.EqualTo(13));
        }

        // Size names are width x height.
        [TestCase("1x1", "Rectangle", 1, 1, 1)]
        [TestCase("1x2", "Rectangle", 1, 2, 2)]
        [TestCase("2x1", "Rectangle", 2, 1, 2)]
        [TestCase("1x3", "Rectangle", 1, 3, 3)]
        [TestCase("3x1", "Rectangle", 3, 1, 3)]
        [TestCase("2x2", "Rectangle", 2, 2, 4)]
        [TestCase("2x3", "Rectangle", 2, 3, 6)]
        [TestCase("3x2", "Rectangle", 3, 2, 6)]
        [TestCase("3x3", "Rectangle", 3, 3, 9)]
        [TestCase("L_EmptyTopRight", "L", 2, 2, 3)]
        [TestCase("L_EmptyTopLeft", "L", 2, 2, 3)]
        [TestCase("L_EmptyBottomRight", "L", 2, 2, 3)]
        [TestCase("L_EmptyBottomLeft", "L", 2, 2, 3)]
        public void FillStartingShapes_EachShape_HasExpectedSize(string id, string family, int width, int height, int cellCount)
        {
            asset.FillStartingShapes();
            asset.TryBuild(out var catalog, out _);

            Assert.That(catalog.TryGet(id, out var shape), Is.True, id);
            Assert.That(shape.Family, Is.EqualTo(family));
            Assert.That(shape.Width, Is.EqualTo(width));
            Assert.That(shape.Height, Is.EqualTo(height));
            Assert.That(shape.Cells.Count, Is.EqualTo(cellCount));
        }

        [Test]
        public void FillStartingShapes_LOrientations_AreAllDifferent()
        {
            asset.FillStartingShapes();
            asset.TryBuild(out var catalog, out _);

            var layouts = catalog.Shapes
                .Where(shape => shape.Family == "L")
                .Select(shape => string.Join(",", shape.Cells.OrderBy(c => c.X).ThenBy(c => c.Y)))
                .Distinct()
                .ToArray();

            Assert.That(layouts, Has.Length.EqualTo(4));
        }

        [Test]
        public void TryBuild_EmptyAsset_ReportsError()
        {
            Assert.That(asset.TryBuild(out var catalog, out var errors), Is.False);
            Assert.That(catalog, Is.Null);
            Assert.That(errors, Has.Count.EqualTo(1));
        }

        [Test]
        public void TryBuild_TwoBadEntries_ReportsBoth()
        {
            asset.AddShape("tooWide", "Rectangle", new Vector2Int(0, 0), new Vector2Int(3, 0));
            asset.AddShape("noCells", "Rectangle");

            Assert.That(asset.TryBuild(out var catalog, out var errors), Is.False);
            Assert.That(catalog, Is.Null);
            Assert.That(errors, Has.Count.EqualTo(2));
        }

        [Test]
        public void TryBuild_DuplicateId_ReportsError()
        {
            asset.AddShape("same", "Rectangle", Vector2Int.zero);
            asset.AddShape("same", "Rectangle", Vector2Int.zero);

            Assert.That(asset.TryBuild(out _, out var errors), Is.False);
            Assert.That(errors[0], Does.Contain("same"));
        }
    }
}
