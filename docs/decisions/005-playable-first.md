# 005: Playable board before generator

**Date:** 2026-11-05

**What:** Rules, camera and mouse drag come first on a hand-made test board. The generator, undo and save follow. This replaces the earlier 1A-1E order.

**Why:** The designer can judge how the game feels much sooner. Movement rules are still built first, in Core, so the 3D scene does not hold game state.

**Accepted risk:** Android and WebGL stay untested until a build is run, and problems found later cost more to fix.

**Alternatives rejected:** The earlier order (headless rules, then generator and lab, then input): feel would have been judged late. Rules inside the scene: faster at first, but breaks the layer rule and needs a rewrite.
