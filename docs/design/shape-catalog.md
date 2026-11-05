# Shape catalog

## Purpose
The fixed list of block shapes. Levels only pick from it; shapes are never invented per level. Art is made per entry.

## Rules
- Every shape is built from 1x1 units and defined once in data.
- Maximum size 3x3.
- Each orientation is a separate entry (1x2 and 2x1 are separate; each L orientation is separate), because art is made separately for each.
- The designer can edit the catalog.
- Cell offsets are measured from the shape's bottom-left corner, which is (0, 0).
- All cells must touch side by side, not only at corners (assumption, designer has not confirmed).

## Starting catalog
1x1, 1x2, 2x1, 1x3, 3x1, 2x2, 2x3, 3x2, 3x3, plus 4 L-tromino orientations.

## Data
Each entry: stable ID, cell offsets, family. Authored as a ScriptableObject and converted to a plain C# record for Core.

## Public interface
`ShapeDefinition.TryCreate(id, family, cells, out shape, out error)`. A shape exposes `Id`, `Family`, `Cells`, `Width`, `Height`.
