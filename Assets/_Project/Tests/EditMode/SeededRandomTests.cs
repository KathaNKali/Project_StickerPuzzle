using System;
using NUnit.Framework;
using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    public sealed class SeededRandomTests
    {
        const int Samples = 10000;

        [Test]
        public void NextUInt_SameSeed_GivesSameSequence()
        {
            var a = new SeededRandom(42);
            var b = new SeededRandom(42);

            for (int i = 0; i < Samples; i++)
            {
                Assert.That(a.NextUInt(), Is.EqualTo(b.NextUInt()), $"Differs at step {i}.");
            }
        }

        [Test]
        public void NextUInt_DifferentSeeds_GiveDifferentSequences()
        {
            for (int seed = 0; seed < 100; seed++)
            {
                var a = new SeededRandom(seed);
                var b = new SeededRandom(seed + 1);
                bool anyDifferent = false;
                for (int i = 0; i < 8 && !anyDifferent; i++)
                {
                    anyDifferent = a.NextUInt() != b.NextUInt();
                }

                Assert.That(anyDifferent, Is.True, $"Seeds {seed} and {seed + 1} matched.");
            }
        }

        // Pinned values: if these change, every saved seed would make a different board.
        [TestCase(0, 880686761u, 653768305u, 297326204u)]
        [TestCase(12345, 2127481080u, 1035555668u, 736530996u)]
        [TestCase(-1, 2148051951u, 398173282u, 3622147859u)]
        public void NextUInt_KnownSeed_GivesPinnedValues(int seed, uint first, uint second, uint third)
        {
            var random = new SeededRandom(seed);

            Assert.That(new[] { random.NextUInt(), random.NextUInt(), random.NextUInt() },
                Is.EqualTo(new[] { first, second, third }));
        }

        [Test]
        public void Seed_AfterConstruction_ReturnsGivenSeed()
        {
            Assert.That(new SeededRandom(-7).Seed, Is.EqualTo(-7));
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        public void Next_ManySamples_StaysWithinBoundsAndHitsEveryValue(int maxExclusive)
        {
            var random = new SeededRandom(7);
            var seen = new bool[maxExclusive];

            for (int i = 0; i < Samples; i++)
            {
                int value = random.Next(maxExclusive);
                Assert.That(value, Is.InRange(0, maxExclusive - 1));
                seen[value] = true;
            }

            Assert.That(seen, Is.All.True);
        }

        [TestCase(-3, 4)]
        [TestCase(5, 6)]
        [TestCase(100, 110)]
        public void Range_ManySamples_StaysWithinBoundsAndHitsEveryValue(int minInclusive, int maxExclusive)
        {
            var random = new SeededRandom(99);
            var seen = new bool[maxExclusive - minInclusive];

            for (int i = 0; i < Samples; i++)
            {
                int value = random.Range(minInclusive, maxExclusive);
                Assert.That(value, Is.InRange(minInclusive, maxExclusive - 1));
                seen[value - minInclusive] = true;
            }

            Assert.That(seen, Is.All.True);
        }

        [Test]
        public void Range_FullIntSpan_StaysWithinBounds()
        {
            var random = new SeededRandom(3);

            for (int i = 0; i < Samples; i++)
            {
                Assert.That(random.Range(int.MinValue, int.MaxValue), Is.LessThan(int.MaxValue));
            }
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Next_MaxNotPositive_Throws(int maxExclusive)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SeededRandom(1).Next(maxExclusive));
        }

        [TestCase(5, 5)]
        [TestCase(5, 2)]
        public void Range_MaxNotAboveMin_Throws(int minInclusive, int maxExclusive)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SeededRandom(1).Range(minInclusive, maxExclusive));
        }

        [Test]
        public void Chance_ZeroAndFull_AreNeverAndAlways()
        {
            var random = new SeededRandom(11);

            for (int i = 0; i < 1000; i++)
            {
                Assert.That(random.Chance(0, 4), Is.False);
                Assert.That(random.Chance(4, 4), Is.True);
            }
        }
    }
}
