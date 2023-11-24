using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Fineallday.DelegateInterface
{
    public class PlayerController : MonoBehaviour, PlayerInputManager.IPlayerActions
    {
        [Flags]
        public enum PlayerInputs
        {
            None = 0, //000000
            Walking = 1 << 0, //000001
            Running = 1 << 1, //000010
            Grounded = 1 << 2, //000100
            Jumping = 1 << 3 //001000
            
            // None = 0, //000000
            // Walking = 1, //000001
            // Running = 2, //000010
            // Grounded = 4, //000100
            // Jumping = 8 //001000
        }

        public PlayerInputs playerInputs;
        
        private PlayerInputManager _playerInputManager;
        public delegate void PlayerMoveState(PlayerController pc);
        public PlayerMoveState _playerMoveState;
        private ICharacterState walk,run,jump,grounded;
        
        void Awake()
        {

           // playerStates &= ~ PlayerStates.Walking;
            
           // playerStates = PlayerStates.Grounded | PlayerStates.Walking;
           // playerStates = PlayerStates.Grounded;
           // playerStates |= PlayerStates.Running;
            
           // var c = playerStates & PlayerStates.Grounded;
            //
            // if (c == PlayerStates.Grounded)
            // {
            //     Debug.Log("Grounded!");
            // }
            
            // if (playerStates.HasFlag(PlayerStates.Grounded)) 
            // {
            //     Debug.Log("Grounded!");
            // }
            
            
            //playerStates = PlayerStates.Grounded;
            // playerStates = PlayerStates.Walking;
            
            
            
            
            _playerInputManager = new PlayerInputManager();
            _playerInputManager.Player.SetCallbacks(this);
            AddStateComponents();
        }

        
        void FixedUpdate()
        {
          if(_playerMoveState != null) _playerMoveState(this);
        }

        private void OnEnable()
        {
            _playerInputManager.Enable();
        }

        private void OnDisable()
        {
            _playerInputManager.Disable();
        }

        private void AddStateComponents()
        {
            walk = gameObject.AddComponent<Walk>();
            run = gameObject.AddComponent<Run>();
            grounded = gameObject.AddComponent<Grounded>();
            jump = gameObject.AddComponent<Jump>();
        }

        private void AddMoveState(ICharacterState state)
        { 
            state.PingStateHandler(this);
            _playerMoveState += state.UpdateStateHandler;
           
        }
        private void RemoveMoveState(ICharacterState state)
        {
            state.PingStateHandler(this);
            _playerMoveState -= state.UpdateStateHandler;
            
        }
        
        public void OnWalk(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                AddMoveState(walk);
                return;
            }
            if (context.phase == InputActionPhase.Canceled)
            {
                RemoveMoveState(walk);
            }
        }
        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                AddMoveState(run);
                return;
            }
            if (context.phase == InputActionPhase.Canceled)
            {
                RemoveMoveState(run);
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
             Debug.Log(grounded.PingStateHandler(this));
            if (context.started && grounded.PingStateHandler(this))
            {
                jump.PingStateHandler(this);
            }
        }

        public void OnTurnLeft(InputAction.CallbackContext context)
        {
        }

        public void OnTurnRight(InputAction.CallbackContext context)
        {
        }

       
    }
}