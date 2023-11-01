using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace AF
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource clipPlayerSource;
        [SerializeField] private AudioClip[] ClipsToPlay;
        
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
            audioMixer.SetFloat("Ambient", volume);
        }

        public void SetFXVolume(float volume)
        {
            audioMixer.SetFloat("FX", volume);
        }

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("Master", volume);
        }

        public void PlayAClip(int index)
        {
            clipPlayerSource.clip = ClipsToPlay[index];
            clipPlayerSource.Play();
        }
    }
}
