# Puzzle rules

## Purpose
The rules of the board, blocks and doors. Implemented in Core (Phase 1). All rules use 2D grid coordinates only; boards, blocks and doors are shown as 3D models. How they are shown and dragged: [board-view](board-view.md).

## Rules
**Movement**
- Grid-snapped. The player drags a block in 4-directional steps through free cells. Visuals animate between cells.
- Every cell of a block must land on a free board cell.

**Doors**
- A door sits on a board edge and has a color, position and opening size.
- Opening size is measured along its edge: rows covered on left/right edges, columns covered on top/bottom edges.
- Boards have 2 to 4 exits. Checked when a level is created (Phase 2), since a board gets its doors one at a time.
- A door's position is its first column from the left (top/bottom edges) or first row from the bottom (left/right edges).

**Exiting** (single named function: `Board.CanExitThroughDoor`)
- The block's color must equal the door's color.
- The block must be touching the door.
- Its cross-section must fit: every cell of its leading edge lies inside the opening span. Exact width match is not required.
- Partial overlap is not allowed (confirmed by designer). See [decision 002](../decisions/002-exit-fit-rule.md).
- The exit happens when the player lets go of the block there (confirmed by designer).
- It leaves by travelling straight out, perpendicular to the edge, and slides fully out.

**No failure**
- No failure screen. Deadlocks are allowed; undo is the remedy for now.

**Assumptions (designer has not confirmed)**
- A door of the wrong color acts like a normal wall.
- Letting go anywhere else leaves the block in its current cell.
- Two doors cannot overlap on the same edge.

**On hold (designer will discuss later)**
- How L shapes are measured against a door. Phase 1 test boards use no L shapes. Needs an answer before the generator can use them. Until then the code measures any shape by its full width along the edge, and no test locks this in for L shapes.

## Data
Grid: (0, 0) is the bottom-left cell seen from above; X grows right, Y grows up.

Board: width, height, doors, blocks. Door: side, position, opening size, color. Block: shape ID, grid position, color. Color and image are independent; color decides the exit, image is a Phase 5 concern.

## Public interface
`Board`: `TryAddBlock`, `TryAddDoor`, `CanMove`, `TryMove`, `CanExitThroughDoor`, `TryExit`, `GetBlockAt`, `IsInside`, `IsCleared`. `Door.TryCreate` builds a door.
