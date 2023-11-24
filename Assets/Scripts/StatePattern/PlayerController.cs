using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fineallday.StatePattern
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerInputManager _playerInputManager;
        public Dictionary<string, bool> InputTriggers = new();
        public Animator _animator;
        public Rigidbody rb;
        private PlayerState _currentState;
        public WalkingState _walkingState;
        public RunningState _runningState;
        public JumpingState _jumpingState;
        public FallingState _fallingState;
        public LandingState _landingState;
        public IdleState _idleState;
        public bool isGrounded => Physics.Raycast(transform.position + new Vector3(0,0.2f,0), Vector3.down, out RaycastHit hit, 0.225f);
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            InputSetUp();
            _walkingState = new WalkingState(this);
            _runningState = new RunningState(this);
            _jumpingState = new JumpingState(this);
            _fallingState = new FallingState(this);
            _landingState = new LandingState(this);
            _idleState = new IdleState(this);
        }

        private void Start()
        {
            ChangeState(_idleState);
        }

        void FixedUpdate()
        {
            _currentState.OnUpdateState();
        }

        public void ChangeState(PlayerState state)
        {
            _currentState?.OnExitState();
            _currentState = state;
            _currentState.OnEnterState();
        }

        private void InputSetUp()
        {
            SetInputDictionnay();
            _playerInputManager = new PlayerInputManager();
            SetInputCallbacks();
        }

        private void SetInputCallbacks()
        {
            _playerInputManager.Player.Walk.performed += OnInputTrigger;
            _playerInputManager.Player.Run.performed += OnInputTrigger;
            _playerInputManager.Player.Jump.performed += OnInputTrigger;
            _playerInputManager.Player.TurnLeft.performed += OnInputTrigger;
            _playerInputManager.Player.TurnRight.performed += OnInputTrigger;
            _playerInputManager.Player.Walk.canceled += OnInputTrigger;
            _playerInputManager.Player.Run.canceled += OnInputTrigger;
            _playerInputManager.Player.Jump.canceled += OnInputTrigger;
            _playerInputManager.Player.TurnLeft.canceled += OnInputTrigger;
            _playerInputManager.Player.TurnRight.canceled += OnInputTrigger;
        }

        private void SetInputDictionnay()
        {
            InputTriggers.Add("Walk", false);
            InputTriggers.Add("Run", false);
            InputTriggers.Add("Jump", false);
            InputTriggers.Add("TurnLeft", false);
            InputTriggers.Add("TurnRight", false);
        }

        void OnInputTrigger(InputAction.CallbackContext context)
        {
            if (context.action.phase == InputActionPhase.Performed)
            {
                InputTriggers[context.action.name] = true;
                return;
            }

            if (context.action.phase == InputActionPhase.Canceled)
            {
                InputTriggers[context.action.name] = false;
            }
        }

        private void OnEnable()
        {
            _playerInputManager.Player.Enable();
        }

        private void OnDisable()
        {
            _playerInputManager.Player.Disable();
        }
    }
}