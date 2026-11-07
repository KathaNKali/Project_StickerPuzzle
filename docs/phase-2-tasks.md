# Phase 2 micro-tasks: waves, then generator

Goal and done criteria: [ROADMAP](ROADMAP.md). Rules: [waves-and-generator](design/waves-and-generator.md). Why this order: [decision 007](decisions/007-waves-before-generator.md). Do one at a time, in order. Routine steps: [WORKFLOW](WORKFLOW.md).

## Part 1: Wave system with 5 dummy levels
- [ ] 1. Level identity: each board layout gets a permanent ID and a name.
- [ ] 2. Level Data asset: ordered list of levels; drag to reorder in the Inspector, with a Check button. *You create the asset.*
- [ ] 3. 5 dummy levels: Inspector button fills each; a test plays a full solution of every one. *You create the assets.*
- [ ] 4. Level flow (code only): current level, next level, finished. Tests.
- [ ] 5. Game scene plays Level Data: starts at level 1; when the last block is fully out, a short adjustable pause, then the next board. Different board sizes and doors work.
- [ ] 6. After the last level: stop, with a placeholder "No more levels" message.
- [ ] 7. Debug: Reset restarts the current level; Next and Previous buttons.
- [ ] 8. Full playtest of all 5 levels, then designer feedback. Core gameplay complete.

## Part 2: Generator scene (after Part 1)
Needs answers first: questions 1, 6 and 10 in [STATUS](STATUS.md).
- [ ] 9. Seeded random numbers: same seed gives the same results on every platform.
- [ ] 10. Saved solution on levels, and the Checker: replays a solution using the real game rules.
- [ ] 11. Generator settings, with plain-language errors when they cannot work.
- [ ] 12. Pathfinder: finds a block's route to a door it fits, past other blocks.
- [ ] 13. Reverse construction: place blocks so each has a route among those placed before it.
- [ ] 14. Tension: prefer placements that block earlier blocks' routes; report a tension score.
- [ ] 15. Stress test: thousands of seeds; every board passes the Checker; same seed gives the same board.
- [ ] 16. Generator scene: enter settings, Create, see the board. *You build the scene.*
- [ ] 17. Checker button and Play Solution button.
- [ ] 18. Save button: names the level and adds it to Level Data.
- [ ] 19. Create real levels and playtest, then designer feedback.

Deferred from Phase 1: tasks 21 (efficiency) and 22 (full playtest).
