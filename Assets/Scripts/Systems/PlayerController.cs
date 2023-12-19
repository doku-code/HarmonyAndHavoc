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
using UnityEngine.UIElements;
using System.Collections;

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
        [SerializeField] private float airSpeed = 2.0f;
        [SerializeField] private bool canTakeFallDamage = false;
        [SerializeField] private float maxFallDamageHeight = 3.0f;
        [SerializeField] private float landingHeight = 2.0f;
        [SerializeField] private float fallDamageMultiplier = 3.0f;        
        private int currentFallDamage;
        [SerializeField] private float defaultGravityScale;
        [SerializeField] private float groundDistance = 1.0f;
        [SerializeField] private float groundedRadius = 0.2f;
        [SerializeField] private Vector2 groundBoxSize = new Vector2(0.95f, 0.01f);
        public float groundedY;
        public int groundedLayer;
        private float highestAirborneY;
        private int numJumps;        
        public ParametersLessDelegate GroundedEvent { get; set; }
        public ParametersLessDelegate JumpedEvent { get; set; }
        public IntegerParameterDelegate LandedEvent { get; set; }

        [Header("Attacking")]
        [SerializeField] private float attackCoolDownTime = 0.3f;
        private float attackCoolDownStartTime;
        public IntegerParameterDelegate IsHurtEvent { get; set; }

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
        private bool isWallColliding;
        private bool wallIsToRight;
        [SerializeField] private float collisionCheckRadius = 0.2f;
        [SerializeField] private float collisionCheckYOffset = 0.3f;
        [SerializeField] private float heightAdjustmentFactor = 0.2f;

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
        [SerializeField] private float stairsDownDecelerationFactor = 0.3f;
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
        private Knowledge lastAttackKnowledge;

        private string[] knowledgeInputNames =
        {
            "Knowledge1",
            "Knowledge2",
            "Knowledge3",
            "Knowledge4"
        };

        [Space]
        [Header("States")]
        [SerializeField] private PlayerState[] statesArray;
        private PlayerStateMachine stateMachine;

        public PlayerStateMachine StateMachine
        {
            get => stateMachine;
        }

        public PlayerState CurrentState
        {
            get => stateMachine.currentState;
        }

        public Dictionary<PlayerState.STATE, PlayerState> States
        {
            get => stateMachine.states;
        }

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

        public float AirSpeed
        {
            get => airSpeed;
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

        public float StairsDownDecelerationFactor
        {
            get => stairsDownDecelerationFactor;
        }

        public float StairsUpAngle
        {
            get => stairsUpAngle;
        }

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

        public bool IsWallColliding
        {
            get => isWallColliding;
        }

        public bool WallIsToRight
        {
            get => wallIsToRight;
        }

        public Vector2 MoveInput
        {
            get => moveInput;
            set => moveInput = value;
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
                if (GroundedEvent is not null)
                {
                    GroundedEvent();
                }
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

            if (JumpedEvent is not null)
            {
                JumpedEvent();
            }
        }
        
        private int CalculateFallDamage(float yDifference)
        {
            return Mathf.FloorToInt(yDifference * fallDamageMultiplier);
        }

        // Will the Player go in Landing state?
        public bool WillLand()
        {
            float highestY = highestAirborneY;
            highestAirborneY = transform.position.y;

            if (highestY - transform.position.y > maxFallDamageHeight)
            {
                //Debug.Log($" ~ ~ ~ D A M A G E ~ ~ ~ highestY={highestY} transform.position.y={transform.position.y}");
                currentFallDamage = CalculateFallDamage(highestY - transform.position.y - maxFallDamageHeight);
                return true;
            }
            currentFallDamage = 0;

            if (highestY - transform.position.y > landingHeight)
            {                
                return true;
            }

            return false;
        }
       
        public void Land(bool takeDamage)
        {
            if (takeDamage && canTakeFallDamage)
            {
                // Do fall damage here?
                playerData.TakeDamage(currentFallDamage);
                Debug.Log($"Taking {currentFallDamage} damage from falling.");
            }

            if (LandedEvent is not null)
            {
                LandedEvent(currentFallDamage);
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

        public bool IsAttackCooledDown()
        {
            //Debug.Log($"Time.time - attackCoolDownStartTime {Time.time - attackCoolDownStartTime} >= {attackCoolDownTime}");
            return Time.time - attackCoolDownStartTime >= attackCoolDownTime;
        }

        public bool WillAttack()
        {
            //Debug.Log($"inputTriggers[\"BasicAttack\"] && IsAttackCooledDown() {inputTriggers["BasicAttack"]} && {IsAttackCooledDown()}");
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
            if (stateMachine.currentState != stateMachine.states[PlayerState.STATE.HURT] &&
                stateMachine.currentState != stateMachine.states[PlayerState.STATE.DEAD] &&
                (stateMachine.currentState != stateMachine.states[PlayerState.STATE.KNOWLEDGE] ||
                ((KnowledgeState)stateMachine.states[PlayerState.STATE.KNOWLEDGE]).knowledge !=
                playerData.EveryKnowledgeDictionary[KnowledgeID.DASH]))
            {
                playerData.TakeDamage(damage);
                HurtState state = (HurtState)stateMachine.states[PlayerState.STATE.HURT];
                state.pushDirection = pushDirection.normalized;
            }
        }

        public void OnOrderChange(int value)
        {
            if (value <= 0)
            {
                if (stateMachine.currentState != stateMachine.states[PlayerState.STATE.DEAD]
                    // We want to prevent fall damage to activate HurtState
                    && stateMachine.currentState != stateMachine.states[PlayerState.STATE.LAND])
                {
                    if(IsHurtEvent is not null)
                    {
                        IsHurtEvent(value);
                    }
                    stateMachine.ChangeState(stateMachine.states[PlayerState.STATE.HURT]);
                }
            }
            else
            {
                stateMachine.ChangeState(stateMachine.states[PlayerState.STATE.IDLE]);
            }
        }

        public void OnDead()
        {
            if (stateMachine.currentState != stateMachine.states[PlayerState.STATE.DEAD])
            {
                stateMachine.ChangeState(stateMachine.states[PlayerState.STATE.DEAD]);
            }
        }

        private void HitEnemy(Collider2D collision)
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();

            int damage = playerData.GetPlayerDamage(lastAttackKnowledge);

            // Calculates pushback direction
            CapsuleCollider2D capsule = collision.gameObject.GetComponent<CapsuleCollider2D>();
            /*Vector3 collisionOffset = new Vector3(capsule.offset.x, capsule.offset.y, 0.0f);
            Vector3 playerColliderOffset = new Vector3(colliderOffset.x, colliderOffset.y, 0.0f);
            Vector2 pushDirection = collision.transform.position + collisionOffset - (transform.position + playerColliderOffset);
            Vector2 newPushDirection = Platformer2DUtilities.RoundVector2Angle(pushDirection, Mathf.PI / 4.0f);
            */
            Vector2 newPushDirection = Platformer2DUtilities.CalculateGroundDifference(collision.transform.position, 
                                                                                       capsule.offset, 
                                                                                       transform.position,
                                                                                       colliderOffset);
            //Debug.Log($"pushDirection={pushDirection} newPushDirection={newPushDirection}");

            if(enemyController.TakeDamage(damage, newPushDirection.normalized))
            {
                playerData.ActualChaos += enemyController.MaxHealth;
                playerData.TotalKills++;
            }
        }

        public bool GetKnowledgeTrigger(AvailableKnowledgePosition knowledgePosition)
        {
            if (knowledgePosition == AvailableKnowledgePosition.NOT_AVAILABLE)
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

        public void ResetLastAttackKnowledgeUsed()
        {
            lastAttackKnowledge = null;
        }

        public void UseKnowledge(KnowledgeID knowledgeID)
        {
            Knowledge knowledge = playerData.EveryKnowledgeDictionary[knowledgeID];

            if (!knowledge.isUtility)
            {
                lastAttackKnowledge = knowledge;                
            }

            KnowledgeState state = (KnowledgeState)stateMachine.states[PlayerState.STATE.KNOWLEDGE];
            state.knowledge = playerData.GetKnowledgeByID(knowledgeID);
            stateMachine.ChangeState(state);
        }

        /*void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 100, 30), "Vanish"))
            {
                Vanish();
            }
        }*/

        public void Vanish()
        {
            stateMachine.ChangeState(stateMachine.states[PlayerState.STATE.VANISH]);
        }

        private void SetBeneathObjectInfo()
        {
            if (beneathObject is null)
            {
                Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;
                RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + spriteBoxProbeOffset.x * v + spriteBoxProbeSize.x / 2.0f * v + spriteBoxProbeSize.y / 2.0f * Vector2.up, v, spriteBoxProbeSize.x, ladderLayer);
                //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y) + spriteBoxProbeOffset.x * v + spriteBoxProbeSize.x / 2.0f * v + spriteBoxProbeSize.y / 2.0f * Vector2.up, v * spriteBoxProbeSize.x, Color.yellow);
                if (rayHit.collider is not null)
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

        public bool WillClimbDownLadder(out Vector2 point)
        {
            RaycastHit2D hit;
            //bool h2 = Raycast(false, ladderLayer, Vector2.down * ladderGroundDistance, 0.01f, Vector2.down);//, false ,true);
            bool h2 = (hit = Physics2D.CircleCast(rb.position + Vector2.down * ladderGroundDistance, groundedRadius, Vector2.down, 0.0f, ladderLayer)).collider is not null;
            //Debug.Log($"{moveInput.x == 0.0f} && {moveInput.y < 0.0f} && h2={h2} moveInput.y = {moveInput.y}");

            point = hit.point;
            bool h3 = IsAboveLadder();
            if (h3)
            {
                point.x = rb.position.x;
                point.y = groundedY;                
            }
            return moveInput.x == 0.0f && moveInput.y < 0.0f && h3;// h2;
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
            if (r2)
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
            float facing = isFacingRight ? 1.0f : -1.0f;
            //if (FindSlopeAtPoint(out float slope, v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight, v, StairsDownHeight))//, true))
            if (Raycast2DHelper.FindSlopeAtPoint(rb.position, out float slope, v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight, Platformer2DUtilities.RotateVector2(Vector2.down, facing * 45.0f), stairsDownHeight, GroundLayer))//, true))
            {
                bool grounded = IsGrounded();
                //Debug.Log($"ClimbingUpStairs slope={slope} grounded={grounded} facing={facing}");
                return grounded && moveInput.x != 0.0f && facing * Mathf.Sign(slope) > 0 && Mathf.Abs(slope) > stairsUpMinSlope && Mathf.Abs(slope) <= stairsUpMaxSlope && slope != Mathf.Infinity;
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
            return FindSlopeBeneath(out slope, Vector2.up * 0.02f + v * colliderSize.x / 2.0f + v * stairsDownGroundX, -v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight);
        }

        public bool FindSlopeBeneath(out float slope, Vector2 offset1, Vector2 offset2)
        {
            return Raycast2DHelper.FindSlopeBeneath(rb.position, out slope, offset1, offset2, StairsDownHeight, GroundLayer);
        }

        public bool FindSlopeAtPoint(out float slope, Vector2 offset, Vector2 direction)
        {
            return Raycast2DHelper.FindSlopeAtPoint(rb.position, out slope, offset, direction, StairsDownHeight, GroundLayer);
        }

        public bool CheckForCollisions()
        {
            CapsuleCollider2D cc = GetComponent<CapsuleCollider2D>();
            Vector2 colliderOffset = cc.offset;
            //Vector2 colliderSize = cc.size;

            return Raycast2DHelper.CheckForCollisions(rb.position, cc, groundLayer, false);
        }

        public bool IsGroundedSlope()
        {
            bool foundSlopeBeneath = FindSlopeBeneath(out float slope);
            return IsGrounded(groundLayer | ladderLayer, Vector2.zero, groundDistance * 2.0f, false) || (foundSlopeBeneath && Mathf.Abs(slope) > stairsUpMinSlope && Mathf.Abs(slope) < stairsUpMaxSlope);
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
            if (hit.collider is not null)
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log($"collision.gameObject.layer={collision.gameObject.layer} == {collision.gameObject.layer == GroundLayer} GroundLayer={(int)GroundLayer}");
            if (1 << collision.gameObject.layer == GroundLayer)
            {
                if (!IsGrounded())
                {
                    isWallColliding = true;
                    //Debug.Log($"YEAH!! collision.normal={collision.GetContact(0).normal}");
                    wallIsToRight = collision.GetContact(0).normal.x < 0;
                }
                else
                {
                    isWallColliding = false;
                }
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            //Debug.Log($"collision.gameObject.layer={collision.gameObject.layer} == {collision.gameObject.layer == GroundLayer} GroundLayer={(int)GroundLayer}");
            if (1 << collision.gameObject.layer == GroundLayer)
            {
                if (!IsGrounded())
                {
                    isWallColliding = true;
                    //Debug.Log($"YEAH!! collision.normal={collision.GetContact(0).normal}");
                    wallIsToRight = collision.GetContact(0).normal.x < 0;
                }
                else
                {
                    isWallColliding = false;
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (1 << collision.gameObject.layer == GroundLayer)
            {
                isWallColliding = false;
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log($"OnTriggerEnter2D collision is null = {collision is null} collision.gameObject.CompareTag(\"Enemy\")={collision.gameObject.CompareTag("Enemy")} collision.gameObject.name={collision.gameObject.name}");
            IInteractible interactibleObject = null;

            if (collision is not null)
            {
                if (collision.gameObject.CompareTag("Enemy") && collision is CapsuleCollider2D)
                {
                    //Debug.Log($"Hit enemy named: {collision.gameObject.name}");

                    HitEnemy(collision);
                }

                if (collision.gameObject.TryGetComponent<IInteractible>(out interactibleObject))
                {
                    interactibleObject.Interact(gameObject);
                }
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            //Debug.Log($"OnTriggerStay2D collision is null = {collision is null} collision.gameObject.CompareTag(\"Enemy\")={collision.gameObject.CompareTag("Enemy")} collision.gameObject.name={collision.gameObject.name}");
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
            //Debug.Log($"OnTriggerExit2D collision is null = {collision is null} collision.gameObject.CompareTag(\"Enemy\")={collision.gameObject.CompareTag("Enemy")} collision.gameObject.name={collision.gameObject.name}");
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

            stateMachine = new PlayerStateMachine(statesArray, this);

            // For debug only
            if(GameManager.Instance is null || playerData.EveryKnowledgeDictionary is null)
            {
                playerData.InitializeData();
            }

            playerData.OnDeadDelegate += OnDead;
            playerData.OnOrderDelegate += OnOrderChange;
            InitializeKnowledges();
            Debug.Log($"Player's Y={transform.position.y}");
            SetHighestAirborneY(true);

            attackCoolDownStartTime = Time.time;
        }

        private void OnDestroy()
        {
            playerData.OnDeadDelegate -= OnDead;
            playerData.OnOrderDelegate -= OnOrderChange;
        }

        private void FixedUpdate()
        {
            SetAirborneInfo();

            frontWall = null;
            //SetFrontWallInfo();

            beneathObject = null;
            SetBeneathObjectInfo();

            stateMachine.Update();           
        }

        private void InitializeKnowledges()
        {
            foreach (var knowledge in playerData.EveryKnowledgeDictionary)
            {
                knowledge.Value.Initialize(this);
            }

            //playerData.KnownKnowledgeDictionary[KnowledgeID.DASH] = true;
            //playerData.KnownKnowledgeDictionary[KnowledgeID.WALL_SLIDE] = true;
            //playerData.KnownKnowledgeDictionary[KnowledgeID.DOUBLE_JUMP] = true;
            //playerData.KnownKnowledgeDictionary[KnowledgeID.GROUND_SLIDE] = true;
            //playerData.KnownKnowledgeDictionary[KnowledgeID.COMBO_ATTACK] = true;
            //playerData.KnownKnowledgeDictionary[KnowledgeID.AOE_ATTACK] = true;
            //playerData.AvailableKnowledgeDictionary[KnowledgeID.DASH] = AvailableKnowledgePosition.POSITION1;
            //playerData.AvailableKnowledgeDictionary[KnowledgeID.WALL_SLIDE] = AvailableKnowledgePosition.POSITION2;
            //playerData.AvailableKnowledgeDictionary[KnowledgeID.DOUBLE_JUMP] = AvailableKnowledgePosition.POSITION4;
            //playerData.AvailableKnowledgeDictionary[KnowledgeID.GROUND_SLIDE] = AvailableKnowledgePosition.POSITION3;
            //playerData.AvailableKnowledgeDictionary[KnowledgeID.COMBO_ATTACK] = AvailableKnowledgePosition.POSITION3;
            //playerData.AvailableKnowledgeDictionary[KnowledgeID.AOE_ATTACK] = AvailableKnowledgePosition.POSITION3;

            //playerData.GetKnowledgeByID(KnowledgeID.DASH).Activate();
            //playerData.GetKnowledgeByID(KnowledgeID.WALL_SLIDE).Activate();
            //playerData.GetKnowledgeByID(KnowledgeID.DOUBLE_JUMP).Activate();
            //playerData.GetKnowledgeByID(KnowledgeID.GROUND_SLIDE).Activate();

            //playerData.GetKnowledgeByID(KnowledgeID.COMBO_ATTACK).Activate();
            //playerData.GetKnowledgeByID(KnowledgeID.AOE_ATTACK).Activate();            

            // Re-activate equipped knowledges
            foreach (var knowledge in playerData.EveryKnowledgeDictionary)
            {
                if (playerData.AvailableKnowledgeDictionary[knowledge.Key] 
                    != AvailableKnowledgePosition.NOT_AVAILABLE)
                {
                    knowledge.Value.Activate();
                }
            }
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
    }
}
