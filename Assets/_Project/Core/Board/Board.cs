using System;
using System.Collections.Generic;

namespace StickerPuzzle.Core
{
    /// <summary>
    /// The grid and the blocks on it. Keeps a cell-by-cell lookup, so asking
    /// "what is in this cell?" is instant and creates no memory garbage.
    /// </summary>
    public sealed class Board
    {
        /// <summary>Default door count for a playable board (puzzle-rules). Generator settings can change it.</summary>
        public const int MinDoors = 2;
        public const int MaxDoors = 4;

        /// <summary>Smallest playable board, in cells per side (designer, 2026-11-07). Rule tests may use smaller boards.</summary>
        public const int MinPlayableSize = 5;

        private readonly Block[] occupancy;
        private readonly List<Block> blocks = new List<Block>();
        private readonly List<Door> doors = new List<Door>();
        private int nextBlockId;

        public Board(int width, int height)
        {
            if (width < 1 || height < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(width), $"Board size must be at least 1x1, got {width}x{height}.");
            }

            Width = width;
            Height = height;
            occupancy = new Block[width * height];
        }

        public int Width { get; }

        public int Height { get; }

        /// <summary>
        /// An independent copy: same size, doors and blocks, with the same block IDs.
        /// Changing the copy never changes this board. Doors are shared because they never change.
        /// </summary>
        public Board Clone()
        {
            var copy = new Board(Width, Height);
            copy.doors.AddRange(doors);
            copy.nextBlockId = nextBlockId;

            for (var i = 0; i < blocks.Count; i++)
            {
                var source = blocks[i];
                var block = new Block(source.Id, source.Shape, source.Position, source.Color);
                copy.blocks.Add(block);

                var cells = block.Shape.Cells;
                for (var c = 0; c < cells.Count; c++)
                {
                    copy.occupancy[copy.IndexOf(block.Position + cells[c])] = block;
                }
            }

            return copy;
        }

