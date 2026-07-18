namespace FarmFarmer.Core
{
    public interface IGameState
    {
        void Enter(GameStateMachine machine);
        void Tick(float deltaTime);
        void Exit();
    }
}
