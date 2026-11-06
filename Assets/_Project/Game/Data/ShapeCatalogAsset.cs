using System;
using System.Collections.Generic;
using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Game
{
    /// <summary>
    /// The designer-editable shape list. Converts to a plain <see cref="ShapeCatalog"/>,
    /// so Core stays free of Unity. Size names are width x height.
    /// </summary>
    [CreateAssetMenu(fileName = "ShapeCatalog", menuName = "Sticker Puzzle/Shape Catalog")]
    public sealed class ShapeCatalogAsset : ScriptableObject
    {
        [Serializable]
        private sealed class Entry
        {
            public string id;
            public string family;
            public List<Vector2Int> cells = new List<Vector2Int>();
        }

        private const string Rectangle = "Rectangle";
        private const string LShape = "L";

        [SerializeField] private List<Entry> shapes = new List<Entry>();

        public int Count => shapes.Count;

        public void AddShape(string id, string family, params Vector2Int[] cells)
        {
            shapes.Add(new Entry { id = id, family = family, cells = new List<Vector2Int>(cells) });
        }

        /// <summary>Replaces the whole list with the 13 starting shapes.</summary>
        public void FillStartingShapes()
        {
            shapes.Clear();

            AddRectangle(1, 1);
            AddRectangle(1, 2);
            AddRectangle(2, 1);
            AddRectangle(1, 3);
            AddRectangle(3, 1);
            AddRectangle(2, 2);
            AddRectangle(2, 3);
            AddRectangle(3, 2);
            AddRectangle(3, 3);

            var bottomLeft = new Vector2Int(0, 0);
            var bottomRight = new Vector2Int(1, 0);
            var topLeft = new Vector2Int(0, 1);
            var topRight = new Vector2Int(1, 1);

            // Named after the corner of the 2x2 area that is left empty.
            AddShape("L_EmptyTopRight", LShape, bottomLeft, bottomRight, topLeft);
            AddShape("L_EmptyTopLeft", LShape, bottomLeft, bottomRight, topRight);
            AddShape("L_EmptyBottomRight", LShape, bottomLeft, topLeft, topRight);
            AddShape("L_EmptyBottomLeft", LShape, bottomRight, topLeft, topRight);
        }

        /// <summary>
        /// Converts the list for the game. If anything is wrong, returns false and
        /// lists every problem, so the designer can fix them all in one go.
        /// </summary>
        public bool TryBuild(out ShapeCatalog catalog, out List<string> errors)
        {
            catalog = null;
            errors = new List<string>();

            var definitions = new List<ShapeDefinition>(shapes.Count);
            foreach (var entry in shapes)
            {
                if (ShapeDefinition.TryCreate(entry.id, entry.family, ToGridCells(entry.cells), out var shape, out var error))
                {
                    definitions.Add(shape);
                }
                else
                {
                    errors.Add(error);
                }
            }

            if (errors.Count > 0)
            {
                return false;
            }

            if (!ShapeCatalog.TryCreate(definitions, out catalog, out var catalogError))
            {
                errors.Add(catalogError);
                return false;
            }

            return true;
        }

        private void AddRectangle(int width, int height)
        {
            var cells = new Vector2Int[width * height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    cells[(y * width) + x] = new Vector2Int(x, y);
                }
            }

            AddShape($"{width}x{height}", Rectangle, cells);
        }

        private static GridPosition[] ToGridCells(List<Vector2Int> cells)
        {
            var result = new GridPosition[cells.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = new GridPosition(cells[i].x, cells[i].y);
            }

            return result;
        }
    }
}
