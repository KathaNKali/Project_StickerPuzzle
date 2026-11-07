using System.Linq;
using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Game;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class LevelSolutionTests
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
            Object.DestroyImmediate(layout);
            Object.DestroyImmediate(catalogAsset);
        }

        [Test]
        public void GetSolution_NoneSaved_ReturnsNull()
        {
            Assert.That(layout.HasSolution, Is.False);
            Assert.That(layout.GetSolution(), Is.Null);
        }

        [Test]
        public void TestBoard_SavedSolution_PassesChecker()
        {
            layout.FillTestBoard();
            Assert.That(layout.TryBuild(catalog, out var board, out var errors), Is.True, string.Join("\n", errors));

            Assert.That(Checker.IsSolvable(board, layout.GetSolution(), out var problem), Is.True, problem);
        }

        [Test]
        public void SetSolution_ThenGet_KeepsStepsAndSeed()
        {
            var saved = new Solution(new[]
            {
                new SolutionStep(3, new[] { Direction.Up, Direction.Left }, 1),
                new SolutionStep(0, new[] { Direction.Right }, 0),
            });

            layout.SetSolution(saved, -12345);
            var loaded = layout.GetSolution();

            Assert.That(layout.Seed, Is.EqualTo(-12345));
            Assert.That(loaded.Steps.Select(s => s.BlockId), Is.EqualTo(new[] { 3, 0 }));
            Assert.That(loaded.Steps.Select(s => s.DoorIndex), Is.EqualTo(new[] { 1, 0 }));
            Assert.That(loaded.Steps[0].Moves, Is.EqualTo(new[] { Direction.Up, Direction.Left }));
            Assert.That(loaded.Steps[1].Moves, Is.EqualTo(new[] { Direction.Right }));
        }

        [Test]
        public void SetSolution_Twice_ReplacesOldSteps()
        {
            layout.FillTestBoard();
            layout.SetSolution(new Solution(new[] { new SolutionStep(0, new[] { Direction.Up }, 0) }), 7);

            Assert.That(layout.GetSolution().Steps.Count, Is.EqualTo(1));
            Assert.That(layout.Seed, Is.EqualTo(7));
        }
    }
}
