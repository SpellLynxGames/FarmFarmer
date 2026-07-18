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
- **Full concept:** see `docs/FARM_FARMER_CONCEPT.md` v0.1 (living doc, version header inside).

## Current Phase

**Phase 1 — economy model UNDER REVISION (see Open Question #5); Phase 2 — architecture scaffold
underway.**
Decisions 3, 5, 6, and 8 below were revised 2026-07-17 after Derek's correction pass on the
Phase 1 comprehension checkpoint — the roster model (#6) in particular replaces the earlier
"parallel lanes" framing. Unity project is open locally (Unity 6000.5.4f1 / URP 17.6.0). First
architecture pass landed 2026-07-17: `FarmFarmer.Core`/`.Data`/`.UI` asmdefs, `BigDouble` number
type, `GameStateMachine`/`IGameState` (Boot/MainMenu/Gameplay/Cutscene stub states) driven by a
`[RuntimeInitializeOnLoadMethod]` bootstrap, core `ScriptableObject` definitions (Hero, Stage,
Enemy, Generator, Upgrade, Roster), a versioned atomic-write save schema, and a shared
`WalletService`. No scenes, prefabs, or UI built yet — this is data/state plumbing only.
`docs/FARM_FARMER_CONCEPT.md` v0.1 drafted 2026-07-17; also resolved former Open Question #5
(floor/stage/lane terminology) and the stage-boss cadence (miniboss/10, boss/25) in the same pass —
see Decision 4. **Same pass revealed the "Phase 1 complete" economy model was validated against a
single-boss-tier, single-currency assumption that no longer holds** once Decision 9 (three-currency
economy) landed — `ff_sim.py` needs a real re-model, not a numbers patch (see Open Question #5,
renumbered). Don't treat old economy docs/sim output as current until that's redone.

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
4. **Floor/stage structure (revised 2026-07-17 — resolves former Open Question #5).** A **floor**
   is the fine-grained kill-tally unit: floors 1-10 within a stage, generally inconsequential on
   their own — reaching floor 10/10 clears the stage. A **stage** is the container; stage-to-stage
   transitions are where difficulty escalates. A **miniboss** appears every 10 stages, a full
   **boss** every 25 stages — the boss (every 25) drops gear loot into the hero's inventory; the
   miniboss (every 10) is not confirmed to drop loot (assumed no loot until confirmed — see Open
   Questions). Failing either knocks the player back one stage. "Lane" (the original parallel-track
   term) is fully superseded by the roster model (#6). Each hero has a small (~5-slot) gear
   inventory; equipped gear grants small combat stat buffs (specifics TBD); unwanted loot can be
   scrapped for FARM resources (see Decision 9). Coin, gems (every 100 stages, meta-progression
   currency), and gear loot are all combat drops — see Decision 9 for the full currency model.
5. **Asset authorship:** AI-assisted art is allowed as part of Derek's own creative process (e.g.
   sketch → AI concept pass → Derek's refinement) — raw/unrefined AI output does not ship.
   Player-facing prose remains fully human-authored; no AI-generated player-facing text ships.
   Code and internal docs remain fair game for AI generation.
6. **Roster-based hero progression** (revised from "parallel lanes"). The player actively controls
   one focused hero at a time, clearing that hero's stage track and pushing to higher stages for
   XP and loot. Unlocking additional heroes builds a roster — EverQuest/WoW/D&D-style class
   archetypes — each with its own independent stage-clearing progression and one unique gameplay
   contribution. Only the focused hero gets full active-play progression (including tap damage);
   unfocused unlocked heroes accrue XP/progress at a reduced background rate (formula TBD — to be
   developed). Coin wallet remains shared across the roster (allocation across heroes is the
   strategy layer) unless later revised. **Balance containment rule unchanged: one master curve
   family for HP/cost/DPS; heroes differ only by offset, multiplier, and one signature mechanic.
   Never author bespoke curves per hero.** Grouping (small-team stage clearing) and Raids
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
8. **Layered-backdrop visual structure (Derek-specified).** Backdrops change **per stage, not per
   floor** — floors 1-10 within a stage share one backdrop; each stage / gameplay section — not
   necessarily a literal Unity scene — gets its own unique backdrop; this applies to combat stages,
   skills areas, and other gameplay sections alike. (Resolved 2026-07-17: the original "one backdrop
   per 10-stage region" cadence is superseded by this per-stage granularity.) Skills retain a shared
   management-view backdrop with
   Melvor-style rows; each generator opens its own detail scene. No full farm-world sim at v1.0.
   Bottom nav: Combat / Estate / Menu. **Locked (2026-07-17):** farm buildings appear as prop
   overlays inside generator scenes, visibly upgrading with generator level — not a full farm-world
   sim, but not purely functional/UI-only either.
9. **Three-currency economy (added 2026-07-17).** Dual economy, not one: **Coin** is the primary
   combat currency, earned continuously from stage kills, shared across the roster, spent on
   combat/hero upgrades. **FARM resources** are the separate Estate/skill currency earned only via
   the Woodworker/Miner/Gatherer skills — mobs never drop FARM resources directly. **Gems** are a
   third, Clicker-Heroes-style meta-progression currency, dropped from combat every 100 stages
   (rate may increase further at very high stages, tuning TBD). **The Farm is explicitly meant to
   feel like its own minigame/half of the game** (Melvor Idle / Adventure Capitalist depth), not a
   bolt-on system. The two halves cross-pollinate in one direction each: skill/resource growth
   grants small incremental bonuses back to combat effectiveness, and unwanted gear loot (see
   Decision 4) can be scrapped for FARM resources — that scrap conversion is the *only* combat→
   Estate currency bridge; there is no reverse (coin never buys FARM resources).

## Proposed, Not Locked (Derek veto pending)

- **Hero roster:** class-based system inspired by EverQuest/WoW/D&D archetypes. Sprout Knight
  (melee/tank) is the confirmed first hero; further roster classes are TBD, each contributing one
  unique gameplay mechanic. Mechanics are not ironed out yet — pipeline item, not blocking Phase 2.
- **Grouping** (small-team stage clearing) and **Raids** (full-roster fight, roster power sums all
  hero progress) — both planned pipeline systems layered on top of the roster, intended as an
  anti-neglect incentive. Architecture must not preclude either.
- **Hero unlock cadence:** hero 1 at start; further heroes unlock on some cadence (earlier draft:
  hero 2 ~stage 25, hero 3 at first prestige) — needs reconfirming now that progression is
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
- **Hero/stage simulation:** only the actively-focused hero receives full-rate progression;
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
   the title/subtitle entirely — the game isn't purely idle (active per-hero stage focus), and
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
5. **Economy model needs a full re-sim (added 2026-07-17).** `ff_sim.py`'s validated numbers
   (`BOSS_EVERY=5`, HP/coin curves) assumed a single boss tier and no FARM-resource/gem economy at
   all — both invalidated by Decisions 4 and 9. Derek's own read: the original constants were
   fairly arbitrary and didn't account for cross-system scaling, so this isn't a quick patch —
   it needs real modeling of the three-currency economy (coin/resources/gems) before Phase 2
   architecture leans on any specific numbers. In progress.
6. **Miniboss loot (every 10 stages):** assumed to drop no gear loot, unlike the boss (every 25) —
   not yet confirmed with Derek.
7. **Gear stat specifics:** equipped gear "grants small combat buffs" per Decision 4 — exact stats
   and scaling not designed yet ("explore later," Derek's words).
8. **Does the Gems currency persist through a prestige reset?** Framed as "meta progression"
   (Decision 9), which strongly implies yes (genre convention: Clicker Heroes rubies survive
   resets), but this isn't explicitly stated anywhere — Decision 7's persists-through-prestige list
   (skill XP, mastery, stockpiles, buildings) doesn't mention gems either way.

~~Former #5, terminology reconciliation (floor/stage/lane)~~ — **resolved 2026-07-17, see Decision 4.**
