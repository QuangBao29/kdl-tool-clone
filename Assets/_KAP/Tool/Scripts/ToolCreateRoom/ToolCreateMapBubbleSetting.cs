using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KAP.Tools;
using Kawaii.IsoTools;
using Kawaii.IsoTools.DecoSystem;
using System.Linq;
using Kawaii.ResourceManager;
using Kawaii.Utils;

namespace KAP.ToolCreateMap
{   
    public class ToolCreateMapBubbleSetting : MonoBehaviour
    {
        [SerializeField] private Bubble _prefabBubble = null;
        [SerializeField] private EditManager _editManager = null;
        [SerializeField] private GameObject _bubbleContent = null;
        [SerializeField] private InputField _inputMapId = null;
        [SerializeField] private AreaManager _areaManager = null;
        [SerializeField] private GameObject _panelListBubble = null;
        [SerializeField] private ToolCreateMapImportDeco _importDeco = null;

        [Header("Deco Item")]
        [SerializeField] private Transform _transGrid = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField] private ToolCreateMapConfigController _configController = null;

        private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";
        
        [Header("Bubble ID Item")]
        [SerializeField] private ListItemGenerator _generator = null;
        [SerializeField] private GameObject _panelDecoBubble = null;
        [SerializeField] private GameObject _panelBubbleID = null;
        [SerializeField] private GameObject _btnBack = null;

