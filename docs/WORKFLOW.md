# Workflow

## Who does what
- **Designer:** makes all changes in the Unity editor: scenes, prefabs, materials, wiring, data assets.
- **Claude:** writes code and tests, then gives short numbered steps to integrate them in Unity: which menu, which object, which field, and what to expect.
- Claude changes scenes, prefabs or assets, or drives Unity, only when the designer asks for help.
- Tests: if Unity is open, Claude asks the designer to close it or to run them from Window > General > Test Runner.

## Session routine
**Start:** read [STATUS](STATUS.md), then only the docs the task needs.

**End:** run the tests; update STATUS (and ROADMAP if a phase changed); record decisions; tell the designer what to playtest and what feedback is needed.

Work on one system per session.

## Adding a feature or system
1. Write or update `docs/design/<system>.md`.
2. Show the designer a short implementation plan. **Wait for approval.**
3. Implement in small steps, with tests, committing per step.
4. Verify by running tests and builds, not by re-reading code.
5. Update [STATUS](STATUS.md) and [ROADMAP](ROADMAP.md). Record decisions in `docs/decisions/`.
6. Give the Unity integration steps, then what to playtest and what feedback is needed.

## Design questions
Claude never invents game design. Unclear points go under "Questions for the designer" in STATUS.md. An assumption used meanwhile must be labelled as one and kept easy to change.

## Branches and commits
- `main` holds approved work. Never commit directly to it.
- One branch per phase or system, for example `phase-1-playable-board`.
- One small commit per step, with a message saying what changed and why. Tests pass before each commit.
- Merge to `main` and push only after designer approval.
- LFS tracks art and audio (`.gitattributes`). Requires Git LFS on the machine, plus `git lfs install` once.

## Testing
- Rules in Core and Game get EditMode tests. New rule, new test.
- PlayMode tests cover scenes loading and running.
- A bug fix starts with a test that fails.
- Close the Unity editor before running `scripts/`; batch mode cannot share an open project.
- Commands: [CLAUDE.md](../CLAUDE.md).

## Docs
- Each fact lives in exactly one place. Link instead of repeating.
- Keep files short. STATUS.md stays under 60 lines; CLAUDE.md stays under 100.
- Design doc sections: purpose, rules, data, public interface.
- Decision doc sections: what, why, date, alternatives rejected.

## Subagents (`.claude/agents/`)
Use one when the work would otherwise fill the main session with search results or logs. Small lookups stay in the main session.

| Agent | When |
|---|---|
| explorer | Finding code across many files |
| reviewer | Before finishing a system, to check against ARCHITECTURE.md |
| test-runner | After code changes, to report only failures |
| balance-analyst | When tuning levels, to flag outliers |
