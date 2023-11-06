using AF;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
//using static UnityEngine.InputManagerEntry;

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
        public StairsClimbingUpState stairsClimbingUpState;
        public StairsClimbingDownState stairsClimbingDownState;
        public LandingState landingState;

        [SerializeField] private float walkSpeed = 1.0f;
        [SerializeField] private float walkAcceleration = 10.0f;
        //[SerializeField] private float runSpeed = 4.0f;
        [SerializeField] private float ladderSpeed = 3.0f;
        [SerializeField] private float ladderAcceleration = 10.0f;
        //[SerializeField] private float ladderGettingUpForce = 1.0f;
        [SerializeField] private float ladderPushUpForce = 2.0f;

        //[SerializeField] private float stairsUpDistanceLow = 0.6f;
        [SerializeField] private float stairsUpDistanceHigh = 0.8f;
        [SerializeField] private float stairsUpDistanceHigh2 = 0.8f;
        [SerializeField] private float stairsUpRayLength = 0.1f;
        [SerializeField] private float stairsUpMinHeight = 0.0f;
        [SerializeField] private float stairsUpHeight = 0.2f;
        [SerializeField] private float stairsUpHeight2 = 0.2f;
        [SerializeField] private float stairsUpDeceleration = 2f;
        [SerializeField] private Vector2 stairsUpFinishTranslate = new Vector2(1.0f, -0.01f);
        [SerializeField] private float stairsUpMinSlope = 0.4f;
        [SerializeField] private float stairsUpMaxSlope = 2.0f;

        [SerializeField] private float stairsDownDistanceLow = 0.6f;
        [SerializeField] private float stairsDownDistanceHigh = 0.8f;
        [SerializeField] private float stairsDownRayLength = 0.1f;
        [SerializeField] private float stairsDownMinHeight = 0.0f;
        [SerializeField] private float stairsDownHeight = 0.2f;
        [SerializeField] private float stairsDownGroundX = 0.3f;
        [SerializeField] private float stairsDownDeceleration = 0.3f;
        // In degrees
        [SerializeField] private float stairsUpAngle = 45.0f;
        [SerializeField] private float stairsDownAngle = 70.0f;
        [SerializeField] private float stairsGroundDistance = 0.2f;
        [SerializeField] private float stairsSpeed = 30.0f;
        [SerializeField] private float stairsAcceleration = 1000.0f;
        [SerializeField] private float jumpForce = 3.0f;
        [SerializeField] private int baseNumJumps = 1;
        [SerializeField] private float airAcceleration = 100.0f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask ladderLayer;

        [SerializeField] private float defaultGravityScale;
        [SerializeField] private float groundDistance = 1.0f;
        [SerializeField] private Vector2 groundBoxSize = new Vector2(0.95f, 0.01f);
        [SerializeField] private float wallDistance = 0.4f;
        [SerializeField] private float wallJumpDuration = 1.0f;
        [SerializeField] private float wallJumpForce = 1.0f;
        [SerializeField] private float ledgeAnimationDuration = 1.0f;
        [SerializeField] private float dashForce = 3.0f;
        [SerializeField] private float groundDashForce = 60.0f;
        [SerializeField] private float groundDashDeceleration = 1000.0f;
        [SerializeField] private float groundDashBailOutNormalizedTime = 0.4f;
        [SerializeField] private float groundDashAcceleration = 500.0f;
        // In degrees
        [SerializeField] private float groundDashAngle = 0.0f;
        // In degrees
        [SerializeField] private float dashDiagAngle = 45.0f;
        [SerializeField] private float wallGripForce = 1000.0f;
        // In degrees
        [SerializeField] private float wallJumpAngle = 45.0f;
        [SerializeField] private float wallGripVelocityTolerance = 0.5f;
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

        private RaycastHit2DInfo hitInfo;

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
        public Collision2D collision;

        public bool IsEventGrounded
        {
            get => isEventGrounded;
        }

        public float GroundDistance
        {
            get => groundDistance;
        }

        public Vector2 GroundDirection
        {
            get => groundDirection;
        }

        public Vector2 GroundDirection2
        {
            get => groundDirection2;
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
        /*
        public float LadderGettingUpForce
        {
            get => ladderGettingUpForce;
        }
        */
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

        public float StairsUpDistanceHigh2
        {
            get => stairsUpDistanceHigh2;
        }

        public float StairsUpHeight
        {
            get => stairsUpHeight;
        }

        public float StairsUpHeight2
        {
            get => stairsUpHeight2;
        }

        public Vector2 StairsUpFinishTranslate
        {
            get => stairsUpFinishTranslate;
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

        public float StairsDownMinHeight
        {
            get => stairsDownMinHeight;
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

        public float StairsUpDeceleration
        {
            get => stairsUpDeceleration;
        }

        public float StairsUpAngle
        {
            get => stairsUpAngle;
        }

        public float StairsDownAngle
        {
            get => stairsDownAngle;
        }

        public float StairsDownGroundX
        {
            get => stairsDownGroundX;
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
            bool grounded = IsCastGrounded(); // && rb.velocity.y < 0.0f;
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
                Vector3 v = new Vector3((IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * wallJumpForce * 1.0f;
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

            if (highestY - transform.position.y > heightDamage)
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
            Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;

            if ((FindSlopeAtPoint(out float slope, v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight, v) && Mathf.Abs(slope) > stairsUpMinSlope && Mathf.Abs(slope) < stairsUpMaxSlope) ||
                Raycast(false, groundLayer, Vector2.up * 0.0f + (isFacingRight ? Vector2.right : -Vector2.right) * 2.5f * colliderSize.x, 0.01f, (isFacingRight ? Vector2.right : -Vector2.right), false, false))
            {
                return false;
            }

            int knowledgeIndex = (int)playerData.AvalaibleKnowledgeDictionary[KnowledgeID.DASH];

            return !hasDashed && knowledgeIndex > 0 && inputTriggers[knowledgeInputNames[knowledgeIndex - 1]] && moveInput.x != 0.0f;
        }

        public void Dash()
        {
            hasDashed = true;

            if (CanTurn())
            {
                Turn();
            }

            Vector3 v;
            
            float angle = groundDashAngle * Mathf.Deg2Rad;
            v = new Vector3((IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * groundDashForce * groundDashAcceleration * Time.fixedDeltaTime;
            rb.AddForce(v, ForceMode2D.Force);            

            dashDirection = moveInput.x * Vector2.right;
        }

        public void ContinueDash()
        {
            Vector3 v;
            
            float angle = groundDashAngle * Mathf.Deg2Rad;
            v = new Vector3((IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * groundDashForce;

            rb.AddForce(v, ForceMode2D.Force);
            
        }

        private void SetFrontWallInfo()
        {
            if (frontWall is null)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + new Vector2(0, colliderSize.y / 2.0f), isFacingRight ? Vector2.right : -Vector2.right, wallDistance, groundLayer);
                //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y), (isFacingRight ? Vector2.right : -Vector2.right) * wallDistance, Color.yellow);

                if (hit.collider is null)
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
            bool h2 = Raycast(false, ladderLayer, Vector2.down * groundDistance, 0.01f, Vector2.down);//, false ,true);

            //Debug.Log($"h2={h2}");

            return moveInput.x == 0.0f && moveInput.y < 0.0f && h2;
        }

        public bool WillGripToWall()
        {
            SetFrontWallInfo();

            bool front = frontWall is not null && (1 << frontWall.layer) == (int)groundLayer && ((isFacingRight && moveInput.x > 0) || (!isFacingRight && moveInput.x < 0));
            bool back = false;

            if (!front)
            {
                // Check also back wall 
                bool backWallHit = Raycast(false, groundLayer, Vector2.zero, wallDistance, isFacingRight ? -Vector2.right : Vector2.right);
                back = backWallHit && ((isFacingRight && moveInput.x < 0) || (!isFacingRight && moveInput.x > 0));
            }
            //Debug.Log($"({front} || {back}) && {Mathf.Abs(rb.velocity.x) <= wallGripVelocityTolerance} rb.velocity.x={rb.velocity.x} moveInput.x={moveInput.x}");
            return (front || back) && Mathf.Abs(rb.velocity.x) <= wallGripVelocityTolerance;
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

        public bool WillClimbUpStairs()
        {
            Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;
            
            if (FindSlopeAtPoint(out float slope, v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight, v))
            {
                //Debug.Log($"ClimbingUpStairs slope={slope}");
                return moveInput.x != 0.0f && Mathf.Abs(slope) > stairsUpMinSlope && Mathf.Abs(slope) <= stairsUpMaxSlope;
            }
            else
            {
                return false;
            }
        }

        public bool WillClimbDownStairs()
        {
            Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;

            bool stairsDown = FindSlopeBeneath(out float slope);//, Vector2.up * 0.02f + v * stairsDownGroundX, -v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight, true);

            bool ret = stairsDown && Mathf.Abs(slope) > stairsUpMinSlope && Mathf.Abs(slope) < stairsUpMaxSlope && Mathf.Abs(rb.velocity.y) <= 0.01f && ((moveInput.x > 0.0f && slope < 0) || (moveInput.x > 0.0f && slope < 0));
            //Debug.Log($"stairsDown={stairsDown} slope={slope} ret={ret} rb.velocity.y={rb.velocity.y} {Mathf.Abs(slope) > stairsUpMinSlope} && {Mathf.Abs(slope) < stairsUpMaxSlope} && {Mathf.Abs(rb.velocity.y) <= 0.01f}");
            return ret;
        }

        public static bool AreNearlyEqual(float f1, float f2)
        {
            return Mathf.Abs(f2 - f1) <= 0.0001f;
        }

        public int stairsDown = 0;

        public bool FindSlopeBeneath(out float slope)
        {
            Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;
            return FindSlopeBeneath(out slope, Vector2.up * 0.02f + v * colliderSize.x / 2.0f + v * stairsDownGroundX, - v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight);
        }

        public bool FindSlopeBeneath(out float slope, Vector2 offset1, Vector2 offset2)
        {
            return FindSlopeBeneath(out slope, offset1, offset2, false);
        }

        public bool FindSlopeBeneath(out float slope, Vector2 offset1, Vector2 offset2, bool willDraw)
        {

            bool retValue = false;
            slope = 0;
            //bool foundStairsBeneath = false;
            //bool stairsDirectionIsRight = false;
            //Vector2 offset2 = v * colliderSize.x / 2.0f + v * stairsDownGroundX;

            retValue = FindSlopeAtPoint(out slope, offset1, Vector2.down, willDraw);

            if (Mathf.Abs(slope) < stairsUpMinSlope)
            {
                //offset2 = -v * stairsUpDistanceHigh + Vector2.up * stairsUpHeight;
                retValue = FindSlopeAtPoint(out slope, offset2, Vector2.down, willDraw);
            }

            return retValue;
        }

        private Vector2 GetPerpendicularVector2(Vector2 source)
        {
            float angleRadians = Mathf.PI / 2.0f;
            return new Vector2(
                source.x * Mathf.Cos(angleRadians) - source.y * Mathf.Sin(angleRadians),
                source.x * Mathf.Sin(angleRadians) + source.y * Mathf.Cos(angleRadians)
            );
        }

        public bool FindSlopeAtPoint(out float slope, Vector2 offset, Vector2 direction)
        {
            return FindSlopeAtPoint(out slope, offset, direction, false);
        }

        public bool FindSlopeAtPoint(out float slope, Vector2 offset, Vector2 direction, bool willDraw)
        {
            Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;
            bool retValue = false;
            slope = 0;
            //bool foundStairsBeneath = false;
            //bool stairsDirectionIsRight = false;
            RaycastHit2D hit = Physics2D.Raycast(rb.position + offset, direction, StairsDownHeight, GroundLayer);
            if (willDraw)
            {
                Debug.DrawRay(transform.position + new Vector3(offset.x, offset.y, transform.position.z), direction * StairsDownHeight, Color.yellow);
            }
            Vector3 vr = transform.position + new Vector3(offset.x, offset.y, transform.position.z);
            if (hit.collider is not null)
            {
                Vector2 perpendicularDirection = GetPerpendicularVector2(direction);
                RaycastHit2D hit2 = Physics2D.Raycast(rb.position + perpendicularDirection * 0.02f + offset, direction, StairsDownHeight, GroundLayer);
                if (willDraw)
                {
                    Debug.DrawRay(transform.position + new Vector3(perpendicularDirection.x, perpendicularDirection.y, transform.position.z) * 0.02f + new Vector3(offset.x, offset.y, transform.position.z), direction * StairsDownHeight, Color.yellow);
                }
                Vector3 vr2 = transform.position + new Vector3(perpendicularDirection.x, perpendicularDirection.y, transform.position.z) * 0.02f + new Vector3(offset.x, offset.y, transform.position.z) - vr;

                //Debug.Log($"vr2 ={vr2} perpendicularDirection={perpendicularDirection} direction={direction}");
                if (hit2.collider is not null)
                {
                    float diffY = hit2.point.y - hit.point.y;
                    float diffX = hit2.point.x - hit.point.x;

                    if (diffX == 0.0f)
                    {
                        slope = Mathf.Infinity;
                    }
                    else
                    {
                        slope = diffY / diffX; //isFacingRight ? 1 : -1;
                    }

                    retValue = true;
                    //Debug.Log($"diffY = {diffY} diffX = {diffX} {hit2.point.y} - {hit.point.y}");
                }
            }
            //Debug.Log($"With offset ({offset}) stairsDown={retValue}");
            //stairsDown = retValue;

            return retValue;
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
            return IsCastGrounded(limitToRay, layerMask, offset, groundDistance);
        }

        public bool IsCastGrounded(bool limitToRay, int layerMask, Vector2 offset, float distance)
        {
            RaycastHit2D[] hits;
            if (limitToRay)
            {
                hits = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y) + offset, Vector2.down, distance, layerMask);
            }
            else
            {
                hits = Physics2D.BoxCastAll(new Vector2(transform.position.x, transform.position.y) + offset, groundBoxSize, 0.0f, Vector2.down, distance, layerMask);
            }
            bool groundFound = false;
            for (int i = 0; i < hits.Length && !groundFound; i++)
            {
                //Debug.Log($"i={i} hits[i].collider is not null={hits[i].collider is not null} normal ={hits[i].normal} layer={hits[i].collider.gameObject.layer} (int)LadderLayer={(int)LadderLayer}");
                groundFound = hits[i].collider is not null && (hits[i].normal == Vector2.up || 1 << hits[i].collider.gameObject.layer == (int)LadderLayer);
                if (hits[i].collider is not null)
                {
                    groundedNormal = hits[i].normal;
                }
                if (groundFound)
                {
                    groundedLayer = hits[i].collider.gameObject.layer;
                    groundedPoint = hits[i].point;
                    groundedDistance = (new Vector2(transform.position.x, transform.position.y) + offset - hits[i].point).y;
                    
                }
            }

            return groundFound;// && rb.velocity.y < -0.001f;
        }

        public float groundedDistance;
        public int groundedLayer;
        public Vector2 groundedNormal;
        //public bool groundedOnLadder;
        public Vector2 groundedPoint;

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

        public bool IsGrounded()
        {
            return IsGrounded(true);
        }

        public bool IsGrounded(bool testVelocity)
        {
            bool directionFound = false;
            if (isEventGrounded)
            {
                for (int i = 0; i < collision.contactCount && !directionFound; i++)
                {
                    directionFound = IsVerticalDirection(collision.GetContact(i).normal);
                    //Debug.Log($"Searching direction={collision.GetContact(i).normal}");
                }
            }
            //return ((isEventGrounded && directionFound) || Raycast(false, groundLayer | ladderLayer, Vector2.zero, 0.2f)) && (rb.velocity.y < -0.001f || !testVelocity);
            //Debug.Log($"{isEventGrounded} && {directionFound} && ({rb.velocity.y < -0.001f} || {!testVelocity})");
            //return isEventGrounded && directionFound && (rb.velocity.y < -0.001f || !testVelocity);
            return Raycast(false, groundLayer | ladderLayer, Vector2.zero, 0.2f) && (rb.velocity.y < -0.001f || !testVelocity);
        }

        public bool IsHorizontalDirection(Vector2 direction)
        {
            return direction == Vector2.right || direction == Vector2.left;
        }

        public bool IsVerticalDirection(Vector2 direction)
        {
            Vector2 differenceUp = direction - Vector2.up;
            Vector2 differenceDown = direction - Vector2.down;
            return (Mathf.Abs(differenceUp.x) < 0.01f && Mathf.Abs(differenceUp.y) < 0.01f) ||
                (Mathf.Abs(differenceDown.x) < 0.01f && Mathf.Abs(differenceDown.y) < 0.01f);
            //return direction == Vector2.up || direction == Vector2.down;
        }
        
        // Event "isGrounded" is used for edge cases that aren't correctly covered by ray- or boxcasts. For example, 
        // the sprite boxcast size ("GroundBoxSize") cannot be too wide (too close to 1) as it breaks the WallSlide cast.
        public void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log("OnCollisionEnter2D() was called.");
            groundDirection = collision.GetContact(0).normal;
            //Debug.Log($"groundDirection={collision.GetContact(0).normal} groundPoint={collision.GetContact(0).point}");
            if (collision.contactCount > 1)
            {
                groundDirection2 = collision.GetContact(1).normal;
                for (int i = 1; i < collision.contactCount; i++)
                {
                    //Debug.Log($"groundDirection{i+1}={collision.GetContact(i).normal} groundPoint{i + 1}={collision.GetContact(i).point}");
                }
            }
            else
            {
                groundDirection2 = Vector2.zero;
            }
            
            isEventGrounded = true;
            this.collision = collision;
        }

        public void OnCollisionStay2D(Collision2D collision)
        {
            //Debug.Log("OnCollisionStay2D() was called.");
            groundDirection = collision.GetContact(0).normal;
            //Debug.Log($"groundDirection={collision.GetContact(0).normal} groundPoint={collision.GetContact(0).point}");
            if (collision.contactCount > 1)
            {
                groundDirection2 = collision.GetContact(1).normal;
                for (int i = 1; i < collision.contactCount; i++)
                {
                    //Debug.Log($"groundDirection{i + 1}={collision.GetContact(i).normal} groundPoint{i + 1}={collision.GetContact(i).point}");
                }
            }
            else
            {
                groundDirection2 = Vector2.zero;
            }

            isEventGrounded = true;
            this.collision = collision;
        }


        public void OnCollisionExit2D(Collision2D collision)
        {
            //Debug.Log("OnCollisionExit2D() was called.");
            isEventGrounded = false;
        }
        
        void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            defaultGravityScale = rb.gravityScale;

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
            stairsClimbingUpState = new StairsClimbingUpState(animator, this);
            stairsClimbingDownState = new StairsClimbingDownState(animator, this);
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