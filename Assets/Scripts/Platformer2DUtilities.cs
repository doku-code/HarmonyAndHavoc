using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class Platformer2DUtilities
    {
        public static Vector2 GetPerpendicularVector2(Vector2 source)
        {
            return RotateVector2(source, 90.0f);
        }

        public static Vector2 RotateVector2(Vector2 source, float angle)
        {
            float angleRadians = angle * Mathf.Deg2Rad;
            return new Vector2(
                source.x * Mathf.Cos(angleRadians) - source.y * Mathf.Sin(angleRadians),
                source.x * Mathf.Sin(angleRadians) + source.y * Mathf.Cos(angleRadians)
            );
        }

        public static bool AreNearlyEqual(float f1, float f2)
        {
            return Mathf.Abs(f2 - f1) <= 0.001f;
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
    }
}
