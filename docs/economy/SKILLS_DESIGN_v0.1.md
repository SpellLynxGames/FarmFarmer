# Farm Farmer — Skills System Design v0.1

**Date:** July 17, 2026
**Decision:** Generators are full trainable skills, Melvor-style (Decision 7, locked). Sim source: `sim/ff_skill_sim.py`.

---

## 1. The core reframe: NPCs are the skill

Melvor's constraint is that the *player* trains one skill at a time. Farm Farmer sidesteps it through fiction: each skill is embodied by a hired NPC — the Woodworker, the Miner, the Gatherer — and hired workers work *all the time, in parallel*, exactly like the combat lanes. The player's Melvor-style decisions are:

1. **Node assignment** — which resource node the NPC works (higher tier = better resource + more XP, slower actions).
2. **Mastery investment** — where the per-node long grind pays out.

Each skill gets its own tab. Portrait UI implication: bottom navigation grows to Combat / Skills / Farm (+ menu). This is the moment the UI stops being a single screen — flag for the Phase 2 layout comp.

## 2. The two-arc structure (Harvest persistence ruling)

**The Harvest resets the battlefield, never the estate.**

| Resets at Harvest | Persists across seasons |
|---|---|
| Combat lane stages | Skill XP and levels |
| Hero levels | Mastery progress |
| Coin wallet | Resource stockpiles |
| — | Farm buildings and their bonuses |

Rationale: Melvor players never lose skill progress, and thematically you reap the crops — the farm remains. This gives the game a fast arc (prestige runs, hours-to-days) inside a slow arc (the estate, weeks), which is the retention structure the whole genre chases.

## 3. Skill math (sim-tuned, locked v0.1)

**XP curve:** XP to next level = `40 × 1.12^level`, cap 99. Same curve for all three skills (master-curve rule extends to the estate side).

**Tier gates and action table** (per skill; identical shape, different resources):

| Tier | Unlock lvl | Action time | XP/action | Sim arrival (always-on) |
|---|---|---|---|---|
| T1 | 1 | 3.0 s | 2 | Day 0 (tutorial) |
| T2 | 15 | 4.0 s | 5 | ~40 min |
| T3 | 32 | 5.0 s | 10 | ~3 h |
| T4 | 50 | 6.0 s | 18 | Day 0.6 |
| T5 | 65 | 7.0 s | 30 | Day 2.3 |
| T6 | 80 | 8.0 s | 48 | Day 8.6 |
| T7 | 91 | 9.0 s | 70 | Day 22 |

**Level pacing (validated):** L30 ≈ 2.5 h · L50 ≈ day 0.6 · L75 ≈ day 5 · L90 ≈ day 20 · **L99 ≈ day 44**. Three unlocks land inside day one (onboarding gratification), then roughly one tier per engagement horizon: day 2, week 2, week 3 — then the 91→99 climb is the six-week crown.

**Resource output:** 1 unit of the tier's resource per action. Higher tiers produce the resources higher farm buildings demand — tier gating *is* building gating.

**Mastery (v1.0-lite):** per-node mastery levels 1–50, earned alongside XP while working that node. +1% action speed per 5 mastery levels; +0.5% double-yield chance per mastery level. Deeper mastery checkpoints (Melvor-style per-node perks) are a LiveOps expansion slot, designed but not built at 1.0.

**Node naming:** all tiers ship as placeholders (T1–T7) in data. Flavor direction for Derek's pass — Woodworking tiers might run sprig → fencepost pine → orchard bough → ironbark; per the authorship policy every shipped name is yours.

## 4. Economy coupling (what resources buy)

Resources + Coin buy **farm buildings** — the permanent-bonus layer that persists through Harvest. Because Coin *resets* at Harvest while resources persist, building purchases naturally become the "spend before you reap" decision at the end of each season: a built-in pre-prestige ritual. Building costs and bonus tables are the next economy deliverable (needs the visible-farm decision, open question 2, since a visible farm makes buildings the art centerpiece).

## 5. Companion finding: Harvest Seed bonus repriced

The run-2 combat simulation (same session) found the +5%-per-Seed bonus fatally weak: run 2 needed 11 h to match run 1's wall and gained +5 stages — a dead prestige loop that would surface as a D2 retention cliff. **Locked at +15% per Seed:** run 2 reaches the old wall in 3 h (~30% of run-1 time) and pushes +20 stages. Workbook updated. When the prestige *tree* arrives (v0.2), raw stacking converts to spent upgrades at equivalent aggregate power.

## 6. Scope impact (honest ledger)

- Phase 3 grows ~3–4 weeks: three skill tabs, XP/mastery systems, node-assignment UI, tier data.
- Art budget grows: 21 resource icons (3 skills × 7 tiers) + 3 NPC characters + skill tab iconography.
- Save schema: per-skill XP/mastery/stockpile blocks, persistent across the Harvest reset boundary — schema design lands in Phase 2 even though skills build in Phase 3.
- Roadmap end-to-end estimate moves from ~9–12 months to **~10–13 months** at part-time pace. The estate arc is the best retention investment in the design; it earns the weeks.
