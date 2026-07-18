"""Farm Farmer economy simulator v0.1
Master-curve model (Decision 6): every hero shares curve shapes, differs by mults.
Greedy allocator bot manages a shared wallet across parallel lanes for SIM_HOURS.
"""
import math, json, sys

# ---- Tunable constants (these become the workbook's blue cells) ----
P = dict(
    HP0=10.0, HP_GROWTH=1.55,          # enemy HP curve
    BOSS_EVERY=5, BOSS_HP_MULT=8.0, BOSS_TIMER=30.0,
    KILLS_PER_STAGE=10,
    COIN0=1.5, COIN_GROWTH=1.15, BOSS_COIN_MULT=10.0,  # coin curve grows SLOWER than HP: this gap creates walls
    LEVEL_COST0=8.0, COST_GROWTH=1.07,  # hero level cost curve
    DPS0=5.0, MILESTONE_EVERY=25, MILESTONE_MULT=4.0,
    FARM_TTK_CAP=30.0,                  # farm the deepest stage with TTK <= cap
    SIM_HOURS=24, DT=30.0,              # sim step seconds
)
HEROES = [
    dict(name="Sprout Knight", dps_mult=1.0,  cost_mult=1.0,   unlock_stage=0,  unlock_cost=0.0),
    dict(name="Hero 2 (ranged)", dps_mult=12.0, cost_mult=150.0, unlock_stage=25, unlock_cost=2.0e4),
]

def hp(s): return P["HP0"] * P["HP_GROWTH"] ** (s - 1)
def is_boss(s): return s % P["BOSS_EVERY"] == 0
def stage_hp(s): return hp(s) * (P["BOSS_HP_MULT"] if is_boss(s) else 1.0)
def coin_per_kill(s):
    c = P["COIN0"] * P["COIN_GROWTH"] ** (s - 1)
    return c * (P["BOSS_COIN_MULT"] if is_boss(s) else 1.0)
def dps(h, lvl):
    if lvl <= 0: return 0.0
    return P["DPS0"] * h["dps_mult"] * lvl * P["MILESTONE_MULT"] ** (lvl // P["MILESTONE_EVERY"])
def level_cost(h, lvl):  # cost to buy level lvl (1-indexed)
    return P["LEVEL_COST0"] * h["cost_mult"] * P["COST_GROWTH"] ** lvl

class Lane:
    def __init__(self, hero):
        self.unlocked = hero["unlock_stage"] == 0
        self.h = hero; self.lvl = 1 if self.unlocked else 0  # first level gifted (tutorial)
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
    def income(self):  # coins/sec at farm stage
        s = self.farm_stage(); t = self.ttk(s)
        return coin_per_kill(s) / t if t < float("inf") else 0.0
    def try_advance(self, dt):
        # push current stage if killable; bosses gated by timer
        s = self.stage
        t = self.ttk(s)
        if t == float("inf"): return
        if is_boss(s) and t > P["BOSS_TIMER"]: return   # boss wall: farm instead
        kills = dt / t
        need = 1 if is_boss(s) else P["KILLS_PER_STAGE"]
        self.kills += kills
        while self.kills >= need:
            self.kills -= need
            self.stage += 1
            self.max_stage = max(self.max_stage, self.stage)
            need = 1 if is_boss(self.stage) else P["KILLS_PER_STAGE"]
            t = self.ttk(self.stage)
            if t == float("inf") or (is_boss(self.stage) and t > P["BOSS_TIMER"]):
                self.kills = 0.0; break

def simulate(log_every=120):
    lanes = [Lane(h) for h in HEROES]
    wallet = 0.0; t = 0.0; end = P["SIM_HOURS"] * 3600
    events, series = [], []
    while t < end:
        dt = P["DT"]
        # unlock check
        for ln in lanes:
            if not ln.unlocked and lanes[0].max_stage >= ln.h["unlock_stage"] and wallet >= ln.h["unlock_cost"]:
                wallet -= ln.h["unlock_cost"]; ln.unlocked = True; ln.lvl = 1
                events.append((t/3600, f"{ln.h['name']} unlocked (lane opens at stage 1)"))
        # income
        wallet += sum(ln.income() for ln in lanes if ln.unlocked) * dt
        # greedy allocator: repeatedly buy globally cheapest next level
        while True:
            options = [(level_cost(ln.h, ln.lvl + 1), ln) for ln in lanes if ln.unlocked]
            options.sort(key=lambda x: x[0])
            if not options or options[0][0] > wallet: break
            cost, ln = options[0]; wallet -= cost; ln.lvl += 1
        # advance
        for ln in lanes:
            if ln.unlocked: ln.try_advance(dt)
        if int(t) % (log_every * 60) < dt:
            series.append(dict(hr=round(t/3600, 2), wallet=wallet,
                               stages=[ln.max_stage for ln in lanes],
                               lvls=[ln.lvl for ln in lanes]))
        t += dt
    return lanes, wallet, events, series

if __name__ == "__main__":
    lanes, wallet, events, series = simulate(log_every=int(sys.argv[1]) if len(sys.argv) > 1 else 120)
    print(json.dumps(dict(events=events, series=series,
        final=dict(stages=[l.max_stage for l in lanes], lvls=[l.lvl for l in lanes],
                   wallet=wallet)), indent=1))
