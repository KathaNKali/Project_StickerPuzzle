using System;
using NUnit.Framework;
using StickerPuzzle.Game;

namespace StickerPuzzle.Tests
{
    public sealed class LevelFlowTests
    {
        [Test]
        public void Constructor_SeveralLevels_StartsAtFirstLevel()
        {
            var flow = new LevelFlow(5);

            Assert.That(flow.CurrentIndex, Is.EqualTo(0));
            Assert.That(flow.IsFinished, Is.False);
        }

        [Test]
        public void Constructor_NoLevels_FinishedStraightAway()
        {
            Assert.That(new LevelFlow(0).IsFinished, Is.True);
        }

        [Test]
        public void Constructor_NegativeCount_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelFlow(-1));
        }

        [Test]
        public void Advance_NotLastLevel_MovesToNextLevel()
        {
            var flow = new LevelFlow(5);

            flow.Advance();

            Assert.That(flow.CurrentIndex, Is.EqualTo(1));
            Assert.That(flow.IsFinished, Is.False);
        }

        [Test]
        public void Advance_LastLevel_Finishes()
        {
            var flow = new LevelFlow(2);

            flow.Advance();
            flow.Advance();

            Assert.That(flow.IsFinished, Is.True);
        }

        [Test]
        public void Advance_AlreadyFinished_StaysFinished()
        {
            var flow = new LevelFlow(1);
            flow.Advance();

            flow.Advance();

            Assert.That(flow.CurrentIndex, Is.EqualTo(1));
            Assert.That(flow.IsFinished, Is.True);
        }

        [Test]
        public void Restart_Finished_BackToFirstLevel()
        {
            var flow = new LevelFlow(2);
            flow.Advance();
            flow.Advance();

            flow.Restart();

            Assert.That(flow.CurrentIndex, Is.EqualTo(0));
            Assert.That(flow.IsFinished, Is.False);
        }
    }
}
