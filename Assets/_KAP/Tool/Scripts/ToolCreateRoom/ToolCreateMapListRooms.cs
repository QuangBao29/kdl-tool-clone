﻿using System.IO;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;
using Kawaii.ResourceManager;
using Kawaii.IsoTools;
using UnityEngine.UI;
using Pathfinding.Serialization.JsonFx;
using KAP.Tools;
using Imba.UI;
using Imba.Utils;
using UnityEngine.PlayerLoop;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapListRooms : MonoBehaviour
    {
        public class EditModeData
        {
            public EditMode Mode;
            public Color TextColor;
            public int DefaultMapId;

            public string ImportPath;
            public string ExportPath;
            public string ScreenshotPath;

            public EditModeData(EditMode mode, string importPath, string exportPath, string screenshotPath, Color textColor, int defaultMapId)
            {
                this.Mode = mode;

                ImportPath = importPath;
                ExportPath = exportPath;
                ScreenshotPath = screenshotPath;

                TextColor = textColor;
                DefaultMapId = defaultMapId;
            }
        }

        [SerializeField] private AreaManager _areaManager = null;
        [SerializeField] private ToolCreateMapImportDeco _importDecoController = null;
        [SerializeField] private ToolScreenShotRemoveBG _toolScreenShot = null;
        [SerializeField] private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField] private ToolCreateMapConfigController _configController = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField] private ToolScreenBound _toolScreenBound = null;

        [Space]
        [SerializeField] private Transform _transGridRoom = null;
        [SerializeField] private ToolCreateMapListRoomItem _prefabRoomItem = null;
        [SerializeField]
        private Deco _prefabDeco = null;

        [Space]
        [SerializeField] private InputField _inputMapId = null;
        [SerializeField] private Text _txtCurEditMode = null;

        [Space]
        [SerializeField] private ToolCreateMapExportBubbleData _toolExportData = null;

        private List<ToolCreateMapListRoomItem> _lstRoomItems = null;
        private ToolCreateMapListRoomItem _selectedItem = null;

        private Dictionary<EditMode, EditModeData> _dctEditModeData = new Dictionary<EditMode, EditModeData>();
        private Dictionary<EditMode, EditModeData> _dctEditModeDataPostion = new Dictionary<EditMode, EditModeData>();
        private Dictionary<EditMode, EditModeData> _dctEditModeDataBubble = new Dictionary<EditMode, EditModeData>();

        private bool _init = false;

        
        private void Awake()
        {
            InitEditModeData();
            ToolEditMode.Instance.OnChangeEditMode += OnTogglEditThemeChange;
            _toolScreenShot.ActionSetupScreenShoot += SetupScreenShoot;
        }

        private void InitEditModeData()
        {
            string _importRoomPath = "Assets/_KAP/_GameResources/Maps/Rooms/";
            string _exportRoomPath = "/_KAP/_GameResources/Maps/Rooms/";
            string _screenshotRoomPath = "_KAP/_GameResources/Textures/Rooms/";

            string _importThemePath = "Assets/_KAP/_GameResources/Maps/RoomThemes/";
            string _exportThemePath = "/_KAP/_GameResources/Maps/RoomThemes/";
            string _screenshotThemePath = "_KAP/_GameResources/Textures/RoomThemes/";
            string _importWonderPath = "Assets/_KAP/_GameResources/Maps/Wonders/";
            string _exportWonderPath = "/_KAP/_GameResources/Maps/Wonders/";
            string _screenshotWonderPath = "_KAP/_GameResources/Textures/Wonders/";

            string _importRoomChallengePath = "Assets/_KAP/_GameResources/Maps/RoomChallenge";
            string _exportRoomChallengePath = "/_KAP/_GameResources/Maps/RoomChallenge";
            string _screenshotRoomChallengePath = "_KAP/_GameResources/Textures/RoomChallenge";

            string _screenshotRoomHomePath = "_KDL/_GameResources/Textures/RoomHome/";
            string _screenshotRoomPlayPath = "_KDL/_GameResources/Textures/RoomPlay/";

            string _importSeparatedRoomsPath = "Assets/_KAP/_GameResources/Maps/SeparatedRooms/";
            string _exportSeparatedRoomsPath = "/_KAP/_GameResources/Maps/SeparatedRooms/";

            string _importOldRoomsPath = "Assets/_KAP/_GameResources/Maps/OldRooms/";
            string _exportOldRoomsPath = "/_KAP/_GameResources/Maps/OldRooms/";

            string _importEventRoomPath = "Assets/_KAP/_GameResources/Maps/Event/";
            string _exportEventRoomPath = "/_KAP/_GameResources/Maps/Event/";
            string _screenshotEventPath = "_KDL/_GameResources/Textures/Event/";

            string _importPoolDecoPath = "Assets/_KAP/_GameResources/Maps/PoolDeco/";
            string _exportPoolDecoPath = "/_KAP/_GameResources/Maps/PoolDeco/";
            string _screenshotPoolDecoPath = "_KDL/_GameResources/Textures/PoolDeco/";

            _dctEditModeData.Add(EditMode.Home, new EditModeData(EditMode.Home, _importThemePath, _exportThemePath, _screenshotRoomHomePath, Color.black, KAPDefine.DefaultMansionID));
            _dctEditModeData.Add(EditMode.Play, new EditModeData(EditMode.Play, _importRoomPath, _exportRoomPath, _screenshotRoomPlayPath, Color.red, KAPDefine.DefaultRoomId));
            _dctEditModeData.Add(EditMode.SeparatedRoom, new EditModeData(EditMode.SeparatedRoom, _importSeparatedRoomsPath, _exportSeparatedRoomsPath, "", Color.yellow, KAPDefine.DefaultRoomId));
            _dctEditModeData.Add(EditMode.OldRoom, new EditModeData(EditMode.OldRoom, _importOldRoomsPath, _exportOldRoomsPath, "", Color.gray, KAPDefine.DefaultOldMansionID));
            _dctEditModeData.Add(EditMode.Event, new EditModeData(EditMode.Event, _importEventRoomPath, _exportEventRoomPath, _screenshotEventPath, Color.green, KAPDefine.DefaultEventRoomID));
            _dctEditModeData.Add(EditMode.PoolDeco, new EditModeData(EditMode.PoolDeco, _importPoolDecoPath, _exportPoolDecoPath, _screenshotPoolDecoPath, Color.cyan, KAPDefine.DefaultPoolDecoID));
        }

        private void OnTogglEditThemeChange(EditMode editMode)
        {
            var targetEditMode = _dctEditModeData[editMode];

            _txtCurEditMode.text = string.Format("{0} Id:", editMode);
            _txtCurEditMode.color = targetEditMode.TextColor;
            _inputMapId.text = targetEditMode.DefaultMapId.ToString();
        }

        private void SetupScreenShoot()
        {
            var targetEditMode = _dctEditModeData[ToolEditMode.Instance.CurrentEditMode];

            _toolScreenShot.SetSaveFolderPath(targetEditMode.ScreenshotPath);
            _toolScreenShot.SetScreenShotName(string.Format("{0}.png", _inputMapId.text));
        }

        // ======================================================================
        #region Room

        private void Setup()
        {
            var lstRooms = _areaManager.ListRooms;
            int roomItemIndex = 0;
            int roomItemCount = _transGridRoom.childCount;

            _lstRoomItems = new List<ToolCreateMapListRoomItem>();

            if (lstRooms != null)
            {
                int i = 0;
                foreach (var room in lstRooms)
                {
                    var roomInfo = (DecoInfo)room.Info;
                    int roomId = 0;
                    if (roomInfo != null && roomInfo.Id > 1000 && ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                    {
                        roomId = roomInfo.Id;
                    }
                    else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                    {
                        roomId = KAPDefine.DefaultRoomPlayId;
                        room.Info = new DecoInfo { Id = roomId };
                    }
                    else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                    {
                        roomId = int.Parse(_inputMapId.text);
                        room.Info = new DecoInfo { Id = roomId };
                        if (!_configController.DctRoomIdPosition.ContainsKey(roomId))
                            _configController.DctRoomIdPosition.Add(roomId, new List<Vector3>());
                    }
                    else if (ToolEditMode.Instance.CurrentEditMode == EditMode.OldRoom)
                    {
                        roomId = roomInfo.Id;
                    }
                    else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Event)
                    {
                        roomId = int.Parse(_inputMapId.text);
                        room.Info = new DecoInfo { Id = roomId };
                        //Debug.LogError("check info");
                    }
                    else if (ToolEditMode.Instance.CurrentEditMode == EditMode.PoolDeco)
                    {
                        roomId = KAPDefine.DefaultPoolDecoID;
                        room.Info = new DecoInfo { Id = roomId };
                    }

                    //Debug.LogError("roomId: " + roomId);
                    room.name = roomId.ToString();
                    ToolCreateMapListRoomItem item = null;
                    if (roomItemIndex < roomItemCount)
                    {
                        item = _transGridRoom.GetChild(roomItemIndex++).GetComponent<ToolCreateMapListRoomItem>();
                    }
                    else item = SGUtils.InstantiateObject<ToolCreateMapListRoomItem>(_prefabRoomItem, _transGridRoom);

                    if (_init && ToolEditMode.Instance.CurrentEditMode != EditMode.OldRoom)
                    {
                        if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                        {
                            var record = _configController.ConfigBubbleHomePosition.GetByRoomId(roomInfo.Id.ToString());
                            int idx = lstRooms.Count;
                            if (record != null)
                            {
                                idx = record.Index;
                            }

                            item.Setup(room, i, idx);
                        }
                        else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                        {
                            item.Setup(room, i, 0);
                        }
                        else
                        {
                            item.Setup(room, i, 0);
                        }
                        item.gameObject.SetActive(true);
                    }
                    else
                    {
                        item.gameObject.SetActive(true);
                        item.Setup(room, i, 0);
                    }

                    //Add Item to List
                    _lstRoomItems.Add(item);
                    //if (ToolEditMode.Instance.CurrentEditMode != EditMode.OldRoom)
                    //{
                    //    item.OnClickRoomItem();
                    //}
                    ++i;
                }
                if (ToolEditMode.Instance.CurrentEditMode != EditMode.OldRoom)
                {
                    foreach (var room in lstRooms)
                    {
                        var roomInfo = (DecoInfo)room.Info;
                        int roomId = roomInfo.Id;
                        if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                        {
                            var lstRecPos = _configController.ListConfigBubbleHomePositionRecords;
                            var lstRecHome = _configController.ListConfigBubbleHomeRecords;
                            foreach (var rec in lstRecPos)
                            {
                                if (rec.RoomId == roomId.ToString())
                                {
                                    var lstPos = rec.GetLstBubblePositionVector3();
                                    List<string> lstRoomDecoId = new List<string>();
                                    foreach (var bubbleRec in lstRecHome)
                                    {
                                        var rID = SGUtils.ParseStringToListInt(bubbleRec.BubbleId, '_')[0];
                                        if (roomId == rID)
                                        {
                                            var decoid = SGUtils.ParseStringToList(bubbleRec.BubbleDecoIds, ';')[0];
                                            var id = SGUtils.ParseStringToList(decoid, '_')[0];
                                            lstRoomDecoId.Add(id);
                                        }
                                    }
                                    room.Foreach((deco) =>
                                    {
                                        var info = (DecoInfo)deco.Info;
                                        if (lstRoomDecoId.Contains(info.Id.ToString()))
                                        {
                                            foreach (var pos in lstPos)
                                            {
                                                var isoPos = pos + room.Position;
                                                if (isoPos == deco.Position)
                                                {
                                                    deco.Info = new DecoInfo { Id = info.Id, Color = info.Color, IsBubble = true };
                                                }
                                            }
                                        }
                                    });
                                    break;
                                }
                            }
                        }
                        if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                        {
                            var lstRecPos = _configController.ListConfigBubblePlayPositionRecords;
                            var lstRecPlay = _configController.ListConfigBubblePlayRecords;
                            foreach (var rec in lstRecPos)
                            {
                                if (rec.RoomId == roomId.ToString())
                                {
                                    var lstPos = rec.GetLstBubblePositionVector3();
                                    List<string> lstRoomDecoId = new List<string>();
                                    foreach (var bubbleRec in lstRecPlay)
                                    {
                                        var rID = SGUtils.ParseStringToListInt(bubbleRec.BubbleId, '_')[0];
                                        if (roomId == rID)
                                        {
                                            var decoid = SGUtils.ParseStringToList(bubbleRec.BubbleDecoIds, ';')[0];
                                            var id = SGUtils.ParseStringToList(decoid, '_')[0];
                                            lstRoomDecoId.Add(id);
                                        }
                                    }
                                    room.Foreach((deco) =>
                                    {
                                        var info = (DecoInfo)deco.Info;
                                        if (lstRoomDecoId.Contains(info.Id.ToString()))
                                        {
                                            foreach (var pos in lstPos)
                                            {
                                                var isoPos = pos + room.Position;
                                                if (isoPos == deco.Position)
                                                {
                                                    deco.Info = new DecoInfo { Id = info.Id, Color = info.Color, IsBubble = true };
                                                }
                                            }
                                        }
                                    });
                                    _toolBubbleDecoSetting.BaseGem.text = rec.BaseGem;
                                    break;
                                }
                            }
                        }
                        if (ToolEditMode.Instance.CurrentEditMode == EditMode.Event)
                        {
                            var lstRec = _configController.ListConfigRoomCloserBetterRecords;
                            foreach (var rec in lstRec)
                            {
                                if (rec.RoomId == roomId.ToString())
                                {
                                    _toolBubbleDecoSetting.BaseGem.text = rec.BaseGem;
                                    break;
                                }
                            }
                        }
                    }
                    //Debug.LogError("count: " + _lstRoomItems.Count);
                    OnUnselectAllItems();
                    //_toolBubbleDecoSetting.OnHideAllItems();
                }
            }
            for (; roomItemIndex < roomItemCount; ++roomItemIndex)
            {
                _transGridRoom.GetChild(roomItemIndex).gameObject.SetActive(false);
            }

            if (_init)
            {
                var count = _configController.ListConfigBubbleHomePositionRecords.Count;
                for (var c = 1; c <= count; c++)
                {
                    var rec = _configController.ListConfigBubbleHomePositionRecords[c - 1];
                    if (rec == null)
                        Debug.LogError("rec null");
                    foreach (var room in _lstRoomItems)
                    {
                        if (room.GetRoomId().ToString() == rec.RoomId)
                        {
                            //Debug.LogError("check: " + rec.RoomId);
                            room.gameObject.transform.SetSiblingIndex(c - 1);
                        }
                    }
                }
            }
            _init = true;
        }
        
        public void OnButtonAddClick()
        {
            var room = _importDecoController.CreateARoom(_areaManager.ListRooms.Count, Vector3.zero, Vector3.one);
            Setup();
            //var item = SGUtils.InstantiateObject<ToolCreateMapListRoomItem>(_prefabRoomItem, _transGridRoom);
            
            //if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            //{
            //    int roomId = KAPDefine.DefaultRoomId;
            //    room.Info = new DecoInfo { Id = roomId };
            //    int idx = _areaManager.ListRooms.Count;
            //    item.gameObject.SetActive(true);
            //    item.Setup(room, roomId, idx);
            //}
            //if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
            //{
            //    int roomId = KAPDefine.DefaultRoomPlayId;
            //    room.Info = new DecoInfo { Id = roomId };
            //    item.gameObject.SetActive(true);
            //    item.Setup(room, roomId, 0);
            //}
            //if (ToolEditMode.Instance.CurrentEditMode == EditMode.Event)
            //{
            //    int roomId = KAPDefine.DefaultEventRoomID;
            //    room.Info = new DecoInfo { Id = roomId };
            //    item.gameObject.SetActive(true);
            //    item.Setup(room, roomId, 0);
            //}
            //if (ToolEditMode.Instance.CurrentEditMode == EditMode.PoolDeco)
            //{
            //    int roomId = KAPDefine.DefaultPoolDecoID;
            //    room.Info = new DecoInfo { Id = roomId };
            //    item.gameObject.SetActive(true);
            //    item.Setup(room, roomId, 0);
            //}
            //Setup();
        }

        public void RemoveARoom(DecoRoot room)
        {
            if (room == null)
                return;
            _areaManager.RemoveRoom(room);
            ToolCreateMapListRoomItem temp = null;
            foreach (var roomItem in _lstRoomItems)
            {
                if (roomItem.GetRoomId() == ((DecoInfo)room.Info).Id)
                {
                    temp = roomItem;
                    Destroy(roomItem.gameObject);
                    Debug.LogError("done");
                }
            }
            _lstRoomItems.Remove(temp);
            Debug.LogError("count of lstroomitem: " + _lstRoomItems.Count);
            Setup();
        }


        public List<ToolCreateMapListRoomItem> GetLstRoomItem()
        {
            return _lstRoomItems;
        }

        public void OnUnselectAllItems()
        {
            foreach (var item in _lstRoomItems)
            {
                if (item.IsSelect)
                    item.UnSelectRoomItem();
            }
        }
        public ToolCreateMapListRoomItem GetSelectedItem()
        {
            return _selectedItem;
        }
        public void SetSelectedItem(ToolCreateMapListRoomItem item)
        {
            _selectedItem = item;
        }
        #endregion
        // ================================================================
        #region Import

        public string GetImportPath()
        {
            var targetEditMode = _dctEditModeData[ToolEditMode.Instance.CurrentEditMode];

            string mapName = _inputMapId.text + ".json";
            return Path.Combine(targetEditMode.ImportPath, mapName);
        }

        public string GetImportPath(string roomId)
        {
            var targetEditMode = _dctEditModeData[ToolEditMode.Instance.CurrentEditMode];

            string mapName = roomId + ".json";
            return Path.Combine(targetEditMode.ImportPath, mapName);
        }

        public void OnButtonImportClick()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(_inputMapId.text))
                return;
            _areaManager.ClearAllRooms();
            string path = GetImportPath();
            var textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if(textAsset == null)
            {
                Debug.LogError(string.Format("Can't load file: {0}", path));
                return;
            }

            var json = textAsset.text;
            var lstRooms = JsonReader.Deserialize<DecoDataTree[]>(json);
            for (var i = 0; i < lstRooms.Length; ++i)
            {
                var roomData = lstRooms[i];

                if (string.IsNullOrEmpty(roomData.Info))
                {
                    var info = new DecoInfo { Id = roomData.RoomId };
                    roomData.Info = JsonWriter.Serialize(info);
                }
                _importDecoController.ImportFromDataTree(roomData);
            }

            Debug.LogError(string.Format("Import success: {0}", path));
            Setup();
