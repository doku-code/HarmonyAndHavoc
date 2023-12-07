using System;
using System.Collections;
using System.Collections.Generic;
using AF;
using UnityEngine;

public class BossSoundActivation : MonoBehaviour
{
    private bool isBossSoundPlaying;
    private MapManager mapManager;

    void Awake()
    {
        mapManager = transform.parent.gameObject.GetComponent<MapManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isBossSoundPlaying && SoundManager.Instance != null)
            SoundManager.Instance.PlayAmbientClip(mapManager.levelAudio.LevelBossAmbient);
        isBossSoundPlaying = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(isBossSoundPlaying && SoundManager.Instance != null)
            SoundManager.Instance.PlayAmbientClip(mapManager.levelAudio.LevelAmbient);
        isBossSoundPlaying = false;
    }
}
