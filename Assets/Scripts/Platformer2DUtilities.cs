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

    }
}
