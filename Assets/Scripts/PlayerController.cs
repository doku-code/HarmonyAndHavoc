//#define _DEBUG_INPUT

using AF;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
//using static UnityEngine.InputManagerEntry;
using charles;

namespace JFM
{
    public struct RaycastHit2DInfo
    {
        public bool hasHit;
        public RaycastHit2D hit;
        public Vector2 probePoint;
    }

    public class PlayerController : MonoBehaviour
    {
        public delegate void ParameterLessDelegate();
        
        private PlayerInput playerInputManager;
        public Dictionary<string, bool> inputTriggers = new Dictionary<string, bool>();

        [NonSerialized] public Animator animator;
        [NonSerialized] public Rigidbody2D rb;
        
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 1.0f;
        [SerializeField] private float walkAcceleration = 10.0f;
        //[SerializeField] private float runSpeed = 4.0f;
        private bool isFacingRight = true;
        private Vector2 moveInput;
        
        [Header("Jumping")]
        [SerializeField] private float jumpForce = 3.0f;
        [SerializeField] private int baseNumJumps = 1;
        [SerializeField] private float airAcceleration = 100.0f;
        [SerializeField] private float heightDamage = 3.0f;
        [SerializeField] private float landingHeight = 2.0f;
        [SerializeField] private float defaultGravityScale;
        [SerializeField] private float groundDistance = 1.0f;
        [SerializeField] private float groundedRadius = 0.2f;
        [SerializeField] private Vector2 groundBoxSize = new Vector2(0.95f, 0.01f);
        public float groundedY;
        public int groundedLayer;
        private float highestAirborneY;
        private int numJumps;
        public ParameterLessDelegate GroundedEvent { get; set; }

        [Header ("Attacking")]
        [SerializeField] private float attackCoolDownTime = 0.3f;
        private float attackCoolDownStartTime;

        [Header("Collisions")]
        [SerializeField] private Vector2 spriteBoxProbeSize = new Vector2(0.9414063f, 0.3f);
        [SerializeField] private Vector2 spriteBoxProbeOffset = new Vector2(0.0f, 0.0f);
        private Vector2 colliderOffset;
        private Vector2 colliderSize;
        [SerializeField] private float wallDistance = 0.4f;
        private RaycastHit2DInfo hitInfo;
        private GameObject frontWall;
        private GameObject beneathObject;
        private Vector2 beneathObjectPosition;

        [Header("Ladders")]
        [SerializeField] private float ladderSpeed = 3.0f;
        [SerializeField] private float ladderAcceleration = 10.0f;
        [SerializeField] private float ladderGroundDistance = 0.1f;
        [SerializeField] private float ladderPushUpForce = 2.0f;
        [SerializeField] private float ladderCenteringSpeed = 10.0f;

        [Header("Stairs")]
        [SerializeField] private float stairsUpDistanceHigh = 0.8f;
        [SerializeField] private float stairsUpHeight = 0.2f;
        [SerializeField] private float stairsUpMinSlope = 1.2f;
        [SerializeField] private float stairsUpMaxSlope = 2.0f;

        [SerializeField] private float stairsDownHeight = 0.2f;
        [SerializeField] private float stairsDownGroundX = 0.3f;
        [SerializeField] private float stairsDownDeceleration = 0.3f;
        [SerializeField] private float stairsDownMinSlope = 0.4f;
        [SerializeField] private float stairsDownMaxSlope = 2.0f;
        // In degrees
        [SerializeField] private float stairsUpAngle = 45.0f;
        //[SerializeField] private float stairsDownAngle = 70.0f;
        [SerializeField] private float stairsGroundDistance = 0.2f;
        [SerializeField] private float stairsSpeed = 30.0f;
        [SerializeField] private float stairsAcceleration = 1000.0f;

