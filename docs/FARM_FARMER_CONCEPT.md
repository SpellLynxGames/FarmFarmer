# Farm Farmer — Concept Document

**Version:** v0.1 — 2026-07-17
**Status:** living doc; seeds `CLAUDE.md`'s Project Identity / Locked Decisions. When one changes,
update both — `CLAUDE.md` is the single source of truth if they ever disagree.

---

## 1. Pitch

Farm Farmer is a portrait-only incremental RPG for Android. You build a roster of heroes, each
modeled on classic RPG class archetypes (EverQuest/WoW/D&D-style), and actively pilot one at a
time through waves of combat — while training a deep bench of gathering and crafting skills that
keeps your whole roster equipped. It's designed for stolen moments: pull the phone out, tap through
a few stages, pocket it. Sessions can run long when you want them to, but the core interaction
never demands two hands or your full attention.

## 2. Core Loop

Each session centers on your currently focused hero: clear floors within a stage, banking coin and
XP as you go. Every 10th stage brings a miniboss, every 25th a full boss — clear it to keep
climbing, fail it and you're knocked back one stage. Between fights, spend coin on upgrades for
your focused hero, or route it toward training your Estate skills (Woodworker, Miner, Gatherer, and
more to come), which run continuously in the background no matter what you're doing in combat.

Periodically you'll choose to reset your combat progress — the prestige loop (working name "The
Harvest," not finalized) — trading your current depth for a permanent global damage bonus and
starting the climb again, faster. Unlocking new heroes builds out your roster; each runs an
independent stage-clearing progression, and while you can only actively pilot one at a time, the
rest inch forward at a reduced background rate.

## 3. Hero Roster

Heroes are class archetypes in the EverQuest/WoW/D&D sense — each contributes one unique gameplay
mechanic, not just a reskinned damage number. **Sprout Knight** (melee/tank) is the confirmed first
hero and carries the game's identity at launch; further roster classes are undecided beyond that.

The player starts single-hero for onboarding, then unlocks additional heroes on a cadence still
TBD. Every hero shares one master balance curve (HP/cost/DPS) and differs only by offset,
multiplier, and its one signature mechanic — no bespoke per-hero curves, ever.

Two systems are planned to sit on top of the roster once it's built out: **Grouping** (small-team
stage clearing) and **Raids** (full-roster fights, roster power summing all hero progress). Both
are anti-neglect incentives — reasons to keep every hero leveled, not just your current favorite.
Neither is designed yet; the architecture just needs to not preclude them.

## 4. Combat Structure

- **Floor** — the fine-grained kill-tally unit. Floors 1-10 make up a stage. Low-stakes on their
  own; reaching floor 10/10 clears the stage.
- **Stage** — the meaningful unit of progression. Difficulty escalates stage-to-stage, not
  floor-to-floor.
- **Miniboss** — every 10th stage. No confirmed loot (assumed none — see Open Questions).
- **Boss** — every 25th stage. Drops gear loot into the hero's inventory.
- **Failure** — failing either a miniboss or a boss knocks you back one stage; you retry from
  there.
- **Backdrop** — changes per stage, not per floor (see Visual Identity).
- **Loot & gear** — each hero carries a small (~5-slot) inventory. Boss drops go there; equipped
  gear grants small combat buffs (exact stats TBD). Unwanted loot can be scrapped for FARM
  resources — see Economy Overview.
- **Gems** — a separate meta-progression currency, dropped every 100 stages (rate may increase
  further at very high stages, tuning TBD) — see Economy Overview.

## 5. Skills / The Estate

Generators are trainable skills, Melvor-depth, not passive idle ticks. Woodworker, Miner, and
Gatherer are the first three — hired NPCs embodying a skill, all training in parallel, always,
independent of what your active hero is doing in combat. **The Farm is explicitly meant to feel
like its own minigame — half the game, in the same way Melvor Idle or Adventure Capitalist's
management layer is — not a system bolted onto combat.**

- XP curve: `40 × 1.12^level`, capped at level 99.
- 7 resource tiers, gated at levels 1 / 15 / 32 / 50 / 65 / 80 / 91.
- Mastery-lite per node.

The full skill list beyond these three still needs to be built out before deciding each skill's
specific gameplay impact — that's a sequencing gap, not an open design question.

Skill/resource growth grants small incremental bonuses back to combat effectiveness — a light
one-way cross-pollination from Estate to Combat. The reverse bridge (Combat to Estate) is scrapping
unwanted gear loot for FARM resources; there is no direct conversion otherwise (coin never buys
FARM resources, and mobs never drop them directly).

