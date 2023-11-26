using AF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JFM
{
    public enum SoundGroupID
    {
        FOOTSTEPS,
        ATTACKS
    }

    [Serializable]
    public struct SoundGroup
    {
        public SoundGroupID id;
        public int fromSoundID;
        public int toSoundID;
    }

    public class EventSoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundGroup[] soundGroups;

        public void PlaySound(int soundID)
        {
            SoundManager.Instance.PlayAClip(soundID);
        }

        public void PlayRandomSound(SoundGroupID soundGroupID)
        {
            SoundGroup group = soundGroups.First(x => x.id == soundGroupID);
            int soundID = Random.Range(group.fromSoundID, group.toSoundID);
            SoundManager.Instance.PlayAClip(soundID);
        }
    }
}