        [Header("Layers")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask ladderLayer;
                
        [Header("Data")]
        [SerializeField] private PlayerData playerData;
        private Knowledge lastKnowledge;

        private string[] knowledgeInputNames =
        {
            "Knowledge1",
            "Knowledge2",
            "Knowledge3",
            "Knowledge4"
        };

        [Space]
        [Header("States")]
        private PlayerState currentState;
        [SerializeField] private PlayerState[] statesArray;
        public Dictionary<PlayerState.STATE, PlayerState> states = new Dictionary<PlayerState.STATE, PlayerState>();
        
        public int BaseNumJumps
        {
            get => baseNumJumps;
            set => baseNumJumps = value;
        }

        public float GroundDistance
        {
            get => groundDistance;
        }

        public float GroundedRadius
        {
            get => groundedRadius;
        }

        public RaycastHit2DInfo HitInfo
        {
            get => hitInfo;
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

        public float AirAcceleration
        {
            get => airAcceleration;
        }

        public float LadderSpeed
        {
            get => ladderSpeed;
        }

        public float LadderAcceleration
        {
            get => ladderAcceleration;
        }

        public float LadderCenteringSpeed
        {
            get => ladderCenteringSpeed;
        }

        public float LadderPushUpForce
        {
            get => ladderPushUpForce;
        }

        public float StairsGroundDistance
        {
            get => stairsGroundDistance;
        }

        public float StairsUpDistanceHigh
        {
            get => stairsUpDistanceHigh;
        }

        public float StairsUpHeight
        {
            get => stairsUpHeight;
        }

        public float StairsUpMinSlope
        {
            get => stairsUpMinSlope;
        }

        public float StairsUpMaxSlope
        {
            get => stairsUpMaxSlope;
        }

        public float StairsDownHeight
        {
            get => stairsDownHeight;
        }

        public float StairsDownMinSlope
        {
            get => stairsDownMinSlope;
        }

        public float StairsDownMaxSlope
        {
            get => stairsDownMaxSlope;
        }

        public float StairsSpeed
        {
            get => stairsSpeed;
        }

        public float StairsAcceleration
        {
            get => stairsAcceleration;
        }

        public float StairsDownDeceleration
        {
            get => stairsDownDeceleration;
        }

        public float StairsUpAngle
        {
            get => stairsUpAngle;
        }

        /*public float StairsDownAngle
        {
            get => stairsDownAngle;
        }*/

        public float StairsDownGroundX
        {
            get => stairsDownGroundX;
        }        

        public float WallDistance
        {
            get => wallDistance;
        }

        public bool IsFacingRight
        {
            get => isFacingRight;
        }

        public float DefaultGravityScale
        {
            get => defaultGravityScale;
        }

        /*public float NumJumps
        {
            get => numJumps;
        }*/

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

        public GameObject FrontWall
        {
            get => frontWall;
        }

        public PlayerData Data
        {
            get => playerData;
        }

        public Vector2 GetBeneathObjectPosition()
        {
            return beneathObjectPosition;
        }

        public GameObject GetBeneathObject()
        {
            return beneathObject;
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
            transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1.0f, 1.0f);
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
            bool grounded = IsGrounded() && Platformer2DUtilities.AreNearlyEqual(rb.velocity.y, 0.0f);
            //bool grippingToWall = IsSlidingOnBackWall();
            //Debug.Log($"grounded={grounded} Platformer2DUtilities.AreNearlyEqual(rb.velocity.y, 0.0f)={Platformer2DUtilities.AreNearlyEqual(rb.velocity.y, 0.0f)}");

            if (grounded /*|| grippingToWall*/)
            {
                ResetJump();
                //hasDashed = false;
                //Debug.Log("Jump reset");
                GroundedEvent();
            }            
        }
        
        public void ResetJump()
        {
            numJumps = baseNumJumps;
        }

        public bool WillJump()
        {
            //Debug.Log($"numJumps={numJumps}");
            return inputTriggers["Jump"] && numJumps > 0;
        }

        public void DepleteJumps()
        {
            numJumps--;
        }
        
        public void Jump()
        {
            DepleteJumps();
            
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            //rb.velocity += Vector2.up * jumpForce;
            //Debug.Log($"rb.velocity={rb.velocity}");            
        }

        // Will the Player go in Landing state?
        public bool WillLand()
        {
            float highestY = highestAirborneY;
            highestAirborneY = transform.position.y;

            if (highestY - transform.position.y > heightDamage)
            {
                Debug.Log($" ~ ~ ~ D A M A G E ~ ~ ~ highestY={highestY} transform.position.y={transform.position.y}");

                return true;
            }

            if (highestY - transform.position.y > landingHeight)
            {
                return true;
            }

            return false;
        }

        public bool IsAttackCooledDown()
        {
            return Time.time - attackCoolDownStartTime >= attackCoolDownTime;
        }

        public bool WillAttack()
        {
            return inputTriggers["BasicAttack"] && IsAttackCooledDown();
        }

        public bool WillAttack(KnowledgeID knowledge)
        {
            return GetKnowledgeTrigger(playerData.AvailableKnowledgeDictionary[knowledge]) && IsAttackCooledDown();
        }

        public void Attack()
        {
            attackCoolDownStartTime = Time.time;
        }

        public void TakeDamage(int damage, Vector2 pushDirection)
        {
            if(currentState != states[PlayerState.STATE.HURT])
            {
                playerData.TakeDamage(damage);
                HurtState state = (HurtState)states[PlayerState.STATE.HURT];
                state.pushDirection = pushDirection.normalized;
            }
        }

        public void OnHit()
        {            
            ChangeState(states[PlayerState.STATE.HURT]);
        }

        public void OnDead()
        {
            ChangeState(states[PlayerState.STATE.DEAD]);
        }

        public bool GetKnowledgeTrigger(AvailableKnowledgePosition knowledgePosition)
        {
            if( knowledgePosition == AvailableKnowledgePosition.NOT_AVAILABLE)
            {
                return false;
            }

            return inputTriggers[knowledgeInputNames[(int)knowledgePosition - 1]];
        }

        public void SetKnowledgeTrigger(AvailableKnowledgePosition knowledgePosition, bool value)
        {
            if (knowledgePosition == AvailableKnowledgePosition.NOT_AVAILABLE)
            {
                return;
            }

            inputTriggers[knowledgeInputNames[(int)knowledgePosition - 1]] = value;
        }        

        public void UseKnowledge(KnowledgeID knowledge)
        {
            lastKnowledge = playerData.EveryKnowledgeDictionary[knowledge];

            KnowledgeState state = (KnowledgeState)states[PlayerState.STATE.KNOWLEDGE];            
            state.knowledge = playerData.GetKnowledgeByID(knowledge);
            ChangeState(state);
        }

        private void SetFrontWallInfo()
        {
            if (frontWall is null)
            {
                Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;
                RaycastHit2D hit1 = Physics2D.Raycast(rb.position + new Vector2(0, colliderSize.y / 2.0f), v, wallDistance, groundLayer);
                
                if (hit1.collider is null)
                {
                    RaycastHit2D hit2 = Physics2D.BoxCast(rb.position + colliderSize / 2.0f + v * wallDistance, colliderSize, 0.0f, v, 0.0f, groundLayer);
                    if (hit2.collider is null)
                    {                     
                        return;
                    }

                    frontWall = hit2.transform.gameObject;
                    return;
                }

                frontWall = hit1.transform.gameObject;
            }
        }

        private void SetBeneathObjectInfo()
        {            
            if (beneathObject is null)
            {
                Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;
                RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + spriteBoxProbeOffset.x * v + spriteBoxProbeSize.x/2.0f * v + spriteBoxProbeSize.y / 2.0f * Vector2.up, v, spriteBoxProbeSize.x, ladderLayer);
                //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y) + spriteBoxProbeOffset.x * v + spriteBoxProbeSize.x / 2.0f * v + spriteBoxProbeSize.y / 2.0f * Vector2.up, v * spriteBoxProbeSize.x, Color.yellow);
                if(rayHit.collider is not null)
                {
                    //Debug.Log($"rayHit.point={rayHit.point}");
                    beneathObjectPosition = new Vector2(Mathf.Floor(rayHit.point.x), Mathf.Floor(rayHit.point.y));
                    beneathObject = rayHit.transform.gameObject;
                }          
                else
                {
                    beneathObject = null;
                }
            }
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
            bool h2 = Raycast(false, ladderLayer, Vector2.down * ladderGroundDistance, 0.01f, Vector2.down);//, false ,true);

            //Debug.Log($"{moveInput.x == 0.0f} && {moveInput.y < 0.0f} && h2={h2} moveInput.y = {moveInput.y}");

            return moveInput.x == 0.0f && moveInput.y < 0.0f && h2;
        }
        
