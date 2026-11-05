# Sticker Puzzle

Casual 3D sliding-block puzzle + 2D sticker collection. Concept: [docs/design/00-vision.md](docs/design/00-vision.md) (long; read only the section you need).

## Session rules
- Read [docs/STATUS.md](docs/STATUS.md) first.
- Read only the docs needed for the task.
- Update STATUS.md at the end of every session.
- The designer makes all changes in the Unity editor and does all git work, to learn the craft. Claude writes code, gives clear integration and git steps, and steps in only when asked. Detail: [docs/WORKFLOW.md](docs/WORKFLOW.md).
- Never run `git add`, `commit`, `merge` or `push` unless the designer asks.
- Keep the game well optimized and efficient. Ask questions when anything is unclear.
- Process (approval gates, per-system steps): [docs/WORKFLOW.md](docs/WORKFLOW.md).

## Engine and platforms
- Unity 6000.3.21f1, URP, 3D. New Input System only.
- Android + WebGL first. iOS + PC later.
- Team: designer (not a programmer) + Claude (all code).

## Folder map
```
Assets/_Project/
  Core/          pure C# rules. No UnityEngine.
  Game/          sessions, waves, save.
  Presentation/  3D board, input, UI.
  EditorTools/   editor-only tools, scene builders, build commands.
  Tests/         EditMode/ and PlayMode/.
  Scenes/        built by the designer in Unity (Spike.unity is generated).
  Data/          ScriptableObjects (shapes, wave recipes).
  Art/           materials, models, images.
docs/            design, decisions, status.
scripts/         one-command tools.
Builds/          output, not in git.
```

## Commands (run from repo root, with the Unity editor closed)
```
scripts/test-editmode.sh    # EditMode tests; prints only failures + a total
scripts/test-playmode.sh    # PlayMode smoke test
scripts/build-scenes.sh     # apply project settings and regenerate scenes
scripts/build-webgl.sh      # Builds/WebGL
scripts/build-android.sh    # Builds/Android/StickerPuzzle.apk
scripts/serve-webgl.sh      # serve Builds/WebGL at http://localhost:8080
```
Editor path override: `UNITY_PATH=/path/to/Unity scripts/...`

## Architecture rules (detail: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md))
- Dependency direction: Presentation -> Game -> Core. EditorTools and Tests may see all. Enforced by assembly definitions.
- Core must never reference UnityEngine.
- Views never own game state.
- Core is 2D grid only; no 3D concepts.
- Saves and data use stable IDs, never asset references.

## Naming
- Namespaces match assemblies: `StickerPuzzle.Core`, `.Game`, `.Presentation`, `.EditorTools`, `.Tests`.
- C#: `PascalCase` types and members, `camelCase` locals and private fields.
- Tests: `Method_Condition_Result`.
- Docs: `docs/design/<system>.md`, `docs/decisions/NNN-title.md`.

## No-go areas
- Do not invent game design. Unclear? Add to "Questions for the designer" in STATUS.md and ask.
- Claude does not change scenes, prefabs or assets unless the designer asks.
- Do not reference assets by object in save data.
- Do not start sticker or image work before Phase 5.
- Ask before any large refactor or deletion.
- Do not hand-edit `Library/`, `Temp/`, `Logs/`, `Builds/`, `UserSettings/`.

## Style with the designer
Simple, easy-to-understand answers, in points. Short replies, explain trade-offs, challenge risks to scope, performance or architecture. Verify by running tests and builds, not by re-reading code. Never paste file contents back.
