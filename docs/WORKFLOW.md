# Workflow

## Who does what
The designer does the Unity and git work by hand, to learn the craft.
- **Designer:** makes all changes in the Unity editor (scenes, prefabs, materials, wiring, data assets) and does all git work (branch, commit, merge, push).
- **Claude:** writes efficient, well-optimized code and tests, then gives short numbered steps to integrate them in Unity (which menu, which object, which field, what to expect) and the GitHub Desktop git steps, with a one-line reason for each.
- Claude changes scenes, prefabs or assets, drives Unity, or runs git changes only when the designer asks for help. Read-only git commands such as `status` and `diff` are fine.
- Claude asks when anything is unclear rather than guessing.
- **Tests:** the designer runs them from Window > General > Test Runner and reports any failures. Claude says which tests to run and what a pass looks like, and does not call code working until the result is back.

## Routine steps for every task
Unless a task says otherwise: click into Unity and wait for the spinner; send any red Console errors; run Test Runner > EditMode > Run All; in GitHub Desktop, tick only the listed files, each new file with its `.meta`; commit; report results, feedback and the next request in one message.

## Session routine
**Start:** read [STATUS](STATUS.md), then only the docs the task needs.

**End:** the designer runs the tests; update STATUS (and ROADMAP if a phase changed); record decisions; tell the designer what to playtest and what feedback is needed.

Work on one system per session.

## Adding a feature or system
1. Write or update `docs/design/<system>.md`.
2. Show the designer a short implementation plan. **Wait for approval.**
3. Implement in small steps, with tests. After each step, give the designer the commit to make.
4. Verify by the designer running tests and builds, not by re-reading code.
5. Update [STATUS](STATUS.md) and [ROADMAP](ROADMAP.md). Record decisions in `docs/decisions/`.
6. Give the Unity integration steps, then what to playtest and what feedback is needed.

## Design questions
Claude never invents game design. Unclear points go under "Questions for the designer" in STATUS.md. An assumption used meanwhile must be labelled as one and kept easy to change.

## Branches and commits
- `main` holds approved work. Never commit directly to it.
- One branch per phase or system, for example `phase-1-playable-board`.
- One small commit per step, with a message saying what changed and why. Tests pass before each commit.
- The designer does the merging to `main` and pushing, when ready.
- LFS tracks art and audio (`.gitattributes`). Use GitHub Desktop, which has its own copy of Git LFS. Terminal git needs Git LFS installed separately.

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
