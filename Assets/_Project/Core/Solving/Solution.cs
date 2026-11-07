using System;
using System.Collections.Generic;

namespace StickerPuzzle.Core
{
    /// <summary>
    /// One drag in a solution: pick up a block, step it cell by cell, let go, and it
    /// leaves through a door. Blocks are referred to by their board ID, doors by their
    /// place in the board's door list.
    /// </summary>
    public sealed class SolutionStep
    {
        private readonly Direction[] moves;

        public SolutionStep(int blockId, IEnumerable<Direction> moves, int doorIndex)
        {
            if (moves == null)
            {
                throw new ArgumentNullException(nameof(moves));
            }

            BlockId = blockId;
            this.moves = new List<Direction>(moves).ToArray();
            DoorIndex = doorIndex;
        }

        public int BlockId { get; }

        /// <summary>The one-cell steps of the drag, in order.</summary>
        public IReadOnlyList<Direction> Moves => moves;

        public int DoorIndex { get; }
    }

    /// <summary>The order in which blocks leave the board, and how each one gets out.</summary>
    public sealed class Solution
    {
        private readonly SolutionStep[] steps;

        public Solution(IEnumerable<SolutionStep> steps)
        {
            if (steps == null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            this.steps = new List<SolutionStep>(steps).ToArray();
            for (var i = 0; i < this.steps.Length; i++)
            {
                if (this.steps[i] == null)
                {
                    throw new ArgumentException($"Step {i + 1} is missing.", nameof(steps));
                }
            }
        }

        public IReadOnlyList<SolutionStep> Steps => steps;
    }
}
