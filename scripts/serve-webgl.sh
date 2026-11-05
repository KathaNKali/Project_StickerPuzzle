#!/usr/bin/env bash
# Serves Builds/WebGL at http://localhost:8080 (Ctrl+C to stop).
cd "$(dirname "$0")/../Builds/WebGL"
exec python3 -m http.server 8080 --bind 127.0.0.1
