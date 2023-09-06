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
        [SerializeField] private ToolCreateMapImportDeco _importDecoController = null;
        [SerializeField] private ScrollRect _scrollItem = null;
        [SerializeField] public TMP_InputField BaseGem = null;
        [Header("Bubble ID Item")]
        [SerializeField] private ListItemGenerator _generator = null;
        [SerializeField] private Toggle _toggleDecoMode = null;

        private List<ToolCreateMapBubbleDecoItems> _lstCurrentBubbleDeco = null;
        private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";

        //BubbleId - list deco id
        [HideInInspector]
        public Dictionary<string, List<string>> _dctBubbleDecoItems = 
            new Dictionary<string, List<string>>();

        public Dictionary<string, List<string>> DctBubbleDecoItems
        {
            get => _dctBubbleDecoItems;
            set => _dctBubbleDecoItems = value;
        }
        public List<ToolCreateMapBubbleDecoItems> LstCurrentbubbleDeco
        {
            set => _lstCurrentBubbleDeco = value;
            get => _lstCurrentBubbleDeco;
        }
        public Toggle ToggleDecoMode
        {
            get => _toggleDecoMode;
            set => _toggleDecoMode = value;
        }
        public void SwapBubbleDeco(Deco curDeco, int id, int color)
        {
            Debug.LogError("swap");
            var allChilds = curDeco.GetAllChilds();
            if (allChilds != null)
            {
                foreach (var child in allChilds)
                {
                    var childDecoInfo = child.ParseInfo<DecoInfo>();
                    string childId = childDecoInfo.Id.ToString() + "_" + childDecoInfo.Color.ToString();
                    if (childDecoInfo != null) _toolBubbleSetting.LstDecoBoxID.Add(childId);
                    else Debug.LogError("child info not found");
                }
            }
            curDeco.Remove();
            var newDeco = _importDecoController.CreateDeco(id, color);
            newDeco.Info = new DecoInfo { Id = id, Color = color, IsBubble = true };
            newDeco.Position = curDeco.Position;
            newDeco.WorldDirect = curDeco.WorldDirect;
            var decoEdit = newDeco.GetComponent<DecoEditDemo>();

            var moveData = _areaManager.Move(newDeco);
            if (moveData.ListOverlaps != null)
            {
                foreach (var decoItem in moveData.ListOverlaps)
                {
                    if (decoItem == newDeco)
                    {
                        continue;
                    }
                    var current = decoItem;
                    var children = current.GetAllChilds();
                    foreach (var child in children)
                    {
                        var childDecoInfo = child.ParseInfo<DecoInfo>();
                        string childId = childDecoInfo.Id.ToString() + "_" + childDecoInfo.Color.ToString();
                        if (childDecoInfo != null) _toolBubbleSetting.LstDecoBoxID.Add(childId);
                        else Debug.LogError("child info not found");
                        child.Remove();
                    }
                    var itemInfo = current.ParseInfo<DecoInfo>();
                    string itemId = itemInfo.Id.ToString() + "_" + itemInfo.Color.ToString();
                    _toolBubbleSetting.LstDecoBoxID.Add(itemId);
                    current.Remove();
                }
            }

            if (_editManager.SetCurrent(decoEdit))
            {
                decoEdit.StartMove();
                decoEdit.EndMove();
            }
            if (decoEdit.EditStatus == KHHEditStatus.Valid) _editManager.SetCurrent(null);
            _toolBubbleSetting.DctDecoInRoom[_toolBubbleSetting.CurrentBubbleID] = newDeco;

            _configController.DctBubbleIdWD[_toolBubbleSetting.CurrentBubbleID] = newDeco.WorldDirect.ToString();
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
                            //CreateBubbleDecoItemsAtBeginning(lst[0], lst[1], roomId, rec.BubbleId);
                        }
                    }
                    if (!check)
                    {
                        var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
                        rootDecoItem.RoomId = curRoomId;
                        rootDecoItem.gameObject.name = rootDecoItem.RoomId.ToString();
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
                                //CreateBubbleDecoItemsAtBeginning(lst[0], lst[1], roomId, rec.BubbleId);
                            }
                        }
                    }
                    if (!check)
                    {
                        var rootDecoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabRootDeco, _content);
                        rootDecoItem.RoomId = curRoomId;
                        rootDecoItem.gameObject.name = rootDecoItem.RoomId.ToString();
                        rootDecoItem.gameObject.SetActive(true);
                    }
                }
            }
        }
        public void OnClearDctRootDecoItems()
        {
            DctBubbleDecoItems.Clear();
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

            //foreach (var root in DctRootDecoItems)
            //{
            //    if (root.Key.RoomId != roomId)
            //        root.Key.gameObject.SetActive(false);
            //}

            //foreach (var root in DctRootDecoItems)
            //{
            //    if (root.Key.RoomId == roomId)
            //    {
            //        ToolCreateMapBubbleDecoItems item = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, root.Key.transform);
            //        item.Image.sprite = FLSprite;
            //        item.RoomId = roomId;
            //        item.BubbleId = bubbleId;
            //        item.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = true };
            //        item.gameObject.SetActive(true);
            //        item.Name.text = idPath;
            //        item.gameObject.name = idPath;
            //        DctRootDecoItems[root.Key].Add(item);
            //        item.Deco = deco;
            //        //Debug.LogError("linked");
            //        return item;
            //    }
            //}
            return null;
        }

        public void OnGenerateItem(List<string> lstID, string bubbleId)
        {
            if (DctBubbleDecoItems[bubbleId].Count != 0)
            {
                var lstDecoID = DctBubbleDecoItems[bubbleId];
                _lstCurrentBubbleDeco = _generator.Setup<ToolCreateMapBubbleDecoItems>(lstDecoID.Count);

                for (var i = 0; i < lstDecoID.Count; i++)
                {
                    var idcolor = SGUtils.ParseStringToListInt(lstDecoID[i], '_');
                    int id, color = 0;
                    if (idcolor.Count > 1)
                    {
                        color = SGUtils.ParseStringToListInt(lstDecoID[i], '_')[1];
                    }
                    id = SGUtils.ParseStringToListInt(lstDecoID[i], '_')[0];
                    OnCreateDeco(_lstCurrentBubbleDeco[i], id, color, bubbleId, _configController.DctBubbleIdPrice[bubbleId][i]);
                }
            }
            else
            {
                _lstCurrentBubbleDeco = _generator.Setup<ToolCreateMapBubbleDecoItems>(lstID.Count);
                var config = _configController.ConfigBubbleHome.GetById(bubbleId);
                var lstPrice = config.GetLstPrice();
                int roomId = SGUtils.ParseStringToListInt(bubbleId, '_')[0];
                int idxBubble = SGUtils.ParseStringToListInt(bubbleId, '_')[1];

                for (var i = 0; i < lstID.Count; i++)
                {
                    if (!DctBubbleDecoItems[bubbleId].Contains(lstID[i]))
                    {
                        DctBubbleDecoItems[bubbleId].Add(lstID[i]);
                    }
                    int id = SGUtils.ParseStringToListInt(lstID[i], '_')[0];
                    int color = SGUtils.ParseStringToListInt(lstID[i], '_')[1];
                    OnCreateDeco(_lstCurrentBubbleDeco[i], id, color, bubbleId, lstPrice[i]);

                    var record = _configController.ConfigBubbleHomePosition.GetByRoomId(roomId.ToString());
                    if (record == null)
                    {
                        Debug.LogError("rec bubble home pos null");
                        return;
                    }
                    foreach (var room in _areaManager.ListRooms)
                    {
                        var infoRoot = (DecoInfo)room.Info;
                        if (infoRoot.Id == roomId)
                        {
                            room.Foreach((deco) =>
                            {
                                var info = (DecoInfo)deco.Info;
                                if (info.Id == id && info.Color == color && info.IsBubble && deco.Position == record.GetLstBubblePositionVector3()[idxBubble] + room.Position)
                                {
                                    if (!_toolBubbleSetting.DctDecoInRoom.ContainsKey(bubbleId))
                                    {
                                        _toolBubbleSetting.DctDecoInRoom.Add(bubbleId, deco);
                                    }
                                    else
                                    {
                                        _toolBubbleSetting.DctDecoInRoom[bubbleId] = deco;
                                    }
                                }
                            });
                        }
                    }
                }
            }
        }
        public void OnGenerateMoreBubbleDeco(string bubbleId, string decoId)
        {
            var config = _configController.ConfigBubbleHome.GetById(bubbleId);
            var lstPrice = config.GetLstPrice();
            int newPrice = 0;
            DctBubbleDecoItems[bubbleId].Add(decoId);
            _lstCurrentBubbleDeco = _generator.Setup<ToolCreateMapBubbleDecoItems>(DctBubbleDecoItems[bubbleId].Count);
            for (int i = 0; i < _lstCurrentBubbleDeco.Count; i++)
            {
                int id = SGUtils.ParseStringToListInt(DctBubbleDecoItems[bubbleId][i], '_')[0];
                int color = SGUtils.ParseStringToListInt(DctBubbleDecoItems[bubbleId][i], '_')[1];
                if (i == lstPrice.Count)
                {
                    OnCreateDeco(_lstCurrentBubbleDeco[i], id, color, bubbleId, newPrice);
                }
                else
                {
                    OnCreateDeco(_lstCurrentBubbleDeco[i], id, color, bubbleId, lstPrice[i]);
                }
            }
            _configController.DctBubbleIdPrice[_toolBubbleSetting.CurrentBubbleID].Add(newPrice);
        }

        public void OnCreateDeco(ToolCreateMapBubbleDecoItems item, int id, int color, string bubbleId, int price)
        {
            var colorPath = color > 0 ? "_" + color : "";
            string idPath = id.ToString() + colorPath;
            var record = _configController.ConfigDeco.GetDecoById(id);
            if (record == null)
                return;
            int roomId = SGUtils.ParseStringToListInt(bubbleId, '_')[0];
            var recordBubble = _configController.ConfigBubbleHome.GetById(bubbleId);
            KawaiiAtlas atlas = null;
#if UNITY_EDITOR
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + record.ThemeId + ".asset", record.ThemeId.ToString());
#endif
            var FLSprite = atlas != null ? atlas.GetSprite(idPath) : null;

            item.Image.sprite = FLSprite;
            item.RoomId = roomId;
            item.SetIndex(recordBubble.Index);
            item.SetPrice(price.ToString());
            item.SetStar(recordBubble.Star.ToString());
            item.BubbleId = bubbleId;
            item.Info = new DecoInfo { Id = id, Color = color};
            item.Name.text = idPath;
            item.gameObject.name = idPath;
        }

        public void OnChangeInputStars(string star)
        {
            foreach (var item in _lstCurrentBubbleDeco)
            {
                item.SetStar(star);
            }
            _configController.DctBubbleIdStar[_toolBubbleSetting.CurrentBubbleID] = star;
        }
        public void OnChangeInputPrice(string price, int itemIndex)
        {
            _configController.DctBubbleIdPrice[_toolBubbleSetting.CurrentBubbleID][itemIndex] = int.Parse(price);
        }
    }
}
