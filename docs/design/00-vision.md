# 1. GAME OVERVIEW & VISION

## 1.1 Game Concept

**Working Title:** TBD

**Genre:** Casual Puzzle / Decoration

**Game Type:** Single-player, persistent puzzle and sticker collection game.

**Core Concept:**

A relaxing casual game combining 3D sliding-block puzzle gameplay with a 2D sticker collection and decoration experience.

Players interact with a persistent puzzle board containing colored blocks with illustrated images displayed on their surfaces. Each block must exit through an exit matching its color.

Successfully exiting a block rewards the player with its associated sticker.

Players collect these stickers and use them to complete themed sticker-book pages. The sticker collection serves as the primary home screen and the central destination for long-term progression.

Unlike traditional level-based puzzle games, the puzzle experience is designed around continuous gameplay. Once the current board configuration is cleared, new blocks or puzzle configurations appear according to the progression system.

The puzzle state is automatically saved after every successful move, allowing players to leave and return without losing their progress.

The initial gameplay uses complete-image blocks (Option A). Future expansions may introduce image fragments (Option B) and hybrid combinations (Option C).

## 1.2 Game Vision

Create an enjoyable and relaxing casual game that combines the satisfaction of solving spatial puzzles with the pleasure of collecting and completing beautiful sticker collections.

The game should offer two complementary experiences:

**1. Puzzle Experience**

A polished 3D environment where players solve sliding-block challenges using logic and spatial awareness.

**2. Sticker Collection Experience**

A charming 2D environment where players collect stickers, complete themed pages and build personal collections.

The sticker collection is the primary home screen and the player's long-term progression destination.

The puzzle board is the primary gameplay activity that generates stickers and drives collection progression.

Both experiences should feel like parts of one cohesive game.

## 1.3 Target Audience

**Primary Audience:** Broad casual gaming audience.

The game should be approachable to players across different age groups and gaming experience levels.

Design considerations:

- Easy-to-understand mechanics.
- Simple and intuitive controls.
- Gradually evolving gameplay complexity.
- Relaxing visual presentation.
- Clear objectives and feedback.
- No requirement for prior puzzle-game experience.
- Age-appropriate artwork, language and content.

The experience should prioritize accessibility, relaxation and approachability.

## 1.4 Target Platforms

The intended product is cross-platform, with a phased release strategy.

| Platform | Development priority |
|---|---|
| Android | Initial |
| Web | Initial |
| iOS | Subsequent expansion |
| PC | Subsequent expansion |

The game should be designed with cross-platform compatibility in mind from the beginning.

Platform-specific considerations:

- Touch-based interaction for mobile.
- Mouse-based interaction for web and PC.
- Responsive layouts for different screen sizes.
- Consistent gameplay rules across platforms.
- Consideration for cross-platform save compatibility.

The implementation agent is responsible for evaluating the appropriate technical approach.

## 1.5 Visual Direction

The game uses two complementary visual styles.

### A. Core Puzzle: 3D

Puzzle gameplay will be presented in a polished 3D environment.

Visual characteristics:

- Three-dimensional blocks with visible depth.
- Illustrated images displayed on the top surface of blocks.
- Distinct and recognizable block colors.
- Matching exits that clearly communicate their required colors.
- Smooth block movement and exit animations.
- Satisfying visual feedback.
- Clean and uncluttered puzzle boards.

**Important distinction between block properties:**

Each block has two independent visual and gameplay properties:

1. Block Color: Determines which exit the block can use.
2. Block Image: Determines which sticker the player receives after successfully exiting the block.

The image displayed on the block does not determine its exit color.

### B. Sticker Collection: 2D

The sticker-book experience will use a 2D visual presentation.

Visual characteristics:

- Illustrated stickers.
- Themed sticker collections.
- Attractive sticker-book pages.
- Clear completed and incomplete sticker slots.
- Smooth sticker acquisition and placement animations.
- Visually satisfying collection completion.

### C. Visual Consistency

Although the puzzle and sticker interfaces use different visual dimensions, their artwork, themes, colors and overall identity should remain consistent.

Transitions between the 3D puzzle and 2D collection experience should feel natural.

## 1.6 Core Game Structure

The game consists of two primary sections.

### A. Sticker Collection Home Screen

The sticker collection is the default screen when the player opens the game.

It serves as the central navigation and progression hub.

The home screen should allow players to:

