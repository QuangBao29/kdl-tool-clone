using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kawaii.ResourceManager;
//using Kawaii.ResourceManager.Editor;
using Kawaii.IsoTools.DecoSystem;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapBubbleDecoSetting : MonoBehaviour
    {
        [SerializeField] private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField] private EditManager _editManager = null;
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField] protected ToolCreateMapConfigController _configController = null;
        [SerializeField] private ToolCreateMapBubbleDecoItems _prefabDecoItems = null;
        [SerializeField] private ToolCreateMapBubbleDecoItems _prefabRootDeco = null;
        [SerializeField] private Transform _content = null;
        [SerializeField]
        private ToolCreateMapListRooms _toolLstRooms = null;
        
        private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";

        [HideInInspector]
        public Dictionary<ToolCreateMapBubbleDecoItems, List<ToolCreateMapBubbleDecoItems>> DctRootDecoItems = 
            new Dictionary<ToolCreateMapBubbleDecoItems, List<ToolCreateMapBubbleDecoItems>>();
        public void OnSetupBubbleInRoom()
        {
            var selectedItem = _toolLstRooms.GetSelectedItem();
            if (selectedItem == null)
            {
                return;
            }
            else
            {
                var curRoomId = selectedItem.GetRoomId();
                var lstRecs = _configController.ListConfigBubbleHomeRecords;
                foreach (var rec in lstRecs)
                {
                    var roomId = SGUtils.ParseStringToListInt(rec.BubbleId, '_')[0];
                    if (roomId == curRoomId)
                    {
                        var decoId = SGUtils.ParseStringToList(rec.BubbleDecoIds, ';')[0];
                        var lst = SGUtils.ParseStringToListInt(decoId, '_');
                        CreateBubbleDecoItemsAtBeginning(lst[0], lst[1], roomId, rec.BubbleId);
                    }
                }
            }
        }
        public void OnUnselectedItems(string roomId)
        {
            foreach (var root in DctRootDecoItems)
            {
                if (root.Key.RoomId.ToString() == roomId)
                {
                    root.Key.gameObject.SetActive(false);
                    break;
                }
            }
        }
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

        public ToolCreateMapBubbleDecoItems CreateDecoItems(int id, int colorId)
        {
            var colorPath = colorId > 0 ? "_" + colorId : "";
            string idPath = id.ToString() + colorPath;
            var config = _configController.ConfigDeco.GetDecoById(id);
            if (config == null)
                return null;

            KawaiiAtlas atlas = null;
#if UNITY_EDITOR
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + config.ThemeId + ".asset", config.ThemeId.ToString());
#endif
            var FLSprite = atlas != null ? atlas.GetSprite(idPath) : null;
            foreach (var root in DctRootDecoItems)
            {
                if (root.Key.BubbleId == _toolBubbleSetting.CurrentBubble.BubbleId)
                {
                    ToolCreateMapBubbleDecoItems deco = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, root.Key.transform);
                    deco.Prefab = _toolBubbleSetting.CurrentBubble;
                    deco.Image.sprite = FLSprite;
                    deco.BubbleIndex = _toolBubbleSetting.CurrentBubble.Index;
                    deco.RoomId = _toolBubbleSetting.CurrentBubble.RoomIndex;
                    deco.SetupBubbleId();
                    deco.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                    deco.gameObject.SetActive(true);
                    deco.Name.text = idPath;
                    deco.gameObject.name = idPath;
                    DctRootDecoItems[root.Key].Add(deco);
                    return deco;
                }
            }

            var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
            rootDecoItem.RoomId = _toolBubbleSetting.CurrentBubble.RoomIndex;
            rootDecoItem.BubbleIndex = _toolBubbleSetting.CurrentBubble.Index;
            rootDecoItem.SetupBubbleId();
            rootDecoItem.gameObject.name = "Bubble: " + rootDecoItem.BubbleId;
            DctRootDecoItems.Add(rootDecoItem, new List<ToolCreateMapBubbleDecoItems>());
            rootDecoItem.gameObject.SetActive(true);

            ToolCreateMapBubbleDecoItems decoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, rootDecoItem.transform);
            decoItem.Prefab = _toolBubbleSetting.CurrentBubble;
            decoItem.Image.sprite = FLSprite;
            decoItem.BubbleIndex = _toolBubbleSetting.CurrentBubble.Index;
            decoItem.RoomId = _toolBubbleSetting.CurrentBubble.RoomIndex;
            decoItem.SetupBubbleId();
            decoItem.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
            decoItem.gameObject.SetActive(true);
            decoItem.Name.text = idPath;
            decoItem.gameObject.name = idPath;
            DctRootDecoItems[rootDecoItem].Add(decoItem);
            return decoItem;
        }
        public ToolCreateMapBubbleDecoItems CreateBubbleDecoItemsAtBeginning(int id, int colorId, int roomId, string bubbleId)
        {
            var colorPath = colorId > 0 ? "_" + colorId : "";
            string idPath = id.ToString() + colorPath;
            var config = _configController.ConfigDeco.GetDecoById(id);
            if (config == null)
                return null;

            KawaiiAtlas atlas = null;
#if UNITY_EDITOR
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + config.ThemeId + ".asset", config.ThemeId.ToString());
#endif
            var FLSprite = atlas != null ? atlas.GetSprite(idPath) : null;

            foreach (var root in DctRootDecoItems)
            {
                if (root.Key.RoomId != roomId)
                    root.Key.gameObject.SetActive(false);
            }
            var check = false;
            foreach (var root in DctRootDecoItems)
            {
                if (root.Key.RoomId == roomId)
                {
                    check = true;
                    foreach (var i in root.Value)
                    {
                        var info = (DecoInfo)i.Info;
                        if (info.Id == id)
                        {
                            root.Key.gameObject.SetActive(true);
                            return i;
                        }
                    }
                    ToolCreateMapBubbleDecoItems item = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, root.Key.transform);
                    item.Image.sprite = FLSprite;
                    item.RoomId = roomId;
                    item.BubbleId = bubbleId;
                    item.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                    item.gameObject.SetActive(true);
                    item.Name.text = idPath;
                    item.gameObject.name = idPath;
                    DctRootDecoItems[root.Key].Add(item);
                    foreach (var r in _areaManager.ListRooms)
                    {
                        var rootInfo = (DecoInfo)r.Info;
                        if (rootInfo.Id == roomId)
                        {
                            r.Foreach((deco) =>
                            {
                                var info = (DecoInfo)deco.Info;
                                if (info.Id == id)
                                {
                                    var idx = SGUtils.ParseStringToListInt(bubbleId, '_')[1];
                                    var lstRecPos = _configController.ListConfigBubbleHomePositionRecords;
                                    foreach (var rec in lstRecPos)
                                    {
                                        if (rec.RoomId == roomId.ToString())
                                        {
                                            var Pos = rec.GetLstBubblePositionVector3()[idx];
                                            var realPos = Pos + r.Position;
                                            if (deco.Position == realPos)
                                            {
                                                item.Deco = deco;
                                                break;
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    }
                    return item;
                }
            }

            if (!check)
            {
                var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
                rootDecoItem.RoomId = roomId;
                rootDecoItem.gameObject.name = rootDecoItem.RoomId.ToString();
                DctRootDecoItems.Add(rootDecoItem, new List<ToolCreateMapBubbleDecoItems>());
                rootDecoItem.gameObject.SetActive(true);

                ToolCreateMapBubbleDecoItems decoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, rootDecoItem.transform);
                decoItem.Image.sprite = FLSprite;
                decoItem.RoomId = roomId;
                decoItem.BubbleId = bubbleId;
                decoItem.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                decoItem.gameObject.SetActive(true);
                decoItem.Name.text = idPath;
                decoItem.gameObject.name = idPath;
                DctRootDecoItems[rootDecoItem].Add(decoItem);
                foreach (var root in _areaManager.ListRooms)
                {
                    var rootInfo = (DecoInfo)root.Info;
                    if (rootInfo.Id == roomId)
                    {
                        root.Foreach((deco) =>
                        {
                            var info = (DecoInfo)deco.Info;
                            if (info.Id == id)
                            {
                                var idx = SGUtils.ParseStringToListInt(bubbleId, '_')[1];
                                var lstRecPos = _configController.ListConfigBubbleHomePositionRecords;
                                foreach (var rec in lstRecPos)
                                {
                                    if (rec.RoomId == roomId.ToString())
                                    {
                                        var Pos = rec.GetLstBubblePositionVector3()[idx];
                                        var realPos = Pos + root.Position;
                                        if (deco.Position == realPos)
                                        {
                                            decoItem.Deco = deco;
                                            break;
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
                return decoItem;
            }
            else return null;
        }
        public ToolCreateMapBubbleDecoItems CreateBubbleDecoItems(int id, int colorId, int roomId, string bubbleId, Deco deco)
        {
            var colorPath = colorId > 0 ? "_" + colorId : "";
            string idPath = id.ToString() + colorPath;
            var config = _configController.ConfigDeco.GetDecoById(id);
            if (config == null)
                return null;

            KawaiiAtlas atlas = null;
#if UNITY_EDITOR
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + config.ThemeId + ".asset", config.ThemeId.ToString());
#endif
            var FLSprite = atlas != null ? atlas.GetSprite(idPath) : null;

            foreach (var root in DctRootDecoItems)
            {
                if (root.Key.RoomId != roomId)
                    root.Key.gameObject.SetActive(false);
            }

            foreach (var root in DctRootDecoItems)
            {
                if (root.Key.RoomId == roomId)
                {
                    ToolCreateMapBubbleDecoItems item = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, root.Key.transform);
                    item.Image.sprite = FLSprite;
                    item.RoomId = roomId;
                    item.BubbleId = bubbleId;
                    item.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                    item.gameObject.SetActive(true);
                    item.Name.text = idPath;
                    item.gameObject.name = idPath;
                    DctRootDecoItems[root.Key].Add(item);
                    item.Deco = deco;
                    Debug.LogError("linked");
                    return item;
                }
            }
            return null;
        }
        public void ImportDecoItems(int id, int colorId, ToolCreateMapBubbleItem bubbleItem)
        {
            var colorPath = colorId > 0 ? "_" + colorId : "";
            string idPath = id.ToString() + colorPath;
            var config = _configController.ConfigDeco.GetDecoById(id);
            if (config == null)
                return;

            KawaiiAtlas atlas = null;
#if UNITY_EDITOR
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + config.ThemeId + ".asset", config.ThemeId.ToString());
#endif
            var FLSprite = atlas != null ? atlas.GetSprite(idPath) : null;

            foreach (var root in DctRootDecoItems)
            {
                if (root.Key.BubbleId == bubbleItem.BubbleId)
                {
                    ToolCreateMapBubbleDecoItems deco = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, root.Key.transform);
                    deco.Prefab = bubbleItem;
                    deco.Image.sprite = FLSprite;
                    deco.RoomId = bubbleItem.RoomIndex;
                    deco.BubbleIndex = bubbleItem.Index;
                    deco.SetupBubbleId();
                    deco.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                    deco.gameObject.SetActive(true);
                    deco.Name.text = idPath;
                    deco.gameObject.name = idPath;
                    DctRootDecoItems[root.Key].Add(deco);
                    return;
                }
            }

            var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
            rootDecoItem.BubbleIndex = bubbleItem.Index;
            rootDecoItem.RoomId = bubbleItem.RoomIndex;
            rootDecoItem.SetupBubbleId();
            rootDecoItem.gameObject.name = "Bubble: " + rootDecoItem.BubbleId;
            DctRootDecoItems.Add(rootDecoItem, new List<ToolCreateMapBubbleDecoItems>());
            rootDecoItem.gameObject.SetActive(false);

            ToolCreateMapBubbleDecoItems decoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, rootDecoItem.transform);
            decoItem.Prefab = bubbleItem;
            decoItem.Image.sprite = FLSprite;
            decoItem.BubbleIndex = bubbleItem.Index;
            decoItem.RoomId = bubbleItem.RoomIndex;
            decoItem.SetupBubbleId();
            decoItem.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
            decoItem.gameObject.SetActive(true);
            decoItem.Name.text = idPath;
            decoItem.gameObject.name = idPath;
            DctRootDecoItems[rootDecoItem].Add(decoItem);
        }

        public void AddRootClone(int RoomIndex, int BubbleIndex)
        {
            var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
            rootDecoItem.RoomId = RoomIndex;
            rootDecoItem.BubbleIndex = BubbleIndex;
            rootDecoItem.SetupBubbleId();
            rootDecoItem.gameObject.name = "Bubble: " + rootDecoItem.BubbleId;
            DctRootDecoItems.Add(rootDecoItem, new List<ToolCreateMapBubbleDecoItems>());
            rootDecoItem.gameObject.SetActive(true);
        }

        public void ClearAll()
        {
            foreach (var root in DctRootDecoItems)
            {
                foreach (var deco in root.Value)
                {
                    root.Value.Remove(deco);
                    Debug.LogError("neu remove dc thi chay cai nay");
                    Destroy(deco.gameObject);
                }
                DctRootDecoItems.Remove(root.Key);
            }
        }
    }
}
