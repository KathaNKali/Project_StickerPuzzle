# Phase 1 micro-tasks

Goal and done criteria: [ROADMAP](ROADMAP.md). Do one task at a time, in order. Mark `[x]` when finished.

**Each task:** Claude writes the code and any Unity steps -> designer does the Unity steps and runs the check -> designer commits -> next task.

## A. Rules (code only; check in Test Runner, EditMode tab)
- [x] 1. Grid basics: cell positions and the 4 directions.
- [x] 2. Shape definition: ID, cells, family, plus a validity check (max 3x3, no duplicates).
- [x] 3. Board and blocks: place blocks and know which cells are taken.
- [x] 4. Moving: one step in a direction; walls and other blocks stop it.
- [ ] 5. Doors: side, position, opening size, color, and which cells they touch.
- [ ] 6. Exit check: `CanExitThroughDoor` (color, touching, fits). Rectangles only; L shapes on hold.
- [ ] 7. Exiting: remove the block and detect a cleared board.

## B. Game layer and data
- [ ] 8. Shape catalog asset: editable in the Inspector, with a button to fill the 13 starting shapes. *You create the asset.*
- [ ] 9. Puzzle session: takes move and let-go commands, updates the board, reports what happened.
- [ ] 10. Test board: temporary 6x6 layout as editable data, no L shapes. *You create the asset.*

## C. Scene (you build it in Unity from my steps; check by pressing Play)
- [ ] 11. Empty game scene with a portrait Game view, light and board base.
- [ ] 12. Camera: nearly top-down, fits the whole board on any portrait screen.
- [ ] 13. Board view: draws the grid cells from game data.
- [ ] 14. Doors and blocks: gray 3D boxes, colored by game data.

## D. Mouse drag
- [ ] 15. Pick up: click a block to select it, release to drop it.
- [ ] 16. Drag: the block follows the mouse one cell at a time, blocked by walls and blocks.
- [ ] 17. Smooth movement between cells.
- [ ] 18. Exit on let-go: letting go at a fitting door triggers the exit.
- [ ] 19. Exit animation: slides straight out through the door.

## E. Finish
- [ ] 20. Reset button (debug) to restore the test board.
- [ ] 21. Efficiency check: Profiler steps, looking for per-frame memory use and unneeded work.
- [ ] 22. Full playtest against the Phase 1 done criteria, then your feedback on drag, camera and exit.
