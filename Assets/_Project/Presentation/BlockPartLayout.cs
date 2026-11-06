using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>
    /// Works out the box for each cell of a graybox block, so any shape looks like one solid piece:
    /// a small gap on outside edges, and no gap between cells of the same block.
    /// </summary>
    public static class BlockPartLayout
    {
        /// <summary>The part's center and size on the flat board, relative to the block's bottom-left cell. X is grid X; Y is grid Y.</summary>
        public static void GetPart(ShapeDefinition shape, int cellIndex, float gap, out Vector2 center, out Vector2 size)
        {
            var cell = shape.Cells[cellIndex];

            var left = HasCell(shape, cell.Step(Direction.Left)) ? -0.5f : -0.5f + gap;
            var right = HasCell(shape, cell.Step(Direction.Right)) ? 0.5f : 0.5f - gap;
            var bottom = HasCell(shape, cell.Step(Direction.Down)) ? -0.5f : -0.5f + gap;
            var top = HasCell(shape, cell.Step(Direction.Up)) ? 0.5f : 0.5f - gap;

            center = new Vector2(cell.X + ((left + right) * 0.5f), cell.Y + ((bottom + top) * 0.5f));
            size = new Vector2(right - left, top - bottom);
        }

        private static bool HasCell(ShapeDefinition shape, GridPosition cell)
        {
            var cells = shape.Cells;
            for (var i = 0; i < cells.Count; i++)
            {
                if (cells[i] == cell)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
