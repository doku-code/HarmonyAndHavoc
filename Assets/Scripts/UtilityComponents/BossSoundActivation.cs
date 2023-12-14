using System;
using System.Collections;
using System.Collections.Generic;
using AF;
using UnityEngine;

public class BossSoundActivation : MonoBehaviour
{
    private bool isBossSoundPlaying;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isBossSoundPlaying && SoundManager.Instance != null)
            SoundManager.Instance.PlayAmbientClip(MapManager.Instance.levelAudio.LevelBossAmbient);
        isBossSoundPlaying = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(isBossSoundPlaying && SoundManager.Instance != null)
            SoundManager.Instance.PlayAmbientClip(MapManager.Instance.levelAudio.LevelAmbient);
        isBossSoundPlaying = false;
    }
}
