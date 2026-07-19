# Farm Farmer — Economy Model Notes v0.2

**Date:** July 19, 2026
**Method:** Combat side — `sim/ff_sim.py` v0.3 (three-currency, two-tier boss cadence, roster
model with focused/background split, prestige runs), 24 simulated hours per run at 30-second
ticks. Farm side — `sim/ff_farm_sim.py` v0.1 (skill XP + building spend track), 70 simulated
days at 5-minute ticks. **No Python exists on the dev machine; every number below was produced
by the PowerShell ports** (`sim/ff_sim_port.ps1`, `sim/ff_farm_sim_port.ps1`), executed
2026-07-19. The `.py` files are the readable spec; the `.ps1` files are the executable of
record. Change one, change both.

> **Status caveat:** as of this writing the v0.3 sim files are still uncommitted working-tree
> changes pending Derek's read-through (CLAUDE.md session TODO #1). Everything below was
> re-executed and verified against those files as they stand — the headline findings
> (prestige stall, background-rate sensitivity) reproduce exactly.

---

## Finding 1 — Additive Seed bonus dies structurally; compounding lives

Decision 7 locks "+15% global DPS per Seed" but the wording is ambiguous between
`1 + 0.15·seeds` (additive) and `1.15^seeds` (compounding). The sim says this is not a
nuance — it decides whether the prestige loop survives:

