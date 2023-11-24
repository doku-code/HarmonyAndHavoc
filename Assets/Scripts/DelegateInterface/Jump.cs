using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fineallday.DelegateInterface
{
    public class Jump : PhysicMovement, ICharacterState
    {
        private ICharacterState _characterStateImplementation;
        [SerializeField] private byte jumpSpeed = 7;
        public Jump()
        {
            _forceMode = ForceMode.Impulse;
            groundSpeed = 1;
        }
        public void UpdateStateHandler(PlayerController pc)
        {
            
        }

        public bool PingStateHandler(PlayerController pc)
        {
            //Debug.Log("Jump");
           Move(new Vector3(0,jumpSpeed,0));
           return true;
        }
    }
}
