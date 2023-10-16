using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JFM
{
    public class PlayerController : MonoBehaviour
    {       
        private PlayerInput playerInputManager;
        public Dictionary<string, bool> inputTriggers = new();

        [NonSerialized] public Animator _animator;
        [NonSerialized] public Rigidbody2D rb;

        private PlayerState _currentState;
        public IdleState _idleState;
        public WalkingState _walkingState;
        
        public JumpingState _jumpingState;
        public WallJumpingState _wallJumpingState;
        public WallGrippingState _wallGrippingState;
        public AirborneState _airborneState;
        public DashingState _dashingState;
        public LadderClimbingState _ladderClimbingState;
        public LedgeClimbingState _ledgeClimbingState;

        /*public FallingState _fallingState;
        public LandingState _landingState;
        */
        [SerializeField] private float walkSpeed = 1.0f;
        [SerializeField] private float walkAcceleration = 10.0f;
        //[SerializeField] private float runSpeed = 4.0f;
        [SerializeField] private float ladderSpeed = 3.0f;
        [SerializeField] private float ladderAcceleration = 10.0f;
        [SerializeField] private float jumpForce = 3.0f;
        [SerializeField] private int baseNumJumps = 1;
        [SerializeField] private float airSpeedMultiplier = 100.0f;
        [SerializeField] private float dashForce = 3.0f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask ladderLayer;

        [SerializeField] private float groundDistance = 1.0f;
        [SerializeField] private float wallDistance = 0.6f;

        [SerializeField] private float wallJumpDuration = 1.0f;
        [SerializeField] private float ledgeAnimationDuration = 1.0f;
        [SerializeField] private float dashDuration = 1.0f;
        [SerializeField] private float wallGripForce = 1000.0f;        

        // In degrees
        [SerializeField] private float wallJumpAngle = 45.0f;
        // In degrees
        [SerializeField] private float dashAngle = 0.0f;
        // In degrees
        [SerializeField] private float dashDiagAngle = 45.0f;

        private int numJumps;

        private bool isFacingRight = true;
        private Vector2 moveInput;

        private bool isGrounded;
        private GameObject ground;
        
        private GameObject frontWall;
        private GameObject beneathObject;

        private bool isGrippingToWall;

        public float WalkSpeed
        {
            get => walkSpeed;
        }

        public float WalkAcceleration
        {
            get => walkAcceleration;
        }

        /* public float RunSpeed
         {
             get => runSpeed;
         }*/

        public float JumpForce
        {
            get => jumpForce;
        }

        public float AirSpeedMultiplier
        {
            get => airSpeedMultiplier;
        }

        public float WallGripForce
        {
            get => wallGripForce;
        }

        public float LadderSpeed
        {
            get => ladderSpeed;
        }

        public float LadderAcceleration
        {
            get => ladderAcceleration;
        }

        public float WallJumpDuration
        {
            get => wallJumpDuration;
        }

        public float LedgeAnimationDuration
        {
            get => ledgeAnimationDuration;
        }

        public float DashDuration
        {
            get => dashDuration;
        }

        public bool IsFacingRight
        {
            get => isFacingRight;
        }

        public LayerMask GroundLayer
        {
            get => groundLayer;
        }

        public LayerMask LadderLayer
        {
            get => ladderLayer;
        }

        public bool GetIsGrippingToWall()
        { 
            return isGrippingToWall; 
        }

        public void Turn()
        {
            isFacingRight = !isFacingRight;
            GetComponent<SpriteRenderer>().flipX = !isFacingRight;
        }  

        public bool CanTurn()
        {
            //Debug.Log($"isFacingRight ({isFacingRight}) && moveInput.x ({moveInput.x})");
            return (isFacingRight && moveInput.x < 0) || (!isFacingRight && moveInput.x > 0);
        }

        public Vector2 MoveInput
        {
            get => moveInput;
        }

        public bool CanJump()
        {
            //Debug.Log($" rb.velocity.y = {rb.velocity.y}");
            bool grounded = IsGrounded();
            bool grippingToWall = IsGrippingToWall();
            if (grounded || grippingToWall)
            {
                //Debug.Log("OH Non");
                numJumps = baseNumJumps;
            }

            return numJumps > 0;
        }

        public void Jump()
        {           
            numJumps--;
            rb.drag = 0.0f;
            //rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            //Debug.Log($"rb.velocity (before)={rb.velocity}");
            if (isGrippingToWall)
            {
                float angle = wallJumpAngle * Mathf.Deg2Rad;// Mathf.PI / 2.0f * 0.5f;// 8.0f / 9.0f;
                Debug.Log($"JUMP from wall {IsFacingRight}  {(IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle)}  {Mathf.Cos(angle)}");
                Vector3 v = new Vector3((IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * jumpForce * 1.0f;
                rb.AddForce(v, ForceMode2D.Impulse);
                //moveInput += new Vector2((IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * jumpForce * 1.0f;
                Debug.Log($"v={v}");
            }
            else
            {                    
                rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
                //moveInput += Vector2.up * jumpForce;
            }
            //Debug.Log($"numJumps = {numJumps}  rb.velocity (after) ={rb.velocity}");
            
        }
        
        public bool CanDash()
        {
            return inputTriggers["Dash"] && moveInput != Vector2.zero;
        }

        public void Dash()
        {
            //numJumps--;

            //Debug.Log($"rb.velocity (before)={rb.velocity}");
            if (CanTurn())
            {
                //Debug.Log("Can Turn");
                Turn();
            }
            Vector3 v;
            if (moveInput.y != 0.0f)
            {
                if (moveInput.x != 0.0f)
                {
                    float angle = dashDiagAngle * Mathf.Deg2Rad;
                    Debug.Log($"Dashing {IsFacingRight}  {(IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle)}  {Mathf.Cos(angle)}");
                    v = new Vector3((IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * dashForce;
                }
                else
                {
                    //Debug.Log($"Dashing {IsFacingRight}  {(IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle)}  {Mathf.Cos(angle)}");
                    v = Vector3.up * moveInput.y * dashForce;                    
                }
            }
            else
            {
                float angle = dashAngle * Mathf.Deg2Rad;
                Debug.Log($"Dashing {IsFacingRight}  {(IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle)}  {Mathf.Cos(angle)}");
                v = new Vector3((IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * dashForce;
            }
            rb.AddForce(v, ForceMode2D.Impulse);            
            Debug.Log($"v={v}");
            
            //Debug.Log($"numJumps = {numJumps}  rb.velocity (after) ={rb.velocity}");

        }

        /*public bool IsInFrontOfWall()
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + new Vector2(0, 0.0f), isFacingRight ? Vector2.right : -Vector2.right, wallDistance, groundLayer);
            //Debug.DrawRay(transform.position + new Vector3(0, 0.0f, 0.0f), (isFacingRight ? Vector2.right : -Vector2.right) * wallDistance, Color.green);

            return (hit.collider is not null);
        }
        */
        
        private void SetFrontWallInfo()
        {
            if (frontWall is null)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + new Vector2(0, 0.0f), isFacingRight ? Vector2.right : -Vector2.right, wallDistance, groundLayer);
                Debug.DrawRay(transform.position + new Vector3(0, 0.0f, 0.0f), (isFacingRight ? Vector2.right : -Vector2.right) * wallDistance, Color.green);

                if(hit.collider is null)
                {
                    return;
                }

                frontWall = hit.transform.gameObject;
            }
        }

        private void SetBeneathObjectInfo()
        {
            if (beneathObject is null)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + new Vector2(0, 0.0f), isFacingRight ? Vector2.right : -Vector2.right, 0.0f, ladderLayer);
                //Debug.DrawRay(transform.position + new Vector3(0, 0.0f, 0.0f), (isFacingRight ? Vector2.right : -Vector2.right) * 0.01f, Color.green);

                if (hit.collider is null)
                {
                    //Debug.Log("Oh non ^$%$#%&@$%@%^$%&");
                    return;
                }

                beneathObject = hit.transform.gameObject;
            }
        }

        public bool IsInFrontOfObjectLayer(Vector2 offset, int layerMask)
        {            
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + offset, isFacingRight ? Vector2.right : -Vector2.right, 0.0f, layerMask);
            //Debug.DrawRay(transform.position + new Vector3(0, 0.0f, 0.0f), (isFacingRight ? Vector2.right : -Vector2.right) * 0.01f, Color.green);

            return (hit.collider is not null);            
        }

        // Checks front wall
        public bool CanClimbLadder()
        {
            SetBeneathObjectInfo();

            if (beneathObject is not null)
            {
                //Debug.Log($"%%%% Test {moveInput.x} {hit.collider is not null}");
                //Debug.Log($"%%%% {1 << beneathObject.layer} == {(int)ladderLayer} {moveInput.x == 0.0f} {Mathf.Abs(rb.velocity.x) <= 0.5f}");
            }
            return (beneathObject is not null && (1 << beneathObject.layer) == (int)ladderLayer);// && moveInput.x == 0.0f);//&& Mathf.Abs(rb.velocity.x) <= 0.5f);
        }

        public bool CanGripToWall()
        {
            SetFrontWallInfo();

            //Debug.Log($"%%%% Test {moveInput.x} {hit.collider is not null}");

            return (frontWall is not null && (1 << frontWall.layer) == (int)groundLayer && ((isFacingRight && moveInput.x > 0) || (!isFacingRight && moveInput.x < 0)) && Mathf.Abs(rb.velocity.x) <= 0.5f);
        }

        // Checks back wall
        public bool IsGrippingToWall()
        {
            
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + new Vector2(0, 0.0f), isFacingRight ? -Vector2.right : Vector2.right, groundDistance, groundLayer);
            //Debug.DrawRay(transform.position + new Vector3(0, 0.0f, 0.0f), (isFacingRight ? -Vector2.right : Vector2.right) * groundDistance, Color.blue);
            //return hit.transform is not null && hit.transform.gameObject.layer == groundLayer && ((isFacingRight && moveInput.x > 0) || (!isFacingRight && moveInput.x < 0));
            //Debug.Log($"%%%% Test {moveInput.x} {hit.collider is not null}");

            if (hit.collider is not null && ((!isFacingRight && moveInput.x > 0) || (isFacingRight && moveInput.x < 0)) && Mathf.Abs(rb.velocity.x) <= 0.5f)
            {
                isGrippingToWall = true;
                //Debug.Log("YEAH");
            }
            else
            {
                //Debug.Log($"hit.collider = {hit.collider} && (({!isFacingRight} && {moveInput.x > 0}) || ({isFacingRight} && {moveInput.x < 0})) && {rb.velocity.x} <= 0.0001f {Mathf.Abs(rb.velocity.x) <= 0.0001f}");
                isGrippingToWall = false;
            }


            return isGrippingToWall;
        }


        public bool IsGrounded() 
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + new Vector2(0, 0.2f), Vector2.down, 1.325f, groundLayer);
            //Debug.DrawRay(transform.position + new Vector3(0, 0.2f, 0.0f), Vector3.down * 1.325f, Color.blue);

            if (hit.collider is null || rb.velocity.y > 0.0f)
            //if (hit.collider is null || moveInput.y > 0.0f)
            {
                isGrounded = false;
                ground = null;
                //Debug.Log("Not Grounded.");
                return false;
            }

            //if (hit.transform is null && hit.transform.gameObject.layer == groundLayer)
            //{

            isGrounded = true;
            ground = hit.collider.gameObject;
            //Debug.Log("Grounded.");
            return true;
            /*} 
            else
            {
                isGrounded = false;
                Debug.Log("Not Grounded.");
                return false;
            }*/
        }

        void Awake()
        {
            _animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();

            InputSetup();
            _walkingState = new WalkingState(_animator, this);
            _jumpingState = new JumpingState(_animator, this);
            _wallJumpingState = new WallJumpingState(_animator, this);
            _wallGrippingState = new WallGrippingState(_animator, this);
            _airborneState = new AirborneState(_animator, this);
            _dashingState = new DashingState(_animator, this);
            _ladderClimbingState = new LadderClimbingState(_animator, this);
            _ledgeClimbingState = new LedgeClimbingState(_animator, this);
            _idleState = new IdleState(_animator, this);
        }

        private void Start()
        {
            _currentState = _idleState;
        }

        private void FixedUpdate()
        {
            frontWall = null;
            beneathObject = null;

            //Debug.Log($"moveInput.x = {moveInput.x}");
            _currentState = _currentState.Process();

            if (isGrounded)
            {
                //moveInput.y = 0.0f;
                //rb.drag = 4.48f;                
            }
            else
            {
                rb.drag = 0.0f;                
            }

            //moveInput -= Vector2.up * rb.gravityScale * Time.fixedDeltaTime;
            //moveInput.x = 0.0f;
        }

        private void InputSetup()
        {
            SetInputDictionnary();

            playerInputManager = new PlayerInput();

            SetInputsCallBacks();
        }

        private void SetInputsCallBacks()
        {
            playerInputManager = new PlayerInput();

            playerInputManager.Player.Move.performed += OnInputMove;
            playerInputManager.Player.Jump.performed += OnInputTrigger;
            playerInputManager.Player.Dash.performed += OnInputTrigger;

            playerInputManager.Player.Move.canceled += OnInputMove;
            playerInputManager.Player.Jump.canceled += OnInputTrigger;                        
            playerInputManager.Player.Dash.canceled += OnInputTrigger;
        }

        private void SetInputDictionnary()
        {
            inputTriggers.Add("Move", false);
            inputTriggers.Add("Jump", false);
            inputTriggers.Add("Dash", false);
        }

        void OnInputTrigger(InputAction.CallbackContext context)
        {
            //Debug.Log("context.action.name = " + context.action.name);
            if (context.action.phase == InputActionPhase.Performed)
            {
                /*if(context.action.name == "Jump")
                {
                    Debug.Log("Jump!!!!!");
                }*/
                inputTriggers[context.action.name] = true;
            }
            else if (context.action.phase == InputActionPhase.Canceled)
            {
                inputTriggers[context.action.name] = false;                
            }
        }

        void OnInputMove(InputAction.CallbackContext context)
        {
            //Debug.Log("context.action.name = " + context.action.name);
            OnInputTrigger(context);
            //if (inputTriggers[context.action.name])
            //{                
            moveInput = context.ReadValue<Vector2>();// * WalkSpeed;
            //Debug.Log($"moveInput = {moveInput}");
            //}            
        }

        private void OnEnable()
        {
            playerInputManager.Player.Enable();
        }
        private void OnDisable()
        {
            playerInputManager.Player.Disable();
        }

        public void ChangeState(PlayerState nextState)
        {
            _currentState.SetNextState(nextState);            
        }


    }
}