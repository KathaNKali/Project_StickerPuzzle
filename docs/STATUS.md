# Status

Plan and phases: [ROADMAP](ROADMAP.md). Phase 0 is finished once the first commit is approved and merged.

## Done
- Doc system, subagents, ignore files.
- Skeleton: assemblies, generated Spike scene, one-command scripts.
- EditMode and PlayMode tests pass.
- Portrait set. New phase order and fixed levels recorded.

## In progress
- Phase 2: waves first, then generator ([phase-2-tasks](phase-2-tasks.md)). Tasks 1-3 done. Tasks 4-5 (level flow, waves in the Game scene) and board pooling (cells, doors and blocks reused between waves) written; not yet compiled, tested or played. Phase 1 tasks 1-20 passed; 21-22 deferred by the designer. The scene uses a stacked UI camera (designer's choice).

- Generator (Part 2): tasks 9 and 10 passed (346 EditMode). Task 10: `Core/Solving/Solution.cs` and `Checker.cs`, `Board.Clone()` and `GetBlockById()`, and `BoardLayoutAsset` now holds a seed and a solution (the test board has its solution filled in). Each solution step stores the block ID, its moves and the door, so the Checker replays the moves exactly and Play Solution can show them. 20 new tests (`CheckerTests`, `LevelSolutionTests`).
- Task 11: `Core/Generation/GeneratorSettings.cs` (size, doors, min/max doors, exact blocks per color, shape weights) with `FindProblems()` in plain language: board under 5x5, door count outside Min/Max Doors (default 2-4, adjustable per settings), doors that overflow, overlap or share a same-color corner, color rows (empty, repeated, negative, no door of that color, no shape fits that color's doors), no blocks, bad or duplicate shape weights, L shapes (rectangles only), shapes too wide for every door, not enough cells. Hand-made levels now also need 5x5 (`Board.MinPlayableSize`); all 5 dummy levels are 6x6 or 8x8. 37 settings tests passed outside Unity. The settings asset comes with task 16. Hand editing of generated boards stays for later (designer). Difficulty plan for task 14 recorded in waves-and-generator.
- Task 12: `Core/Solving/Pathfinder.cs` finds the shortest drag that lets a block leave through a door (or any door of its color), around blocks that stay still. Always at least one step (exit rule). Reuses its memory, so no garbage per call. `Board` gained `CanOccupy` and `CanExitThroughDoorAt` (the exit rule at any spot; `CanExitThroughDoor` now calls it, still one rule). 15 new tests (`PathfinderTests`); all 219 Core tests passed outside Unity.
- Task 13 written: `Core/Generation/Generator.cs`, `TryCreate(settings, seed, out board, out solution, out problem)`. Places blocks in reverse removal order; each needs a route out (Pathfinder) among the blocks placed before it, and never starts at a door it fits. Shapes picked by weight from those that fit a door of the block's color; spots in random order. If a run gets stuck (an early block parks in front of another color's door), it starts a fresh run on the same seeded sequence, up to 100 runs, then says so plainly. Final board is rebuilt so block IDs run 0, 1, 2. `Board.RemoveBlock` added (tools only). 11 new tests (`GeneratorTests`, incl. 200 seeds through the Checker); all 230 Core tests passed outside Unity. No tension yet (task 14). Waiting for the designer's EditMode run: expected 411 passes (tasks 11-13 together).

## Next
- Generator (Part 2): approved order is tasks 9-13, then 16-18, then 14-15. Plan in phase-2-tasks (Part 2 plan). After tasks 11-13 pass, task 16 (Generator scene; you build it from written steps). Playtest the Game scene (tasks 4-8, pooling) before or alongside it.
- Parked by the designer: pooling and waves playtest (tasks 4-8). Pooling code is committed (27121e8) but untested; the generator scene reuses BoardView, so test pooling before or with task 16.
- Later: Phase 1 tasks 21-22 in [phase-1-tasks](phase-1-tasks.md).

## Blockers
- None for Phase 1 (graybox has no art files).
- Git LFS 3.8.1 is installed at `~/.local/bin/git-lfs`. Terminal git finds it only once that folder is on PATH in `~/.zshrc`. GitHub Desktop has its own copy.

- **Before real art or audio (Phase 5 at the latest):** GitHub refused LFS uploads because of the account's LFS budget. Designer to check GitHub billing. TextMesh Pro stays out of LFS, in `.gitattributes`.

## Not verified
- No WebGL or Android build has finished. The designer chose to skip builds for now; the build scripts are untested.
- Gzip WebGL settings: unproven ([decision 004](decisions/004-build-settings.md)).

## Not in git
- Unity tutorial template files (`Assets/TutorialInfo`, `Assets/Readme.asset`): only the two scripts are in git, in commit `cdb7a37`; the rest is not. Not used by the game. See question 5.
- `Assets/_Recovery/0.unity`: a Unity recovery scene from 5 Nov, 13:00. Ignored, not deleted. See question 11.
- `Assets/Resources`: empty folder Unity created.

## Questions for the designer
1. **L shapes:** how are they measured against a door? On hold. The generator uses rectangles only until this is settled.
2. **Undo:** can it bring back a block that has exited, or return to a previous level? Needed by Phase 3.
3. **Assumptions to confirm:** a wrong-color door acts as a wall; letting go elsewhere leaves the block in place; a shape's cells must touch side by side; two doors cannot overlap on one edge; shape sizes are width x height.
4. **Early build:** run one quick build after Phase 1 to catch platform problems early? Recommended.
5. **Template files:** OK to delete them, along with `Assets/Scenes/SampleScene`? Recommended.
7. **Corner doors:** same-color doors sharing a corner are disallowed for now. Revisit later.
8. **Names:** final game name and Android ID. Current placeholders: "Sticker Puzzle", `com.kathankali.stickerpuzzle`.
11. **Recovery scene:** does `Assets/_Recovery/0.unity` hold anything you need? If not, it can be deleted.
