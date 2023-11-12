using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace AF
{
    public class SoundManager : MonoBehaviour
    {
        [NonSerialized] public string sMasterVolume   = "Master";
        [NonSerialized] public string sFXVolume       = "FX";
        [NonSerialized] public string sAmbientVolume  = "Ambient";

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource clipPlayerSource;
        [SerializeField] private AudioClip[] clipsToPlay;
        [SerializeField] private AudioClip[] npcSounds;
        
        public static SoundManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        
        public void SetAmbientVolume(float volume)
        {
            audioMixer.SetFloat(sAmbientVolume, volume);
            SaveSoundSettings(sAmbientVolume, volume);
        }

        public void SetFXVolume(float volume)
        {
            audioMixer.SetFloat(sFXVolume, volume);
            SaveSoundSettings(sFXVolume, volume);
        }

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat(sMasterVolume, volume);
            SaveSoundSettings(sMasterVolume,volume);
        }

        public void PlayAClip(int index)
        {
            clipPlayerSource.clip = clipsToPlay[index];
            clipPlayerSource.Play();
        }
        
        public void PlayNpcSounds(int index)
        {
            clipPlayerSource.clip = npcSounds[index];
            clipPlayerSource.Play();
        }
        
        public void SaveSoundSettings(string settingName, float value)
        {
            PlayerPrefs.SetFloat(settingName, value);
        }
        
        public void LoadSoundSetting(Slider sliderToSet, string settingName)
        {
            sliderToSet.value = PlayerPrefs.GetFloat(settingName);
        }
    }
}
