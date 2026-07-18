# CLAUDE.md — Farm Farmer Project Context

This file bridges claude.ai design sessions and Claude Code implementation sessions.
claude.ai = design office (docs, economy math, planning). Claude Code = implementation (code, commits).
**Update this file whenever a decision changes.** Treat it as the single source of truth for project state.

---

## Project Identity

- **Title:** Farm Farmer (trademark-clear as of Jul 17). No subtitle baked into the title for now —
  see Open Questions for the "Idle RPG" subtitle discussion.
- **Genre:** Incremental RPG — roster-based active/background hero progression
  (Clicker Heroes stage-pushing × Melvor Idle resource depth). Not a pure idle game: the player
  actively focuses one hero at a time.
- **Engine:** Unity 6, URP, 2D
- **Platform:** Android first. iOS deferred (no Mac in pipeline yet). Portrait orientation, locked.
- **Audience declaration:** 13+ (deliberate; avoids Play Families Policy / COPPA scope)
- **Full concept:** see `docs/FARM_FARMER_CONCEPT.md` (living doc, version header inside) — **not yet
  delivered from the design office; do not assume it exists until it lands in this repo.**

## Current Phase

**Phase 1 — economy model COMPLETE (combat + skills, sim-validated). Phase 2 — architecture
scaffold underway.**
Decisions 3, 5, 6, and 8 below were revised 2026-07-17 after Derek's correction pass on the
Phase 1 comprehension checkpoint — the roster model (#6) in particular replaces the earlier
"parallel lanes" framing. Unity project is open locally (Unity 6000.5.4f1 / URP 17.6.0). First
architecture pass landed 2026-07-17: `FarmFarmer.Core`/`.Data`/`.UI` asmdefs, `BigDouble` number
type, `GameStateMachine`/`IGameState` (Boot/MainMenu/Gameplay/Cutscene stub states) driven by a
`[RuntimeInitializeOnLoadMethod]` bootstrap, core `ScriptableObject` definitions (Hero, Stage,
Enemy, Generator, Upgrade, Roster), a versioned atomic-write save schema, and a shared
`WalletService`. No scenes, prefabs, or UI built yet — this is data/state plumbing only.
`docs/FARM_FARMER_CONCEPT.md` still doesn't exist; drafting it is in progress separately,
checkpointed section-by-section with Derek.

## Locked Decisions

1. Android-first, portrait-only. Never add landscape support.
2. Monetization: free-to-play. IAP funds convenience and gameplay-boosting purchases — explicitly
   not pay-to-win. Rewarded ads are opt-in only. No forced ads, no energy systems. Rather than a
   classic "remove ads" purchase (which fights an ads-opt-in model), sell a permanent Egg Inc.-style
   "ad-free bonus" IAP that auto-grants all rewarded-ad bonuses without watching — reads as pure
   convenience, expected best-converting IAP. Time skips as consumables; a starter pack. Subscription
   ("Golden Harvest Pass": bonus XP, raised offline cap, cosmetic flair) is real but explicitly
   post-launch/LiveOps — Google Play subscription compliance overhead (cancellation flow, restore
   purchase handling) isn't worth taking on at 1.0.
3. **Prestige loop is a core, locked mechanic; its name is NOT locked.** Working title "The
   Harvest" / working prestige-currency name "Harvest Seeds" are placeholders — "Harvest" may
   collide with a separate, distinct mechanic for collecting idle/offline resource accrual on
   return, and these two concepts need different names before implementation. See Open Questions.
4. Stage structure: 10-kill tally, boss every 5th stage, boss fail knocks back one stage.
5. **Asset authorship:** AI-assisted art is allowed as part of Derek's own creative process (e.g.
   sketch → AI concept pass → Derek's refinement) — raw/unrefined AI output does not ship.
   Player-facing prose remains fully human-authored; no AI-generated player-facing text ships.
   Code and internal docs remain fair game for AI generation.
6. **Roster-based hero progression** (revised from "parallel lanes"). The player actively controls
   one focused hero at a time, clearing that hero's floor track and pushing to higher floors for
   XP and loot. Unlocking additional heroes builds a roster — EverQuest/WoW/D&D-style class
   archetypes — each with its own independent floor-clearing progression and one unique gameplay
   contribution. Only the focused hero gets full active-play progression (including tap damage);
   unfocused unlocked heroes accrue XP/progress at a reduced background rate (formula TBD — to be
   developed). Coin wallet remains shared across the roster (allocation across heroes is the
   strategy layer) unless later revised. **Balance containment rule unchanged: one master curve
   family for HP/cost/DPS; heroes differ only by offset, multiplier, and one signature mechanic.
   Never author bespoke curves per hero.** Grouping (small-team floor clearing) and Raids
   (full-roster fights) are planned expansions of this same roster system — see Proposed, Not
   Locked.
7. **Generators are trainable skills (Melvor-depth).** Woodworker/Miner/Gatherer are hired NPCs
   embodying skills — all run in parallel, always. XP curve 40×1.12^lvl cap 99; 7 resource tiers
   gated at levels 1/15/32/50/65/80/91; mastery-lite per node. **The prestige reset clears the
   battlefield, never the estate:** combat progression, hero levels, coin reset; skill XP, mastery,
   stockpiles, buildings persist. Harvest Seed bonus locked at +15% global DPS per Seed (run-2 sim
   proved 5% was a dead prestige loop). The full skill list must be built out before deciding each
   skill's specific gameplay impact — sequencing note, not a change to the mechanic above. Spec:
   docs/economy/SKILLS_DESIGN_v0.1.md.
8. **Layered-backdrop visual structure (Derek-specified).** Each floor / gameplay section — not
   necessarily a literal Unity scene — gets its own unique backdrop; this applies to combat floors,
   skills areas, and other gameplay sections alike. (The original "one backdrop per 10-stage
   region, 10 regions across stages 1-100" cadence is superseded by floor-based terminology and
   needs reconciling — see Open Questions.) Skills retain a shared management-view backdrop with
   Melvor-style rows; each generator opens its own detail scene. No full farm-world sim at v1.0.
   Bottom nav: Combat / Estate / Menu. **Locked (2026-07-17):** farm buildings appear as prop
   overlays inside generator scenes, visibly upgrading with generator level — not a full farm-world
   sim, but not purely functional/UI-only either.

## Proposed, Not Locked (Derek veto pending)

- **Hero roster:** class-based system inspired by EverQuest/WoW/D&D archetypes. Sprout Knight
  (melee/tank) is the confirmed first hero; further roster classes are TBD, each contributing one
  unique gameplay mechanic. Mechanics are not ironed out yet — pipeline item, not blocking Phase 2.
- **Grouping** (small-team floor clearing) and **Raids** (full-roster fight, roster power sums all
  hero progress) — both planned pipeline systems layered on top of the roster, intended as an
  anti-neglect incentive. Architecture must not preclude either.
- **Hero unlock cadence:** hero 1 at start; further heroes unlock on some cadence (earlier draft:
  hero 2 ~floor/stage 25, hero 3 at first prestige) — needs reconfirming now that progression is
  roster-based rather than parallel-lane-based. First hour plays single-hero for onboarding.
- **Title subtitle/tagline:** "Farm Farmer" is the trademark-clear title. Whether "Idle RPG" (or an
  alternative like "Incremental RPG") ships as a subtitle/store tagline is undecided — see Open
  Questions for Claude's recommendation (currently: drop it from the title, use it only in store
  metadata/keywords).

