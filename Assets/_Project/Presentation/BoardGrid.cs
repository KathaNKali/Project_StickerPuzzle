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

        /// <summary>The grid cell containing a world position. The result can be off the board.</summary>
        public GridPosition WorldToCell(Vector3 world)
        {
            return new GridPosition(
                Mathf.FloorToInt(world.x + (width * 0.5f)),
                Mathf.FloorToInt(world.z + (height * 0.5f)));
        }

        /// <summary>Where a ray from the camera meets a flat plane at this world height. False if it never does.</summary>
        public static bool TryGetPointAtHeight(Ray ray, float worldHeight, out Vector3 point)
        {
            var plane = new Plane(Vector3.up, new Vector3(0f, worldHeight, 0f));
            if (plane.Raycast(ray, out var distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }

            point = default;
            return false;
        }

        /// <summary>
        /// Where a block's bottom-left cell ends up once it has slid straight out through a door:
        /// its full length past the edge, plus some clearance beyond the door's bar.
        /// </summary>
        public Vector3 GetExitPosition(Block block, Door door, float clearance)
        {
            var start = CellToWorld(block.Position);

            switch (door.Side)
            {
                case BoardSide.Top: return start + (Vector3.forward * (block.Shape.Height + clearance));
                case BoardSide.Bottom: return start + (Vector3.back * (block.Shape.Height + clearance));
                case BoardSide.Left: return start + (Vector3.left * (block.Shape.Width + clearance));
                case BoardSide.Right: return start + (Vector3.right * (block.Shape.Width + clearance));
                default: throw new InvalidOperationException($"Unknown side: {door.Side}");
            }
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
