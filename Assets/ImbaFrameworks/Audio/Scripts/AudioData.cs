
using UnityEngine;


namespace Imba.Audio
{
    
    [System.Serializable]
    public class AudioData
    {
        public string audioName;

        public AudioType type;

        public AudioClip audioClip;

        [Range(0f, 1f)] public float volume;

        [Range(0, 256)] public int priority;

        [Range(0, 1)] public float spatialBlend;

        [HideInInspector] public AudioSource source;

        public bool isLooping;

        public bool playOnAwake;
        
        public AudioData()
        {
            volume = 1f;
            priority = 128;
            spatialBlend = 0;
            isLooping = false;
            playOnAwake = false;
        }

    }
}