        /// <summary>The block with this ID, or null if it is not on the board.</summary>
        public Block GetBlockById(int id)
        {
            for (var i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].Id == id)
                {
                    return blocks[i];
                }
            }

            return null;
        }

        public IReadOnlyList<Block> Blocks => blocks;

        public IReadOnlyList<Door> Doors => doors;

        /// <summary>True when no blocks are left on the board.</summary>
        public bool IsCleared => blocks.Count == 0;

        public bool IsInside(GridPosition cell)
        {
            return cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;
        }

        /// <summary>The block covering this cell, or null if it is free or off the board.</summary>
        public Block GetBlockAt(GridPosition cell)
        {
            return IsInside(cell) ? occupancy[IndexOf(cell)] : null;
        }

        /// <summary>True if every cell of the shape would be on the board and free.</summary>
        public bool CanPlace(ShapeDefinition shape, GridPosition position)
        {
            if (shape == null)
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var cells = shape.Cells;
            for (var i = 0; i < cells.Count; i++)
            {
                var cell = position + cells[i];
                if (!IsInside(cell) || occupancy[IndexOf(cell)] != null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Adds a block if it fits. If not, returns false and changes nothing.</summary>
        public bool TryAddBlock(ShapeDefinition shape, GridPosition position, string color, out Block block)
        {
            if (string.IsNullOrWhiteSpace(color))
            {
                throw new ArgumentException("Block needs a color.", nameof(color));
            }

            if (!CanPlace(shape, position))
            {
                block = null;
                return false;
            }

            block = new Block(nextBlockId++, shape, position, color);
            blocks.Add(block);

            var cells = shape.Cells;
            for (var i = 0; i < cells.Count; i++)
            {
                occupancy[IndexOf(position + cells[i])] = block;
            }

            return true;
        }

        /// <summary>
        /// True if the block can step one cell this way: every cell it would cover
        /// must be on the board and either free or already part of this same block.
        /// </summary>
        public bool CanMove(Block block, Direction direction)
        {
            RequireOnBoard(block);
            return CanOccupy(block, block.Position.Step(direction));
        }

        /// <summary>
        /// True if the block would fit at this position: every cell on the board and either free
        /// or already part of this same block. Does not check that a route there exists.
        /// </summary>
        public bool CanOccupy(Block block, GridPosition position)
        {
            RequireOnBoard(block);

            var cells = block.Shape.Cells;
            for (var i = 0; i < cells.Count; i++)
            {
                var cell = position + cells[i];
                if (!IsInside(cell))
                {
                    return false;
                }

                var occupant = occupancy[IndexOf(cell)];
                if (occupant != null && occupant != block)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Steps the block one cell if it can. If not, returns false and changes nothing.</summary>
        public bool TryMove(Block block, Direction direction)
        {
            if (!CanMove(block, direction))
            {
                return false;
            }

            var cells = block.Shape.Cells;
            for (var i = 0; i < cells.Count; i++)
            {
                occupancy[IndexOf(block.Position + cells[i])] = null;
            }

            block.Position = block.Position.Step(direction);
            for (var i = 0; i < cells.Count; i++)
            {
                occupancy[IndexOf(block.Position + cells[i])] = block;
            }

            return true;
        }

        /// <summary>
        /// Adds a door if it fits its edge and does not overlap another door on that edge.
        /// If not, returns false, says why and changes nothing.
        /// </summary>
        public bool TryAddDoor(Door door, out string error)
        {
            if (door == null)
            {
                throw new ArgumentNullException(nameof(door));
            }

            var edgeLength = door.Side == BoardSide.Top || door.Side == BoardSide.Bottom ? Width : Height;
            if (door.Position + door.OpeningSize > edgeLength)
            {
                error = $"{door.Side} door at {door.Position} with opening {door.OpeningSize} does not fit an edge of length {edgeLength}.";
                return false;
            }

            for (var i = 0; i < doors.Count; i++)
            {
                var other = doors[i];
                if (other.Side == door.Side
                    && door.Position < other.Position + other.OpeningSize
                    && other.Position < door.Position + door.OpeningSize)
                {
                    error = $"{door.Side} door at {door.Position} overlaps the door at {other.Position}.";
                    return false;
                }
            }

            if (SharesCornerWithSameColorDoor(door))
            {
                error = $"{door.Side} door at {door.Position} shares a corner with a door of the same color ({door.Color}).";
                return false;
            }

            doors.Add(door);
            error = null;
            return true;
        }

        /// <summary>The door this block could leave through right now, or null if there is none.</summary>
        public Door FindExitDoor(Block block)
        {
            RequireOnBoard(block);

            for (var i = 0; i < doors.Count; i++)
            {
                if (CanExitThroughDoor(block, doors[i]))
                {
                    return doors[i];
                }
            }

            return null;
        }

        /// <summary>True if this cell is in the board's edge row or column, directly against the door's opening.</summary>
        public bool IsTouchingDoor(Door door, GridPosition cell)
        {
            if (door == null)
            {
                throw new ArgumentNullException(nameof(door));
            }

            if (!IsInside(cell))
            {
                return false;
            }

            switch (door.Side)
            {
                case BoardSide.Top: return cell.Y == Height - 1 && door.CoversAlongEdge(cell.X);
                case BoardSide.Bottom: return cell.Y == 0 && door.CoversAlongEdge(cell.X);
                case BoardSide.Left: return cell.X == 0 && door.CoversAlongEdge(cell.Y);
                case BoardSide.Right: return cell.X == Width - 1 && door.CoversAlongEdge(cell.Y);
                default: throw new InvalidOperationException($"Unknown side: {door.Side}");
            }
        }

        /// <summary>
        /// THE exit rule (docs/design/puzzle-rules.md). True if the door is the block's color,
        /// the block is touching it, and the block's full width along that edge fits inside
        /// the opening. Exact width is not required; partial overlap is not allowed.
        /// </summary>
        public bool CanExitThroughDoor(Block block, Door door)
        {
            RequireOnBoard(block);
            return CanExitThroughDoorAt(block, door, block.Position);
        }

        /// <summary>
        /// The same exit rule as <see cref="CanExitThroughDoor"/>, for the block as if it stood at this
        /// position. Lets the pathfinder test spots without moving the block. Ignores other blocks.
        /// </summary>
        public bool CanExitThroughDoorAt(Block block, Door door, GridPosition position)
        {
            RequireOnBoard(block);

            if (door == null)
            {
                throw new ArgumentNullException(nameof(door));
            }

            if (!doors.Contains(door))
            {
                throw new ArgumentException("Door is not on this board.", nameof(door));
            }

            if (!string.Equals(block.Color, door.Color, StringComparison.Ordinal))
            {
                return false;
            }

            var minX = position.X;
            var minY = position.Y;
            var maxX = minX + block.Shape.Width - 1;
            var maxY = minY + block.Shape.Height - 1;

            // The two ends of the block's side facing the door. The opening has no gaps,
            // so if both ends are inside it, everything between them is too.
            GridPosition firstEnd;
            GridPosition lastEnd;
            switch (door.Side)
            {
                case BoardSide.Top:
                    firstEnd = new GridPosition(minX, maxY);
                    lastEnd = new GridPosition(maxX, maxY);
                    break;
                case BoardSide.Bottom:
                    firstEnd = new GridPosition(minX, minY);
                    lastEnd = new GridPosition(maxX, minY);
                    break;
                case BoardSide.Left:
                    firstEnd = new GridPosition(minX, minY);
                    lastEnd = new GridPosition(minX, maxY);
                    break;
                case BoardSide.Right:
                    firstEnd = new GridPosition(maxX, minY);
                    lastEnd = new GridPosition(maxX, maxY);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown side: {door.Side}");
            }

            return IsTouchingDoor(door, firstEnd) && IsTouchingDoor(door, lastEnd);
        }

        /// <summary>Removes the block if it can exit through this door. If not, returns false and changes nothing.</summary>
        public bool TryExit(Block block, Door door)
        {
            if (!CanExitThroughDoor(block, door))
            {
                return false;
            }

            RemoveBlock(block);
            return true;
        }

        /// <summary>
        /// Takes the block off the board without any exit rule. For tools such as the generator,
        /// never for play. Block IDs are not reused.
        /// </summary>
        public void RemoveBlock(Block block)
        {
            RequireOnBoard(block);

            var cells = block.Shape.Cells;
            for (var i = 0; i < cells.Count; i++)
            {
                occupancy[IndexOf(block.Position + cells[i])] = null;
            }

            blocks.Remove(block);
        }

        /// <summary>
        /// A block in a corner could otherwise touch two doors of its own color at once.
        /// The designer has ruled this out for now (docs/design/puzzle-rules.md).
        /// </summary>
        private bool SharesCornerWithSameColorDoor(Door door)
        {
            for (var i = 0; i < doors.Count; i++)
            {
                var other = doors[i];
                if (other.Side != door.Side
                    && string.Equals(other.Color, door.Color, StringComparison.Ordinal)
                    && (TouchesAtCell(door, other, new GridPosition(0, 0))
                        || TouchesAtCell(door, other, new GridPosition(Width - 1, 0))
                        || TouchesAtCell(door, other, new GridPosition(0, Height - 1))
                        || TouchesAtCell(door, other, new GridPosition(Width - 1, Height - 1))))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TouchesAtCell(Door first, Door second, GridPosition cell)
        {
            return IsTouchingDoor(first, cell) && IsTouchingDoor(second, cell);
        }

        private void RequireOnBoard(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            if (GetBlockAt(block.Position + block.Shape.Cells[0]) != block)
            {
                throw new ArgumentException($"Block {block.Id} is not on this board.", nameof(block));
            }
        }

        private int IndexOf(GridPosition cell)
        {
            return (cell.Y * Width) + cell.X;
        }
    }
}
