using System;
using System.Collections.Generic;
using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StickerPuzzle.Tests
{
    public sealed class LevelDataTests
    {
        private readonly List<Object> created = new List<Object>();
        private LevelDataAsset levelData;
        private ShapeCatalog catalog;

        [SetUp]
        public void SetUp()
        {
            levelData = ScriptableObject.CreateInstance<LevelDataAsset>();
            created.Add(levelData);

            var catalogAsset = ScriptableObject.CreateInstance<ShapeCatalogAsset>();
            created.Add(catalogAsset);
            catalogAsset.FillStartingShapes();
            Assert.That(catalogAsset.TryBuild(out catalog, out _), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var asset in created)
            {
                Object.DestroyImmediate(asset);
            }

            created.Clear();
        }

        /// <summary>A valid, playable level with no ID yet.</summary>
        private BoardLayoutAsset MakeLevelWithoutId(string assetName)
        {
            var level = ScriptableObject.CreateInstance<BoardLayoutAsset>();
            level.name = assetName;
            level.FillTestBoard();
            created.Add(level);
            return level;
        }

        private BoardLayoutAsset MakeLevel(string assetName)
        {
            var level = MakeLevelWithoutId(assetName);
            level.EnsureLevelId();
            return level;
        }

        [Test]
        public void GetLevel_SeveralLevels_ReturnsListedOrder()
        {
            var first = MakeLevel("First");
            var second = MakeLevel("Second");

            levelData.AddLevel(first);
            levelData.AddLevel(second);

            Assert.That(levelData.LevelCount, Is.EqualTo(2));
            Assert.That(levelData.GetLevel(0), Is.SameAs(first));
            Assert.That(levelData.GetLevel(1), Is.SameAs(second));
        }

        [Test]
        public void FindProblems_ValidLevels_ReportsNothing()
        {
            levelData.AddLevel(MakeLevel("First"));
            levelData.AddLevel(MakeLevel("Second"));

            Assert.That(levelData.FindProblems(catalog), Is.Empty);
        }

        [Test]
        public void FindProblems_NoCatalog_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => levelData.FindProblems(null));
        }

        [Test]
        public void FindProblems_EmptyList_ReportsIt()
        {
            var problems = levelData.FindProblems(catalog);

            Assert.That(problems, Has.Count.EqualTo(1));
            Assert.That(problems[0], Does.Contain("no levels"));
        }

        [Test]
        public void FindProblems_EmptyRow_ReportsRowNumber()
        {
            levelData.AddLevel(MakeLevel("First"));
            levelData.AddLevel(null);

            var problems = levelData.FindProblems(catalog);

            Assert.That(problems, Has.Count.EqualTo(1));
            Assert.That(problems[0], Does.Contain("Row 2 is empty"));
        }

        [Test]
        public void FindProblems_SameLevelTwice_ReportsBothRows()
        {
            var level = MakeLevel("First");
            levelData.AddLevel(level);
            levelData.AddLevel(level);

            var problems = levelData.FindProblems(catalog);

            Assert.That(problems, Has.Count.EqualTo(1));
            Assert.That(problems[0], Does.Contain("Row 2").And.Contain("row 1"));
        }

        [Test]
        public void FindProblems_TwoLevelsShareId_ReportsBothRows()
        {
            var level = MakeLevel("First");
            var copy = Object.Instantiate(level);
            copy.name = "Copy";
            created.Add(copy);
            levelData.AddLevel(level);
            levelData.AddLevel(copy);

            var problems = levelData.FindProblems(catalog);

            Assert.That(problems, Has.Count.EqualTo(1));
            Assert.That(problems[0], Does.Contain("permanent ID").And.Contain("Row 2").And.Contain("row 1"));
        }

        [Test]
        public void FindProblems_LevelHasNoId_ReportsIt()
        {
            levelData.AddLevel(MakeLevelWithoutId("First"));

            var problems = levelData.FindProblems(catalog);

            Assert.That(problems, Has.Count.EqualTo(1));
            Assert.That(problems[0], Does.Contain("no permanent ID"));
        }

        [Test]
        public void FindProblems_LevelHasBoardProblem_ReportsRowAndName()
        {
            var broken = ScriptableObject.CreateInstance<BoardLayoutAsset>();
            broken.name = "Broken";
            broken.EnsureLevelId();
            created.Add(broken);
            levelData.AddLevel(MakeLevel("First"));
            levelData.AddLevel(broken);

            var problems = levelData.FindProblems(catalog);

            Assert.That(problems, Is.Not.Empty);
            Assert.That(problems, Has.All.Contain("Row 2 'Broken'"));
        }

        [Test]
        public void FindProblems_SeveralProblems_ReportsAll()
        {
            levelData.AddLevel(null);
            levelData.AddLevel(MakeLevelWithoutId("First"));
            levelData.AddLevel(null);

            var problems = levelData.FindProblems(catalog);

            Assert.That(problems, Has.Count.EqualTo(3));
        }
    }
}