- View their sticker collections.
- Browse themed sticker books.
- Identify collected and missing stickers.
- View page and collection completion progress.
- Access completed collections.
- Navigate to the puzzle section through a prominent Play button.

### B. Puzzle Section

The puzzle section contains the persistent sliding-block puzzle board.

It is not structured around traditional individual levels.

Players continuously interact with the board, clearing blocks and earning stickers.

The board evolves as players progress through the game.

The puzzle experience should preserve its state when players leave and return.

## 1.7 Core Puzzle Mechanics

### A. Image-Based Blocks

Every block has:

- A defined color.
- An illustrated image displayed on its top surface.
- A movement direction and spatial footprint defined by the puzzle configuration.
- An associated sticker reward.

### B. Matching Exits

The puzzle board contains exits with designated colors.

A block can successfully exit only through an exit matching its own color.

The block's displayed image has no influence on exit matching.

### C. Block Movement

Players move blocks according to the established puzzle movement rules.

Movement must respect the board boundaries, occupied spaces, obstacles and other applicable puzzle constraints.

### D. Sticker Rewards

Whenever a block successfully exits the board, the player receives its associated sticker.

The sticker reward is added to the player's collection.

### E. Continuous Board Progression

The puzzle does not end after completing a traditional level.

New gameplay configurations are introduced as the player progresses.

Board progression may include:

- Fresh blocks appearing on the existing board.
- New board configurations.
- Progressively more complex arrangements.
- New blockers and special mechanics.

These approaches may be combined depending on progression and future design decisions.

## 1.8 Continuous Gameplay Loop

The primary gameplay loop is:

1. Player opens the game and lands on the sticker collection home screen.
2. Player enters the puzzle section.
3. The game restores the previously saved puzzle state, if one exists.
4. Player observes a board containing image-based blocks and matching exits.
5. Player moves blocks according to the puzzle rules.
6. Blocks successfully exit through matching-color exits.
7. Each successfully cleared block awards its associated sticker.
8. Earned stickers are added to the collection.
9. Once the current board configuration is cleared, the game introduces new blocks or a new puzzle configuration.
10. The player continues interacting with the evolving puzzle.
11. The player can leave the puzzle at any time.
12. On returning, the player resumes from the previously saved state.
13. Players can return to the sticker collection home screen to view their progress.

The experience should feel continuous rather than divided into conventional level-completion screens.

## 1.9 Persistent Gameplay & Save System

The puzzle board is designed as a persistent gameplay environment.

The game should save the player's progress after every successful move.

The saved state should preserve the information required to restore the puzzle exactly as the player left it.

At minimum, this includes:

- Current board configuration.
- Current block positions.
- Remaining blocks.
- Cleared blocks.
- Available exits and their states.
- Current progression state.
- Earned sticker rewards.
- Relevant puzzle-specific information.

The save system should prevent successfully collected stickers or completed block movements from being lost or rewarded multiple times after restoring a previous state.

The exact persistence architecture and data structure will be determined by the implementation agent.

## 1.10 Player Freedom & No-Move Handling

The intended experience is relaxed and non-punishing.

Players should be free to experiment with block movements without being forced into traditional failure states.

Initial design principles:

- No conventional level failure screen.
- No forced restart after an unsuccessful move sequence.
- Players can experiment with available movements.
- No mandatory penalty for making incorrect moves.
- The player should be able to leave and resume at any time.

The game should prioritize experimentation and relaxation over pressure and punishment.

Additional recovery mechanics, if needed, can be introduced after prototype testing.

## 1.11 Sticker Collection & Reward Structure

Sticker acquisition uses a combination of predetermined and random rewards.

**Predetermined Rewards:**

Specific blocks or progression milestones can award specific stickers.

This supports structured collection progression and ensures players can work toward particular collection goals.

**Random Rewards:**

Some blocks or reward opportunities may provide randomly selected stickers from an eligible collection.

This introduces variety and discovery.

The exact balance between predetermined and random rewards will be established during economy and progression design.

All successfully earned stickers must be recorded in the player's collection.

## 1.12 Future Gameplay Evolution

The initial version focuses on complete-image blocks.

Future gameplay expansions may introduce the following mechanics.

**Option A: Complete Image Blocks**

Each block contains a complete illustration. Successfully exiting it awards the corresponding complete sticker.

This is the initial MVP mechanic.

