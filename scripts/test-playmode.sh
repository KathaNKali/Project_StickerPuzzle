#!/usr/bin/env bash
# Runs the PlayMode smoke test. Prints only failures and a total.
source "$(dirname "$0")/_common.sh"
run_tests PlayMode
