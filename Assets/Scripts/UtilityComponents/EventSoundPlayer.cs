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
        ATTACKS,
        LANDING,
        JUMPING,
        HURTING
    }

    [Serializable]
    public class SoundGroup
    {
        public SoundGroupID id;        
        public AudioClip[] sounds;
    }

    public class EventSoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundGroup[] soundGroups;        

        private void Start()
        {
            PlayerController player = GetComponent<PlayerController>();
            if(player is not null )
            {
                player.LandedEvent += OnLand;
                player.JumpedEvent += OnJump;
                player.IsHurtEvent += OnIsHurt;
            }
        }

        private void OnLand(int value)
        {
            //if (value == 0)
            //{
            //PlaySound(SoundGroupID.LANDING);
            Debug.Log($"Land ={value}");
            SoundGroupID soundGroupID = SoundGroupID.LANDING;
            SoundGroup group = soundGroups.First(x => x.id == soundGroupID);
            SoundManager.Instance.PlayFullTimeFxClip(group.sounds[0]);
            /*}
            else
            {
                PlayRandomSound(SoundGroupID.LANDING, 1, 3);
            }*/
        }

        private void OnJump()
        {
            PlaySound(SoundGroupID.JUMPING);
        }

        private void OnIsHurt(int value)
        {
            PlaySound(SoundGroupID.HURTING);
        }

        // Plays the first sound in that sound group
        public void PlaySound(SoundGroupID soundGroupID)
        {
            PlaySound(soundGroupID, 0);
        }

        public void PlaySound(SoundGroupID soundGroupID, int soundID)
        {
            SoundGroup group = soundGroups.First(x => x.id == soundGroupID);
            try
            {
                SoundManager.Instance.PlayFxClip(group.sounds[soundID]);
            }
            catch(Exception e)
            {
                //Debug.Log($"PlaySound(SoundGroupID, int) throwed an exception: {e.Message}");
            }
        }

        public void PlayRandomSound(SoundGroupID soundGroupID)
        {
            SoundGroup group = soundGroups.First(x => x.id == soundGroupID);
            int soundID = Random.Range(0, group.sounds.Length);
            try
            {
                SoundManager.Instance.PlayFxClip(group.sounds[soundID]);
            }
            catch (Exception e)
            {
                //Debug.Log($"PlayRandomSound(SoundGroupID) throwed an exception: {e.Message}");
            }
        }

        public void PlayRandomSound(SoundGroupID soundGroupID, int fromID, int toID)
        {
            SoundGroup group = soundGroups.First(x => x.id == soundGroupID);
            int soundID = Random.Range(fromID, toID + 1);
            try
            {
                SoundManager.Instance.PlayFxClip(group.sounds[soundID]);
            }
            catch( Exception e)
            {
                //Debug.Log($"PlayRandomSound(SoundGroupID, int, int) throwed an exception: {e.Message}");
            }
        }
    }
}