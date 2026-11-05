using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>
    /// Converts between grid cells and world positions, in one place. The board is centered on
    /// the world origin: grid X is world X, grid Y is world Z, and one cell is one world unit.
    /// </summary>
    public readonly struct BoardGrid
    {
        private readonly float originX;
        private readonly float originZ;

        public BoardGrid(int width, int height)
        {
            originX = (width - 1) * -0.5f;
            originZ = (height - 1) * -0.5f;
        }

        /// <summary>The world position of a cell's center, at the given world height.</summary>
        public Vector3 CellToWorld(GridPosition cell, float worldHeight = 0f)
        {
            return new Vector3(originX + cell.X, worldHeight, originZ + cell.Y);
        }
    }
}
