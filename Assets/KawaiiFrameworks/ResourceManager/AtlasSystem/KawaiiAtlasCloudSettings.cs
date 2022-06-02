using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kawaii.ResourceManager
{
    public class KawaiiAtlasCloudSettings : ScriptableObject
    {
        public enum AtlasType
        {
            None = -1,
            All = 0,
            StreamingAsset,
            Cloud
        }
        [Serializable]
        public class CloudSetting
        {
            public string AtlasName;
            public AtlasType Type = AtlasType.StreamingAsset;
        }

        public List<CloudSetting> LstSettings = new List<CloudSetting>();

        public void Add(string atlasName)
        {
            var setting = LstSettings.Find(item => {
                return item.AtlasName == atlasName;
            });
            if (setting == null)
                LstSettings.Add(new CloudSetting
                {
                    AtlasName = atlasName
                });
        }

        public AtlasType GetTypeOfAtlas(string atlasName)
        {
            var setting = LstSettings.Find(item => {
                return item.AtlasName == atlasName;
            });
            if(setting == null)
            {
                Debug.LogError(string.Format("Setting {0} Is Missing!", atlasName));
                return AtlasType.None;
            }
            return setting.Type;
        }
    }
}

