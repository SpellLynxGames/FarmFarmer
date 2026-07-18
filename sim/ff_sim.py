"""Farm Farmer economy simulator v0.2 -- DRAFT, unvalidated.
Master-curve model (Decision 6): every hero shares curve shapes, differs by mults.

v0.2 changes from the locked v0.1 numbers (see CLAUDE.md Open Question #5):
  - Roster/focus model replaces parallel lanes: only the focused hero gets full-rate
    progression; unfocused unlocked heroes accrue at BACKGROUND_RATE_FRACTION (placeholder,
    formula TBD -- CLAUDE.md Open Question #1).
  - Single BOSS_EVERY=5 replaced by two tiers: miniboss every 10 stages (no loot), boss
    every 25 (drops gear loot, scrapped here into an abstract FARM-resource value).
  - Gems added: dropped every 100 stages, tracked as a separate meta-progression currency,
    not spent anywhere in this sim.

None of the new constants below (BACKGROUND_RATE_FRACTION, MINIBOSS_*, SCRAP_VALUE_PER_BOSS,
GEMS_PER_DROP) are sim-validated -- they're structural placeholders so the *shape* of the
economy is correct while the actual numbers get tuned. This does NOT model The Farm/resource
economy as its own system (Derek wants that to be minigame-depth in its own right); it only
tracks the combat-side inflow into it via loot scrapping.
"""
import math, json, sys

# ---- Tunable constants (these become the workbook's blue cells) ----
P = dict(
    HP0=10.0, HP_GROWTH=1.55,          # enemy HP curve
    MINIBOSS_EVERY=10, MINIBOSS_HP_MULT=3.0, MINIBOSS_TIMER=15.0,
    BOSS_EVERY=25, BOSS_HP_MULT=8.0, BOSS_TIMER=30.0,
    KILLS_PER_STAGE=10,
    COIN0=1.5, COIN_GROWTH=1.15, MINIBOSS_COIN_MULT=4.0, BOSS_COIN_MULT=10.0,
    LEVEL_COST0=8.0, COST_GROWTH=1.07,  # hero level cost curve
    DPS0=5.0, MILESTONE_EVERY=25, MILESTONE_MULT=4.0,
    FARM_TTK_CAP=30.0,                  # farm the deepest stage with TTK <= cap
    BACKGROUND_RATE_FRACTION=0.1,       # PLACEHOLDER -- unfocused hero's rate vs. focused
    SCRAP_VALUE_PER_BOSS=5.0,           # PLACEHOLDER -- avg FARM-resource value of scrapped loot
    GEMS_PER_DROP=1.0,                  # PLACEHOLDER -- gems per every-100-stage drop
    SIM_HOURS=24, DT=30.0,              # sim step seconds
)
HEROES = [
    dict(name="Sprout Knight", dps_mult=1.0,  cost_mult=1.0,   unlock_stage=0,  unlock_cost=0.0),
    dict(name="Hero 2 (ranged)", dps_mult=12.0, cost_mult=150.0, unlock_stage=25, unlock_cost=2.0e4),
]

def hp(s): return P["HP0"] * P["HP_GROWTH"] ** (s - 1)
def is_boss(s): return s % P["BOSS_EVERY"] == 0
def is_miniboss(s): return not is_boss(s) and s % P["MINIBOSS_EVERY"] == 0
def is_gem_stage(s): return s % 100 == 0

def stage_hp(s):
    if is_boss(s): return hp(s) * P["BOSS_HP_MULT"]
    if is_miniboss(s): return hp(s) * P["MINIBOSS_HP_MULT"]
    return hp(s)

def coin_per_kill(s):
    c = P["COIN0"] * P["COIN_GROWTH"] ** (s - 1)
    if is_boss(s): return c * P["BOSS_COIN_MULT"]
    if is_miniboss(s): return c * P["MINIBOSS_COIN_MULT"]
    return c

def wall_timer(s):
    if is_boss(s): return P["BOSS_TIMER"]
    if is_miniboss(s): return P["MINIBOSS_TIMER"]
    return float("inf")

