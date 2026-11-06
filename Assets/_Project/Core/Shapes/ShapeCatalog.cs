using System.Collections.Generic;

namespace StickerPuzzle.Core
{
    /// <summary>The fixed list of shapes the game can use, looked up by stable ID.</summary>
    public sealed class ShapeCatalog
    {
        private readonly List<ShapeDefinition> shapes;
        private readonly Dictionary<string, ShapeDefinition> shapesById;

        private ShapeCatalog(List<ShapeDefinition> shapes, Dictionary<string, ShapeDefinition> shapesById)
        {
            this.shapes = shapes;
            this.shapesById = shapesById;
        }

        public IReadOnlyList<ShapeDefinition> Shapes => shapes;

        /// <summary>Builds a catalog if there is at least one shape and every ID is unique.</summary>
        public static bool TryCreate(IEnumerable<ShapeDefinition> definitions, out ShapeCatalog catalog, out string error)
        {
            catalog = null;

            var list = new List<ShapeDefinition>();
            var byId = new Dictionary<string, ShapeDefinition>();

            if (definitions != null)
            {
                foreach (var shape in definitions)
                {
                    if (byId.ContainsKey(shape.Id))
                    {
                        error = $"Two shapes use the ID '{shape.Id}'.";
                        return false;
                    }

                    byId.Add(shape.Id, shape);
                    list.Add(shape);
                }
            }

            if (list.Count == 0)
            {
                error = "The catalog has no shapes.";
                return false;
            }

            catalog = new ShapeCatalog(list, byId);
            error = null;
            return true;
        }

        public bool TryGet(string id, out ShapeDefinition shape)
        {
            if (id == null)
            {
                shape = null;
                return false;
            }

            return shapesById.TryGetValue(id, out shape);
        }
    }
}
