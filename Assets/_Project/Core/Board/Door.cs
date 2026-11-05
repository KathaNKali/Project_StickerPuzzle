using System;

namespace StickerPuzzle.Core
{
    /// <summary>The board edge a door sits on, seen from above.</summary>
    public enum BoardSide
    {
        Top,
        Bottom,
        Left,
        Right,
    }

    /// <summary>
    /// An exit on a board edge. Immutable. Position and opening size are measured along
    /// the edge: columns from the left on Top/Bottom, rows from the bottom on Left/Right.
    /// </summary>
    public sealed class Door
    {
        private Door(BoardSide side, int position, int openingSize, string color)
        {
            Side = side;
            Position = position;
            OpeningSize = openingSize;
            Color = color;
        }

        public BoardSide Side { get; }

        /// <summary>The first column or row the opening covers.</summary>
        public int Position { get; }

        /// <summary>How many columns or rows the opening covers.</summary>
        public int OpeningSize { get; }

        /// <summary>Stable color ID. A block needs the same color to use this door.</summary>
        public string Color { get; }

        /// <summary>The way a block travels when it leaves through this door.</summary>
        public Direction ExitDirection
        {
            get
            {
                switch (Side)
                {
                    case BoardSide.Top: return Direction.Up;
                    case BoardSide.Bottom: return Direction.Down;
                    case BoardSide.Left: return Direction.Left;
                    case BoardSide.Right: return Direction.Right;
                    default: throw new InvalidOperationException($"Unknown side: {Side}");
                }
            }
        }

        /// <summary>Builds a door if the data is valid. If not, returns false and says why.</summary>
        public static bool TryCreate(BoardSide side, int position, int openingSize, string color, out Door door, out string error)
        {
            door = null;

            if (position < 0)
            {
                error = $"Door position cannot be negative, got {position}.";
                return false;
            }

            if (openingSize < 1)
            {
                error = $"Door opening must be at least 1, got {openingSize}.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(color))
            {
                error = "Door has no color.";
                return false;
            }

            door = new Door(side, position, openingSize, color);
            error = null;
            return true;
        }

        /// <summary>True if this column or row, measured along the door's edge, is inside the opening.</summary>
        public bool CoversAlongEdge(int columnOrRow)
        {
            return columnOrRow >= Position && columnOrRow < Position + OpeningSize;
        }
    }
}
