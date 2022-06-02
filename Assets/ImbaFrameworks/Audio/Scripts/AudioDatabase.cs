using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Imba.Audio
{
    [Serializable]
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "Imba/Audio Database", order = 1)]
    public class AudioDatabase: ScriptableObject
    {
        [SerializeField]
        private List<AudioData> database;
 
        public List<AudioData> Database => database;
        
        public void Reset()
        {
            database = new List<AudioData>
            {
                new AudioData()
            };
        }
    }

}