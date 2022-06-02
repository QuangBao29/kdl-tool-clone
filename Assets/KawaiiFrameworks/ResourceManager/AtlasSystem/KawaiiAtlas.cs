using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.ResourceManager
{
    public class KawaiiAtlas : ScriptableObject
    {
        public List<Sprite> LstSprites;
        public List<GameObject> LstPrefabs;
        public List<TextAsset> LstTexts;

        Dictionary<string, Sprite> dicSprite = new Dictionary<string, Sprite>();
        Dictionary<string, GameObject> dicPrefab = new Dictionary<string, GameObject>();
        Dictionary<string, TextAsset> dicText = new Dictionary<string, TextAsset>();
        public void Setup()
        {
            dicSprite.Clear();
            foreach (var iter in LstSprites)
            {
                if (iter != null)
                {
                    dicSprite[iter.name] = iter;
                }
            }
            dicPrefab.Clear();
            foreach (var iter in LstPrefabs)
            {
                if (iter != null)
                {
                    dicPrefab[iter.name] = iter;
                }
            }

            dicText.Clear();
            foreach (var iter in LstTexts)
            {
                if (iter != null)
                {
                    dicText[iter.name] = iter;
                }
            }
        }

        public Sprite GetSprite(string spriteName, bool setDefault = false)
        {
            Sprite result = null;
            dicSprite.TryGetValue(spriteName, out result);
            //if (result == null && setDefault)
                //result = SGResourceManager.Instance.sprDefault;
            return result;
        }

        public GameObject GetPrefab(string prefabName)
        {
            GameObject result = null;
            dicPrefab.TryGetValue(prefabName, out result);
            return result;
        }

        public T GetPrefab<T>(string prefabName) where T : Component
        {
            GameObject obj = null;
            if (dicPrefab.TryGetValue(prefabName, out obj))
            {
                return obj.GetComponent<T>();
            }
            return null;
        }

        public string GetText(string fileName)
        {
            TextAsset result = null;
            dicText.TryGetValue(fileName, out result);
            if(result != null)
                return result.text;
            return "";
        }
    }
}

