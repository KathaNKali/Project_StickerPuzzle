using System;
using StickerPuzzle.Core;

namespace StickerPuzzle.Game
{
    /// <summary>
    /// One player's play of a board. Input sends commands here, the rules in Core decide,
    /// and views listen to the events to animate. Views never change the board themselves.
    /// </summary>
    public sealed class PuzzleSession
    {
        private bool movedThisDrag;

        public PuzzleSession(Board board)
        {
            Board = board ?? throw new ArgumentNullException(nameof(board));
        }

        /// <summary>A block moved one cell. Its Position is already the new cell.</summary>
        public event Action<Block> BlockMoved;

        /// <summary>A block left the board through this door.</summary>
        public event Action<Block, Door> BlockExited;

        /// <summary>The last block has left the board.</summary>
        public event Action BoardCleared;

        public Board Board { get; }

        /// <summary>The block being dragged, or null.</summary>
        public Block HeldBlock { get; private set; }

        /// <summary>Starts a drag on the block in this cell. False if there is none, or one is already held.</summary>
        public bool TryPickUp(GridPosition cell)
        {
            if (HeldBlock != null)
            {
                return false;
            }

            var block = Board.GetBlockAt(cell);
            if (block == null)
            {
                return false;
            }

            HeldBlock = block;
            movedThisDrag = false;
            return true;
        }

        /// <summary>Moves the held block one cell. False if nothing is held or the way is blocked.</summary>
        public bool TryStep(Direction direction)
        {
            if (HeldBlock == null || !Board.TryMove(HeldBlock, direction))
            {
                return false;
            }

            movedThisDrag = true;
            BlockMoved?.Invoke(HeldBlock);
            return true;
        }

        /// <summary>
        /// Moves the held block toward a target position, one legal cell at a time, so it can
        /// never jump over anything. It tries the longer axis first, then the other one. Every
        /// step gets nearer the target, so this always ends. Returns the number of cells moved.
        /// </summary>
        public int DragHeldToward(GridPosition target)
        {
            if (HeldBlock == null)
            {
                return 0;
            }

            var steps = 0;
            while (HeldBlock.Position != target && TryStepToward(target))
            {
                steps++;
            }

            return steps;
        }

        private bool TryStepToward(GridPosition target)
        {
            var offset = target - HeldBlock.Position;
            var horizontal = offset.X > 0 ? Direction.Right : Direction.Left;
            var vertical = offset.Y > 0 ? Direction.Up : Direction.Down;
            var horizontalFirst = Math.Abs(offset.X) >= Math.Abs(offset.Y);

            if (horizontalFirst)
            {
                return (offset.X != 0 && TryStep(horizontal)) || (offset.Y != 0 && TryStep(vertical));
            }

            return (offset.Y != 0 && TryStep(vertical)) || (offset.X != 0 && TryStep(horizontal));
        }

        /// <summary>
        /// Ends the drag. The block exits if it moved at least one cell during this drag
        /// and now fits a door; otherwise it stays in its cell. False if nothing was held.
        /// </summary>
        public bool LetGo()
        {
            if (HeldBlock == null)
            {
                return false;
            }

            var block = HeldBlock;
            HeldBlock = null;

            var door = movedThisDrag ? Board.FindExitDoor(block) : null;
            if (door != null && Board.TryExit(block, door))
            {
                BlockExited?.Invoke(block, door);

                if (Board.IsCleared)
                {
                    BoardCleared?.Invoke();
                }
            }

            return true;
        }
    }
}
