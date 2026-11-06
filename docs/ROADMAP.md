# Roadmap

Status values: not started / in progress / done. Why this order: [decision 005](decisions/005-playable-first.md).

| Phase | Goal | Deliverables | Done means | Main risk | Effort (sessions) | Status |
|---|---|---|---|---|---|---|
| 0 | Finish setup | Docs, skeleton, first commit | Docs complete; tests run from one command; work saved in git | Git LFS not installed | Under 1 | done (not merged to main) |
| 1 | Playable board | Shape catalog, movement and exit rules, portrait camera, mouse drag, exit animation, test board | In the Unity editor, the designer drags blocks; walls and blocks stop them; they leave only through a fitting same-color door | Drag may not feel right at first | 2-3 | in progress: efficiency check and full playtest deferred |
| 2 | Waves | Generator scene (Create, Checker, Save), Level Data, generator, continuous waves, auto-play | A cleared board spawns the next level; every saved level passes the Checker; the game stops after the last level | Waves may be solvable but boring | 3-5 | planning: [phase-2-tasks](phase-2-tasks.md) |
| 3 | Undo and save | Undo, save/restore, kill-and-resume tests | Closing mid-level resumes exactly; no reward is given twice | Undo and rewards interacting | 1-2 | not started |
| 4 | First real build | 10 tuned levels, debug tools, touch input, Android + WebGL builds | Designer playtests 10 levels on a phone and in a browser | Platform problems found late | 2-3 | not started |
| 5 | Stickers | Sticker rewards, collection home screen | Cleared blocks add stickers to the book | Scope growth | 3-4 | not started |
| Later | Hand editing of levels, full-search Checker, more editor tools, image splitting, polish, iOS, PC | Scoped when reached | n/a | n/a | n/a | not started |

Each phase's plan needs designer approval before it starts. See [WORKFLOW](WORKFLOW.md).
