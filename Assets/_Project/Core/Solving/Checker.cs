using System;

namespace StickerPuzzle.Core
{
    /// <summary>
    /// Replays a solution with the real game rules, on a copy of the board, so the board
    /// passed in never changes. Each step is one drag: every move must be legal, the block
    /// must move at least once, and on letting go it must leave through the step's door,
    /// the same door the game would pick. At the end the board must be empty.
    /// </summary>
    public static class Checker
    {
        /// <summary>True if the solution clears the board. If not, says which step failed and why.</summary>
        public static bool IsSolvable(Board board, Solution solution, out string problem)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (solution == null || solution.Steps.Count == 0)
            {
                problem = "The level has no saved solution.";
                return false;
            }

            var copy = board.Clone();
            var steps = solution.Steps;
            for (var i = 0; i < steps.Count; i++)
            {
                if (!TryReplay(copy, steps[i], out problem))
                {
                    problem = $"Step {i + 1}: {problem}";
                    return false;
                }
            }

            if (!copy.IsCleared)
            {
                problem = $"After the last step, {copy.Blocks.Count} block(s) are still on the board.";
                return false;
            }

            problem = null;
            return true;
        }

        private static bool TryReplay(Board board, SolutionStep step, out string problem)
        {
            var block = board.GetBlockById(step.BlockId);
            if (block == null)
            {
                problem = $"block {step.BlockId} is not on the board.";
                return false;
            }

            if (step.DoorIndex < 0 || step.DoorIndex >= board.Doors.Count)
            {
                problem = $"door {step.DoorIndex} does not exist; the board has {board.Doors.Count} door(s).";
                return false;
            }

            if (step.Moves.Count == 0)
            {
                problem = $"block {step.BlockId} does not move. A block only leaves after moving.";
                return false;
            }

            for (var m = 0; m < step.Moves.Count; m++)
            {
                if (!board.TryMove(block, step.Moves[m]))
                {
                    problem = $"block {step.BlockId} cannot move {step.Moves[m]} (move {m + 1}).";
                    return false;
                }
            }

            // Same choice as letting go in the game: the first door the block fits.
            var exitDoor = board.FindExitDoor(block);
            if (exitDoor == null)
            {
                problem = $"block {step.BlockId} does not fit a door after its moves.";
                return false;
            }

            var expectedDoor = board.Doors[step.DoorIndex];
            if (exitDoor != expectedDoor)
            {
                problem = $"block {step.BlockId} would leave through door {IndexOf(board, exitDoor)}, not door {step.DoorIndex}.";
                return false;
            }

            board.TryExit(block, exitDoor);
            problem = null;
            return true;
        }

        private static int IndexOf(Board board, Door door)
        {
            for (var i = 0; i < board.Doors.Count; i++)
            {
                if (board.Doors[i] == door)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