        public bool IsAboveLadder()
        {
            //if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
            //player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down)
            //    )
            bool r1 = Raycast(false, ladderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up); //, false, true) &&
                                                                                         //bool r2 = player.Raycast(false, player.LadderLayer, Vector2.down * 0.5f, 0.3f, Vector2.down);
            RaycastHit2D hit = Physics2D.CircleCast(rb.position, groundedRadius, Vector2.down, 0.7f, ladderLayer);
            bool r2 = hit.collider is not null;
            if(r2)
            {
                groundedLayer = hit.collider.gameObject.layer;
                groundedY = hit.point.y;
            }
            //bool r2 = Physics2D.BoxCast(player.rb.position, player.ColliderSize, 0.0f, Vector2.down, 0.7f, player.LadderLayer).collider is not null;
            //Debug.Log($"r1={r1} r2={r2}");
            return (!r1 && r2);
        }

        public bool WillClimbUpStairs()
        {
            Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;
            //if (FindSlopeAtPoint(out float slope, v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight, v, StairsDownHeight))//, true))
            if (Raycast2DHelper.FindSlopeAtPoint(rb.position, out float slope, v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight, Platformer2DUtilities.RotateVector2(Vector2.down, (isFacingRight ? 1.0f : -1.0f) * 45.0f), stairsDownHeight, GroundLayer))//, true))
            {
                
                bool grounded = IsGrounded();
                //Debug.Log($"ClimbingUpStairs slope={slope} grounded={grounded}");
                return grounded && moveInput.x != 0.0f && Mathf.Abs(slope) > stairsUpMinSlope && Mathf.Abs(slope) <= stairsUpMaxSlope && slope != Mathf.Infinity;
            }
            else
            {
                return false;
            }
        }