def dps(h, lvl):
    if lvl <= 0: return 0.0
    return P["DPS0"] * h["dps_mult"] * lvl * P["MILESTONE_MULT"] ** (lvl // P["MILESTONE_EVERY"])

def level_cost(h, lvl):  # cost to buy level lvl (1-indexed)
    return P["LEVEL_COST0"] * h["cost_mult"] * P["COST_GROWTH"] ** lvl

class Hero:
    def __init__(self, hero_def):
        self.unlocked = hero_def["unlock_stage"] == 0
        self.h = hero_def; self.lvl = 1 if self.unlocked else 0  # first level gifted (tutorial)
        self.stage = 1; self.max_stage = 1; self.kills = 0.0

    def ttk(self, s):
        d = dps(self.h, self.lvl)
        return stage_hp(s) / d if d > 0 else float("inf")

    def farm_stage(self):
        # deepest cleared-or-current stage with TTK <= cap (never below 1)
        s = min(self.stage, self.max_stage)
        while s > 1 and self.ttk(s) > P["FARM_TTK_CAP"]:
            s -= 1
        return s

    def income_rate(self):  # coins/sec at farm stage
        s = self.farm_stage(); t = self.ttk(s)
        return coin_per_kill(s) / t if t < float("inf") else 0.0

    def try_advance(self, dt):
        # push current stage if killable; miniboss/boss gated by their own timer wall
        s = self.stage
        t = self.ttk(s)
        if t == float("inf"): return 0.0, 0
        timer = wall_timer(s)
        if t > timer: return 0.0, 0  # walled: farm instead of advancing
        kills = dt / t
        need = 1 if (is_boss(s) or is_miniboss(s)) else P["KILLS_PER_STAGE"]
        self.kills += kills
        scrap_gained = 0.0
        gems_gained = 0
        while self.kills >= need:
            self.kills -= need
            cleared_stage = self.stage
            if is_boss(cleared_stage): scrap_gained += P["SCRAP_VALUE_PER_BOSS"]
            if is_gem_stage(cleared_stage): gems_gained += P["GEMS_PER_DROP"]
            self.stage += 1
            self.max_stage = max(self.max_stage, self.stage)
            need = 1 if (is_boss(self.stage) or is_miniboss(self.stage)) else P["KILLS_PER_STAGE"]
            t = self.ttk(self.stage)
            timer = wall_timer(self.stage)
            if t == float("inf") or t > timer:
                self.kills = 0.0; break
        return scrap_gained, gems_gained

def choose_focus(heroes):
    # Simplifying assumption: focus whichever unlocked hero is least progressed, catching the
    # newest/weakest hero up rather than optimally snowballing one hero. Not a claim about
    # actual player behavior -- just a deterministic policy to simulate against.
    unlocked = [h for h in heroes if h.unlocked]
    return min(unlocked, key=lambda h: h.max_stage) if unlocked else None

def simulate(log_every=120):
    heroes = [Hero(h) for h in HEROES]
    wallet = 0.0; farm_resources = 0.0; gems = 0
    t = 0.0; end = P["SIM_HOURS"] * 3600
    events, series = [], []
    while t < end:
        dt = P["DT"]
        # unlock check (gated on the focused/lead hero's depth, same as before)
        lead = max((h.max_stage for h in heroes), default=1)
        for h in heroes:
            if not h.unlocked and lead >= h.h["unlock_stage"] and wallet >= h.h["unlock_cost"]:
                wallet -= h.h["unlock_cost"]; h.unlocked = True; h.lvl = 1
                events.append((t / 3600, f"{h.h['name']} unlocked"))

        focused = choose_focus(heroes)

        # income: focused hero full rate, unfocused unlocked heroes at background rate
        for h in heroes:
            if not h.unlocked: continue
            rate = 1.0 if h is focused else P["BACKGROUND_RATE_FRACTION"]
            wallet += h.income_rate() * dt * rate

        # greedy allocator: repeatedly buy globally cheapest next level across the roster
        while True:
            options = [(level_cost(h.h, h.lvl + 1), h) for h in heroes if h.unlocked]
            options.sort(key=lambda x: x[0])
            if not options or options[0][0] > wallet: break
            cost, h = options[0]; wallet -= cost; h.lvl += 1

        # advance: focused hero full dt, unfocused unlocked heroes at background rate
        for h in heroes:
            if not h.unlocked: continue
            rate = 1.0 if h is focused else P["BACKGROUND_RATE_FRACTION"]
            scrap, gm = h.try_advance(dt * rate)
            farm_resources += scrap; gems += gm

        if int(t) % (log_every * 60) < dt:
            series.append(dict(hr=round(t / 3600, 2), wallet=wallet, farm_resources=farm_resources,
                                gems=gems, stages=[h.max_stage for h in heroes],
                                lvls=[h.lvl for h in heroes]))
        t += dt
    return heroes, wallet, farm_resources, gems, events, series

if __name__ == "__main__":
    heroes, wallet, farm_resources, gems, events, series = simulate(
        log_every=int(sys.argv[1]) if len(sys.argv) > 1 else 120)
    print(json.dumps(dict(events=events, series=series,
        final=dict(stages=[h.max_stage for h in heroes], lvls=[h.lvl for h in heroes],
                   wallet=wallet, farm_resources_from_scrap=farm_resources, gems=gems)),
        indent=1))
