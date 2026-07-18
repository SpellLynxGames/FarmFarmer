using System;
using System.Collections.Generic;
using UnityEngine;
using FarmFarmer.Core;

namespace FarmFarmer.Data
{
    // Resolves HeroSaveState.heroId against RosterDefinition/HeroDefinition and tracks which hero
    // is focused. Lives in FarmFarmer.Data (not Core, unlike WalletService) because Core cannot
    // reference Data -- the asmdef dependency only runs Data -> Core.
    public class RosterService
    {
        public static RosterService Instance { get; } = new RosterService();

        private readonly Dictionary<string, HeroDefinition> _byId = new Dictionary<string, HeroDefinition>();
        private readonly RosterDefinition _roster;

        public HeroDefinition FocusedHeroDefinition { get; private set; }
        public HeroSaveState FocusedHeroState { get; private set; }

        public event Action<string> FocusedHeroChanged;

        private RosterService()
        {
            _roster = Resources.Load<RosterDefinition>("Data/SO_RosterDefinition");
            if (_roster == null)
            {
                Debug.LogError("RosterService: SO_RosterDefinition not found under Resources/Data");
                return;
            }

            foreach (var hero in _roster.heroes)
            {
                _byId[hero.heroId] = hero;
            }
        }

        public HeroDefinition GetDefinition(string heroId)
        {
            _byId.TryGetValue(heroId, out var def);
            return def;
        }

        // First-run bootstrap: gives the lowest-unlockOrder hero a starting HeroSaveState if the
        // save has none yet, and resolves the currently-focused hero either way.
        public void Hydrate(SaveData save)
        {
            if (save.heroes.Count == 0 && _roster != null && _roster.heroes.Count > 0)
            {
                var starter = _roster.heroes[0];
                foreach (var hero in _roster.heroes)
                {
                    if (hero.unlockOrder < starter.unlockOrder) starter = hero;
                }

                var state = new HeroSaveState
                {
                    heroId = starter.heroId,
                    isUnlocked = true,
                    currentStage = 1,
                    currentFloor = 1,
                    level = 1,
                    xp = 0,
                };
                save.heroes.Add(state);
                save.focusedHeroId = starter.heroId;
            }

            if (string.IsNullOrEmpty(save.focusedHeroId) && save.heroes.Count > 0)
            {
                save.focusedHeroId = save.heroes[0].heroId;
            }

            SetFocus(save.focusedHeroId, save);
        }

        public void SetFocus(string heroId, SaveData save)
        {
            HeroSaveState state = null;
            foreach (var hero in save.heroes)
            {
                if (hero.heroId == heroId) { state = hero; break; }
            }

            if (state == null) return;

            save.focusedHeroId = heroId;
            FocusedHeroState = state;
            FocusedHeroDefinition = GetDefinition(heroId);
            FocusedHeroChanged?.Invoke(heroId);
        }
    }
}
