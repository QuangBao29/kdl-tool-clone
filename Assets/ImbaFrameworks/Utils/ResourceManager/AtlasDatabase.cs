using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace Imba.Utils
{
    [Serializable]
    [CreateAssetMenu(fileName = "AtlasDatabase", menuName = "Imba/Atlas Database", order = 1)]
    public class AtlasDatabase: ScriptableObject
    {
        [SerializeField]
        private List<SpriteAtlasData> database;

        public List<SpriteAtlasData> Database => database;

        public void Reset()
        {
            database = new List<SpriteAtlasData>();
        }
    }

}