        public bool WillClimbDownStairs()
        {
            Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;

            bool stairsDown = Raycast2DHelper.FindSlopeBeneath(rb.position, out float slope, Vector2.up * 0.02f + v * stairsDownGroundX, -v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight, StairsDownHeight, GroundLayer, (isFacingRight ? -1.0f : 1.0f) * 45.0f, false);

            // Doing a second slope test to avoid getting the case where the first slope test falls on a corner (two lines intersecting). A corner automatically is interpreted as a slope.
            bool stairsDown2 = Raycast2DHelper.FindSlopeBeneath(rb.position, out float slope2, Vector2.up * 0.02f + v * (stairsDownGroundX + 0.04f), -v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight, StairsDownHeight, GroundLayer, (isFacingRight ? -1.0f : 1.0f) * 45.0f, false);
            
            // Validating the first slope test
            if (Platformer2DUtilities.AreNearlyEqual(slope2, 0.0f) || slope2 == Mathf.Infinity)
            {
                slope = slope2;
            }

            bool ret = IsGrounded() && stairsDown && Mathf.Abs(slope) > stairsDownMinSlope && Mathf.Abs(slope) < stairsDownMaxSlope && rb.velocity.y <= 0.01f && ((moveInput.x > 0.0f && slope < 0) || (moveInput.x > 0.0f && slope < 0)) && slope != Mathf.Infinity;
            //Debug.Log($"IsGrounded()={IsGrounded()} stairsDown={stairsDown} slope={slope} ret={ret} rb.velocity.y={rb.velocity.y} {Mathf.Abs(slope) > stairsDownMinSlope} && {Mathf.Abs(slope) < stairsDownMaxSlope} && {rb.velocity.y <= 0.01f} ({(moveInput.x > 0.0f && slope < 0)} || {(moveInput.x > 0.0f && slope < 0)})");
            return ret;
        }

