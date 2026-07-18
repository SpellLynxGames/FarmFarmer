# Scene & UI Architecture — v0.1 (proposal)

**Version:** v0.1 — 2026-07-18
**Status:** DRAFT PROPOSAL, not locked. Nothing in this doc overrides `CLAUDE.md`; where this doc
makes a call `CLAUDE.md` left open (mainly: is a "scene" per Decision 8 a literal Unity scene or an
in-scene panel?), it's flagged explicitly below and needs Derek's confirmation before code leans on
it. This exists to give scene/UI code something concrete to build against instead of guessing
per-feature.

---

## 1. What's actually decided already (from CLAUDE.md)

- `GameStateMachine`/`IGameState` drives app flow: Boot → MainMenu → Gameplay → Cutscene.
- Bottom nav inside Gameplay: **Combat / Estate / Menu** (Decision 8).
- Backdrops change per stage (Combat) and Estate has one shared management-view backdrop with
  Melvor-style rows, each generator opening its own detail "scene" — wording deliberately left
  ambiguous between a literal Unity scene and a full-screen view (Decision 8).
- Canvas: Screen Space Overlay, Scale With Screen Size, 1080×1920 reference resolution, portrait
  only (Conventions section).
- Farm buildings appear as prop overlays inside generator scenes, visibly upgrading with generator
  level (Decision 8, locked 2026-07-17).

## 2. Proposal: Unity scene vs. in-scene panel

**Recommendation: minimize actual `SceneManager` scene loads.** Mobile scene loads cause visible
hitches; an idle/incremental game lives or dies on snappy navigation between Combat/Estate/generator
views, which players hit constantly. Concretely:

| App-flow state | Unity scene? | Notes |
|---|---|---|
| Boot | Dedicated minimal scene (index 0 in Build Settings) | Just enough to show a loading indicator while `BootState` loads the save and hydrates `WalletService`. Currently `SampleScene.unity` occupies this slot and needs repurposing/renaming. |
| MainMenu | Separate real scene | Infrequent transitions in/out (once per app launch, basically), no snappiness pressure. |
| Gameplay | **One persistent scene** for the whole Combat/Estate/Menu experience | Bottom nav + a content root that swaps **panels** (`SetActive`, not scene loads) for Combat / Estate / Menu / generator-detail / hero-roster / gear-inventory. This is the part of Decision 8 that was left ambiguous — proposing panels, not literal scenes, for every generator detail view. |
| Cutscene | Additive overlay on top of Gameplay | Load additively, don't replace Gameplay — resume state cleanly when it unloads (hero-unlock moments, prestige animation). |

**This needs your explicit confirmation**, since Decision 8's text ("each generator opens its own
detail scene") could genuinely have meant a literal Unity scene per generator (heavier, but gives
each one true scene-level isolation for lighting/backdrop work). The panel approach can still give
each generator its own unique backdrop (Decision 8) via a per-generator background sprite/prefab
swapped into the panel — it just avoids the scene-load hitch.

## 3. Screen inventory

| Screen / panel | Parent | Contents (first pass) | Status |
|---|---|---|---|
| Boot / loading | Boot scene | Splash + loading indicator while save loads | Not built |
| Main Menu | MainMenu scene | Play button, settings/legal entry point | Not built |
| Combat | Gameplay → Combat panel | Combat viewport (enemy, hero, tap target), upgrade/shop strip, hero-switcher control | Not built |
| Estate | Gameplay → Estate panel | Melvor-style scrollable generator rows, shared management backdrop | Not built |
| Generator detail | Gameplay → generator-detail panel | Per-generator backdrop w/ farm-building prop overlays, resource-tier progress, mastery | Not built |
| Hero roster / switch | Gameplay → overlay or Combat sub-panel | List of unlocked heroes, tap to focus; shows background-progress indicator for unfocused heroes | Not built |
| Hero gear / inventory | Gameplay → overlay from Combat or roster view | ~5-slot inventory (Decision 4), equip/scrap actions | Not built |
| Menu | Gameplay → Menu panel | Settings, IAP/shop, prestige entry point | Not built |
| Prestige confirmation | Gameplay → overlay from Menu | Shows projected Seed payout, confirm/cancel reset | Not built |
| Cutscene | Additive overlay scene | Hero unlock / prestige narrative beats | Not built |

## 4. Combat layout proposal (thumb-zone framing)

Carried over from the original design-office pitch for this project, not yet re-confirmed in this
repo, so treat as proposal: **top ~55-60% of the portrait screen is the combat viewport** (enemy,
hero, floor/stage HUD); **bottom ~40-45% is the thumb zone** — upgrade list, hero-switcher, tap
button — mirroring the "Clicker Heroes desktop layout rotated 90°" reasoning: every actionable tap
target sits where a one-handed grip's thumb naturally rests. Needs your confirmation before it
drives actual Canvas layout work.

## 5. Open items this doc surfaces (not yet in CLAUDE.md's Open Questions — pending your call)

1. Confirm literal-scene vs. panel approach for generator details (Section 2).
2. Confirm the Combat viewport/thumb-zone split (Section 4), or provide a different layout intent.
3. Cutscene scope: what actually triggers `CutsceneState` today? (Hero unlock? Prestige? Both?
   Neither yet designed.)
4. Hero roster/switch UI: does switching focus require a confirmation step, or is it instant
   tap-to-focus? Affects whether background-hero progress needs a visible "away" indicator.
