using NUnit.Framework;
using StickerPuzzle.Game;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class LevelIdentityTests
    {
        private BoardLayoutAsset level;
        private BoardLayoutAsset otherLevel;

        [SetUp]
        public void SetUp()
        {
            level = ScriptableObject.CreateInstance<BoardLayoutAsset>();
            otherLevel = ScriptableObject.CreateInstance<BoardLayoutAsset>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(level);
            Object.DestroyImmediate(otherLevel);
        }

        [Test]
        public void EnsureLevelId_NoId_CreatesOne()
        {
            Assert.That(level.EnsureLevelId(), Is.True);
            Assert.That(level.LevelId, Is.Not.Empty);
        }

        [Test]
        public void EnsureLevelId_AlreadyHasId_KeepsIt()
        {
            level.EnsureLevelId();
            var firstId = level.LevelId;

            Assert.That(level.EnsureLevelId(), Is.False);
            Assert.That(level.LevelId, Is.EqualTo(firstId));
        }

        [Test]
        public void EnsureLevelId_TwoLevels_GetDifferentIds()
        {
            level.EnsureLevelId();
            otherLevel.EnsureLevelId();

            Assert.That(level.LevelId, Is.Not.EqualTo(otherLevel.LevelId));
        }

        [Test]
        public void LevelId_SurvivesRenamingAndRefilling()
        {
            level.EnsureLevelId();
            var firstId = level.LevelId;

            level.SetLevelName("Renamed");
            level.FillTestBoard();

            Assert.That(level.LevelId, Is.EqualTo(firstId));
        }

        [Test]
        public void DisplayName_NoName_UsesAssetName()
        {
            level.name = "Level_01";

            Assert.That(level.DisplayName, Is.EqualTo("Level_01"));
        }

        [Test]
        public void DisplayName_WithName_UsesIt()
        {
            level.name = "Level_01";
            level.SetLevelName("First steps");

            Assert.That(level.DisplayName, Is.EqualTo("First steps"));
        }
    }
}
