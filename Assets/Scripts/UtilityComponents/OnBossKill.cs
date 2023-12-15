using System.Collections;
using AF;
using charles;
using JFM;
using UnityEngine;

public class OnBossKill : MonoBehaviour
{
    [SerializeField] private float waitDelay = 2.0f;
    private EnemyController controller;

    void Awake()
    {
        if (GameManager.Instance.data.CurrentPlayerMapProgression[GameManager.Instance.currentMap])
        {
            Destroy(gameObject);
            return;
        }

        controller = GetComponent<EnemyController>();
        controller.OnDeadDelegate += OnKill;
    }

    private void OnKill()
    {
        SoundManager.Instance.PlayAmbientClip(MapManager.Instance.levelAudio.LevelAmbient);
        StartCoroutine(OnKillCoroutine());
    }

    private IEnumerator OnKillCoroutine()
    {
        yield return new WaitForSeconds(waitDelay);
                
        MapManager.Instance.UnlockDoor();        
    }
}
