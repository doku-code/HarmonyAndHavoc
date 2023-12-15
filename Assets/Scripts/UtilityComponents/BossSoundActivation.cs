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
        bool isBossDead = GameManager.Instance.data.CurrentPlayerMapProgression[GameManager.Instance.currentMap];

        if (other.CompareTag("Player") && other is CapsuleCollider2D)
        {
            if (!isBossDead && !isBossSoundPlaying && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayAmbientClip(MapManager.Instance.levelAudio.LevelBossAmbient);
                isBossSoundPlaying = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other is CapsuleCollider2D)
        {
            if (isBossSoundPlaying && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayAmbientClip(MapManager.Instance.levelAudio.LevelAmbient);
                isBossSoundPlaying = false;
            }
        }
    }
}
