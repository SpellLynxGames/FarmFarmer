using UnityEngine;

namespace FarmFarmer.Core
{
    public class CutsceneState : IGameState
    {
        public void Enter(GameStateMachine machine)
        {
            Debug.Log("[GameStateMachine] Entering Cutscene");
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
        }
    }
}
