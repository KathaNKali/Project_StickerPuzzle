using System;

namespace StickerPuzzle.Core
{
    /// <summary>
    /// Seeded random numbers that give the same sequence on every platform.
    /// Uses PCG32 (seeded through SplitMix64) with integer maths only, so
    /// results never depend on System.Random or floating point.
    /// One instance per generation run; not thread-safe.
    /// </summary>
    public sealed class SeededRandom
    {
        const ulong Multiplier = 6364136223846793005UL;
        const ulong Increment = 1442695040888963407UL;

        ulong state;

        public SeededRandom(int seed)
        {
            Seed = seed;
            state = 0UL;
            NextUInt();
            state += SplitMix64(unchecked((ulong)(long)seed));
            NextUInt();
        }

        /// <summary>The seed this sequence started from.</summary>
        public int Seed { get; }

        /// <summary>Next raw 32-bit value.</summary>
        public uint NextUInt()
        {
            ulong old = state;
            state = unchecked(old * Multiplier + Increment);
            uint xorShifted = (uint)(((old >> 18) ^ old) >> 27);
            int rotation = (int)(old >> 59);
            return (xorShifted >> rotation) | (xorShifted << (-rotation & 31));
        }

        /// <summary>A whole number from 0 up to, but not including, maxExclusive.</summary>
        public int Next(int maxExclusive)
        {
            if (maxExclusive <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), maxExclusive, "Must be greater than 0.");
            }

            return (int)NextBounded((uint)maxExclusive);
        }

        /// <summary>A whole number from minInclusive up to, but not including, maxExclusive.</summary>
        public int Range(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), maxExclusive, "Must be greater than minInclusive.");
            }

            uint span = unchecked((uint)(maxExclusive - minInclusive));
            return unchecked(minInclusive + (int)NextBounded(span));
        }

        /// <summary>True with a chance of numerator out of denominator.</summary>
        public bool Chance(int numerator, int denominator)
        {
            if (denominator <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(denominator), denominator, "Must be greater than 0.");
            }

            return Next(denominator) < numerator;
        }

        // Unbiased: rejects the few low values that would favour small results.
        uint NextBounded(uint bound)
        {
            uint threshold = unchecked(0u - bound) % bound;
            while (true)
            {
                uint value = NextUInt();
                if (value >= threshold)
                {
                    return value % bound;
                }
            }
        }

        static ulong SplitMix64(ulong value)
        {
            unchecked
            {
                ulong z = value + 0x9E3779B97F4A7C15UL;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                return z ^ (z >> 31);
            }
        }
    }
}
