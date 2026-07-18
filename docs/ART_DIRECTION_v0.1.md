# Farm Farmer — Art Direction & Visual Structure v0.1

**Date:** July 17, 2026
**Decision 8 (locked, Derek-specified):** Layered-backdrop visual structure. No full farm-world sim at 1.0.
**Authorship note:** this document is direction and scope only. Every shipped asset is human-drawn per the asset authorship policy. All names below are placeholder direction for Derek's rewrite.

---

## 1. The three visual layers

**Combat (top of portrait screen):** one backdrop per *region* — a 10-stage bracket, Clicker Heroes style. Stages 1–100 = 10 regions. Past 100, regions cycle with palette-shift variants on the same cadence as Blight enemy remixes, so world and bestiary escalate together.

**Skills management (Estate tab):** a single backdrop behind Melvor-style skill rows — Woodworker / Miner / Gatherer, each row showing level, current node, action progress, stockpile. Rows are the information layer; they stay uniform and readable.

**Generator detail (tap a row):** each skill opens its own scene — the Woodworker's grove, the Miner's quarry, the Gatherer's herb garden. This is where visual personality lives: the NPC at work, the current node visible, action feedback.

**Proposed, not locked:** farm buildings render as prop overlays *inside* the generator scenes (build the Sawmill → it appears in the grove). Three scenes that visibly accumulate structures across the six-week estate arc delivers the farm-rebuilding fantasy at bounded cost. Alternative if rejected: buildings as cards on a fourth tab.

## 2. Regions (worldbuilding skeleton)

Each region = 10 stages, themed backdrop, themed enemy subset, region boss at stage ×10 (the every-5th-stage boss cadence means bosses at ×5 are "lieutenants," ×10 are region bosses). Placeholder ladder for Derek's pass:

| Region | Stages | Direction sketch |
|---|---|---|
| 1 | 1–10 | Seedling rows — tutorial ground, worm fodder |
| 2 | 11–20 | Vegetable patch proper — carrot rogues |
| 3 | 21–30 | Orchard edge |
| 4 | 31–40 | Beehive meadow |
| 5 | 41–50 | Compost heap — where the Rot began |
| 6 | 51–60 | Flooded irrigation ditches |
| 7 | 61–70 | Greenhouse ruins (run-1 wall territory — make it ominous) |
| 8 | 71–80 | Root cellar mouth |
| 9 | 81–90 | The deep cellar |
| 10 | 91–100 | Rot heartland (run-2 wall territory) |

Design rule: region themes should telegraph difficulty escalation at a glance — the world literally darkens and rots as stages climb.

## 3. Asset ledger (1.0 commission list)

| Asset | Count | Notes |
|---|---|---|
| Combat region backdrops | 10 | Portrait, top-half composition; safe zone for enemy + hero sprites |
| Palette-shift variants | 0 drawn | Runtime tint/grade of the 10 — engineering, not art |
| Skills management backdrop | 1 | Calm, recedes behind UI rows |
| Generator detail scenes | 3 | Grove, quarry, herb garden; NPC-at-work focal point |
| Building prop overlays | ~9 (2–4 per scene) | If overlay proposal is adopted |
| Resource tier icons | 21 | 3 skills × 7 tiers |
| NPC characters | 3 | Woodworker, Miner, Gatherer |
| Hero sprites | 3 | Sprout Knight + Hero 2 + Hero 3, with attack/idle/hit animation states |
| Enemy roster | ~12–15 base + Blight recolors | Distributed across regions |

Roughly **14 backdrop paintings, 6 characters + enemies, ~30 icons/props** — a finite, commissionable scope. This ledger is the input for contractor quotes in Phase 4.

## 4. UI navigation implication

Bottom nav at 1.0: **Combat · Estate · Menu** (three items, thumb zone). Skills management and generator scenes both live under Estate; if the building-overlay proposal is adopted, no fourth tab is needed. Fewer tabs = the portrait one-hand pillar holds.

## 5. Answers on the board

- Open question "visible farm vs stat-only" — **closed** by this structure.
- Region framework closes "how do enemies distribute" from the concept roster.
- Still open: tap-relevance curve duration; Hero 2/3 identity (blocks hero sprite briefs in this ledger).
