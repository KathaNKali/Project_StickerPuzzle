using System;
using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>
    /// Converts between grid data and world positions, in one place. The board is centered on
    /// the world origin: grid X is world X, grid Y is world Z, and one cell is one world unit.
    /// </summary>
    public readonly struct BoardGrid
    {
        private readonly int width;
        private readonly int height;

        public BoardGrid(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>The world position of a cell's center, at the given world height.</summary>
        public Vector3 CellToWorld(GridPosition cell, float worldHeight = 0f)
        {
            return new Vector3(
                ((width - 1) * -0.5f) + cell.X,
                worldHeight,
                ((height - 1) * -0.5f) + cell.Y);
        }

        /// <summary>The center of a door's bar, placed just outside the board edge.</summary>
        public Vector3 GetDoorCenter(Door door, float outwardOffset, float worldHeight)
        {
            var halfWidth = width * 0.5f;
            var halfHeight = height * 0.5f;
            var middleOfOpening = door.Position + (door.OpeningSize * 0.5f);

            switch (door.Side)
            {
                case BoardSide.Top: return new Vector3(middleOfOpening - halfWidth, worldHeight, halfHeight + outwardOffset);
                case BoardSide.Bottom: return new Vector3(middleOfOpening - halfWidth, worldHeight, -halfHeight - outwardOffset);
                case BoardSide.Left: return new Vector3(-halfWidth - outwardOffset, worldHeight, middleOfOpening - halfHeight);
                case BoardSide.Right: return new Vector3(halfWidth + outwardOffset, worldHeight, middleOfOpening - halfHeight);
                default: throw new InvalidOperationException($"Unknown side: {door.Side}");
            }
        }

        /// <summary>The size of a door's bar: as long as its opening, along its edge.</summary>
        public static Vector3 GetDoorSize(Door door, float thickness, float barHeight)
        {
            return door.Side == BoardSide.Top || door.Side == BoardSide.Bottom
                ? new Vector3(door.OpeningSize, barHeight, thickness)
                : new Vector3(thickness, barHeight, door.OpeningSize);
        }
    }
}
