using System.Collections;
using System.Collections.Generic;
using Fineallday.DelegateInterface;
using UnityEngine;

namespace Fineallday.DelegateInterface
{
    public class Walk : PhysicMovement, ICharacterState
    {
        public Walk()
        {
            groundSpeed = 10;
            _forceMode = ForceMode.Force;
        }

        public void UpdateStateHandler(PlayerController pc)
        {
            // check for stairs
            //new vector forward + up for stairs
            if (pc.playerInputs.HasFlag(PlayerController.PlayerInputs.Running)) return;
            Move(transform.forward);
        }

        public bool PingStateHandler(PlayerController pc){
        
            pc.playerInputs ^= PlayerController.PlayerInputs.Walking;
            
            return true;
        }
    }
}