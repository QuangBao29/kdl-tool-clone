using System;
using System.Collections.Generic;
using UnityEngine;


namespace Imba.Audio
{
    [Serializable]
    [CreateAssetMenu(fileName = "AudioDataManager", menuName = "Imba/AudioDataManager", order = 1)]
    public class AudioDataManager: ScriptableObject
    {
        
        private const string AUDIO_DATA_MANAGER_FILE_NAME = "AudioDataManager";
        //private const string AUDIO_DATA_MANAGER_PATH = "ImbaFrameworks/Audio/Resources/";
        
        [SerializeField]
        private List<AudioDatabase> database;

        public List<AudioDatabase> Database => database;

        private static AudioDataManager _instance;
        
        public static AudioDataManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance =  (AudioDataManager) Resources.Load(AUDIO_DATA_MANAGER_FILE_NAME, typeof(AudioDataManager));
                
//                #if UNITY_EDITOR
                        //                if (_instance == null)
                        //                {
                        //                    string simpleResourcesPath = AUDIO_DATA_MANAGER_PATH + AUDIO_DATA_MANAGER_FILE_NAME;
                        //                    _instance = (AudioDataManager) Resources.Load(simpleResourcesPath, typeof(AudioDataManager));
                        //                }
                        //                #endif
                
                if(_instance == null) Debug.LogError("Cannot load audiodatamanager " + AUDIO_DATA_MANAGER_FILE_NAME);
                
                return _instance;
            }
        }

       


#if UNITY_EDITOR

        private List<string> _listAudioNames;

        public List<string> ListAudioNames
        {
            get
            {
                if (_listAudioNames != null && _listAudioNames.Count > 0)
                {
                    //Debug.Log(_listAudioNames.Count);
                    return _listAudioNames;
                }

                Reload();

                
                return _listAudioNames;
            }
        }
        
        public void Reset()
        {
            _listAudioNames = null;
            database = new List<AudioDatabase>();
        }
        
        
        public void Reload()
        {
            _listAudioNames = new List<string>();

            if (database == null) return;
             
            foreach (var d in database)
            {
                if(d == null) continue;
                //Debug.Log("sound db " + d.name );
                foreach (var s in d.Database)
                {
                    //Debug.Log("sound db " + s.audioName );
                    _listAudioNames.Add(s.audioName);
                }
            }
                
            //Debug.Log(_listAudioNames.Count);
        }
        

#endif
	
        
    }

}