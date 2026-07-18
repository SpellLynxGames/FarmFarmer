using UnityEngine;

namespace FarmFarmer.Core
{
    public class GameplayState : IGameState
    {
        public void Enter(GameStateMachine machine)
        {
            Debug.Log("[GameStateMachine] Entering Gameplay");
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
        }
    }
}
