using StickerPuzzle.Core;

namespace StickerPuzzle.Tests
{
    /// <summary>Shapes for tests. Independent of the real catalog, so editing it cannot break rule tests.</summary>
    internal static class TestShapes
    {
        public static readonly ShapeDefinition Single = Make("single", 0, 0);
        public static readonly ShapeDefinition TwoWide = Make("twoWide", 0, 0, 1, 0);
        public static readonly ShapeDefinition TwoTall = Make("twoTall", 0, 0, 0, 1);
        public static readonly ShapeDefinition TwoByTwo = Make("twoByTwo", 0, 0, 1, 0, 0, 1, 1, 1);

        /// <summary>Covers bottom-left, bottom-right and top-left. The top-right cell is empty.</summary>
        public static readonly ShapeDefinition LShape = Make("lShape", 0, 0, 1, 0, 0, 1);

        private static ShapeDefinition Make(string id, params int[] xy)
        {
            var cells = new GridPosition[xy.Length / 2];
            for (var i = 0; i < cells.Length; i++)
            {
                cells[i] = new GridPosition(xy[i * 2], xy[(i * 2) + 1]);
            }

            ShapeDefinition.TryCreate(id, "Test", cells, out var shape, out var error);
            return shape ?? throw new System.InvalidOperationException(error);
        }
    }
}
