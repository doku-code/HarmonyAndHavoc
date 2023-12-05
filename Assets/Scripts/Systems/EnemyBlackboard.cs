using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBlackboard", menuName = "EnemyBlackboard")]
public class EnemyBlackboard : ScriptableObject
{
    public float lastHitTime;
    public RectTransform healthBar;

    public EnemyBlackboard Clone()
    {
        return Instantiate(this);
    }
}
