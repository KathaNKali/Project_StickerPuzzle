# Board view

## Purpose
Shows the board in 3D and lets the player drag blocks (Phase 1). Holds no game state. Rules: [puzzle-rules](puzzle-rules.md).

## Rules
- Portrait screen. Nearly top-down camera.
- The whole board stays visible on any portrait screen shape.
- Graybox art for now.
- Mouse first. Touch arrives in Phase 4.
- Dragging moves the block one grid step at a time toward the pointer. The animation between cells is only visual.
- Letting go at a fitting door starts the exit. Letting go elsewhere leaves the block in place (assumption).

## Data
Reads board state from Game. Colors map to materials by color ID.

## Public interface
Defined in Phase 1.