**As-found sequence (each run holding only the previous run's payout):**

| Run | Additive: held → earned | Compounding: held → earned |
|---|---|---|
| 1 | 0 → **9** (depth 66) | 0 → **9** (depth 66) |
| 2 | 9 → **13** (depth 75) | 9 → **15** (depth 80) |
| 3 | 13 → **14** (depth 75) | 15 → **20** |
| 4 | 14 → **14** (depth 75) — flat | 20 → **24** (depth 100) |

**Cumulative-bank sequence (Seeds accumulate, which is how `PrestigeService` now works):**

| Run | Additive: bank → earned (depth) | Compounding: bank → earned (depth) |
|---|---|---|
| 1 | 0 → 9 (66) | 0 → 9 (66) |
| 2 | 9 → 13 (75) | 9 → 15 (80) |
| 3 | 22 → 16 (**80**, +5 stages) | 24 → 24 (**102**, +22 stages) |

Banking softens the additive stall but can't fix it: seed income is roughly linear per run
while difficulty is exponential (HP ×1.55/stage), so additive depth gains shrink every run
(+9, +5, …) where compounding gains *grow* (+14, +22, …). This is structural, not a tuning
miss — no value of the additive percentage changes the shape, only how many runs the decay
takes.

**Recommendation: compounding (`×1.15^Seeds`), Clicker Heroes structure.** The game code now
carries both readings behind `CombatBalanceDefinition.seedBonusCompounding` (default `false`
= literal locked wording), so Derek's call is an Inspector checkbox, not a code change.

## Finding 2 — Gem arrival is hostage to the Seed decision

Gems drop every 100 stages (Decision 9). Day-one depth is 66, so **no gems exist on day
one** under any setting. Downstream:

- **Compounding:** run 3 (cumulative bank) clears stage 100 at hour ~23.8 — the first gem
  lands on roughly the player's third or fourth day. Reasonable meta-currency pacing.
- **Additive:** run 3 tops out at stage 80. Stage 100 needs another ~1.55^20 ≈ 6,400× of
  relative power — many runs of decaying gains away. The first gem is effectively unreachable
  on any onboarding-relevant timescale.

If the additive reading wins anyway, `GEM_EVERY=100` must drop (or the whole gem cadence
re-derives) — the two knobs cannot be tuned independently.

## Finding 3 — BACKGROUND_RATE_FRACTION is first-order, not a detail

Sweep of the unfocused-hero rate (24h day one, all else locked):

| Rate | Day-one depth (lead hero) | Seeds earned |
|---|---|---|
| 0.05 | 63 | 8 |
| 0.10 | 66 | 9 |
| 0.25 | 70 | 12 |

A "minor" background-rate tweak moves day-one seed income ±50%. Two design consequences:

1. This number must be tuned *with* the prestige pacing, not after it.
2. The sim's focus policy (focus the weakest hero) lets a background hero out-push the
   focused one at higher rates — the intended play pattern inverts. The rate formula
   (Open Question #1) needs a real design pass before this constant means anything.

## Finding 4 — Farm economy pacing and the cross-feed sizing

`ff_farm_sim.py` v0.1 (building-track proposal on top of the locked skill curves):

- Skill level 99: day ~48. Tier unlocks land at days 0 / 0.02 / 0.12 / 0.6 / 2.3 / 8.7 / 22.4
  — early tiers cascade, late tiers are genuine long-arc goals.
- Building track (cost `50 × 1.4^(n−1)` units, tier-gated every 4 levels): level 25 of 25 at
  day ~53. Levels go from minutes (B1–B8 inside day one) to weeks (B24→B25 ≈ 17 days) —
  Adventure Capitalist cadence against linear resource income, as intended.
- Fully built estate: **+150% global DPS** (25 levels × 2%/level × 3 skills). Worth ≈ 6.6
  Seeds under compounding (ln 2.5 / ln 1.15) — inside the "mid-single-digit Seeds,
  meaningful-not-dominant" target. Under additive it's ≈ 10 Seeds, i.e. more than a full
  prestige run — another point against additive.
- Day-one scrap inflow (4 boss clears): 1,200 T1 + 900 T2 units. All four T1 building
  levels cost 355 units combined — the bridge singlehandedly funds the early building track,
  then fades to marginal as costs go geometric. That's the right shape for a bridge that must
  never become the farm's main income (Decision 9).

## Gem sink proposal (unblocks gem-amount tuning — session TODO #3)

Recommendation: **an Ancients-style persistent upgrade tree** ("the Grove" as a working name —
deliberately not "Harvest"-anything, see Open Q #4), over ruby-style one-off QoL purchases:

- Fits "meta-progression currency" (Decision 9) — permanent, build-defining choices rather
  than consumable conveniences, and it settles Open Q #8 the way genre convention already
  leans (gems persist through prestige; the game code now assumes this, flagged).
- Node effects should touch *both* halves of the game (e.g. +background hero rate, +skill
  action speed, +boss timer seconds, +scrap yield) so the meta currency is the one place the
  two economies meet as a player choice.
- Costs geometric per node level, first node priced ≈ 3–5 gems so the first purchase lands
  within a week of the first drop (see Finding 2 pacing).
- Rewarded-ad and IAP hooks (Decision 2) then have a clean, non-pay-to-win surface: sell/grant
  *time*, never Grove levels.

Until Derek reacts to a sink shape, `GEMS_PER_DROP` stays untunable; only arrival timing is
real. Nothing above is locked.

## What the game code now implements (2026-07-19 pass)

Implemented against these findings, all numbers as flagged placeholders on
`CombatBalanceDefinition`: miniboss/boss wall timers (15s/30s, mirroring the sim) with
knock-back-one-stage on expiry (Decision 4); gem drops every 100 stages into a persistent
gems wallet; `PrestigeService` ("clears the battlefield, never the estate") with the seed
payout formula and the additive/compounding toggle; coin-wallet save persistence (bug fix —
it previously zeroed on every restart). EditMode tests pin the formulas to this doc's numbers.

## Known gaps carried forward

- Seed payout summing across the roster is still a sim assumption, not a decision.
- Miniboss loot (Open Q #6), gear stats (Open Q #7), mastery accrual rate: still unspecified;
  none are modeled.
- Coin component of building costs is not modeled; it interacts with the pre-prestige spend
  ritual and needs Derek's intent first.
- The focus policy inversion (Finding 3) means all roster-level pacing numbers here are
  policy-dependent; re-run after the background-rate formula is designed.
