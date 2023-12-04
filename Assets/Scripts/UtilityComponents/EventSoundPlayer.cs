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
        JUMPING
    }

    [Serializable]
    public struct SoundGroup
    {
        public SoundGroupID id;        
        public AudioClip[] sounds;
    }

    public class EventSoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundGroup[] soundGroups;        

        private AudioSource soundSource;

        private void Start()
        {
            soundSource = GetComponent<AudioSource>();

            PlayerController player = GetComponent<PlayerController>();
            if(player is not null )
            {
                player.LandedEvent += OnLand;
                player.JumpedEvent += OnJump;
            }
        }

        private void OnLand(int value)
        {
            if (value == 0)
            {
                PlaySound(SoundGroupID.LANDING, 0);
            }
            else
            {
                PlayRandomSound(SoundGroupID.LANDING, 1, 3);
            }
        }

        private void OnJump()
        {
            PlaySound(SoundGroupID.JUMPING, 0);
        }
        
        public void PlaySound(SoundGroupID soundGroupID, int soundID)
        {
            SoundGroup group = soundGroups.First(x => x.id == soundGroupID);
            try
            {
                soundSource.clip = group.sounds[soundID];
                soundSource.Play();
            }
            catch(Exception e)
            {

            }
        }

        public void PlayRandomSound(SoundGroupID soundGroupID)
        {
            if (soundGroupID == SoundGroupID.ATTACKS)
                return;

            SoundGroup group = soundGroups.First(x => x.id == soundGroupID);
            int soundID = Random.Range(0, group.sounds.Length);
            //try
            //{
                soundSource.clip = group.sounds[soundID];
                soundSource.Play();
            /*}
            catch (Exception e)
            {

            }*/
        }

        public void PlayRandomSound(SoundGroupID soundGroupID, int fromID, int toID)
        {
            SoundGroup group = soundGroups.First(x => x.id == soundGroupID);
            int soundID = Random.Range(fromID, toID + 1);
            try
            {
                soundSource.clip = group.sounds[soundID];
                soundSource.Play();
            }
            catch( Exception e)
            {

            }
        }
    }
}