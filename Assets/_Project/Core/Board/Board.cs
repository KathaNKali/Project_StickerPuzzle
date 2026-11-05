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

        private int IndexOf(GridPosition cell)
        {
            return (cell.Y * Width) + cell.X;
        }
    }
}
