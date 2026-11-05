# Board view

## Purpose
Shows the board in 3D and lets the player drag blocks (Phase 1). Holds no game state. Rules: [puzzle-rules](puzzle-rules.md).

## Rules
- Lives in `Assets/_Project/Scenes/Game.unity`, built by the designer.
- Rules and clicks use grid math, not physics, so scene objects need no colliders.
- Portrait screen. Nearly top-down, orthographic camera (designer's choice): blocks look the same size anywhere on the board.
- The whole board stays visible on any portrait screen shape.
- Graybox art for now.
- Mouse first. Touch arrives in Phase 4.
- Dragging moves the block one grid step at a time toward the pointer. The animation between cells is only visual.
- Letting go at a fitting door starts the exit. Letting go elsewhere leaves the block in place (assumption).

## Data
Reads board state from Game. Colors map to materials by color ID.

The Phase 1 test board is `Assets/_Project/Data/BoardLayout_TestBoard.asset` (`BoardLayoutAsset`): temporary, editable, with Inspector buttons to fill and check it. A test plays through a full solution of the default layout.

## World layout
The board is centered on the world origin and lies flat: grid X is world X, grid Y is world Z, and one cell is one world unit.

## Public interface
- `GameBootstrap`: builds the board and session from the data assets at start, and exposes `Session`.
- `BoardView`: `ShowBoard(board)` draws the base, cells, doors and blocks.
- `BoardGrid`: `CellToWorld(cell, worldHeight)`, `GetDoorCenter`, `GetDoorSize` convert grid data to world.
- `ColorPaletteAsset` (`Assets/_Project/Data/ColorPalette.asset`): matches each color ID to a material. IDs must match the layout exactly, such as `red`.
- `BlockView`: one per block, with `Model` as its game data.
- `BoardCamera`: `SetBoardSize(width, height)`. Pitch and padding are tunable in the Inspector.
