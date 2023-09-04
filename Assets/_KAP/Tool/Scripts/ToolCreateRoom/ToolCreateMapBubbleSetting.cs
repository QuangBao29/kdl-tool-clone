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

        private bool _isInit = false;
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
        private void Init()
        {
            var lstConfig = _configController.ConfigBubbleHomePosition.GetIndexField();
            foreach (var config in lstConfig)
            {
                var roomId = config.Key;
                var count = _configController.ConfigBubbleHomePosition.GetByRoomId(roomId).GetLstBubblePositionVector3().Count;
                for (var i = 0; i < count; i++)
                {
                    string bubbleId = roomId + "_" + i;
                    Debug.LogError("bubbleId: " + bubbleId);
                    if (!_toolBubbleDecoSetting.DctBubbleDecoItems.ContainsKey(bubbleId))
                    {
                        _toolBubbleDecoSetting.DctBubbleDecoItems.Add(bubbleId, new List<string>());
                    }
                }
            }
            _isInit = true;
        }
        #region Bubble Id Item
        public void OnGenerateItem(string roomId)
        {
            if (!_isInit)
            {
                Init();
            }
            int count = 0;
            Debug.LogError("dict: " + _toolBubbleDecoSetting.DctBubbleDecoItems.Count);
            foreach (var pair in _toolBubbleDecoSetting.DctBubbleDecoItems)
            {
                var lstID = SGUtils.ParseStringToListInt(pair.Key, '_');
                Debug.LogError("checK: roomId " + lstID[0] + " " + roomId);
                if (lstID[0].ToString() == roomId)
                {
                    count++;
                }
            }
            Debug.LogError("count " + count);
            _lstCurBubbleIDItem = _generator.Setup<ToolCreateMapBubbleIDItems>(count);
            for (var i = 0; i < count; i++)
            {
                string bubbleId = roomId + "_" + i;
                var item = _lstCurBubbleIDItem[i];
                item.SetBubbleID(bubbleId);
            }
            OnShowPanel();
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

