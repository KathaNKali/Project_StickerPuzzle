# 006: Fixed levels saved as exact boards

**Date:** 2026-11-05

**What:** All players get the same saved boards in the same order. A level stores its exact board and solution as well as its settings. Seed mode only affects what Create builds in the generator scene.

**Why:** Fixed boards are easier to tune, and improving the generator later cannot silently change a tuned level. The trade-off: a saved level does not pick up generator improvements unless it is created again.

**Alternatives rejected:** Generating a new board each time: harder to tune; may return later. Saving only settings and seed: smaller, but tied to the generator never changing.
