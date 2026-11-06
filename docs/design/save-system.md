# Save system

## Purpose
Persist the puzzle so the player can leave and resume exactly (Phase 3).

## Rules
- Snapshot after every successful move.
- Written atomically, with a version number.
- Restoring must never grant a reward twice.
- References use stable IDs, never asset object references.

## Data
Current wave's board size and exits; block positions; remaining spawn queue; RNG seed and state; undo history.

## Public interface
Defined in Phase 3.
