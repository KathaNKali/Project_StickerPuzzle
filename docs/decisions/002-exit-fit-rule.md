# 002: Exit fit rule

**Date:** 2026-11-05 (decided by designer in planning; recorded here)

**What:** A block exits through a same-color door if it is touching the door and its cross-section fits fully inside the opening. Details: [puzzle-rules](../design/puzzle-rules.md).

**Why:** More forgiving than exact width, and a block never visually clips through the wall. Kept behind one named function so it is easy to change.

**Alternatives rejected:**
- Exact width match: not required, per designer.
- Partial overlap: not allowed, per designer.
