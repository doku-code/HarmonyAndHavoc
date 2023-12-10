using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace JFM
{
    public class Platformer2DUtilities
    {



        public static Vector2 GetPerpendicularVector2(Vector2 source)
        {
            return RotateVector2(source, 90.0f);
        }

        public static Vector2 RotateVector2(Vector2 source, float angleDegrees)
        {
            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            return new Vector2(
                source.x * Mathf.Cos(angleRadians) - source.y * Mathf.Sin(angleRadians),
                source.x * Mathf.Sin(angleRadians) + source.y * Mathf.Cos(angleRadians)
            );
        }

        public static bool AreNearlyEqual(float f1, float f2)
        {
            return Mathf.Abs(f2 - f1) <= 0.001f;
        }

        public static bool AreNearlyEqual(Vector2 v1, Vector2 v2)
        {
            return AreNearlyEqual(v1.x, v2.x) && AreNearlyEqual(v1.y, v2.y);
        }

        public static void DebugDrawCircle(Vector2 position, float radius, Color color)
        {
            int numSegments = 16;
            float radSubdivisions = Mathf.PI * 2 / numSegments;

            Vector2 lastPoint = new Vector2(radius * Mathf.Cos(0), radius * Mathf.Sin(0));
            Vector2 point;
            for (float curAngle = 0; curAngle < Mathf.PI * 2; curAngle += radSubdivisions)
            {
                point = new Vector2(radius * Mathf.Cos(curAngle + radSubdivisions), radius * Mathf.Sin(curAngle + radSubdivisions));
                //Debug.Log("v0 = " + v0);
                Debug.DrawLine(position + lastPoint, position + point, color);
                lastPoint = point;
            }
        }

        public static Vector2 RoundVector2Angle(Vector2 vector, float roundingAngle)
        {
            float angle = Mathf.Atan2(vector.y, vector.x);            
            float quarter = angle / roundingAngle;
            quarter = Mathf.Round(quarter);
            float newAngle = quarter * roundingAngle;            
            return new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
        }

        public static Color HexStringToColor(string hexString)
        {
            int r, g, b;

            try
            {
                int hex = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);

                b = 0xff & hex;
                hex >>= 8;
                g = 0xff & hex;
                hex >>= 8;
                r = 0xff & hex;
            }
            catch(Exception e) 
            {
                return Color.white;
            }
            
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
        }

        public static float GetClipLength(Animator animator, string name)
        {
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == name)
                {
                    return clip.length;
                }
            }

            return 0.0f;
        }

        public static void LimitVelocity(Rigidbody2D rb, float maxVelocity)
        {
            if (rb.velocity.magnitude > maxVelocity)
            {
                /*if (rb.velocity.magnitude > 15.0f)
                {
                    Debug.Break();
                }*/
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
        }

        public static Vector2 CalculateGroundDifference(Vector2 position1, Vector2 offset1, Vector2 position2, Vector2 offset2)
        {
            /*Vector3 collisionOffset = new Vector3(offset1.x, offset1.y, 0.0f);
            Vector3 playerColliderOffset = new Vector3(offset2.x, offset2.y, 0.0f);*/
            Vector2 pushDirection = position1 + offset1 - (position2 + offset2);
            return RoundVector2Angle(pushDirection, Mathf.PI / 4.0f);

        }

        public static Vector2 GetReflectedVector2(Vector2 vector, Vector2 normal)
        {
            Vector2 inversedDirection = -vector;
            float dot = Vector2.Dot(inversedDirection, normal);
            Vector2 v = (normal * dot - inversedDirection) * 2.0f;
            return inversedDirection + v;
        }
    }
}
