using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBlackboard", menuName = "EnemyBlackboard")]
public class EnemyBlackboard : ScriptableObject
{
    public float lastHitTime;
    public RectTransform healthBar;
    public float lastSeenPlayer;
    public float playerForgetTime;

    private void OnEnable()
    {
        lastSeenPlayer = -999.0f;
        playerForgetTime = -999.0f;
    }

    public EnemyBlackboard Clone()
    {
        return Instantiate(this);
    }
}
