"""Farm Farmer skill XP pacing sim v0.1 — locked constants.
NPC works best unlocked tier 24/7 (estate arc). See docs/economy/SKILLS_DESIGN_v0.1.md
"""
A, B = 40, 1.12          # XP to next level = A * B^level, cap 99
TIERS = [(1,3.0,2),(15,4.0,5),(32,5.0,10),(50,6.0,18),(65,7.0,30),(80,8.0,48),(91,9.0,70)]

def simulate(days=70):
    lvl, xp, t, marks = 1, 0.0, 0.0, {}
    while t < days*86400 and lvl < 99:
        u, dur, gain = max(tt for tt in TIERS if tt[0] <= lvl)
        xp += gain; t += dur
        need = A * B**lvl
        while lvl < 99 and xp >= need:
            xp -= need; lvl += 1; need = A * B**lvl
            marks[lvl] = t/86400
    return marks

if __name__ == "__main__":
    m = simulate()
    for L in (10,30,50,75,90,99):
        print(f"L{L}: day {m.get(L,0):.2f}")
