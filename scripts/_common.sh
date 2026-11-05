#!/usr/bin/env bash
# Shared setup for the one-command scripts. Sourced, not run directly.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

VERSION="$(awk '/^m_EditorVersion:/{print $2}' ProjectSettings/ProjectVersion.txt)"
UNITY="${UNITY_PATH:-/Applications/Unity/Hub/Editor/$VERSION/Unity.app/Contents/MacOS/Unity}"

if [ ! -x "$UNITY" ]; then
  echo "Unity $VERSION not found at: $UNITY" >&2
  echo "Install it with Unity Hub, or set UNITY_PATH." >&2
  exit 3
fi

if [ -f Temp/UnityLockfile ]; then
  if lsof Temp/UnityLockfile >/dev/null 2>&1 || pgrep -if "Unity.*$ROOT" >/dev/null 2>&1; then
    echo "The project is open in the Unity editor. Close it and run this again." >&2
    exit 3
  fi
  # Unity leaves the lock behind when batch mode aborts; nothing is using it.
  rm -f Temp/UnityLockfile
fi

mkdir -p Logs

show_log_errors() {
  grep -E 'error CS[0-9]+|Scripts have compiler errors|BuildFailedException|\[BUILD\]|Aborting batchmode|No valid Unity Editor license' "$1" | sort -u | head -30 || true
  echo "Full log: $1"
}

# Usage: run_tests EditMode|PlayMode
run_tests() {
  local platform="$1"
  local results="$ROOT/Logs/TestResults-$platform.xml"
  local log="$ROOT/Logs/Test-$platform.log"
  local code=0

  rm -f "$results"
  "$UNITY" -batchmode -projectPath "$ROOT" -runTests -testPlatform "$platform" \
    -testResults "$results" -logFile "$log" || code=$?

  if [ ! -f "$results" ]; then
    echo "$platform: no results produced (exit $code)."
    show_log_errors "$log"
    exit 1
  fi

  grep -o '<test-case [^>]*result="Failed"[^>]*>' "$results" \
    | sed -E 's/.*fullname="([^"]*)".*/FAILED: \1/' || true
  sed -n 's/.*<test-run [^>]*total="\([0-9]*\)".*passed="\([0-9]*\)" failed="\([0-9]*\)".*/'"$platform"': \1 total, \2 passed, \3 failed/p' "$results" | head -1

  if [ "$code" -ne 0 ]; then
    echo "Details: $results"
    exit "$code"
  fi
}

# Usage: run_build WebGL|Android MethodName
run_build() {
  local target="$1"
  local method="$2"
  local log="$ROOT/Logs/Build-$target.log"
  local code=0

  "$UNITY" -batchmode -projectPath "$ROOT" -buildTarget "$target" \
    -executeMethod "StickerPuzzle.EditorTools.BuildCommands.$method" -logFile "$log" || code=$?

  show_log_errors "$log"
  exit "$code"
}
