using UnityEngine;
using UnityEngine.Audio;

namespace AF
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        //[SerializeField] private AudioMixerGroup fxGroup;
        
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
            audioMixer.SetFloat("Ambient", Mathf.Log10(volume) * 20);
        }

        public void SetFXVolume(float volume)
        {
            audioMixer.SetFloat("FX", Mathf.Log10(volume) * 20);
        }

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }
        
        /*public void SetAmbientSound(AudioClip clip)
        {
            audioMixer.SetFloat("Ambient", Mathf.Log10(volume) * 20);
        }

        public void SetFXSound(AudioClip clip)
        {
            audioMixer.SetFloat("FX", Mathf.Log10(volume) * 20);
        }

        public void SetMasterSound(AudioClip clip)
        {
            audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }*/
    }
}
