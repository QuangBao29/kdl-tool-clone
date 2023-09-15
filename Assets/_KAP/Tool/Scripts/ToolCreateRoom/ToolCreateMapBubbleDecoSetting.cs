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
        private int _oldDirect;
        private Vector3 _oldPos = Vector3.one;

        //BubbleId - list deco id
        [HideInInspector]
        public Dictionary<string, List<string>> _dctBubbleDecoItems = 
            new Dictionary<string, List<string>>();

        public Dictionary<string, List<string>> DctBubbleDecoItems
        {
            get => _dctBubbleDecoItems;
            set => _dctBubbleDecoItems = value;
        }
        public List<ToolCreateMapBubbleDecoItems> LstCurrentBubbleDeco
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
            if (curDeco != null)
            {
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
                _oldPos = curDeco.Position;
                _oldDirect = curDeco.WorldDirect;
            }
            //else
            //{
            //    var bubbldeId = _toolBubbleSetting.CurrentBubbleID;
            //    var roomId = SGUtils.ParseStringToListInt(bubbldeId, '_')[0];
            //    var bubbleIdx = SGUtils.ParseStringToListInt(bubbldeId, '_')[1];
            //    if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            //    {
            //        var record = _configController.ConfigBubbleHome.GetById(bubbldeId);
            //        var recordPos = _configController.ConfigBubbleHomePosition.GetByRoomId(roomId.ToString());
            //    }
            //    else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
            //    {
            //        var recordPos = _configController.ConfigBubblePlayPosition.GetByRoomId(roomId.ToString());
            //    }
            //}
            var newDeco = _importDecoController.CreateDeco(id, color);
            newDeco.Info = new DecoInfo { Id = id, Color = color, IsBubble = true };
            //newDeco.Position = pos;
            //newDeco.WorldDirect = direct;
            newDeco.Position = _oldPos;
            newDeco.WorldDirect = _oldDirect;
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
                    if (itemInfo.Id / 100000 < 22)
                    {
                        string itemId = itemInfo.Id.ToString() + "_" + itemInfo.Color.ToString();
                        _toolBubbleSetting.LstDecoBoxID.Add(itemId);
                        current.Remove();
                    }
                }
            }

            if (_editManager.SetCurrent(decoEdit))
            {
                decoEdit.StartMove();
                decoEdit.EndMove();
            }
            if (decoEdit.EditStatus == KHHEditStatus.Valid) _editManager.SetCurrent(null);
            _toolBubbleSetting.DctDecoInRoom[_toolBubbleSetting.CurrentBubbleID] = newDeco;
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                _configController.DctBubbleIdWD[_toolBubbleSetting.CurrentBubbleID] = newDeco.WorldDirect.ToString();
            }
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

        public void OnGenerateItem(string bubbleId, ToolCreateMapBubbleIDItems item)
        {
            //Debug.LogError("bubbleID: " + bubbleId + " " + item.IsInit);
            if (item.IsInit)
            {
                var lstDecoID = DctBubbleDecoItems[bubbleId];
                LstCurrentBubbleDeco = _generator.Setup<ToolCreateMapBubbleDecoItems>(lstDecoID.Count);

                for (var i = 0; i < lstDecoID.Count; i++)
                {
                    var idcolor = SGUtils.ParseStringToListInt(lstDecoID[i], '_');
                    int id, color = 0;
                    if (idcolor.Count > 1)
                    {
                        color = SGUtils.ParseStringToListInt(lstDecoID[i], '_')[1];
                    }
                    id = SGUtils.ParseStringToListInt(lstDecoID[i], '_')[0];
                    if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                    {
                        OnCreateDeco(LstCurrentBubbleDeco[i], id, color, bubbleId, _configController.DctBubbleIdPrice[bubbleId][i]);
                    }
                    else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                    {
                        OnCreateDeco(LstCurrentBubbleDeco[i], id, color, bubbleId);
                    }
                    
                }
            }
            else
            {
                if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                {
                    List<string> lstID = new List<string>();
                    List<int> lstPrice = new List<int>();
                    var recordHome = _configController.ConfigBubbleHome.GetById(bubbleId);
                    if (recordHome == null)
                    {
                        var tempDeco = _toolBubbleSetting.DctDecoInRoom[bubbleId];
                        var info = (DecoInfo)tempDeco.Info;
                        lstID.Add(info.Id + "_" + info.Color);
                        //Debug.LogError("bubbleID: " + bubbleId + " id: " + info.Id + "_" + info.Color);
                    }
                    else
                    {
                        lstID = recordHome.GetLstBubbleDeco();
                    }
                    LstCurrentBubbleDeco = _generator.Setup<ToolCreateMapBubbleDecoItems>(lstID.Count);
                    var config = _configController.ConfigBubbleHome.GetById(bubbleId);
                    if (config == null)
                    {
                        lstPrice.Add(0);
                    }
                    else
                    {
                        lstPrice = config.GetLstPrice();
                    }

                    for (var i = 0; i < lstID.Count; i++)
                    {
                        if (!DctBubbleDecoItems[bubbleId].Contains(lstID[i]))
                        {
                            DctBubbleDecoItems[bubbleId].Add(lstID[i]);
                        }
                        int id = SGUtils.ParseStringToListInt(lstID[i], '_')[0];
                        int color = SGUtils.ParseStringToListInt(lstID[i], '_')[1];
                        OnCreateDeco(LstCurrentBubbleDeco[i], id, color, bubbleId, lstPrice[i]);
                    }
                }
                else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                {
                    List<string> lstID = new List<string>();
                    var recordHome = _configController.ConfigBubblePlay.GetById(bubbleId);
                    if (recordHome == null)
                    {
                        var tempDeco = _toolBubbleSetting.DctDecoInRoom[bubbleId];
                        var info = (DecoInfo)tempDeco.Info;
                        lstID.Add(info.Id + "_" + info.Color);
                        //Debug.LogError("bubbleID: " + bubbleId + " id: " + info.Id + "_" + info.Color);
                    }
                    else
                    {
                        lstID = recordHome.GetLstBubbleDeco();
                    }
                    LstCurrentBubbleDeco = _generator.Setup<ToolCreateMapBubbleDecoItems>(lstID.Count);

                    for (var i = 0; i < lstID.Count; i++)
                    {
                        if (!DctBubbleDecoItems[bubbleId].Contains(lstID[i]))
                        {
                            DctBubbleDecoItems[bubbleId].Add(lstID[i]);
                        }
                        int id = SGUtils.ParseStringToListInt(lstID[i], '_')[0];
                        int color = SGUtils.ParseStringToListInt(lstID[i], '_')[1];
                        OnCreateDeco(LstCurrentBubbleDeco[i], id, color, bubbleId);
                    }
                }
                item.IsInit = true;
            }
        }
        public void OnGenerateMoreBubbleDeco(string bubbleId, string decoId)
        {
            int newPrice = 0;
            if (!DctBubbleDecoItems[bubbleId].Contains(decoId))
                DctBubbleDecoItems[bubbleId].Add(decoId);
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                _configController.DctBubbleIdPrice[_toolBubbleSetting.CurrentBubbleID].Add(newPrice);
            }
            LstCurrentBubbleDeco = _generator.Setup<ToolCreateMapBubbleDecoItems>(DctBubbleDecoItems[bubbleId].Count);
            //Debug.LogError("count: " + LstCurrentBubbleDeco.Count);
            for (int i = 0; i < LstCurrentBubbleDeco.Count; i++)
            {
                int id = SGUtils.ParseStringToListInt(DctBubbleDecoItems[bubbleId][i], '_')[0];
                int color = SGUtils.ParseStringToListInt(DctBubbleDecoItems[bubbleId][i], '_')[1];
                if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                {
                    OnCreateDeco(LstCurrentBubbleDeco[i], id, color, bubbleId, _configController.DctBubbleIdPrice[bubbleId][i]);
                }
                else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                {
                    OnCreateDeco(LstCurrentBubbleDeco[i], id, color, bubbleId);
                }
            }
            
        }

        public void OnCreateDeco(ToolCreateMapBubbleDecoItems item, int id, int color, string bubbleId, int price = -1)
        {
            var colorPath = color > 0 ? "_" + color : "";
            string idPath = id.ToString() + colorPath;
            var record = _configController.ConfigDeco.GetDecoById(id);
            if (record == null)
                return;
            int roomId = SGUtils.ParseStringToListInt(bubbleId, '_')[0];
            int bubbleIdx = SGUtils.ParseStringToListInt(bubbleId, '_')[1];
            
            KawaiiAtlas atlas = null;
#if UNITY_EDITOR
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + record.ThemeId + ".asset", record.ThemeId.ToString());
#endif
            var FLSprite = atlas != null ? atlas.GetSprite(idPath) : null;

            item.Image.sprite = FLSprite;
            item.RoomId = roomId;
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                var recordBubble = _configController.ConfigBubbleHome.GetById(bubbleId);
                if (recordBubble == null)
                {
                    item.SetIndex(bubbleIdx.ToString());
                    item.SetStar("0");
                }
                else
                {
                    item.SetIndex(recordBubble.Index);
                    item.SetStar(recordBubble.Star.ToString());
                }
                item.SetPrice(price.ToString());
            }
            
            item.BubbleId = bubbleId;
            item.Info = new DecoInfo { Id = id, Color = color};
            item.Name.text = idPath;
            item.gameObject.name = idPath;
        }

        public void OnChangeColorDecos(int oldColor, int newColor)
        {
            var curInfo = (DecoInfo)_editManager.Current.deco.Info;
            if (ToggleDecoMode.isOn)
            {
                for (var i = 0; i < LstCurrentBubbleDeco.Count; i++)
                {
                    var item = LstCurrentBubbleDeco[i];
                    var info = (DecoInfo)item.Info;
                    if (info.Id == curInfo.Id && info.Color == oldColor)
                    {
                        OnCreateDeco(LstCurrentBubbleDeco[i], info.Id, newColor, item.BubbleId, int.Parse(item.GetPrice()));
                        //Debug.LogError("name: " + item.gameObject.name);
                        var curId = DctBubbleDecoItems[item.BubbleId][i];
                        DctBubbleDecoItems[item.BubbleId][i] = SGUtils.ParseStringToList(curId, '_')[0] + "_" + newColor;
                        break;
                    }
                }
            }
        }

        public void OnRemoveBubbleDecos(ToolCreateMapBubbleDecoItems item)
        {
            var deco = _toolBubbleSetting.DctDecoInRoom[_toolBubbleSetting.CurrentBubbleID];
            var info = (DecoInfo)item.Info;
            item.gameObject.SetActive(false);
            LstCurrentBubbleDeco.Remove(item);
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                _configController.DctBubbleIdPrice[_toolBubbleSetting.CurrentBubbleID].Remove(int.Parse(item.GetPrice()));
            }
            DctBubbleDecoItems[_toolBubbleSetting.CurrentBubbleID].Remove(info.Id + "_" + info.Color);

            if (LstCurrentBubbleDeco.Count > 0)
            {
                var swapInfo = (DecoInfo)LstCurrentBubbleDeco[0].Info;
                SwapBubbleDeco(deco, swapInfo.Id, swapInfo.Color);
            }
            else
            {
                if (deco != null)
                {
                    var allChilds = deco.GetAllChilds();
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
                    deco.Remove();
                    _toolBubbleSetting.DctDecoInRoom[_toolBubbleSetting.CurrentBubbleID] = null;
                    if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                    {
                        _configController.DctBubbleIdStar[_toolBubbleSetting.CurrentBubbleID] = "";
                        _configController.DctBubbleIdWD[_toolBubbleSetting.CurrentBubbleID] = "";
                    }
                }
            }
        }

        public void OnChangeInputStars(string star)
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                foreach (var item in LstCurrentBubbleDeco)
                {
                    item.SetStar(star);
                }
                _configController.DctBubbleIdStar[_toolBubbleSetting.CurrentBubbleID] = star;
            }
        }
        public void OnChangeInputPrice(string price, int itemIndex)
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                _configController.DctBubbleIdPrice[_toolBubbleSetting.CurrentBubbleID][itemIndex] = int.Parse(price);
            }
        }
    }
}
