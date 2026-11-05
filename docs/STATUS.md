# Status

Plan and phases: [ROADMAP](ROADMAP.md). Phase 0 is finished once the first commit is approved and merged.

## Done
- Doc system, subagents, ignore files.
- Skeleton: assemblies, generated Spike scene, one-command scripts.
- EditMode and PlayMode tests pass.
- Portrait set. New phase order and fixed levels recorded.

## In progress
- Nothing. Waiting for designer approval of the Phase 1 plan.

## Next
- Phase 1: playable board.

## Blockers
- None for Phase 1 (graybox has no art files).
- **Before any art or audio is added:** install Git LFS. Git settings on this machine require it, but it is not installed, so git refuses `.png` and similar files.

## Not verified
- No WebGL or Android build has finished. The designer chose to skip builds for now; the build scripts are untested.
- Gzip WebGL settings: unproven ([decision 004](decisions/004-build-settings.md)).

## Not in git
- Unity tutorial template files (`Assets/TutorialInfo`, `Assets/Readme.asset`): left out because of the LFS issue. Not used by the game. See question 5.
- `Assets/_Recovery/0.unity`: a Unity recovery scene from 5 Nov, 13:00. Ignored, not deleted. See question 7.
- `Assets/Resources`: empty folder Unity created.

## Questions for the designer
1. **L shapes:** how are they measured against a door? On hold, to discuss later. Needed before Phase 2's generator can use them.
2. **Undo:** can it bring back a block that has exited, or return to a previous level? Needed by Phase 3.
3. **Assumptions to confirm:** a wrong-color door acts as a wall; letting go elsewhere leaves the block in place.
4. **Early build:** run one quick build after Phase 1 to catch platform problems early? Recommended.
5. **Template files:** OK to delete them, along with `Assets/Scenes/SampleScene`? Recommended.
6. **Names:** final game name and Android ID. Current placeholders: "Sticker Puzzle", `com.kathankali.stickerpuzzle`.
7. **Recovery scene:** does `Assets/_Recovery/0.unity` hold anything you need? If not, it can be deleted.
