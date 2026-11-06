# Status

Plan and phases: [ROADMAP](ROADMAP.md). Phase 0 is finished once the first commit is approved and merged.

## Done
- Doc system, subagents, ignore files.
- Skeleton: assemblies, generated Spike scene, one-command scripts.
- EditMode and PlayMode tests pass.
- Portrait set. New phase order and fixed levels recorded.

## In progress
- Phase 1, task 17 (smooth movement): code written, waiting for the designer's playtest and Test Runner result. Task 16's result was not reported separately. An exited block stays visible until task 18.

## Next
- Remaining micro-tasks, one at a time: [phase-1-tasks](phase-1-tasks.md).

## Blockers
- None for Phase 1 (graybox has no art files).
- Git LFS works through GitHub Desktop, which has its own copy. Terminal git cannot handle `.png` and similar files unless Git LFS is installed separately, so use Desktop.

## Not verified
- No WebGL or Android build has finished. The designer chose to skip builds for now; the build scripts are untested.
- Gzip WebGL settings: unproven ([decision 004](decisions/004-build-settings.md)).

## Not in git
- Unity tutorial template files (`Assets/TutorialInfo`, `Assets/Readme.asset`): only the two scripts are in git, in commit `cdb7a37`; the rest is not. Not used by the game. See question 5.
- `Assets/_Recovery/0.unity`: a Unity recovery scene from 5 Nov, 13:00. Ignored, not deleted. See question 9.
- `Assets/Resources`: empty folder Unity created.

## Questions for the designer
1. **L shapes:** how are they measured against a door? On hold, to discuss later. Needed before Phase 2's generator can use them.
2. **Undo:** can it bring back a block that has exited, or return to a previous level? Needed by Phase 3.
3. **Assumptions to confirm:** a wrong-color door acts as a wall; letting go elsewhere leaves the block in place; a shape's cells must touch side by side; two doors cannot overlap on one edge; shape sizes are width x height.
4. **Early build:** run one quick build after Phase 1 to catch platform problems early? Recommended.
5. **Template files:** OK to delete them, along with `Assets/Scenes/SampleScene`? Recommended.
6. **Opposite doors (Phase 2):** on a board as narrow as a block is wide, the block can touch same-color doors on both sides. Which one should it use, or should this be disallowed too?
7. **Corner doors:** same-color doors sharing a corner are disallowed for now. Revisit later.
8. **Names:** final game name and Android ID. Current placeholders: "Sticker Puzzle", `com.kathankali.stickerpuzzle`.
9. **Recovery scene:** does `Assets/_Recovery/0.unity` hold anything you need? If not, it can be deleted.
