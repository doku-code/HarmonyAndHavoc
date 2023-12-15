using UnityEngine;

namespace JFM
{
    public class Raycast2DHelper
    {
        public static bool CheckGrounded(Vector2 position, float radius, float distance, LayerMask layerMask, int groundLayer, out int hitLayer)
        {
            // detect downwards
            Vector2 direction;
            direction.x = 0;
            direction.y = -1;
            hitLayer = 0;

            RaycastHit2D[] hitRecs = Physics2D.CircleCastAll(position, radius, direction, distance, layerMask);
            //Debug.Log($"hitRecs.Length={hitRecs.Length}");
            //Platformer2DUtilities.DebugDrawCircle(position, radius, Color.yellow);
            if (hitRecs.Length > 0)
            {
                int i = 0;
                bool found = false;
                for (; i < hitRecs.Length; i++)
                {
                    if (1 << hitRecs[i].collider.gameObject.layer == groundLayer)
                    {
                        //Debug.Log($"groundLayer={groundLayer}");
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    i = 0;
                }
                //groundY = hitRecs[i].point.y;
                hitLayer = hitRecs[i].collider.gameObject.layer;
                //Debug.Log($"(2)hitLayer={hitLayer}");

                return true;
            }

            RaycastHit2D[] rayHitRecs = Physics2D.RaycastAll(position, direction, distance + radius, layerMask);
            //Debug.Log($"rayHitRecs.Length={rayHitRecs.Length}");

            if (rayHitRecs.Length > 0)
            {
                if (hitLayer == 0)
                {
                    int i = 0;
                    while (i < rayHitRecs.Length)
                    {
                        if (rayHitRecs[i].collider.gameObject.layer == groundLayer)
                        {
                            hitLayer = rayHitRecs[i].collider.gameObject.layer;
                            break;
                        }
                        //Debug.Log($"hitLayer={hitLayer}");
                        
                        i++;
                    }
                }

                return true;
            }            

            return false;
        }        

        public static bool FindSlopeBeneath(Vector2 position, out float slope, Vector2 offset1, Vector2 offset2, float distance, int groundLayer)
        {
            return FindSlopeBeneath(position, out slope, offset1, offset2, distance, groundLayer, 0.0f);
        }

        public static bool FindSlopeBeneath(Vector2 position, out float slope, Vector2 offset1, Vector2 offset2, float distance, int groundLayer, float angle)
        {

            return FindSlopeBeneath(position, out slope, offset1, offset2, distance, groundLayer, 0.0f, false);
        }

        public static bool FindSlopeBeneath(Vector2 position, out float slope, Vector2 offset1, Vector2 offset2, float distance, int groundLayer, float angle, bool willDraw)
        {
            bool retValue = false;
            slope = 0;

            retValue = FindSlopeAtPoint(position, out slope, offset1, Platformer2DUtilities.RotateVector2(Vector2.down, angle), distance, groundLayer, willDraw);
            //Debug.Log($"slope1={slope}");

            return retValue;
        }
        
        public static bool FindSlopeAtPoint(Vector2 point, out float slope, Vector2 offset, Vector2 direction, float distance, int groundLayer)
        {
            return FindSlopeAtPoint(point, out slope, offset, direction, distance, groundLayer, false);
        }

        public static bool FindSlopeAtPoint(Vector2 point, out float slope, Vector2 offset, Vector2 direction, float distance, int groundLayer, bool willDraw)
        {
            //Vector2 v = isFacingRight ? Vector2.right : -Vector2.right;
            bool retValue = false;
            slope = 0;
            //bool foundStairsBeneath = false;
            //bool stairsDirectionIsRight = false;
            RaycastHit2D hit = Physics2D.Raycast(point + offset, direction, distance, groundLayer);
            if (willDraw)
            {
                Debug.DrawRay(new Vector3(point.x, point.y, 0.0f) + new Vector3(offset.x, offset.y, 0.0f), direction * distance, Color.yellow);
            }
            Vector3 vr = new Vector3(point.x, point.y, 0.0f) + new Vector3(offset.x, offset.y, 0.0f);
            if (hit.collider is not null)
            {
                Vector2 perpendicularDirection = Platformer2DUtilities.GetPerpendicularVector2(direction);
                RaycastHit2D hit2 = Physics2D.Raycast(point + perpendicularDirection * 0.02f + offset, direction, distance, groundLayer);
                if (willDraw)
                {
                    Debug.DrawRay(new Vector3(point.x, point.y, 0.0f) + new Vector3(perpendicularDirection.x, perpendicularDirection.y, 0.0f) * 0.02f + new Vector3(offset.x, offset.y, 0.0f), direction * distance, Color.yellow);
                }
                Vector3 vr2 = new Vector3(point.x, point.y, 0.0f) + new Vector3(perpendicularDirection.x, perpendicularDirection.y, 0.0f) * 0.02f + new Vector3(offset.x, offset.y, 0.0f) - vr;

                //Debug.Log($"vr2 ={vr2} perpendicularDirection={perpendicularDirection} direction={direction}");
                if (hit2.collider is not null)
                {
                    float diffY = hit2.point.y - hit.point.y;
                    float diffX = hit2.point.x - hit.point.x;

                    if (Platformer2DUtilities.AreNearlyEqual(diffX, 0.0f))
                    {
                        slope = Mathf.Infinity;
                    }
                    else
                    {
                        slope = diffY / diffX; //isFacingRight ? 1 : -1;
                    }

                    retValue = true;
                    //Debug.Log($"diffY = {diffY} diffX = {diffX} {hit2.point.y} - {hit.point.y} slope={slope}");
                }
            }
            //Debug.Log($"With offset ({offset}) stairsDown={retValue}");
            //stairsDown = retValue;

            return retValue;
        }

        public static bool CheckForCollisions(Vector2 position,
                                        CapsuleCollider2D capsule,
                                        int layerMask,                                        
                                        bool debug
                                      )
        {         
            float distance = capsule.size.y - capsule.size.x;
            float radius = capsule.size.x / 2.0f;

            position = position +
                            Vector2.up * capsule.offset.y -
                            Vector2.up * (capsule.size.y / 2.0f - radius) +
                            Vector2.right * capsule.offset.x;
            
            if (debug)
            {
                Debug.Log($"position={position}");
            }

            RaycastHit2D collisionHit = Physics2D.CircleCast(
                position,
                radius,
                Vector2.up,
                distance,
                layerMask
            );
            if (debug)
            {
                Platformer2DUtilities.DebugDrawCircle(position, radius, Color.yellow);
                Platformer2DUtilities.DebugDrawCircle(position + Vector2.up * distance, radius, Color.yellow);

                //Debug.Break();
                if (collisionHit.collider is not null)
                {
                    Debug.Log($"collider.gameObject.layer={collisionHit.collider.gameObject.layer}");
                }
            }
            return collisionHit.collider is not null;
        }
    }
}
