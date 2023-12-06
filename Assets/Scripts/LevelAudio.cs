using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelAudio", menuName = "LevelAudio")]
public class LevelAudio : ScriptableObject
{
    public AudioClip LevelAmbient;
    public AudioClip LevelBossAmbient;
}
