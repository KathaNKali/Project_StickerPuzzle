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

        public IReadOnlyList<Block> Blocks => blocks;

        public IReadOnlyList<Door> Doors => doors;

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

            var target = block.Position.Step(direction);
            var cells = block.Shape.Cells;
            for (var i = 0; i < cells.Count; i++)
            {
                var cell = target + cells[i];
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

            doors.Add(door);
            error = null;
            return true;
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