        public bool FindSlopeBeneath(out float slope)
        {
            Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;
            return FindSlopeBeneath(out slope, Vector2.up * 0.02f + v * colliderSize.x / 2.0f + v * stairsDownGroundX, - v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight);
        }

        public bool FindSlopeBeneath(out float slope, Vector2 offset1, Vector2 offset2)
        {
            return Raycast2DHelper.FindSlopeBeneath(rb.position, out slope, offset1, offset2, StairsDownHeight, GroundLayer);
        }

        public bool FindSlopeAtPoint(out float slope, Vector2 offset, Vector2 direction)
        {
            return Raycast2DHelper.FindSlopeAtPoint(rb.position, out slope, offset, direction, StairsDownHeight, GroundLayer);
        }

        public bool IsGrounded()
        {
            return IsGrounded(groundLayer | ladderLayer);
        }

        public bool IsGrounded(int layerMask)
        {
            return IsGrounded(layerMask, Vector2.zero);
        }

        public bool IsGrounded(int layerMask, Vector2 offset)
        {
            return IsGrounded(layerMask, offset, groundDistance, false);
        }

        public bool IsGrounded(int layerMask, Vector2 offset, float distance, bool overrideVelocity)
        {            
            bool groundRaycast = Raycast2DHelper.CheckGrounded(rb.position + offset, groundedRadius, distance, layerMask, groundLayer, out groundedLayer);
            bool grounded = groundRaycast && (rb.velocity.y <= 0.001f || overrideVelocity);

            //Debug.Log($"groundRaycast={groundRaycast} rb.velocity.y={rb.velocity.y} rb.velocity.y <= 0.001f = {rb.velocity.y <= 0.001f}");

            if (grounded)
            {
                //Debug.Log($"groundY={groundY}");
            }

            return grounded;
        }
            
        public bool Raycast(bool doBoxCast, int layerMask, Vector2 offset, float distance)
        {
            return Raycast(doBoxCast, layerMask, offset, distance, Vector2.down);
        }

        public bool Raycast(bool doBoxCast, int layerMask, Vector2 offset, float distance, Vector2 direction)
        {
            return Raycast(doBoxCast, layerMask, offset, distance, direction, false);
        }

        public bool Raycast(bool doBoxCast, int layerMask, Vector2 offset, float distance, Vector2 direction, bool willBreak)
        {
            return Raycast(doBoxCast, layerMask, offset, distance, direction, false, false);
        }

