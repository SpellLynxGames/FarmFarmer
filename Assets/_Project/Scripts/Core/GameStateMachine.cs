namespace FarmFarmer.Core
{
    // Plain C# on purpose -- keeps app-flow logic testable without a scene or MonoBehaviour.
    // See GameStateMachineRunner for the Unity Update-loop bridge.
    public class GameStateMachine
    {
        public IGameState Current { get; private set; }

        public void ChangeState(IGameState nextState)
        {
            Current?.Exit();
            Current = nextState;
            Current?.Enter(this);
        }

        public void Tick(float deltaTime)
        {
            Current?.Tick(deltaTime);
        }
    }
}
