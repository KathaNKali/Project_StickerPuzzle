using System;

namespace StickerPuzzle.Core
{
    /// <summary>
    /// A cell on the board, seen from above. (0, 0) is the bottom-left cell;
    /// X grows to the right and Y grows upward.
    /// </summary>
    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public readonly int X;
        public readonly int Y;

        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static GridPosition operator +(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.X + b.X, a.Y + b.Y);
        }

        public static GridPosition operator -(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator ==(GridPosition a, GridPosition b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(GridPosition a, GridPosition b)
        {
            return !a.Equals(b);
        }

        /// <summary>The neighbouring cell one step away in the given direction.</summary>
        public GridPosition Step(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return new GridPosition(X, Y + 1);
                case Direction.Down: return new GridPosition(X, Y - 1);
                case Direction.Left: return new GridPosition(X - 1, Y);
                case Direction.Right: return new GridPosition(X + 1, Y);
                default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public bool Equals(GridPosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (X * 397) ^ Y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