#endif
        }

        public void OnButtonImportNewClick()
        {
            if (string.IsNullOrEmpty(_inputMapId.text))
                return;
            int roomId = Int32.Parse(_inputMapId.text);
            _areaManager.ClearAllRooms();
            //_toolBubbleDecoSetting.OnClearDctRootDecoItems();
            string path = "";
            path = GetImportPath();
            var json = FileSaving.Load(path);
            if (!string.IsNullOrEmpty(json))
            {
                var lstRooms = JsonReader.Deserialize<Dictionary<string, DecoDataArray[]>>(json);
                _importDecoController.Import(lstRooms);
            }
            
            Setup();

            _configController.InitDataConfig();

            _toolBubbleSetting.Init();

            CountDecoUnpack();
            string txt = _txtCurEditMode.text.Substring(0, _txtCurEditMode.text.Length - 4);
            if (txt == EditMode.Play.ToString()
                || txt == EditMode.Event.ToString())
                _toolScreenBound.InitRoomPlay(roomId);
            else
                _toolScreenBound.Init();
        }
        public void OnClickScreenshotAllPlayRoom()
        {
            string txt = _txtCurEditMode.text.Substring(0, _txtCurEditMode.text.Length - 4);
            if (txt == EditMode.Play.ToString())
                StartCoroutine("OnButtonScreenShotMultiRoomClick");
            else if(txt == EditMode.Event.ToString())
                StartCoroutine("OnButtonScreenShotMultiRoomEventClick");


        }
        IEnumerator OnButtonScreenShotMultiRoomClick()
        {
            foreach(var record in _configController.ConfigBubblePlayPosition.Records)
            {
                Debug.LogError("record Id: " + record.RoomId);
                _inputMapId.text = record.RoomId;

                OnButtonImportNewClick();

                _toolScreenShot.OnScreenShotClick();

                yield return new WaitForSeconds(2);
            }
        }

        IEnumerator OnButtonScreenShotMultiRoomEventClick()
        {
            foreach (var record in _configController.ConfigRoomCloserBetter.Records)
            {
                Debug.LogError("record Id: " + record.RoomId);
                _inputMapId.text = record.RoomId;

                OnButtonImportNewClick();

                _toolScreenShot.OnScreenShotClick();

                yield return new WaitForSeconds(2);
            }
        }
        #endregion
        // ================================================================
        #region Export 

        public string GetExportPath()
        {
            var targetEditMode = _dctEditModeData[ToolEditMode.Instance.CurrentEditMode];

            string mapName = string.Format("{0}.json", _inputMapId.text);
            return Application.dataPath + Path.Combine(targetEditMode.ExportPath, mapName);
        }

        public void OnButtonExportClick()
        {
            if (string.IsNullOrEmpty(_inputMapId.text))
                return;
            
            var targetEditMode = _dctEditModeData[ToolEditMode.Instance.CurrentEditMode];
            string mess = string.Format("export <b>{0}</b>: <color=blue>{1}</color>", targetEditMode.Mode.ToString().ToUpper(), _inputMapId.text);

            UIManager.ShowMessage("", mess, UIMessageBox.MessageBoxType.OK_Cancel, (result) =>
            {
                if (result == UIMessageBox.MessageBoxAction.Accept)
                {
                    var data = _areaManager.Export(null);
                    string path = GetExportPath();
                    FileSaving.Save(path, JsonWriter.Serialize(data));
                    Debug.LogError(string.Format("Export success: {0} - {1}", path, data));
                    UIManager.ShowAlertMessage("Export success !!!");
                }

                return true;
            });
        }

        public void OnButtonExportNewClick()
        {
            if (string.IsNullOrEmpty(_inputMapId.text))
                return;
            string mess = "";
            var targetEditMode = _dctEditModeData[ToolEditMode.Instance.CurrentEditMode];
            Debug.LogError("current Mode: " + targetEditMode.Mode);
            mess = string.Format("export {0}: {1}", targetEditMode.Mode, _inputMapId.text);

            UIManager.ShowMessage("", mess, UIMessageBox.MessageBoxType.OK_Cancel, (result) =>
            {
                if (result == UIMessageBox.MessageBoxAction.Accept)
                {
                    var data = _areaManager.Export(null);
                    string path = GetExportPath();
                    Debug.Log("path: " + path);
                    Debug.Log("data: " + JsonWriter.Serialize(data));
                    FileSaving.Save(path, JsonWriter.Serialize(data));
                    Debug.LogError("Export new success");
                }

                return true;
            });
        }
        public void ExportData()
        {
            var data = _areaManager.Export(null);
            string path = GetExportPath();
            Debug.Log("path: " + path);
            Debug.Log("data: " + JsonWriter.Serialize(data));
            FileSaving.Save(path, JsonWriter.Serialize(data));
            Debug.LogError("Export Json success");
        }

        public void OnExportSeparatedRoomsInTheme()
        {
            foreach (var r in _areaManager.ListRooms)
            {
                var info = (DecoInfo)r.Info;
                var roomId = info.Id.ToString();
                string mapName = string.Format("{0}.json", _inputMapId.text + roomId);
                var targetEditMode = _dctEditModeData[EditMode.SeparatedRoom];
                string path = Application.dataPath + Path.Combine(targetEditMode.ExportPath, mapName);
                string mess = string.Format("export {0}: {1}", "Separated Rooms", _inputMapId.text);

                var data = _toolExportData.Export(null, r);
                //Debug.LogError("path: " + path);
                //Debug.LogError("data: " + JsonWriter.Serialize(data));
                FileSaving.Save(path, JsonWriter.Serialize(data));
                Debug.LogError("Export Separated Rooms " + _inputMapId.text + roomId + " success");
            }
        }

        public void OnExportJsonPoolDeco()
        {
            //get list old decos
            List<int> lstOldDecos = new List<int>();
            var path = Path.Combine(_dctEditModeData[EditMode.OldRoom].ImportPath, "2520031.json");
            var json = FileSaving.Load(path);
            if (!string.IsNullOrEmpty(json))
            {
                var lstRooms = JsonReader.Deserialize<Dictionary<string, DecoDataArray[]>>(json);
                var lstLevels = new List<int>();
                foreach (var iter in lstRooms)
                {
                    int level = 0;
                    if (int.TryParse(iter.Key, out level))
                        lstLevels.Add(level);
                }

                SGUtils.BubbleSort<int>(lstLevels, (l1, l2) => { return l1 < l2; });
                foreach (var level in lstLevels)
                {
                    var lstDecos = lstRooms[level.ToString()];
                    foreach (var deco in lstDecos)
                    {
                        if (!string.IsNullOrEmpty(deco.Info))
                        {
                            var info = JsonReader.Deserialize<DecoInfo>(deco.Info);
                            if (!lstOldDecos.Contains(info.Id) && info.Id / 100000 < 22)
                                lstOldDecos.Add(info.Id);
                        }
                    }
                }
            }

            //get list current deco reward
            List<int> lstDecoReward = new List<int>();
            var recordsReward = _configController.ConfigBubbleHomePosition.GetIndexField();
            foreach (var rec in recordsReward)
            {
                var strReward = rec.Value[0].LstDecoReward;
                var lst = SGUtils.ParseStringToList(strReward, ';');
                foreach (var item in lst)
                {
                    var id = SGUtils.ParseStringToListInt(item, '_')[0];
                    if (!lstDecoReward.Contains(id))
                    {
                        lstDecoReward.Add(id);
                    }
                }
            }

            //export json Pool Deco
            List<Deco> lstDeco = new List<Deco>();
            var dataResult = new Dictionary<int, List<Dictionary<string, object>>>();
            var records = _configController.ConfigDeco.GetIndexField<int>("Id");
            int count = 0;
            foreach (var rec in records)
            {
                int x = rec.Value[0].SizeX;
                int y = rec.Value[0].SizeY;
                int z = rec.Value[0].SizeZ;
                if (rec.Key / 100000 < 22 && (!lstOldDecos.Contains(rec.Key)) && (!lstDecoReward.Contains(rec.Key)) && x*y*z < 27)
                {
                    count++;
                    //Deco deco = new Deco();
                    var deco = SGUtils.InstantiateObject<Deco>(_prefabDeco, null);
                    deco.Info = new DecoInfo { Id = rec.Key };
                    if (deco == null)
                        Debug.LogError("deco null");
                    _toolExportData.ExportDataPoolDeco(dataResult, deco);
                }
            }
            Debug.LogError("count: " + count);
            var exportPath = Application.dataPath + Path.Combine(_dctEditModeData[EditMode.PoolDeco].ExportPath, KAPDefine.DefaultPoolDecoID.ToString() + ".json");
            var data = _toolExportData.ExportPoolDeco(dataResult);
            string newJson = JsonWriter.Serialize(data);
            FileSaving.Save(exportPath, newJson);

            Debug.LogError("new json: " + newJson);

            Debug.LogError("Export Json success");
        }

        #endregion

        private void CountDecoUnpack()
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
            {
                int count8 = 0;     //can put another on it + thamr
                int count24 = 0;    //can put on another deco
                int count32 = 0;    //wall hang
                int total = 0;
                if (_areaManager.ListRooms.Count == 0)
                    return;
                var root = _areaManager.ListRooms[0];
                var infoRoot = (DecoInfo)root.Info;
                root.Foreach((deco) => {
                    var info = (DecoInfo)deco.Info;
                    if (!info.IsBubble && info.Id / 100000 < 22 && info.Id != infoRoot.Id)
                    {
                        total++;
                        var parentInfo = (DecoInfo)deco.PieceParent.DecoParent.Info;
                        if (parentInfo.Id == infoRoot.Id)
                        {
                            count8++;
                        }
                        else if (parentInfo.Id / 100000 > 22)
                        {
                            count32++;
                        }
                        else if (parentInfo.Id / 100000 < 22)
                        {
                            count24++;
                        }
                    }
                });
                Debug.LogError(infoRoot.Id + ": On floor: " + count8 + 
                    "; On decos: " + count24 + "; Wallhang: " + count32);
            }
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Event)
            {
                int count8 = 0;     //can put another on it + thamr
                int count24 = 0;    //can put on another deco
                int count32 = 0;    //wall hang
                int total = 0;
                if (_areaManager.ListRooms.Count == 0)
                    return;
                var root = _areaManager.ListRooms[0];
                var infoRoot = (DecoInfo)root.Info;
                root.Foreach((deco) => {
                    var info = (DecoInfo)deco.Info;
                    if (!info.IsStatic && info.Id / 100000 < 22 && info.Id != infoRoot.Id)
                    {
                        total++;
                        var parentInfo = (DecoInfo)deco.PieceParent.DecoParent.Info;
                        if (parentInfo.Id == infoRoot.Id)
                        {
                            count8++;
                        }
                        else if (parentInfo.Id / 100000 > 22)
                        {
                            count32++;
                        }
                        else if (parentInfo.Id / 100000 < 22)
                        {
                            count24++;
                        }
                    }
                });
                Debug.LogError(infoRoot.Id + ": On floor: " + count8 +
                    "; On decos: " + count24 + "; Wallhang: " + count32);
            }
        }
    }
}