        public bool Raycast(bool doBoxCast, int layerMask, Vector2 offset, float distance, Vector2 direction, bool willBreak, bool willDraw)
        {
            RaycastHit2D hit;
            if (!doBoxCast)
            {
                hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + offset, direction, distance, layerMask);
                
                if (willDraw)
                    Debug.DrawRay(new Vector2(transform.position.x, transform.position.y) + offset, direction * distance, Color.yellow);

                if (willBreak)
                    Debug.Break();
            }
            else
            {
                hit = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y) + offset, groundBoxSize, 0.0f, direction, distance, layerMask);
            }

            hitInfo.hit = hit;
            if(hit.collider is not null)
            {
                hitInfo.probePoint = hit.point;
                hitInfo.hasHit = true;
                //Debug.Log($"hit.normal={hit.normal} distance={hit.distance}");
                return true;
            }

            hitInfo.probePoint = new Vector2(transform.position.x, transform.position.y) + offset + direction * distance;
            hitInfo.hasHit = false;
            return false;
        }
        [SerializeField] private float thres = 0.05f;
        public void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log($"OnTriggerEnter2D collision is null = {collision is null} collision.gameObject.CompareTag(\"Enemy\")={collision.gameObject.CompareTag("Enemy")} collision.gameObject.name={collision.gameObject.name}");

            if(collision is not null && collision.gameObject.CompareTag("Enemy"))
            {
                //Debug.Log($"Hit enemy named: {collision.gameObject.name}");
                
                EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
                int damage = playerData.GetPlayerDamage(null);
                CapsuleCollider2D capsule = collision.gameObject.GetComponent<CapsuleCollider2D>();
                Vector3 collisionOffset = new Vector3(capsule.offset.x, capsule.offset.y, 0.0f);
                Vector3 playerColliderOffset = new Vector3(colliderOffset.x, colliderOffset.y, 0.0f); 
                Vector2 pushDirection = collision.transform.position + collisionOffset - (transform.position + playerColliderOffset);
                float angle = Mathf.Atan2(pushDirection.y, pushDirection.x);
                const float fortyFive = Mathf.PI / 4.0f;
                float quarter = angle / fortyFive;
                float remainder = angle % fortyFive;
                Debug.Log($"pushDirection={pushDirection} angle={angle * Mathf.Rad2Deg}");
                enemyController.TakeDamage(damage, pushDirection.normalized);
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            //Debug.Log("OnTriggerStay2D");
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
            //Debug.Log("OnTriggerExit2D");
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            defaultGravityScale = rb.gravityScale;
            rb.isKinematic = true;

            CapsuleCollider2D cc = GetComponent<CapsuleCollider2D>();
            colliderOffset = cc.offset;
            colliderSize = cc.size;

            InputSetup();
            
            for (int i = 0; i < statesArray.Length; i++)
            {
                PlayerState state = statesArray[i];
                state.Initialize(this);
                states.Add(state.name, state);
            }

            playerData.InitializeData();        // To remove in the future
            playerData.OnDeadDelegate += OnDead;
            playerData.OnHitDelegate += OnHit;
            InitializeKnowledges();

            SetHighestAirborneY(true);

            attackCoolDownStartTime = Time.time;
        }

        private void Start()
        {
            currentState = states[PlayerState.STATE.IDLE];
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

        private void InitializeKnowledges()
        {
            foreach (var knowledge in playerData.EveryKnowledgeDictionary)
            {
                knowledge.Value.Initialize(this);
            }

            playerData.KnownKnowledgeDictionary[KnowledgeID.DASH] = true;
            playerData.KnownKnowledgeDictionary[KnowledgeID.WALL_SLIDE] = true;
            playerData.KnownKnowledgeDictionary[KnowledgeID.DOUBLE_JUMP] = true;            
            playerData.AvailableKnowledgeDictionary[KnowledgeID.DASH] = AvailableKnowledgePosition.POSITION1;
            playerData.AvailableKnowledgeDictionary[KnowledgeID.WALL_SLIDE] = AvailableKnowledgePosition.POSITION2;
            playerData.AvailableKnowledgeDictionary[KnowledgeID.DOUBLE_JUMP] = AvailableKnowledgePosition.POSITION4;
            playerData.GetKnowledgeByID(KnowledgeID.DASH).Activate();
            playerData.GetKnowledgeByID(KnowledgeID.WALL_SLIDE).Activate();
            playerData.GetKnowledgeByID(KnowledgeID.DOUBLE_JUMP).Activate();
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
            playerInputManager.Player.Inventory.performed += OnInputTrigger;

            playerInputManager.Player.Move.canceled += OnInputMove;
            playerInputManager.Player.Jump.canceled += OnInputTrigger;                        
            playerInputManager.Player.Knowledge1.canceled += OnInputTrigger;
            playerInputManager.Player.Knowledge2.canceled += OnInputTrigger;
            playerInputManager.Player.Knowledge3.canceled += OnInputTrigger;
            playerInputManager.Player.Knowledge4.canceled += OnInputTrigger;
            playerInputManager.Player.BasicAttack.canceled += OnInputTrigger;
            playerInputManager.Player.Inventory.canceled += OnInputTrigger;
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
            inputTriggers.Add("Inventory", false);
        }

        void OnInputTrigger(InputAction.CallbackContext context)
        {
#if _DEBUG_INPUT
            Debug.Log($"{context.action.name} has been {context.action.phase}");
#endif
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
#if _DEBUG_INPUT
            Debug.Log($"moveInput = {moveInput}");                        
#endif
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
