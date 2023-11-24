using UnityEngine;

namespace Fineallday.DelegateInterface
{
    public class Run : PhysicMovement, ICharacterState
    {
        public Run()
        {
            groundSpeed = 20;
            _forceMode = ForceMode.Force;
        }

        public void UpdateStateHandler(PlayerController pc)
        {
            if (!pc.playerInputs.HasFlag(PlayerController.PlayerInputs.Walking)) return;
            Move(transform.forward);
        }

        public bool PingStateHandler(PlayerController pc)
        {
            pc.playerInputs ^= PlayerController.PlayerInputs.Running;
            return true;
            
        }
    }
}