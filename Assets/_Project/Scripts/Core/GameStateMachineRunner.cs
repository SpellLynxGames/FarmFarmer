using UnityEngine;

namespace FarmFarmer.Core
{
    // Bridges Unity's Update loop to the state machine.
    public class GameStateMachineRunner : MonoBehaviour
    {
        public GameStateMachine Machine { get; } = new GameStateMachine();

        private void Update()
        {
            Machine.Tick(Time.deltaTime);
        }
    }
}