**Option B: Image Fragments**

Blocks contain fragments of larger illustrations. Players collect multiple fragments to complete a sticker.

**Option C: Hybrid Image Mechanics**

The board combines complete-image blocks and image fragments, potentially alongside special blocks and blockers.

The introduction of these mechanics should be gradual.

Future mechanics should expand strategic depth without compromising the accessibility and relaxing nature of the game.

## 1.13 Core Design Pillars

**Pillar 1: Relaxing Gameplay**

Players should feel comfortable experimenting and progressing without unnecessary pressure.

**Pillar 2: Satisfying Puzzle Solving**

Every movement, successful exit and board interaction should provide clear and satisfying feedback.

**Pillar 3: Rewarding Collection**

Every successful puzzle interaction should contribute to a meaningful sticker collection journey.

**Pillar 4: Visual Enjoyment**

The 3D puzzle environment and 2D sticker experience should both feel polished, attractive and cohesive.

**Pillar 5: Continuous Progression**

The game should provide an evolving puzzle experience without relying on conventional level boundaries.

**Pillar 6: Gradual Discovery**

New puzzle mechanics, blockers and collection systems should be introduced progressively.

## 1.14 Initial Product Scope (MVP)

The initial development version should focus on validating the primary gameplay and collection experience.

### Included in MVP

- 3D sliding-block puzzle gameplay.
- Complete-image blocks.
- Matching-color exits.
- Block movement and collision rules.
- Persistent puzzle board.
- Continuous board progression.
- Automatic saving after successful moves.
- Restoration of previously saved puzzle state.
- Sticker rewards for successfully cleared blocks.
- Combination of predetermined and random sticker rewards.
- 2D sticker collection home screen.
- Themed sticker-book pages.
- Basic collection progress tracking.
- Navigation between collection and puzzle sections.
- Android and web support.

### Not required for MVP

- Image-fragment mechanics.
- Hybrid image-block mechanics.
- Complex special blockers.
- Multiplayer.
- Social features.
- Live events.
- Advanced monetization systems.
- Complex room-building or decoration systems.

These features may be considered in future development phases.

## 1.15 Provisional Design Decisions

The following decisions represent the current design direction, not immutable requirements.

| Area | Current decision | Status |
|---|---|---|
| Home screen | Sticker collection | Provisional |
| Puzzle structure | Continuous persistent board | Provisional |
| Board progression | Replenishment, new configurations and progressive difficulty | Provisional |
| Block matching | Block color must match exit color | Provisional |
| Block artwork | Image displayed on top of block | Provisional |
| Player freedom | No conventional failure or forced restart | Provisional |
| Sticker rewards | Predetermined and random combination | Provisional |
| Save behavior | Save after every successful move | Provisional |
| Initial mechanic | Complete-image blocks (A) | Initial MVP direction |
| Future mechanics | Image fragments (B) and hybrid (C) | Future expansion |

These decisions are based on the initial game concept and may be revised during development, prototyping and playtesting.

The implementation agent should identify any design ambiguities, technical limitations or conflicting requirements and propose alternatives rather than silently making major gameplay changes.

## 1.16 Technical Decision Ownership

This document defines the desired player experience, gameplay behavior and product requirements.

The implementation agent (Claude) is responsible for evaluating and proposing:

- Suitable technology stack.
- Game engine and rendering approach.
- Application architecture.
- Code organization and modularity.
- Puzzle-system implementation.
- Sticker and collection data models.
- Cross-platform compatibility.
- Persistent save systems.
- Asset management.
- Testing and deployment strategy.

Technical decisions should satisfy the product requirements while keeping the game extensible for future mechanics.

Significant architectural decisions should be documented with their rationale and trade-offs.

## 1.17 Initial Product Success Criteria

The prototype should demonstrate that:

1. Players can understand and interact with the core puzzle.
2. Block movement feels responsive and predictable.
3. Blocks can only exit through matching-color exits.
4. Every successful block exit correctly awards its associated sticker.
5. The sticker collection accurately reflects earned rewards.
6. Players can leave and resume the puzzle without losing progress.
7. New blocks or configurations can be introduced after board completion.
8. The collection home screen provides a clear overview of progression.
9. The 3D puzzle and 2D collection experiences feel like one cohesive game.
10. The game operates reliably on the initial target platforms.

Commercial success and retention targets will be defined separately after prototype testing.