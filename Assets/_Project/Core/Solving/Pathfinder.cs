using System;
using System.Collections.Generic;

namespace StickerPuzzle.Core
{
    /// <summary>
    /// Finds the shortest drag that takes a block to a spot where it can leave through a door,
    /// moving one cell at a time around the other blocks, which stay still. Uses the real
    /// board rules, never moves anything, and reuses its memory between calls, so one
    /// instance can be asked thousands of times without creating garbage.
    /// </summary>
    public sealed class Pathfinder
    {
        private static readonly Direction[] StepOrder = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        private bool[] visited = new bool[0];
        private int[] cameFrom = new int[0];
        private Direction[] stepTaken = new Direction[0];
        private int[] queue = new int[0];
        private readonly List<Direction> reversed = new List<Direction>();

        /// <summary>
        /// Shortest route for the block to leave through this door. The route always has at least
        /// one step, since a block must move during the drag it leaves on. Returns false and leaves
        /// the route empty if there is none.
        /// </summary>
        public bool TryFindRoute(Board board, Block block, Door door, List<Direction> route)
        {
            if (door == null)
            {
                throw new ArgumentNullException(nameof(door));
            }

            return Search(board, block, door, route, out _);
        }

        /// <summary>
        /// Shortest route for the block to leave through any door it can use. If two doors are equally
        /// close, the search's fixed step order decides, so the same board always gives the same answer.
        /// Returns false if no door can be reached.
        /// </summary>
        public bool TryFindExit(Board board, Block block, List<Direction> route, out Door door)
        {
            return Search(board, block, null, route, out door);
        }

        /// <summary>Breadth-first search over the block's positions. A null target door means any door.</summary>
        private bool Search(Board board, Block block, Door targetDoor, List<Direction> route, out Door foundDoor)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            route.Clear();
            foundDoor = null;

            var width = board.Width;
            var cellCount = width * board.Height;
            PrepareBuffers(cellCount);

            var start = block.Position;
            var startIndex = start.X + (start.Y * width);
            visited[startIndex] = true;
            queue[0] = startIndex;
            var head = 0;
            var tail = 1;

            while (head < tail)
            {
                var currentIndex = queue[head++];
                var current = new GridPosition(currentIndex % width, currentIndex / width);

                for (var d = 0; d < StepOrder.Length; d++)
                {
                    var direction = StepOrder[d];
                    var next = current.Step(direction);
                    if (!board.CanOccupy(block, next))
                    {
                        continue;
                    }

                    // Checked before "visited", so a block already at its exit can step away and back.
                    var door = FindDoorAt(board, block, targetDoor, next);
                    if (door != null)
                    {
                        foundDoor = door;
                        BuildRoute(startIndex, currentIndex, direction, route);
                        return true;
                    }

                    var nextIndex = next.X + (next.Y * width);
                    if (visited[nextIndex])
                    {
                        continue;
                    }

                    visited[nextIndex] = true;
                    cameFrom[nextIndex] = currentIndex;
                    stepTaken[nextIndex] = direction;
                    queue[tail++] = nextIndex;
                }
            }

            return false;
        }

        private static Door FindDoorAt(Board board, Block block, Door targetDoor, GridPosition position)
        {
            if (targetDoor != null)
            {
                return board.CanExitThroughDoorAt(block, targetDoor, position) ? targetDoor : null;
            }

            var doors = board.Doors;
            for (var i = 0; i < doors.Count; i++)
            {
                if (board.CanExitThroughDoorAt(block, doors[i], position))
                {
                    return doors[i];
                }
            }

            return null;
        }

        /// <summary>Walks back from the last position to the start, then flips the steps into order.</summary>
        private void BuildRoute(int startIndex, int lastIndex, Direction finalStep, List<Direction> route)
        {
            reversed.Clear();
            reversed.Add(finalStep);
            for (var index = lastIndex; index != startIndex; index = cameFrom[index])
            {
                reversed.Add(stepTaken[index]);
            }

            for (var i = reversed.Count - 1; i >= 0; i--)
            {
                route.Add(reversed[i]);
            }
        }

        /// <summary>Grows the buffers only when a bigger board arrives; otherwise just clears "visited".</summary>
        private void PrepareBuffers(int cellCount)
        {
            if (visited.Length < cellCount)
            {
                visited = new bool[cellCount];
                cameFrom = new int[cellCount];
                stepTaken = new Direction[cellCount];
                queue = new int[cellCount];
                return;
            }

            Array.Clear(visited, 0, cellCount);
        }
    }
}