        private List<string> _lstDecoBoxID = new List<string>();
        private List<ToolCreateMapBubbleIDItems> _lstCurBubbleIDItem = new List<ToolCreateMapBubbleIDItems>();
        //bubbleId - Deco in room
        private Dictionary<string, Deco> _dctDecoInRoom = new Dictionary<string, Deco>();
        private string _bubbleId;
        public List<ToolCreateMapBubbleIDItems> LstCurBubbleIDItem
        {
            set => _lstCurBubbleIDItem = value;
            get => _lstCurBubbleIDItem;
        }
        public string CurrentBubbleID
        {
            set => _bubbleId = value;
            get => _bubbleId;
        }
        public Dictionary<string, Deco> DctDecoInRoom
        {
            get => _dctDecoInRoom;
            set => _dctDecoInRoom = value;
        }
        public List<string> LstDecoBoxID
        {
            get => _lstDecoBoxID;
            set => _lstDecoBoxID = value;
        }
        public void Init()
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                var lstConfig = _configController.ConfigBubbleHomePosition.GetIndexField();
                foreach (var config in lstConfig)
                {
                    var roomId = config.Key;
                    var count = config.Value[0].GetLstBubblePositionVector3().Count;
                    for (var i = 0; i < count; i++)
                    {
                        string bubbleId = roomId + "_" + i;
                        var rec = _configController.ConfigBubbleHome.GetById(bubbleId);

                        if (!_toolBubbleDecoSetting.DctBubbleDecoItems.ContainsKey(bubbleId))
                        {
                            _toolBubbleDecoSetting.DctBubbleDecoItems.Add(bubbleId, rec.GetLstBubbleDeco());
                        }
                    }
                }
                foreach (var room in _areaManager.ListRooms)
                {
                    var roomInfo = (DecoInfo)room.Info;
                    var record = _configController.ConfigBubbleHomePosition.GetByRoomId(roomInfo.Id.ToString());
                    var lstPos = record.GetLstBubblePositionVector3();
                    var count = lstPos.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var bubbleId = roomInfo.Id + "_" + i;
                        room.Foreach((deco) =>
                        {
                            var decoInfo = (DecoInfo)deco.Info;
                            if (decoInfo.IsBubble && deco.Position == room.Position + lstPos[i])
                            {
                                if (!DctDecoInRoom.ContainsKey(bubbleId))
                                {
                                    DctDecoInRoom.Add(bubbleId, deco);
                                }
                                else
                                {
                                    DctDecoInRoom[bubbleId] = deco;
                                }
                            }
                        });
                    }
                }
            }
            else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
            {
                var rootInfo = (DecoInfo)_areaManager.ListRooms[0].Info;
                var rec = _configController.ConfigBubblePlayPosition.GetByRoomId(rootInfo.Id.ToString());
                var c = rec.GetLstBubblePositionVector3().Count;
                for (var i = 0; i < c; i++)
                {
                    var bubbleId = rootInfo.Id + "_" + i;
                    var config = _configController.ConfigBubblePlay.GetById(bubbleId);
                    if (!_toolBubbleDecoSetting.DctBubbleDecoItems.ContainsKey(bubbleId))
                    {
                        _toolBubbleDecoSetting.DctBubbleDecoItems.Add(bubbleId, config.GetLstBubbleDeco());
                    }
                    //Debug.LogError("check count: " + _toolBubbleDecoSetting.DctBubbleDecoItems[bubbleId].Count);
                }

                foreach (var room in _areaManager.ListRooms)
                {
                    var roomInfo = (DecoInfo)room.Info;
                    var record = _configController.ConfigBubblePlayPosition.GetByRoomId(roomInfo.Id.ToString());
                    var lstPos = record.GetLstBubblePositionVector3();
                    var count = lstPos.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var bubbleId = roomInfo.Id + "_" + i;
                        room.Foreach((deco) =>
                        {
                            var decoInfo = (DecoInfo)deco.Info;
                            if (decoInfo.IsBubble && deco.Position == room.Position + lstPos[i])
                            {
                                //Debug.LogError("found deco");
                                if (!DctDecoInRoom.ContainsKey(bubbleId))
                                {
                                    DctDecoInRoom.Add(bubbleId, deco);
                                }
                                else
                                {
                                    DctDecoInRoom[bubbleId] = deco;
                                }
                            }
                        });
                    }
                }
            }
        }
        #region Bubble Id Item
        public void OnGenerateItem(string roomId)
        {
            int count = 0;
            foreach (var pair in _toolBubbleDecoSetting.DctBubbleDecoItems)
            {
                var lstID = SGUtils.ParseStringToListInt(pair.Key, '_');
                if (lstID[0].ToString() == roomId)
                {
                    count++;
                }
            }
            //Debug.LogError("count check: " + count);
            _lstCurBubbleIDItem = _generator.Setup<ToolCreateMapBubbleIDItems>(count);
            for (var i = 0; i < count; i++)
            {
                string bubbleId = roomId + "_" + i;
                var item = _lstCurBubbleIDItem[i];
                item.SetBubbleID(bubbleId);
                //item.OnClickGenerateDecoItem();
            }
            OnClickBackToBubbleID();
        }
        
        public void OnHidePanel()
        {
            _panelBubbleID.SetActive(false);
        }
        public void OnShowPanel()
        {
            _panelBubbleID.SetActive(true);
        }
        public void OnShowViewDecoBubble()
        {
            OnHidePanel();
            _panelDecoBubble.SetActive(true);
            _btnBack.SetActive(true);
        }
        public void OnClickBackToBubbleID()
        {
            OnShowPanel();
            _panelDecoBubble.SetActive(false);
            _btnBack.SetActive(false);
        }
        #endregion

        public void OnClickDecoBox()
        {
            if (LstDecoBoxID.Count > 0)
            {
                var idPath = LstDecoBoxID[0];
                var listId = SGUtils.ParseStringToListInt(idPath, '_');
                int id = listId[0];
                int color = listId[1];
                if (listId.Count > 1) color = listId[1];
                var deco = _importDeco.CreateDeco(id, color);
                deco.Info = new DecoInfo { Id = id, Color = color, IsBubble = false };
                deco.Position = IsoWorld.WorldToIso(Camera.main.transform.position, 0);
                var decoEdit = deco.GetComponent<DecoEditDemo>();
                if (_editManager.SetCurrent(decoEdit))
                {
                    decoEdit.StartMove();
                    decoEdit.EndMove();
                    _editManager.editTool.SetValid(decoEdit.EditStatus);
                }
                LstDecoBoxID.Remove(idPath);
            }
        }
    }
}

