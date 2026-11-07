using System.Collections.Generic;
using NUnit.Framework;
using StickerPuzzle.EditorTools;
using StickerPuzzle.Game;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class LevelIdentityRepairTests
    {
        private readonly List<BoardLayoutAsset> created = new List<BoardLayoutAsset>();

        [TearDown]
        public void TearDown()
        {
            foreach (var level in created)
            {
                Object.DestroyImmediate(level);
            }

            created.Clear();
        }

        private BoardLayoutAsset MakeLevel(string assetName)
        {
            var level = ScriptableObject.CreateInstance<BoardLayoutAsset>();
            level.name = assetName;
            created.Add(level);
            return level;
        }

        private BoardLayoutAsset MakeCopy(BoardLayoutAsset original, string assetName)
        {
            var copy = Object.Instantiate(original);
            copy.name = assetName;
            created.Add(copy);
            return copy;
        }

        private static List<BoardLayoutAsset> None => new List<BoardLayoutAsset>();

        private static List<BoardLayoutAsset> Only(BoardLayoutAsset level) => new List<BoardLayoutAsset> { level };

        [Test]
        public void Repair_NewLevel_GetsId()
        {
            var level = MakeLevel("Level");

            var changed = LevelIdentity.Repair(None, Only(level));

            Assert.That(level.LevelId, Is.Not.Empty);
            Assert.That(changed, Is.EqualTo(Only(level)));
        }

        [Test]
        public void Repair_LevelAlreadyValid_NothingChanges()
        {
            var level = MakeLevel("Level");
            level.EnsureLevelId();
            level.SetLevelName("First steps");
            var id = level.LevelId;

            var changed = LevelIdentity.Repair(None, Only(level));

            Assert.That(level.LevelId, Is.EqualTo(id));
            Assert.That(level.DisplayName, Is.EqualTo("First steps"));
            Assert.That(changed, Is.Empty);
        }

        [Test]
        public void Repair_CopyOfSettledLevel_CopyGetsNewIdOriginalUntouched()
        {
            var original = MakeLevel("Level");
            original.EnsureLevelId();
            var originalId = original.LevelId;
            var copy = MakeCopy(original, "Level 1");

            var changed = LevelIdentity.Repair(Only(original), Only(copy));

            Assert.That(original.LevelId, Is.EqualTo(originalId));
            Assert.That(copy.LevelId, Is.Not.EqualTo(originalId));
            Assert.That(copy.LevelId, Is.Not.Empty);
            Assert.That(changed, Is.EqualTo(Only(copy)));
        }

        [Test]
        public void Repair_CopyOfNamedLevel_CopyUsesItsFileName()
        {
            var original = MakeLevel("Level");
            original.EnsureLevelId();
            original.SetLevelName("First steps");
            var copy = MakeCopy(original, "Level 1");

            LevelIdentity.Repair(Only(original), Only(copy));

            Assert.That(original.DisplayName, Is.EqualTo("First steps"));
            Assert.That(copy.DisplayName, Is.EqualTo("Level 1"));
        }

        [Test]
        public void Repair_OriginalAndCopyArriveTogether_FirstKeepsId()
        {
            var original = MakeLevel("Level");
            original.EnsureLevelId();
            var originalId = original.LevelId;
            var copy = MakeCopy(original, "Level 1");

            LevelIdentity.Repair(None, new List<BoardLayoutAsset> { original, copy });

            Assert.That(original.LevelId, Is.EqualTo(originalId));
            Assert.That(copy.LevelId, Is.Not.EqualTo(originalId));
        }

        [Test]
        public void Repair_TwoCopiesArriveTogether_AllIdsDiffer()
        {
            var original = MakeLevel("Level");
            original.EnsureLevelId();
            var firstCopy = MakeCopy(original, "Level 1");
            var secondCopy = MakeCopy(original, "Level 2");

            LevelIdentity.Repair(Only(original), new List<BoardLayoutAsset> { firstCopy, secondCopy });

            Assert.That(new[] { original.LevelId, firstCopy.LevelId, secondCopy.LevelId }, Is.Unique);
        }

        [Test]
        public void SortOriginalsFirst_CopyNamedWithSpace_OriginalComesFirst()
        {
            var paths = new List<string> { "Assets/Level 1.asset", "Assets/Level.asset" };

            LevelIdentity.SortOriginalsFirst(paths);

            Assert.That(paths[0], Is.EqualTo("Assets/Level.asset"));
        }
    }
}
