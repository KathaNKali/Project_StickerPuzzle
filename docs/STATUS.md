# Status

Plan and phases: [ROADMAP](ROADMAP.md). Phase 0 is finished once the first commit is approved and merged.

## Done
- Doc system, subagents, ignore files.
- Skeleton: assemblies, generated Spike scene, one-command scripts.
- EditMode and PlayMode tests pass.
- Portrait set. New phase order and fixed levels recorded.

## In progress
- Phase 2: waves first, then generator ([phase-2-tasks](phase-2-tasks.md)). Tasks 1-3 done. Tasks 4-5 (level flow, waves in the Game scene) and board pooling (cells, doors and blocks reused between waves) written; not yet compiled, tested or played. Phase 1 tasks 1-20 passed; 21-22 deferred by the designer. The scene uses a stacked UI camera (designer's choice).

## Next
- Designer wires Level Data into GameBootstrap and playtests tasks 4-5. Then task 6 (No more levels message), ideally in a fresh session.
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
1. **L shapes:** how are they measured against a door? On hold, to discuss later. Needed before Phase 2's generator can use them.
2. **Undo:** can it bring back a block that has exited, or return to a previous level? Needed by Phase 3.
3. **Assumptions to confirm:** a wrong-color door acts as a wall; letting go elsewhere leaves the block in place; a shape's cells must touch side by side; two doors cannot overlap on one edge; shape sizes are width x height.
4. **Early build:** run one quick build after Phase 1 to catch platform problems early? Recommended.
5. **Template files:** OK to delete them, along with `Assets/Scenes/SampleScene`? Recommended.
6. **Opposite doors (Phase 2):** on a board as narrow as a block is wide, the block can touch same-color doors on both sides. Which one should it use, or should this be disallowed too?
7. **Corner doors:** same-color doors sharing a corner are disallowed for now. Revisit later.
8. **Names:** final game name and Android ID. Current placeholders: "Sticker Puzzle", `com.kathankali.stickerpuzzle`.
9. **Seed (generator):** a typed number plus a New Seed button? Recommended.
11. **Recovery scene:** does `Assets/_Recovery/0.unity` hold anything you need? If not, it can be deleted.
