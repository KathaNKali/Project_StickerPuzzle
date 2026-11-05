# 004: Initial build settings

**Date:** 2026-11-05

**What:** WebGL uses Gzip with the decompression fallback on. Android builds ARM64 with IL2CPP (template default). Portrait is set in `ProjectSetup`.

**Why:** Gzip with the fallback runs on any web host without special server settings, at the cost of a slightly larger download than Brotli.

**Not yet proven:** the designer chose to skip builds in Phase 0, so no build has finished. Revisit at the first real build (Phase 4 at the latest).

**Alternatives rejected:** Brotli: smaller, but the host must be configured for it.