## 6. Prestige

Name pending (see Open Questions) — currently "The Harvest," currency "Harvest Seeds." The
mechanic itself is locked: prestiging clears the battlefield, never the Estate. Combat progression,
hero levels, and coin reset; skill XP, mastery, stockpiles, and buildings all persist. Each Seed is
worth a flat **+15% global DPS** — locked at that number specifically because an earlier sim run
showed 5% produced a dead prestige loop (no real incentive to reset).

## 7. Economy Overview

Three currencies, not one:

- **Coin** — primary combat currency, earned continuously from stage kills. Shared across the
  whole roster (deciding which hero to fund is the strategy layer, not just which hero to play).
  Spent on combat/hero upgrades.
- **FARM resources** — the separate Estate/skill currency, earned only through the Woodworker/
  Miner/Gatherer skills. Mobs never drop it directly; the only combat-side inflow is scrapping
  unwanted gear loot (see Combat Structure).
- **Gems** — a third, Clicker-Heroes-style meta-progression currency dropped from combat every 100
  stages.

**`docs/economy/` and `sim/` are currently stale** — the existing sim-validated numbers assumed a
single boss tier and a single currency, both superseded by the structure above. Full curve math
(HP scaling, cost compounding, prestige payout, and now three-currency balance) needs a real
re-model before those docs can be trusted again; this doc doesn't attempt that math itself, only
the shape of the systems it needs to tune.

## 8. Visual Identity

Each stage — and each gameplay section more broadly (skills areas, other non-combat screens) —
gets its own unique backdrop; not necessarily a literal Unity scene. Floors within a stage share
one backdrop; the jump only happens at stage boundaries. Skills share one management-view backdrop
with Melvor-style rows, and each generator opens its own detail scene, where farm buildings appear
as prop overlays that visibly upgrade with generator level. No full farm-world simulation at v1.0.
Bottom nav is Combat / Estate / Menu.

## 9. Monetization

Free-to-play. IAP funds convenience and gameplay-boosting purchases — explicitly not pay-to-win.
Rewarded ads are opt-in only; no forced ads, no energy systems.

Rather than a classic "remove ads" purchase (which fights an ads-opt-in model), the plan is a
permanent Egg Inc.-style "ad-free bonus" IAP that auto-grants all rewarded-ad bonuses without
watching — reads as pure convenience, and is the expected best-converting IAP. Around that: time
skips as consumables, and a starter pack.

A subscription ("Golden Harvest Pass": bonus XP, raised offline cap, cosmetic flair) is real but
explicitly post-launch/LiveOps — Google Play's subscription compliance overhead (cancellation flow,
restore purchase handling) isn't worth taking on at 1.0. Note: this name shares the same "Harvest"
collision flagged in Open Questions.

## 10. Platform & Audience

Android first, portrait orientation, locked — never landscape. iOS is deferred pending Mac
availability in the pipeline. Audience declaration is 13+, a deliberate choice to stay outside Play
Families Policy / COPPA scope.

## 11. Open Questions

Kept in sync with `CLAUDE.md`'s Open Questions section — check there for the current list rather
than trusting this copy if the two drift:

1. Background progression rate formula for unfocused roster heroes.
2. Title/subtitle: "Farm Farmer" is trademark-clear; whether "Idle RPG" or "Incremental RPG" ships
   as a subtitle/tagline is undecided (leaning toward neither in the literal title).
3. Hero roster identity beyond Sprout Knight — no further classes, names, or species chosen; and
   grouping/raid mechanics aren't designed yet.
4. Prestige-naming collision: "Harvest" currently names three different things (the prestige loop,
   a planned offline-return/idle-collection mechanic, and the Golden Harvest Pass subscription).
   These need distinct names before implementation locks them in.
5. Economy model needs a full re-sim: the old sim-validated numbers assumed one boss tier and one
   currency, both superseded by the floor/stage/boss and three-currency decisions above. In
   progress; don't trust `docs/economy/` or `sim/` output as current until it's redone.
6. Miniboss loot (every 10 stages) — assumed none, unlike the boss (every 25); not yet confirmed.
7. Gear stat specifics — equipped gear grants "small combat buffs," exact stats/scaling TBD.
8. Does the Gems currency persist through a prestige reset? Strongly implied by "meta progression"
   framing but never explicitly stated.
