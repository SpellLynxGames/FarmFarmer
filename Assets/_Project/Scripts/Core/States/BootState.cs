using UnityEngine;

namespace FarmFarmer.Core
{
    // Placeholder: this is where save-data load will happen before handing off to MainMenu.
    public class BootState : IGameState
    {
        public void Enter(GameStateMachine machine)
        {
            Debug.Log("[GameStateMachine] Entering Boot");

            var save = SaveService.Load();
            WalletService.Instance.SetBalance(save.sharedWallet);

            machine.ChangeState(new MainMenuState());
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
        }
    }
}
