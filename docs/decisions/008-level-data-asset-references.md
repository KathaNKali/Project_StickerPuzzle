# 008: Level Data points directly at level assets

**Date:** 2026-11-07

**What:** Level Data is an ordered list of direct references to level assets. Player saves store only a level's permanent ID, never a reference. This is an exception to the general "data uses stable IDs" rule in [CLAUDE.md](../../CLAUDE.md), and it applies only to this authored list.

**Why:** Dragging named levels into order is simple for the designer, and Unity includes listed levels in builds automatically. Saves are what must survive renaming and reordering, and they still use IDs.

**Alternatives rejected:** A list of ID text: the designer cannot see which level is which, and the game would still need another way to find each asset.