## Architecture Intents (for Phase 2+)

- Bootstrap via `[RuntimeInitializeOnLoadMethod]`; no scene-dependent initialization.
- `GameStateMachine` / `IGameState` pattern for app flow (Boot, MainMenu, Gameplay, Cutscene).
- Data-driven everything: `ScriptableObject` definitions for enemies, stages, generators, upgrades,
  **and heroes/roster** (roster size must be data, not code — LiveOps hero additions).
- Event-driven economy: currency changes broadcast via events; UI subscribes, never polls.
- Object pooling for enemies, floating damage numbers, coin VFX.
- **Idle math:** use a BigDouble/break-infinity style number type from day one. Standard `double`
  overflows idle-game scaling; retrofitting is painful.
- **Hero/floor simulation:** only the actively-focused hero receives full-rate progression;
  unfocused unlocked heroes accrue progress at a reduced background rate (rate formula TBD).
  Offline progression must apply that same focused/background split retroactively from the saved
  UTC timestamp; guard against clock tampering.
- Saves: local-first, atomic write (write temp, then rename), versioned schema. Save schema stores
  per-hero state as a list from day one, even while only one hero exists.
- Cloud save: deferred decision, architecture must not preclude it.

## Conventions

- **Namespace root:** `FarmFarmer.Core`, `FarmFarmer.UI`, `FarmFarmer.Data`, etc.
- **Asset naming:** `PF_` prefabs, `SO_` ScriptableObjects, `Icon_`, `SFX_`, `MUS_`, `FX_`, `UI_`.
- **Code comments:** personal-notes style, not formal documentation. Comment the why, not the what.
- **Canvas:** Screen Space Overlay, Scale With Screen Size, reference resolution 1080x1920 (portrait).
- Regions in files over ~50 lines.

## Repo Layout (target)

```
/docs        — GDD, concept docs, economy model exports
/sim         — Python economy/skill simulators used to tune constants
/Assets      — Unity project (when created)
CLAUDE.md    — this file
README.md    — public-facing summary
```

## Open Questions

1. **Background progression rate** for unfocused roster heroes — confirmed to exist (they gain XP
   while not focused), exact formula/tuning still to be developed.
2. **Title/subtitle:** trademark-clear title is "Farm Farmer" alone. Whether to append "Idle RPG"
   (or an alternative) as a subtitle/tagline is open. Claude's recommendation: drop "Idle RPG" from
   the title/subtitle entirely — the game isn't purely idle (active per-hero floor focus), and
   "Incremental RPG" is the more accurate, audience-recognized umbrella term (Melvor/Clicker Heroes
   crowd already uses it) — reserve it for store metadata/keywords rather than the literal title,
   so genre positioning stays flexible while the roster/grouping/raids design settles. Also still
   pending: real trademark search (Phase 5); store listing must lead with combat, not crops.
3. **Hero roster identity:** roster model confirmed (EverQuest/WoW/D&D-style classes, each with a
   unique gameplay contribution), but no specific classes/names/species chosen beyond Sprout
   Knight, and grouping/raid mechanics aren't designed yet.
4. **Prestige-naming collision:** "Harvest" is the working name for at least three distinct
   concepts now — the prestige/reset loop, a separate planned offline-return/idle-collection
   mechanic, and the "Golden Harvest Pass" subscription tier (see Decision 2). These need distinct
   names before implementation locks them in.
5. **Terminology reconciliation:** "floor" (new roster description), "stage" (original 10-kill-tally
   /boss-every-5th structure), and "lane" (original parallel-track term, now superseded by the
   roster model) need to converge on one consistent vocabulary before Phase 2 architecture work
   locks in naming.
