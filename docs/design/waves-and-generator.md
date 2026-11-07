# Levels and generator

"Level" and "wave" mean the same thing: one board, spawned when the previous one is cleared.

## Purpose
Lets the designer create, check, save and order levels (Phase 2).

## Rules
**Levels in play**
- Built in two parts: first the wave system with 5 hand-made dummy levels, then the generator. See [decision 007](../decisions/007-waves-before-generator.md).
- Only "fully cleared" spawning is built for now (assumption, designer has not confirmed).
- When the last block is fully out, a short adjustable pause, then the next board (assumption; can be tuned).
- Levels are fixed: every player gets the same saved boards in the same order. See [decision 006](../decisions/006-fixed-levels.md).
- A new level spawns only when the refill threshold is reached. The threshold is a setting; default 0 means fully cleared.
- Different levels can have different boards. Colors may repeat across blocks.
- After the last level the game stops, with a plain placeholder message until the designer designs that moment.

**Generator scene (designer tool, never seen by players)**
- The designer enters the settings under Data, then clicks **Create** to build and show a board.
- If the settings cannot work, Create says why instead of making a bad board.
- A level never starts with a block touching a door it fits, and same-color doors never share a corner. See [puzzle-rules](puzzle-rules.md).
- **Checker** button: replays the board's solution and says "Solvable" or "Not solvable".
- A button auto-plays the solution so the designer can watch it.
- **Save** stores the level under a name in Level Data.

**Level Data**
- A list where levels can be renamed and rearranged. It points directly at the level assets; saves use only IDs. See [decision 008](../decisions/008-level-data-asset-references.md).
- **Check Levels** reports an empty list, empty rows, a level listed twice, missing or shared IDs, and each level's board problems.
- Each level has a hidden permanent ID, so renaming or reordering never breaks a player's save.
- The editor assigns the ID automatically. A copied level gets its own new ID, and its name resets to its file name.
- Inspector > Reset wipes the ID, so that level gets a new one. Never use it on a released level.

**Generator**
- Seeded and deterministic. Changing the seed gives a different board.
- Reverse construction: place blocks in reverse removal order, so each block has a path to its exit among the blocks placed before it. See [decision 003](../decisions/003-reverse-construction.md).
- Prefer placements that later blocks will obstruct, so the puzzle has tension.
- Must support spawning into a board that already has blocks.
- Picks shapes only from the [shape catalog](shape-catalog.md).

## Data
Settings: grid width and height; exits (side, position, width, color); block count; shape weights; colors; refill rule; seed mode.

Level: permanent ID, name, settings, exact board, solution. Authored as a ScriptableObject and converted to a plain C# record for Core.

## Later phases
Hand editing of boards, and a full-search Checker with a "Too complex to tell" answer.

## Public interface
Defined in Phase 2.
