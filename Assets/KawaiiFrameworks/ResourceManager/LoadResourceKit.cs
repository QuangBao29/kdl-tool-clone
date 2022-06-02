using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Kawaii.ResourceManager
{
    public class LoadResourceKit
    {
        private Dictionary<string, Dictionary<string, Sprite>> _allSprites;
        private Dictionary<string, KawaiiAtlas> _allAtlas;
       
        private Dictionary<string, ResourceRequest> _isLoadingAtlasFromResource = new Dictionary<string, ResourceRequest>();

        public LoadResourceKit(Dictionary<string, KawaiiAtlas> allAtlas, Dictionary<string, Dictionary<string, Sprite>> allSprites)
        {
            _allAtlas = allAtlas;
            _allSprites = allSprites;
        }

        public KawaiiAtlas LoadAtlas(string path, string key)
        {
            KawaiiAtlas atlas = null;
            if (!_allAtlas.TryGetValue(key, out atlas))
            {
                atlas = Resources.Load<KawaiiAtlas>(path);
                if (atlas != null)
                    atlas.Setup();
                _allAtlas[key] = atlas;
            }
            return atlas;
        }

        public IEnumerator LoadAtlasConroutine(string path, string key)
        {
            KawaiiAtlas atlas = null;
            if (!_allAtlas.TryGetValue(key, out atlas))
            {
                ResourceRequest request = null;
                if (!_isLoadingAtlasFromResource.TryGetValue(key, out request) || request == null)
                {
                    Debug.LogWarning("load resource " + path);
                    request = Resources.LoadAsync<KawaiiAtlas>(path);
                    _isLoadingAtlasFromResource[key] = request;
                }
                while (!request.isDone)
                    yield return 0;
                if (request.asset != null)
                {
                    atlas = request.asset as KawaiiAtlas;
                    atlas.Setup();
                    _allAtlas[key] = atlas;
                }
                _isLoadingAtlasFromResource.Remove(key);
            }
            yield return atlas;
        }

        public async Task<KawaiiAtlas> LoadAtlasAsync(string path, string key)
        {
            KawaiiAtlas atlas = null;
            if (!_allAtlas.TryGetValue(key, out atlas))
            {
                ResourceRequest request = null;
                if (!_isLoadingAtlasFromResource.TryGetValue(key, out request) || request == null)
                {
                    Debug.LogWarning("load resource " + path);
                    request = Resources.LoadAsync<KawaiiAtlas>(path);
                    _isLoadingAtlasFromResource[key] = request;
                }
                while (!request.isDone)
                    await Task.Yield();
                if (request.asset != null)
                {
                    atlas = request.asset as KawaiiAtlas;
                    atlas.Setup();
                    _allAtlas[key] = atlas;
                }
                _isLoadingAtlasFromResource.Remove(key);
            }
            return atlas;
        }

        public Sprite LoadSprite(string path, string texture, string spriteName)
        {
            Dictionary<string, Sprite> dicSpriteInTheme = null;
            if (!_allSprites.TryGetValue(texture, out dicSpriteInTheme))
            {
                dicSpriteInTheme = new Dictionary<string, Sprite>();
                _allSprites[texture] = dicSpriteInTheme;

                var allSprObj = Resources.LoadAll<Sprite>(path);
                if (allSprObj != null)
                {
                    foreach (var spr in allSprObj)
                    {
                        dicSpriteInTheme[spr.name] = spr;
                    }
                }
            }
            Sprite sprite = null;
            dicSpriteInTheme.TryGetValue(spriteName, out sprite);
            return sprite;
        }

        public static string LoadText(string path)
        {
            var textAsset = Resources.Load<TextAsset>(path);
            if (textAsset != null)
                return textAsset.text;
            return null;
        }

        public static T LoadPrefab<T>(string path) where T : MonoBehaviour
        {
            return Resources.Load<T>(path);
        }

    }
}

