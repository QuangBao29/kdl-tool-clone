using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kawaii.ResourceManager;
using Kawaii.IsoTools.DecoSystem;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapBubbleDecoSetting : MonoBehaviour
    {
        [SerializeField] private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField] private EditManager _editManager = null;
        [SerializeField] protected ToolCreateMapConfigController _configController = null;
        [SerializeField] private ToolCreateMapBubbleDecoItems _prefabDecoItems = null;
        [SerializeField] private ToolCreateMapBubbleDecoItems _prefabRootDeco = null;
        [SerializeField] private Transform _content = null;

        private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";

        [HideInInspector]
        public Dictionary<ToolCreateMapBubbleDecoItems, List<ToolCreateMapBubbleDecoItems>> DctRootDecoItems = 
            new Dictionary<ToolCreateMapBubbleDecoItems, List<ToolCreateMapBubbleDecoItems>>();

        public void AddDecoToCurrentBubble()
        {
            var id = 0;
            var colorId = 0;
            if (_editManager.Current != null)
            {
                id = _editManager.Current.deco.ParseInfo<DecoInfo>().Id;
                colorId = _editManager.Current.deco.ParseInfo<DecoInfo>().Color;
            }
            else return;
            if (!_toolBubbleSetting.CurrentBubble.DctDecoIdColor.ContainsKey(id))
            {
                _toolBubbleSetting.CurrentBubble.DctDecoIdColor.Add(id, new List<int>());
                _toolBubbleSetting.CurrentBubble.DctDecoIdColor[id].Add(colorId);
                CreateDecoItems(id, colorId);
            }
            else
            {
                if (!_toolBubbleSetting.CurrentBubble.DctDecoIdColor[id].Contains(colorId))
                {
                    _toolBubbleSetting.CurrentBubble.DctDecoIdColor[id].Add(colorId);
                    CreateDecoItems(id, colorId);
                }
            }
        }

        public void RemoveDecoFromBubble()
        {
            var id = 0;
            var colorId = 0;
            if (_editManager.Current != null)
            {
                id = _editManager.Current.deco.ParseInfo<DecoInfo>().Id;
                colorId = _editManager.Current.deco.ParseInfo<DecoInfo>().Color;
            }
            else return;

            if (_toolBubbleSetting.CurrentBubble.DctDecoIdColor.ContainsKey(id))
            {
                if (_toolBubbleSetting.CurrentBubble.DctDecoIdColor[id].Contains(colorId))
                {
                    _toolBubbleSetting.CurrentBubble.DctDecoIdColor[id].Remove(colorId);
                    if (_toolBubbleSetting.CurrentBubble.DctDecoIdColor[id].Count == 0) 
                        _toolBubbleSetting.CurrentBubble.DctDecoIdColor.Remove(id);
                    Debug.LogError("dict count: " + _toolBubbleSetting.CurrentBubble.DctDecoIdColor.Count);

                    foreach (var root in DctRootDecoItems)
                    {
                        if (root.Key.BubbleId == _toolBubbleSetting.CurrentBubble.BubbleId)
                        {
                            foreach (var deco in root.Value)
                            {
                                var decoInfo = deco.ParseInfo<DecoInfo>();
                                if (decoInfo.Id == id && decoInfo.Color == colorId)
                                {
                                    root.Value.Remove(deco);
                                    Destroy(deco.gameObject);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                else
                {
                    Debug.LogError("this color does not in List");
                }
            }
            else
            {
                Debug.LogError("does not contain Id");
            }
        }

        public void CreateDecoItems(int id, int colorId)
        {
            var colorPath = colorId > 0 ? "_" + colorId : "";
            string idPath = id.ToString() + colorPath;
            var config = _configController.ConfigDeco.GetDecoById(id);
            if (config == null)
                return;

            KawaiiAtlas atlas = null;
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + config.ThemeId + ".asset",
                config.ThemeId.ToString());
            var FLSprite = atlas != null ? atlas.GetSprite(idPath) : null;

            foreach (var root in DctRootDecoItems)
            {
                if (root.Key.BubbleId == _toolBubbleSetting.CurrentBubble.BubbleId)
                {
                    ToolCreateMapBubbleDecoItems deco = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, root.Key.transform);
                    deco.Prefab = _toolBubbleSetting.CurrentBubble;
                    deco.Image.sprite = FLSprite;
                    deco.BubbleIndex = _toolBubbleSetting.CurrentBubble.Index;
                    deco.RoomIndex = _toolBubbleSetting.CurrentBubble.RoomIndex;
                    deco.SetupBubbleId();
                    deco.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                    deco.gameObject.SetActive(true);
                    deco.Name.text = idPath;
                    DctRootDecoItems[root.Key].Add(deco);
                    return;
                }
            }

            var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
            rootDecoItem.RoomIndex = _toolBubbleSetting.CurrentBubble.RoomIndex;
            rootDecoItem.BubbleIndex = _toolBubbleSetting.CurrentBubble.Index;
            rootDecoItem.SetupBubbleId();
            Debug.LogError("BubbleId: " + rootDecoItem.BubbleId);
            rootDecoItem.gameObject.name = "Bubble: " + rootDecoItem.BubbleId;
            DctRootDecoItems.Add(rootDecoItem, new List<ToolCreateMapBubbleDecoItems>());
            rootDecoItem.gameObject.SetActive(true);

            ToolCreateMapBubbleDecoItems decoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, rootDecoItem.transform);
            decoItem.Prefab = _toolBubbleSetting.CurrentBubble;
            decoItem.Image.sprite = FLSprite;
            decoItem.BubbleIndex = _toolBubbleSetting.CurrentBubble.Index;
            decoItem.RoomIndex = _toolBubbleSetting.CurrentBubble.RoomIndex;
            decoItem.SetupBubbleId();
            decoItem.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
            decoItem.gameObject.SetActive(true);
            decoItem.Name.text = idPath;
            DctRootDecoItems[rootDecoItem].Add(decoItem);
        }

        public void ImportDecoItems(int id, int colorId, int bubbleIndex)
        {
            var colorPath = colorId > 0 ? "_" + colorId : "";
            string idPath = id.ToString() + colorPath;
            var config = _configController.ConfigDeco.GetDecoById(id);
            if (config == null)
                return;

            KawaiiAtlas atlas = null;
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + config.ThemeId + ".asset",
                config.ThemeId.ToString());
            var FLSprite = atlas != null ? atlas.GetSprite(idPath) : null;

            foreach (var root in DctRootDecoItems)
            {
                if (root.Key.BubbleIndex == bubbleIndex)
                {
                    ToolCreateMapBubbleDecoItems deco = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, root.Key.transform);
                    deco.transform.GetChild(0).GetComponent<Image>().sprite = FLSprite;
                    deco.BubbleIndex = bubbleIndex;
                    deco.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                    deco.gameObject.SetActive(true);
                    deco.transform.GetChild(1).GetComponent<Text>().text = idPath;
                    DctRootDecoItems[root.Key].Add(deco);
                    return;
                }
            }

            var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
            rootDecoItem.BubbleIndex = bubbleIndex;
            rootDecoItem.gameObject.name = "Bubble: " + rootDecoItem.BubbleIndex;
            DctRootDecoItems.Add(rootDecoItem, new List<ToolCreateMapBubbleDecoItems>());
            rootDecoItem.gameObject.SetActive(true);

            ToolCreateMapBubbleDecoItems decoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, rootDecoItem.transform);
            decoItem.Image.sprite = FLSprite;
            decoItem.BubbleIndex = bubbleIndex;
            decoItem.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
            decoItem.gameObject.SetActive(true);
            decoItem.Name.text = idPath;
            DctRootDecoItems[rootDecoItem].Add(decoItem);
        }
    }

}
