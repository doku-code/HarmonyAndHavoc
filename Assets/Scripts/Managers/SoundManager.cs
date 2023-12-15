using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace AF
{
    public class SoundManager : MonoBehaviour
    {
        [NonSerialized] public string sMasterVolume   = "Master";
        [NonSerialized] public string sFXVolume       = "FX";
        [NonSerialized] public string sAmbientVolume  = "Ambient";

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource fxSource;
        [SerializeField] private AudioSource ambientSource;
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

        public void PlayFxClip(int index)
        {
            fxSource.PlayOneShot(clipsToPlay[index]);
        }

        public void PlayFxClip(AudioClip clip)
        {
            fxSource.PlayOneShot(clip);
        }

        public void PlayFullTimeFxClip(AudioClip clip)
        {
            if (!fxSource.isPlaying)
            {
                fxSource.PlayOneShot(clip);
            }
        }

        public void PlayAmbientClip(AudioClip clip)
        {
            ambientSource.loop = true;
            ambientSource.clip = clip;
            ambientSource.Play();
        }
        
        public void PlayNpcSounds(int index)
        {
            fxSource.clip = npcSounds[index];
            fxSource.Play();
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
