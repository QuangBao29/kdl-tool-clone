using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kawaii.ResourceManager;
using KAP.Tools;
using Kawaii.IsoTools.DecoSystem;
using TMPro;
using Kawaii.Utils;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapBubbleDecoSetting : MonoBehaviour
    {
        [SerializeField] private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField] private EditManager _editManager = null;
        [SerializeField] private AreaManager _areaManager = null;
        [SerializeField] protected ToolCreateMapConfigController _configController = null;
        [SerializeField] private ToolCreateMapBubbleDecoItems _prefabDecoItems = null;
        [SerializeField] private ToolCreateMapBubbleDecoItems _prefabRootDeco = null;
        [SerializeField] private Transform _content = null;
        [SerializeField] private ToolCreateMapListRooms _toolLstRooms = null;
        [SerializeField] private ScrollRect _scrollItem = null;
        [SerializeField] public TMP_InputField BaseGem = null;
        [Header("Bubble ID Item")]
        [SerializeField] private ListItemGenerator _generator = null;

        private List<ToolCreateMapBubbleDecoItems> _lstBubbleDecoItems = null;
        private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";

        [HideInInspector]
        public Dictionary<ToolCreateMapBubbleDecoItems, List<ToolCreateMapBubbleDecoItems>> _dctRootDecoItems = 
            new Dictionary<ToolCreateMapBubbleDecoItems, List<ToolCreateMapBubbleDecoItems>>();

        public Dictionary<ToolCreateMapBubbleDecoItems, List<ToolCreateMapBubbleDecoItems>> DctRootDecoItems
        {
            get => _dctRootDecoItems;
            set => _dctRootDecoItems = value;
        }
        public void OnSelectRootDecoItems()
        {
            var selectedItem = _toolLstRooms.GetSelectedItem();
            if (selectedItem == null)
            {
                Debug.LogError("selectedItem = null");
                return;
            }
            else
            {
                OnHideAllItems();
                //foreach (var root in DctRootDecoItems)
                //{
                //    if (root.Key.RoomId == selectedItem.GetRoomId())
                //    {
                //        root.Key.gameObject.SetActive(true);
                //        return;
                //    }
                //}
                if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                {
                    var curRoomId = selectedItem.GetRoomId();
                    var lstRecs = _configController.ListConfigBubbleHomeRecords;
                    var check = false;
                    foreach (var rec in lstRecs)
                    {
                        var roomId = SGUtils.ParseStringToListInt(rec.BubbleId, '_')[0];
                        if (roomId == curRoomId)
                        {
                            check = true;
                            var decoId = SGUtils.ParseStringToList(rec.BubbleDecoIds, ';')[0];
                            var lst = SGUtils.ParseStringToListInt(decoId, '_');
                            CreateBubbleDecoItemsAtBeginning(lst[0], lst[1], roomId, rec.BubbleId);
                        }
                    }
                    if (!check)
                    {
                        var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
                        rootDecoItem.RoomId = curRoomId;
                        rootDecoItem.gameObject.name = rootDecoItem.RoomId.ToString();
                        DctRootDecoItems.Add(rootDecoItem, new List<ToolCreateMapBubbleDecoItems>());
                        rootDecoItem.gameObject.SetActive(true);
                    }
                }
                if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                {
                    var curRoomId = selectedItem.GetRoomId();
                    var lstRecs = _configController.ListConfigBubblePlayRecords;
                    var check = false;
                    foreach (var rec in lstRecs)
                    {
                        var roomId = SGUtils.ParseStringToListInt(rec.BubbleId, '_')[0];
                        if (roomId == curRoomId)
                        {
                            var bubbledecoids = SGUtils.ParseStringToList(rec.BubbleDecoIds, ';');
                            string decoId = "";
                            if (bubbledecoids.Count > 0)
                            {
                                decoId = bubbledecoids[0];
                                check = true;
                                var lst = SGUtils.ParseStringToListInt(decoId, '_');
                                CreateBubbleDecoItemsAtBeginning(lst[0], lst[1], roomId, rec.BubbleId);
                            }
                        }
                    }
                    if (!check)
                    {
                        var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
                        rootDecoItem.RoomId = curRoomId;
                        rootDecoItem.gameObject.name = rootDecoItem.RoomId.ToString();
                        DctRootDecoItems.Add(rootDecoItem, new List<ToolCreateMapBubbleDecoItems>());
                        rootDecoItem.gameObject.SetActive(true);
                    }
                }
            }
        }
        public ToolCreateMapBubbleDecoItems OnGetBubbleDecoItemWithIndex(int roomId, int index)
        {
            foreach (var pair in DctRootDecoItems)
            {
                if (pair.Key.RoomId == roomId)
                {
                    foreach (var item in pair.Value)
                    {
                        if (item.GetIndex() == index.ToString())
                            return item;
                    }
                }
            }
            return null;
        }
        public void OnAddToScrollRect(string id)
        {
            foreach (var root in DctRootDecoItems)
            {
                if (root.Key.RoomId.ToString() == id)
                {
                    _scrollItem.content = root.Key.gameObject.GetComponent<RectTransform>();
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
        public void OnHideAllItems()
        {
            foreach (var root in DctRootDecoItems)
            {
                root.Key.gameObject.SetActive(false);
            }
        }
        public void OnClearDctRootDecoItems()
        {
            foreach (var item in DctRootDecoItems)
            {
                Destroy(item.Key.gameObject);
            }
            DctRootDecoItems.Clear();
        }
        public void OnRemoveRootFromDict(string roomId)
        {
            ToolCreateMapBubbleDecoItems temp = new ToolCreateMapBubbleDecoItems();
             foreach (var pair in DctRootDecoItems)
             {
                if (pair.Key.RoomId.ToString() == roomId)
                {
                    temp = pair.Key;
                    Destroy(pair.Key.gameObject);
                    break;
                }
             }
            DctRootDecoItems.Remove(temp);
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
                    root.Key.gameObject.SetActive(true);
                    ToolCreateMapBubbleDecoItems item = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, root.Key.transform);
                    item.Image.sprite = FLSprite;
                    item.RoomId = roomId;
                    item.BubbleId = bubbleId;
                    //Debug.LogError("check room bubble id " + roomId + " " + bubbleId);
                    item.SetIndex(SGUtils.ParseStringToList(bubbleId, '_')[1]);
                    item.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                    item.gameObject.SetActive(true);
                    item.Name.text = idPath;
                    item.gameObject.name = idPath;
                    //Debug.LogError("item check:" + item.BubbleId);
                    if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                    {
                        item.SetStar(_configController.ConfigBubbleHome.GetById(bubbleId).Star.ToString());
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
                                        var rec = _configController.ConfigBubbleHomePosition.GetByRoomId(roomId.ToString());
                                        var Pos = rec.GetLstBubblePositionVector3()[idx];
                                        var realPos = Pos + r.Position;
                                        if (deco.Position == realPos)
                                        {
                                            item.Deco = deco;
                                        }
                                    }
                                });
                            }
                        }
                    }
                    else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                    {
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
                                        //Debug.LogError("dung id");
                                        var idx = SGUtils.ParseStringToListInt(bubbleId, '_')[1];
                                        var rec = _configController.ConfigBubblePlayPosition.GetByRoomId(roomId.ToString());
                                        var Pos = rec.GetLstBubblePositionVector3()[idx];
                                        var realPos = Pos + r.Position;
                                        //Debug.LogError("check 1 " + realPos + "  " + deco.Position);
                                        if (deco.Position == realPos)
                                        {
                                            //Debug.LogError("co lun 1");
                                            item.Deco = deco;
                                        }
                                    }
                                });
                            }
                        }
                    }
                    return item;
                }
            }

            if (!check)
            {
                //Debug.LogError("abc");
                var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
                rootDecoItem.RoomId = roomId;
                rootDecoItem.gameObject.name = rootDecoItem.RoomId.ToString();
                DctRootDecoItems.Add(rootDecoItem, new List<ToolCreateMapBubbleDecoItems>());
                rootDecoItem.gameObject.SetActive(true);

                ToolCreateMapBubbleDecoItems decoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, rootDecoItem.transform);
                decoItem.Image.sprite = FLSprite;
                decoItem.RoomId = roomId;
                decoItem.BubbleId = bubbleId;
                decoItem.SetIndex(SGUtils.ParseStringToList(bubbleId, '_')[1]);
                decoItem.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                decoItem.gameObject.SetActive(true);
                decoItem.Name.text = idPath;
                decoItem.gameObject.name = idPath;
                if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                {
                    decoItem.SetStar(_configController.ConfigBubbleHome.GetById(bubbleId).Star.ToString());
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
                                    var rec = _configController.ConfigBubbleHomePosition.GetByRoomId(roomId.ToString());
                                    var Pos = rec.GetLstBubblePositionVector3()[idx];
                                    var realPos = Pos + root.Position;
                                    if (deco.Position == realPos)
                                    {
                                        decoItem.Deco = deco;
                                    }
                                }
                            });
                        }
                    }
                }
                else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                {
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
                                    var rec = _configController.ConfigBubblePlayPosition.GetByRoomId(roomId.ToString());
                                    var Pos = rec.GetLstBubblePositionVector3()[idx];
                                    var realPos = Pos + root.Position;
                                    //Debug.LogError("check 2 " + realPos + "  " + deco.Position);
                                    if (deco.Position == realPos)
                                    {
                                        //Debug.LogError("co lun 2");
                                        decoItem.Deco = deco;
                                    }
                                }
                            });
                        }
                    }
                }
                return decoItem;
            }
            else return null;
        }
        public ToolCreateMapBubbleDecoItems CreateBubbleDecoItemsAtBeginningTemp(Deco deco, int id, int colorId, int roomId, string bubbleId)
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
                    root.Key.gameObject.SetActive(true);
                    ToolCreateMapBubbleDecoItems item = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, root.Key.transform);
                    item.Image.sprite = FLSprite;
                    item.RoomId = roomId;
                    item.BubbleId = bubbleId;
                    item.SetIndex(SGUtils.ParseStringToList(bubbleId, '_')[1]);
                    item.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                    item.gameObject.SetActive(true);
                    item.Name.text = idPath;
                    item.gameObject.name = idPath;
                    if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                    {
                        DctRootDecoItems[root.Key].Add(item);
                        item.Deco = deco;
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
                decoItem.SetIndex(SGUtils.ParseStringToList(bubbleId, '_')[1]);
                decoItem.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
                decoItem.gameObject.SetActive(true);
                decoItem.Name.text = idPath;
                decoItem.gameObject.name = idPath;
                if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                {
                    DctRootDecoItems[rootDecoItem].Add(decoItem);
                    decoItem.Deco = deco;
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
                    //Debug.LogError("linked");
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

        public void OnGenerateItem(List<string> lstID, string bubbleId)
        {
            _lstBubbleDecoItems = _generator.Setup<ToolCreateMapBubbleDecoItems>(lstID.Count);
            var config = _configController.ConfigBubbleHome.GetById(bubbleId);
            var lstPrice = config.GetLstPrice();
            for (var i = 0; i < lstID.Count; i++)
            {
                int id = SGUtils.ParseStringToListInt(lstID[i], '_')[0];
                int color = SGUtils.ParseStringToListInt(lstID[i], '_')[1];
                OnCreateDeco(_lstBubbleDecoItems[i], id, color, bubbleId, lstPrice[i]);
            }
        }

        public void OnCreateDeco(ToolCreateMapBubbleDecoItems item, int id, int color, string bubbleId, int price)
        {
            var colorPath = color > 0 ? "_" + color : "";
            string idPath = id.ToString() + colorPath;
            var config = _configController.ConfigDeco.GetDecoById(id);
            if (config == null)
                return;
            int roomId = SGUtils.ParseStringToListInt(bubbleId, '_')[0];
            var configbubble = _configController.ConfigBubbleHome.GetById(bubbleId);
            KawaiiAtlas atlas = null;
#if UNITY_EDITOR
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + config.ThemeId + ".asset", config.ThemeId.ToString());
#endif
            var FLSprite = atlas != null ? atlas.GetSprite(idPath) : null;

            item.Image.sprite = FLSprite;
            item.RoomId = roomId;
            item.SetIndex(configbubble.Index);
            item.SetPrice(price.ToString());
            item.SetStar(configbubble.Star.ToString());
            item.BubbleId = bubbleId;
            item.Info = new DecoInfo { Id = id, Color = color/*, IsBubble = true*/ };
            item.Name.text = idPath;
            item.gameObject.name = idPath;
        }

        public void OnChangeInputStars(string star)
        {
            foreach (var item in _lstBubbleDecoItems)
            {
                item.SetStar(star);
            }
        }
    }
}
