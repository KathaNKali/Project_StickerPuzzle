namespace StickerPuzzle.Core
{
    /// <summary>A block on the board. Only <see cref="Board"/> can change where it is.</summary>
    public sealed class Block
    {
        internal Block(int id, ShapeDefinition shape, GridPosition position, string color)
        {
            Id = id;
            Shape = shape;
            Position = position;
            Color = color;
        }

        /// <summary>Unique on its board.</summary>
        public int Id { get; }

        public ShapeDefinition Shape { get; }

        /// <summary>The board cell of the shape's bottom-left corner.</summary>
        public GridPosition Position { get; internal set; }

        /// <summary>Stable color ID. Decides which door the block can use.</summary>
        public string Color { get; }
    }
}
