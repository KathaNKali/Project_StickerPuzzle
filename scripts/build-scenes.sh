#!/usr/bin/env bash
# Applies project settings and regenerates scenes.
source "$(dirname "$0")/_common.sh"
LOG="$ROOT/Logs/Setup.log"
CODE=0
"$UNITY" -batchmode -quit -projectPath "$ROOT" \
  -executeMethod StickerPuzzle.EditorTools.ProjectSetup.Apply -logFile "$LOG" || CODE=$?
if [ "$CODE" -eq 0 ]; then echo "Setup done."; else show_log_errors "$LOG"; fi
exit "$CODE"
