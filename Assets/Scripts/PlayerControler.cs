using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    private PlayerInputManager _inputManager;
    private Rigidbody rb;
    private Animator _animator;
    // Start is called before the first frame update
    void Awake()
    {
        _inputManager = new PlayerInputManager();
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        _inputManager.Player.Walk.performed += OnWalk;
        _inputManager.Player.Run.performed += OnRun;
        _inputManager.Player.Jump.performed += OnJump;
        _inputManager.Player.Walk.canceled += OnStopWalk;
        _inputManager.Player.Run.canceled += OnStopRun;
        _inputManager.Player.Jump.canceled += OnStopJump;
        
    }

    void OnWalk(InputAction.CallbackContext value)
    {
        _animator.SetBool("Walk", true);
    }
    
    void OnRun(InputAction.CallbackContext value)
    {
        _animator.SetBool("Run", true);
    }
    void OnJump(InputAction.CallbackContext value)
    {
        _animator.SetBool("Jump", true);
    }
    void OnStopWalk(InputAction.CallbackContext value)
    {
        _animator.SetBool("Walk", false);
    }
    
    void OnStopRun(InputAction.CallbackContext value)
    {
        _animator.SetBool("Run", false);
    }
    void OnStopJump(InputAction.CallbackContext value)
    {
        _animator.SetBool("Jump", false);
    }


    private void OnEnable()
    {
        _inputManager.Player.Walk.Enable();
        _inputManager.Player.Run.Enable();
        _inputManager.Player.Jump.Enable();
    }

}
