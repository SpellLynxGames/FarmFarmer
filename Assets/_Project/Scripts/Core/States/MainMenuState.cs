using UnityEngine;

namespace FarmFarmer.Core
{
    public class MainMenuState : IGameState
    {
        public void Enter(GameStateMachine machine)
        {
            Debug.Log("[GameStateMachine] Entering MainMenu");
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
        }
    }
}
