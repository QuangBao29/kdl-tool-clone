#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Kawaii.ResourceManager.Editor
{
    public class ResourceManagerEditor
    {
        private static Dictionary<string, KawaiiAtlas> _allAtlas = new Dictionary<string, KawaiiAtlas>();
        public static KawaiiAtlas LoadAtlas(string path, string key)
        {
            KawaiiAtlas atlas = null;
            if(!_allAtlas.TryGetValue(key, out atlas))
            {
                atlas = AssetDatabase.LoadAssetAtPath<KawaiiAtlas>(path);
                if(atlas != null)
                    atlas.Setup();
                _allAtlas[key] = atlas;
            }
            return atlas;
        }

        private static Dictionary<string, Sprite> _allSprites = new Dictionary<string, Sprite>();
        public static Sprite LoadSprite(string path, string spriteName)
        {
            Sprite sprite = null;
            if(!_allSprites.TryGetValue(spriteName, out sprite))
            {
                var  objArr = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach(var obj in objArr)
                {
                    var spr = (Sprite)obj;
                    _allSprites[spr.name] = spr;
                    if (spr.name == spriteName)
                        sprite = spr; 
                }
            }
            return sprite;
        }
    }

}
#endif