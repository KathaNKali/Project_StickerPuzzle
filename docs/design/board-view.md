# Board view

## Purpose
Shows the board in 3D and lets the player drag blocks (Phase 1). Holds no game state. Rules: [puzzle-rules](puzzle-rules.md).

## Rules
- Lives in `Assets/_Project/Scenes/Game.unity`, built by the designer.
- Rules and clicks use grid math, not physics, so scene objects need no colliders.
- Portrait screen. Nearly top-down camera.
- The whole board stays visible on any portrait screen shape.
- Graybox art for now.
- Mouse first. Touch arrives in Phase 4.
- Dragging moves the block one grid step at a time toward the pointer. The animation between cells is only visual.
- Letting go at a fitting door starts the exit. Letting go elsewhere leaves the block in place (assumption).

## Data
Reads board state from Game. Colors map to materials by color ID.

The Phase 1 test board is `Assets/_Project/Data/BoardLayout_TestBoard.asset` (`BoardLayoutAsset`): temporary, editable, with Inspector buttons to fill and check it. A test plays through a full solution of the default layout.

## Public interface
Defined in Phase 1.
