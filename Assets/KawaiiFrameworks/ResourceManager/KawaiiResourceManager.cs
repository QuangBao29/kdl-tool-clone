using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.ResourceManager
{
    public class KawaiiResourceManager : MonoBehaviour
    {
        private static readonly Dictionary<string, bool> _neverUnloadAtlas = new Dictionary<string, bool>();
        private static readonly Dictionary<string, KawaiiAtlas> _allAtlas = new Dictionary<string, KawaiiAtlas>();
        private static readonly Dictionary<string, Dictionary<string, Sprite>> _allResourceSprite = new Dictionary<string, Dictionary<string, Sprite>>();

        private static LoadResourceKit _resourceKit = null;
        private static LoadStreamingAssetKit _streamingAssetKit = null;
        private static LoadCloudKit _cloutKit = null;

        public static KawaiiAtlas GetAtlas(string key)
        {
            KawaiiAtlas atlas = null;
            _allAtlas.TryGetValue(key, out atlas);
            return atlas;
        }

        public static LoadResourceKit ResourceKit
        {
            get
            {
                if (_resourceKit == null)
                    _resourceKit = new LoadResourceKit(_allAtlas, _allResourceSprite);
                return _resourceKit;
            }
        }


        public static LoadStreamingAssetKit StreamingAssetKit
        {
            get
            {
                if (_streamingAssetKit == null)
                    _streamingAssetKit = new LoadStreamingAssetKit(_allAtlas);
                return _streamingAssetKit;
            }
        }

        public static LoadCloudKit CloudKit
        {
            get
            {
                if (_cloutKit == null)
                    _cloutKit = new LoadCloudKit(_allAtlas);
                return _cloutKit;
            }
        }

        public void ClearAll()
        {
            List<string> lstRemoveAtlasName = new List<string>();
            foreach(var iter in _allAtlas)
            {
                if (_neverUnloadAtlas.ContainsKey(iter.Key))
                    continue;
                lstRemoveAtlasName.Add(iter.Key);
                _streamingAssetKit.Clear(iter.Key);
            }
            foreach (var atlas in lstRemoveAtlasName)
                _allAtlas.Remove(atlas);
        }

        public static void AddNerverUnloadAtlas(string atlasName)
        {
            if (string.IsNullOrEmpty(atlasName))
                return;
            _neverUnloadAtlas[atlasName] = true;
        }

        public static void RemoveNerverUnloadAtlas(string atlasName)
        {
            if (string.IsNullOrEmpty(atlasName))
                return;
            _neverUnloadAtlas.Remove(atlasName);
        }
    }
}

