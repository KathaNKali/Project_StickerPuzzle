# Phase 2 micro-tasks: levels (PROPOSED, awaiting designer approval)

Goal and done criteria: [ROADMAP](ROADMAP.md). Rules: [waves-and-generator](design/waves-and-generator.md). Do one at a time, in order. Routine steps: [WORKFLOW](WORKFLOW.md).

## A. Level data and proof (code only)
- [ ] 1. Seeded random numbers: same seed gives the same results on every platform.
- [ ] 2. Level record: ID, name, settings, exact board, solution. Builds a fresh Board.
- [ ] 3. Checker: replays a level's solution using the real game rules.

## B. Generator (code only)
- [ ] 4. Generator settings, with plain-language errors when they cannot work.
- [ ] 5. Pathfinder: finds a block's route to a door it fits, past other blocks.
- [ ] 6. Reverse construction: place blocks so each has a route among those placed before it.
- [ ] 7. Tension: prefer placements that block earlier blocks' routes; report a tension score.
- [ ] 8. Stress test: thousands of seeds; every board passes the Checker; same seed gives the same board.

## C. Generator scene (designer builds it in Unity)
- [ ] 9. Level asset and Level Data asset: rename and drag to reorder in the Inspector.
- [ ] 10. Generator scene: enter settings, Create, see the board.
- [ ] 11. Checker button and Play Solution button.
- [ ] 12. Save button: names the level and adds it to Level Data.

## D. Levels in the game
- [ ] 13. Game scene plays Level Data in order; a cleared board loads the next level.
- [ ] 14. After the last level: stop, with a placeholder message. Debug next and previous buttons.
- [ ] 15. Full playtest with a few created levels, then designer feedback.

Deferred from Phase 1: tasks 21 (efficiency) and 22 (full playtest).
