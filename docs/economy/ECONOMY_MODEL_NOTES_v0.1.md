# Farm Farmer — Economy Model Notes v0.1

**Date:** July 17, 2026
**Method:** Greedy-allocator player simulation (`sim/ff_sim.py`), 24 simulated hours at 30-second ticks, two lanes, shared wallet. Constants selected by parameter sweep (~100 simulated player-days) scored against pacing targets. Workbook: `FF_ECONOMY_MODEL_v0.1.xlsx` (live formulas; blue-on-yellow cells are the design levers).

---

## Findings that changed the design

1. **Hero level 1 must be a gift.** A level-0 hero has zero DPS, zero income, and the economy never starts. The tutorial's first "purchase" is free (or the hero ships at level 1). This is now a hard requirement for the Phase 3 tutorial spec.

2. **Coin must scale WITH enemy HP, and pace must be governed by costs.** We tested the intuitive alternative — coin growing slower than HP (1.15× vs 1.55×) — and it produces a *hard ceiling*, not a soft wall: once the farm stage caps, income freezes and progression stops dead (stage 65 forever). With coin ∝ HP, income scales with DPS, progression stays alive indefinitely, and the wall pace is controlled entirely by `COST_GROWTH` vs the milestone multiplier. **`COST_GROWTH` is the single most sensitive number in the game:** 1.07 produces runaway (stage 110 in a day), 1.085 is brutal. 1.075 is locked.

3. **`LEVEL_COST0` is the first-hour throttle.** At 8, hour one burned 60% of day-one depth (stage 50+). At 40, hour one lands at stage ~28–30 — fast enough for onboarding dopamine, slow enough that day one still has a journey. It only affects the early game; the late curve is insensitive to it.

4. **Hero 2's mults make catch-up a feature.** DPS ×40 / cost ×120 means his DPS-per-coin overtakes lane 1 at scale: he rockets from stage 1 to parity (~stage 50) in ~3 hours of wall time. Watching the new lane tear through content lane 1 crawled over is the emotional payoff of the parallel-lane decision. Preserve the ratio `dps_mult > cost_mult^(log-relative)` for every future hero.

## Locked v0.1 constants

| Lever | Value | Role |
|---|---|---|
| HP0 / HP_GROWTH | 10 / **1.55×** per stage | Enemy HP curve |
| Boss | every 5th, HP ×8, 30 s timer, coin ×10 | Progression gate |
| Coin per kill | HP ÷ **25** | Income (scales with HP — see finding 2) |
| LEVEL_COST0 / COST_GROWTH | **40** / **1.075×** per level | First-hour throttle / wall pace |
| DPS0, milestones | 5, ×4 every 25 levels | Master hero curve |
| Hero 2 | DPS ×40, cost ×120, gate L1-stage 25, price 100,000 coins | Lane 2 modifier set |
| Harvest Seeds | floor((maxStage−40)/5) per lane, +5% global DPS each | Prestige payout |

## The simulated Day One (validated milestones)

| Moment | Time |
|---|---|
| Lane 1 stage 10 | 15 min |
| Hero 2 unlock (stage-25 gate + 100k coins) | ~56 min |
| Lane 1 stage 50 | 3.75 h |
| Lane 2 catches lane 1 (~stage 50) | 4 h |
| Soft wall zone (stage 65) | ~10.75 h |
| **First Harvest recommended** (progress < 1 stage/h) | **~10 h** |
| First Harvest payout (both lanes ~70) | **12 Seeds → +60% run-2 DPS** |

The +60% restart bonus means run 2 reaches the run-1 wall in roughly a third of the time and pushes past it — the compounding the genre lives on.

## Known simplifications (v0.2 backlog)

- No tap damage (adds active-play burst; will pull early milestones ~10–20% sooner, not change the shape).
- No offline-cap modeling, no rewarded-ad boosts (both are multipliers on the same curves; layer into the sim before pricing Time Skips in Phase 5).
- Boss "retry" is modeled as farm-until-DPS-suffices; real knockdown-one-stage behavior is equivalent at this resolution.
- Hero 3 (unlocks at first Harvest) not yet simulated — needs run-2 simulation with Seed bonuses. Next sim milestone.
- Generators (Wood/Stone/Herbs) are resource-side, not coin-side: intentionally out of scope here. The passive-vs-trainable decision (open question 1) gets its own model.

## Phase 2 implications

The grey-box prototype should hardcode exactly these constants and nothing else. If the prototype's first hour doesn't feel like the simulated first hour, the *sim* is wrong and gets re-tuned against reality — the spreadsheet serves the game, never the reverse.
