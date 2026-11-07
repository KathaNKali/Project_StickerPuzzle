using System;

namespace StickerPuzzle.Game
{
    /// <summary>
    /// Tracks which level (wave) is being played, in Level Data order, and when there are none left.
    /// Knows only positions in the list, not the levels themselves.
    /// </summary>
    public sealed class LevelFlow
    {
        public LevelFlow(int levelCount)
        {
            if (levelCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(levelCount), "Level count cannot be negative.");
            }

            LevelCount = levelCount;
        }

        public int LevelCount { get; }

        /// <summary>The level being played, from 0. Equals LevelCount once every level is finished.</summary>
        public int CurrentIndex { get; private set; }

        /// <summary>True once the last level is cleared, or straight away if there are no levels.</summary>
        public bool IsFinished => CurrentIndex >= LevelCount;

        /// <summary>Moves on after the current level is cleared. Does nothing once finished.</summary>
        public void Advance()
        {
            if (!IsFinished)
            {
                CurrentIndex++;
            }
        }

        /// <summary>Goes back to the first level.</summary>
        public void Restart()
        {
            CurrentIndex = 0;
        }
    }
}
