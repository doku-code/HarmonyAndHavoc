using AF;
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

        [NonSerialized] public Animator animator;
        [NonSerialized] public Rigidbody2D rb;

        private PlayerState currentState;
        public IdleState idleState;
        public WalkingState walkingState;
        
        public JumpingState jumpingState;
        public WallJumpingState wallJumpingState;
        public WallGrippingState wallGrippingState;
        public AirborneState airborneState;
        public CrouchedState crouchedState;
        public CrouchedAttackState crouchedAttackState;
        public BasicAttackState basicAttackState;
        public DashingState dashingState;
        public LadderClimbingState ladderClimbingState;
        public LandingState landingState;

        [SerializeField] private float walkSpeed = 1.0f;
        [SerializeField] private float walkAcceleration = 10.0f;
        //[SerializeField] private float runSpeed = 4.0f;
        [SerializeField] private float ladderSpeed = 3.0f;
        [SerializeField] private float ladderAcceleration = 10.0f;
        [SerializeField] private float groundLadderDistance = 1.2f;
        [SerializeField] private float jumpForce = 3.0f;
        [SerializeField] private int baseNumJumps = 1;
        [SerializeField] private float airSpeedMultiplier = 100.0f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask ladderLayer;

        [SerializeField] private float groundDistance = 1.0f;
        [SerializeField] private Vector2 groundBoxSize = new Vector2(0.95f, 0.01f);
        [SerializeField] private float wallDistance = 0.6f;
        [SerializeField] private float wallJumpDuration = 1.0f;
        [SerializeField] private float ledgeAnimationDuration = 1.0f;
        [SerializeField] private float dashForce = 3.0f;
        [SerializeField] private float groundDashForce = 60.0f;
        [SerializeField] private float groundDashDeceleration = 1000.0f;
        [SerializeField] private float groundDashBailOutNormalizedTime = 0.4f;
        // In degrees
        [SerializeField] private float groundDashAngle = 0.0f;
        // In degrees
        [SerializeField] private float dashDiagAngle = 45.0f;        
        [SerializeField] private float wallGripForce = 1000.0f;        
        // In degrees
        [SerializeField] private float wallJumpAngle = 45.0f;

        [SerializeField] private float heightDamage = 3.0f;
        [SerializeField] private float landingHeight = 2.0f;

        private float highestAirborneY;

        private int numJumps;
        private bool hasDashed;
        private Vector2 dashDirection;

        private bool isFacingRight = true;
        private Vector2 moveInput;

        private bool isGrounded;
        private GameObject ground;
        
        private GameObject frontWall;
        private GameObject beneathObject;
        private Vector2 beneathObjectPosition;

        private bool isGrippingToWall;

        [SerializeField] private Vector2 spriteBoxProbeSize = new Vector2(0.9414063f, 0.3f);
        private Vector2 colliderOffset;
        private Vector2 colliderSize;
        [SerializeField] private Vector2 spriteBoxProbeOffset = new Vector2(0.0f, 0.0f);        

        [SerializeField] private PlayerData playerData;
        private Knowledge lastKnowledge;

        private string[] knowledgeInputNames =
        {
            "Knowledge1",
            "Knowledge2",
            "Knowledge3",
            "Knowledge4"
        };

        private int[] knowledgeIndices;

        private bool isEventGrounded;
        private Vector2 groundDirection;
        private Vector2 groundDirection2;

        public bool IsEventGrounded
        {
            get => isEventGrounded;
        }

        public Vector2 GroundDirection
        {
            get => groundDirection;
        }

        public Vector2 GroundDirection2
        {
            get => groundDirection2;
        }

        public float WalkSpeed
        {
            get => walkSpeed;
        }

        public float WalkAcceleration
        {
            get => walkAcceleration;
        }

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

        public float WallDistance
        {
            get => wallDistance;
        }

        public float LedgeAnimationDuration
        {
            get => ledgeAnimationDuration;
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

        public Vector2 ColliderOffset
        {
            get => colliderOffset;
        }
        public Vector2 ColliderSize
        {
            get => colliderSize;
        }

        public Vector2 DashDirection
        {
            get => dashDirection;
        }

        public float GroundDashDeceleration
        {
            get => groundDashDeceleration;
        }

        public float GroundDashBailOutNormalizedTime
        {
            get => groundDashBailOutNormalizedTime;
        }

        public PlayerData Data 
        { 
            get => playerData; 
        }

        public bool GetIsGrippingToWall()
        { 
            return isGrippingToWall; 
        }

        public Vector2 GetBeneathObjectPosition()
        {
            return beneathObjectPosition;
        }

        public void SetHighestAirborneY()
        {
            SetHighestAirborneY(false);
        }

        public void SetHighestAirborneY(bool force)
        {
            if (highestAirborneY < transform.position.y || force)
            {
                highestAirborneY = transform.position.y;
            }
        }

        public void Turn()
        {
            isFacingRight = !isFacingRight;
            GetComponent<SpriteRenderer>().flipX = !isFacingRight;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }

        public bool CanTurn()
        {
            return (isFacingRight && moveInput.x < 0) || (!isFacingRight && moveInput.x > 0);
        }

        public Vector2 MoveInput
        {
            get => moveInput;
            set => moveInput = value;
        }
        
        public void SetAirborneInfo()
        {
            bool grounded = IsCastGrounded() && rb.velocity.y < 0.0f;
            bool grippingToWall = IsGrippingToWall();
            if (grounded || grippingToWall)
            {
                numJumps = baseNumJumps;
                hasDashed = false;
            }            
        }

        public bool WillJump()
        {            
            return inputTriggers["Jump"] && numJumps > 0;
        }

        public void Jump()
        {           
            numJumps--;
            
            if (isGrippingToWall)
            {
                float angle = wallJumpAngle * Mathf.Deg2Rad;
                Debug.Log($"JUMP from wall {IsFacingRight}  {(IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle)}  {Mathf.Cos(angle)}");
                Vector3 v = new Vector3((IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * jumpForce * 1.0f;
                rb.AddForce(v, ForceMode2D.Impulse);                
            }
            else
            {                    
                rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);                
            }
        }                

        // Will the Player go in Landing state?
        public bool WillLand()
        {
            float highestY = highestAirborneY;
            highestAirborneY = transform.position.y;

            if (highestY - transform.position.y > heightDamage )
            {
                Debug.Log(" ~ ~ ~ D A M A G E ~ ~ ~");

                return true;
            }

            if (highestY - transform.position.y > landingHeight)
            {
                return true;
            }

            return false;
        }        

        public bool GetInputTriggersFromKnowledge(KnowledgeID knowledge)
        {
            return inputTriggers[knowledgeInputNames[knowledgeIndices[(int)knowledge]]];
        }

        public void SetInputTriggersFromKnowledge(KnowledgeID knowledge, bool value)
        {
            inputTriggers[knowledgeInputNames[knowledgeIndices[(int)knowledge]]] = value;
        }
        
        public bool WillDash()
        {
            int knowledgeIndex = (int)playerData.AvalaibleKnowledgeDictionary[KnowledgeID.DASH];
            
            return !hasDashed && knowledgeIndex > 0 && inputTriggers[knowledgeInputNames[knowledgeIndex - 1]] && moveInput != Vector2.zero;
        }

        public void Dash()
        {
            hasDashed = true;
            
            if (CanTurn())
            {
                Turn();
            }
            
            Vector3 v;
            if (moveInput.y != 0.0f)
            {
                if (moveInput.x != 0.0f)
                {
                    float angle = dashDiagAngle * Mathf.Deg2Rad;
                    v = new Vector3((IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * dashForce;
                }
                else
                {
                    v = Vector3.up * moveInput.y * dashForce;                    
                }
            }
            else
            {
                float angle = groundDashAngle * Mathf.Deg2Rad;
                v = new Vector3((IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * groundDashForce;
            }
            rb.AddForce(v, ForceMode2D.Impulse);

            dashDirection = moveInput;

            //moveInput = Vector2.zero;                     
        }             
        
        private void SetFrontWallInfo()
        {
            if (frontWall is null)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + new Vector2(0, 0.0f), isFacingRight ? Vector2.right : -Vector2.right, wallDistance, groundLayer);
                
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
                RaycastHit2D hit = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y) + spriteBoxProbeOffset, spriteBoxProbeSize, 0.0f, isFacingRight ? Vector2.right : -Vector2.right, 0.0f, ladderLayer);
                
                if (hit.collider is null)
                {
                    beneathObject = null;

                    return;
                }

                beneathObject = hit.transform.gameObject;
                beneathObjectPosition = new Vector2(Mathf.Floor(hit.point.x), Mathf.Floor(hit.point.y));
            }
        }

        public bool IsInFrontOfObjectLayer(Vector2 offset, int layerMask)
        {            
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + offset, isFacingRight ? Vector2.right : -Vector2.right, 0.0f, layerMask);
            
            return (hit.collider is not null);            
        }

        public bool CanClimbLadder()
        {
            SetBeneathObjectInfo();
            
            return (beneathObject is not null && (1 << beneathObject.layer) == (int)ladderLayer);
        }

        public bool WillClimbLadder()
        {
            SetBeneathObjectInfo();
            
            return (moveInput.x == 0.0f && moveInput.y != 0.0f && beneathObject is not null && (1 << beneathObject.layer) == (int)ladderLayer);
        }

        public bool WillClimbDownLadder()
        {                        
            return (moveInput.x == 0.0f && moveInput.y < 0.0f && Raycast(true, ladderLayer, Vector2.zero, groundLadderDistance));
        }

        public bool WillGripToWall()
        {
            SetFrontWallInfo();

            bool backWallHit = Raycast(true, groundLayer, Vector2.zero, wallDistance, isFacingRight ? -Vector2.right : Vector2.right);

            bool front = frontWall is not null && (1 << frontWall.layer) == (int)groundLayer && ((isFacingRight && moveInput.x > 0) || (!isFacingRight && moveInput.x < 0));
            bool back = backWallHit && ((isFacingRight && moveInput.x < 0) || (!isFacingRight && moveInput.x > 0));
            return (front || back) && Mathf.Abs(rb.velocity.x) <= 0.5f;
        }

        // Checks back wall
        public bool IsGrippingToWall()
        {            
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), isFacingRight ? -Vector2.right : Vector2.right, wallDistance, groundLayer);
            
            if (hit.collider is not null && ((!isFacingRight && moveInput.x > 0) || (isFacingRight && moveInput.x < 0)) && Mathf.Abs(rb.velocity.x) <= 0.5f)
            {
                isGrippingToWall = true;                
            }
            else
            {
                isGrippingToWall = false;
            }

            return isGrippingToWall;
        }

        public bool IsCastGrounded()
        {
            return IsCastGrounded(false);
        }

        public bool IsCastGrounded(bool limitToRay)
        {
            return IsCastGrounded(limitToRay, groundLayer | ladderLayer, Vector2.zero);
        }

        public bool IsCastGrounded(bool limitToRay, int layerMask)
        {
            return IsCastGrounded(limitToRay, layerMask, Vector2.zero);
        }

        public bool IsCastGrounded(bool limitToRay, int layerMask, Vector2 offset) 
        {
            RaycastHit2D hit;
            if (limitToRay)
            {
                hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + offset, Vector2.down, groundDistance, layerMask);
            }
            else
            {
                hit = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y) + offset, groundBoxSize, 0.0f, Vector2.down, groundDistance, layerMask);
            }

            return (hit.collider is not null);// && rb.velocity.y < -0.001f);            
        }

        public bool Raycast(bool limitToRay, int layerMask, Vector2 offset, float distance)
        {
            return Raycast(limitToRay, layerMask, offset, distance, Vector2.down);
        }

        public bool Raycast(bool limitToRay, int layerMask, Vector2 offset, float distance, Vector2 direction)
        {
            return Raycast(limitToRay, layerMask, offset, distance, direction, false);
        }

        public bool Raycast(bool limitToRay, int layerMask, Vector2 offset, float distance, Vector2 direction, bool willBreak)
        {
            RaycastHit2D hit;
            if (limitToRay)
            {
                hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + offset, direction, distance, layerMask);
                Debug.DrawRay(new Vector2(transform.position.x, transform.position.y) + offset, direction * distance, Color.yellow);
                if(willBreak)
                    Debug.Break();
            }
            else
            {
                hit = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y) + offset, groundBoxSize, 0.0f, direction, distance, layerMask);
            }

            return (hit.collider is not null);
        }

        public bool IsGrounded()
        {
            return IsGrounded(true);
        }

        public bool IsGrounded(bool testVelocity)
        {
            return ((isEventGrounded && (IsVerticalDirection(groundDirection) || IsVerticalDirection(groundDirection2))) || Raycast(true, groundLayer | ladderLayer, Vector2.zero, 0.2f)) && (rb.velocity.y < -0.001f || !testVelocity);
        }

        public bool IsHorizontalDirection(Vector2 direction)
        {
            return direction == Vector2.right || direction == Vector2.left;
        }

        public bool IsVerticalDirection(Vector2 direction)
        {
            return direction == Vector2.up || direction == Vector2.down;
        }

        // Event "isGrounded" is used for edge cases that aren't correctly covered by ray- or boxcasts. For example, 
        // the sprite boxcast size ("GroundBoxSize") cannot be too wide (too close to 1) as it breaks the WallSlide cast.
        public void OnCollisionEnter2D(Collision2D collision)
        {
            groundDirection = collision.GetContact(0).normal;
            Debug.Log($"groundDirection={collision.GetContact(0).normal})");
            if (collision.contactCount > 1)
            {
                groundDirection2 = collision.GetContact(1).normal;
                for (int i = 1; i < collision.contactCount; i++)
                {
                    Debug.Log($"groundDirection{i+1}={collision.GetContact(1).normal})");
                }
            }
            else
            {
                groundDirection2 = Vector2.zero;
            }
            
            isEventGrounded = true;
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            isEventGrounded = false;
        }
        
        void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
                        
            BoxCollider2D bc = GetComponent<BoxCollider2D>();
            colliderOffset = bc.offset;
            colliderSize = bc.size;

            InputSetup();
            walkingState = new WalkingState(animator, this);
            jumpingState = new JumpingState(animator, this);
            wallJumpingState = new WallJumpingState(animator, this);
            wallGrippingState = new WallGrippingState(animator, this);
            airborneState = new AirborneState(animator, this);
            crouchedState = new CrouchedState(animator, this);
            crouchedAttackState = new CrouchedAttackState(animator, this);
            basicAttackState = new BasicAttackState(animator, this);
            dashingState = new DashingState(animator, this);
            ladderClimbingState = new LadderClimbingState(animator, this);
            landingState = new LandingState(animator, this);  
            idleState = new IdleState(animator, this);

            playerData.InitializeData();
            playerData.AvalaibleKnowledgeDictionary[KnowledgeID.DASH] = AvalaibleKnowledgePosition.POSITION1;

            knowledgeIndices = new int[Enum.GetNames(typeof(KnowledgeID)).Length];
            knowledgeIndices[(int)KnowledgeID.DASH] = 0;
            //knowledgeIndices[(int)KnowledgeID.WALL_SLIDE] = 1;        // Wall slide does not have a button, it's the D-pad!
            knowledgeIndices[(int)KnowledgeID.GROUND_SLIDE] = 2;
            //knowledgeIndices[(int)KnowledgeID.DOUBLE_JUMP] = 3;       // Double jump does not have a dedicated button, it's the jump button!
        }

        private void Start()
        {
            currentState = idleState;
        }

        private void FixedUpdate()
        {
            SetAirborneInfo();

            frontWall = null;
            SetFrontWallInfo();
            beneathObject = null;
            SetBeneathObjectInfo();

            currentState = currentState.Process();            
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
            playerInputManager.Player.Knowledge1.performed += OnInputTrigger;
            playerInputManager.Player.Knowledge2.performed += OnInputTrigger;
            playerInputManager.Player.Knowledge3.performed += OnInputTrigger;
            playerInputManager.Player.Knowledge4.performed += OnInputTrigger;
            playerInputManager.Player.BasicAttack.performed += OnInputTrigger;

            playerInputManager.Player.Move.canceled += OnInputMove;
            playerInputManager.Player.Jump.canceled += OnInputTrigger;                        
            playerInputManager.Player.Knowledge1.canceled += OnInputTrigger;
            playerInputManager.Player.Knowledge2.canceled += OnInputTrigger;
            playerInputManager.Player.Knowledge3.canceled += OnInputTrigger;
            playerInputManager.Player.Knowledge4.canceled += OnInputTrigger;
            playerInputManager.Player.BasicAttack.canceled += OnInputTrigger;
        }

        private void SetInputDictionnary()
        {
            inputTriggers.Add("Move", false);
            inputTriggers.Add("Jump", false);
            inputTriggers.Add("Knowledge1", false);
            inputTriggers.Add("Knowledge2", false);
            inputTriggers.Add("Knowledge3", false);
            inputTriggers.Add("Knowledge4", false);
            inputTriggers.Add("BasicAttack", false);
        }

        void OnInputTrigger(InputAction.CallbackContext context)
        {
            //Debug.Log("context.action.name = " + context.action.name);
            if (context.action.phase == InputActionPhase.Performed)
            {                
                inputTriggers[context.action.name] = true;
            }
            else if (context.action.phase == InputActionPhase.Canceled)
            {
                inputTriggers[context.action.name] = false;                
            }
        }

        void OnInputMove(InputAction.CallbackContext context)
        {
            OnInputTrigger(context);
            moveInput = context.ReadValue<Vector2>();
            //Debug.Log($"moveInput = {moveInput}");                        
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
            currentState.SetNextState(nextState);            
        }        
    }
}