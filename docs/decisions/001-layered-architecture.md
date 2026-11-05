# 001: Layered architecture with assembly definitions

**Date:** 2026-11-05 (decided by designer in planning; recorded here)

**What:** Core (pure C#), Game, Presentation, Editor tools, Tests, each an assembly definition. See [ARCHITECTURE](../ARCHITECTURE.md).

**Why:** Rules can be fully unit-tested without Unity. A wrong-way dependency fails at compile time rather than relying on discipline.

**Alternatives rejected:** Not recorded in the planning summary.
