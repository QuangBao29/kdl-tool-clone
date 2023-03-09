using System.IO;
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

        [Space]
        [SerializeField] private Transform _transGridRoom = null;
        [SerializeField] private ToolCreateMapListRoomItem _prefabRoomItem = null;

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

            string _importDecoRewardsPath = "Assets/_KAP/_GameResources/Maps/DecoRewards/";
            string _exportDecoRewardsPath = "/_KAP/_GameResources/Maps/DecoRewards/";

            //_dctEditModeData.Add(EditMode.Room, new EditModeData(EditMode.Room, _importRoomPath, _exportRoomPath, _screenshotRoomPath, Color.black, KAPDefine.DefaultRoomId));
            //_dctEditModeData.Add(EditMode.Theme, new EditModeData(EditMode.Theme, _importThemePath, _exportThemePath, _screenshotThemePath, Color.red, KAPDefine.DefaultRoomThemeId));
            //_dctEditModeData.Add(EditMode.Wonder, new EditModeData(EditMode.Wonder, _importWonderPath, _exportWonderPath, _screenshotWonderPath, Color.blue, KAPDefine.DefaultWonderId));
            //_dctEditModeData.Add(EditMode.RoomChallenge, new EditModeData(EditMode.RoomChallenge, _importRoomChallengePath, _exportRoomChallengePath, _screenshotRoomChallengePath ,Color.yellow, KAPDefine.DefaultRoomChallengeID));
            _dctEditModeData.Add(EditMode.Home, new EditModeData(EditMode.Home, _importThemePath, _exportThemePath, _screenshotRoomHomePath, Color.black, KAPDefine.DefaultMansionID));
            _dctEditModeData.Add(EditMode.Play, new EditModeData(EditMode.Play, _importRoomPath, _exportRoomPath, _screenshotRoomPlayPath, Color.red, KAPDefine.DefaultRoomId));
            _dctEditModeData.Add(EditMode.SeparatedRoom, new EditModeData(EditMode.SeparatedRoom, _importSeparatedRoomsPath, _exportSeparatedRoomsPath, "", Color.yellow, KAPDefine.DefaultRoomId));
            _dctEditModeData.Add(EditMode.DecoReward, new EditModeData(EditMode.DecoReward, _importDecoRewardsPath, _exportDecoRewardsPath, "", Color.black, KAPDefine.DefaultRoomPlayKDLID));
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
                foreach(var room in lstRooms)
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
                    }
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
                                room.Foreach((deco) => {
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
                                break;
                            }
                        }
                    }

                    Debug.LogError("roomId: " + roomId);
                    room.name = roomId.ToString();
                    ToolCreateMapListRoomItem item = null;
                    if (roomItemIndex < roomItemCount)
                        item = _transGridRoom.GetChild(roomItemIndex++).GetComponent<ToolCreateMapListRoomItem>();
                    else
                        item = SGUtils.InstantiateObject<ToolCreateMapListRoomItem>(_prefabRoomItem, _transGridRoom);

                    if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                    {
                        var record = _configController.ConfigBubbleHomePosition.GetByRoomId(roomInfo.Id.ToString());
                        int idx = 0;
                        if (record != null)
                            idx = record.Index;
                        else return;
                        item.gameObject.SetActive(true);
                        item.Setup(room, i, idx);
                    }
                    if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                    {
                        item.gameObject.SetActive(true);
                        item.Setup(room, i, 0);
                    }
                    if (ToolEditMode.Instance.CurrentEditMode == EditMode.DecoReward)
                    {
                        item.gameObject.SetActive(true);
                        item.Setup(room, i, 0);
                    }

                    //Add Item to List
                    _lstRoomItems.Add(item);
                    item.OnClickRoomItem();
                    ++i;
                }
                OnUnselectAllItems();
                _toolBubbleDecoSetting.OnHideAllItems();
            }




            for (; roomItemIndex < roomItemCount; ++roomItemIndex)
                _transGridRoom.GetChild(roomItemIndex).gameObject.SetActive(false);
        }

        public void OnButtonAddClick()
        {
            _importDecoController.CreateARoom(_areaManager.ListRooms.Count, Vector3.zero, Vector3.one);
            Setup();
            //_toolBubbleSetting.UpdateListNumOfBubbleInARoom();
            //_toolBubbleSetting.DebugForCheck();
        }

        public void RemoveARoom(DecoRoot room)
        {
            if (room == null)
                return;
            _areaManager.RemoveRoom(room);
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
            _areaManager.ClearAllRooms();
            _toolBubbleDecoSetting.OnClearDctRootDecoItems();
            string path = "";
            path = GetImportPath();
            var json = FileSaving.Load(path);
            if (!string.IsNullOrEmpty(json))
            {
                var lstRooms = JsonReader.Deserialize<Dictionary<string, DecoDataArray[]>>(json);
                _importDecoController.Import(lstRooms);
            }
            Setup();
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

        public void OnExportSeparatedRoomsInTheme()
        {
            var temp = _inputMapId.text;
            foreach (var r in _areaManager.ListRooms)
            {
                var info = (DecoInfo)r.Info;
                _inputMapId.text = info.Id.ToString();
                string mapName = string.Format("{0}.json", temp + _inputMapId.text);
                var targetEditMode = _dctEditModeData[EditMode.SeparatedRoom];
                string path = Application.dataPath + Path.Combine(targetEditMode.ExportPath, mapName);
                string mess = string.Format("export {0}: {1}", "Separated Rooms", _inputMapId.text);

                var data = _toolExportData.Export(null, r);
                Debug.LogError("path: " + path);
                Debug.LogError("data: " + JsonWriter.Serialize(data));
                FileSaving.Save(path, JsonWriter.Serialize(data));
                Debug.LogError("Export Separated Rooms " + _inputMapId.text + " success");
            }
            _inputMapId.text = temp;
        }

        //public void OnButtonExportDecoKDLClick()
        //{
        //    if (string.IsNullOrEmpty(_inputMapId.text))
        //        return;

        //    string mess = "";
        //    var targetEditMode = _dctEditModeData[ToolEditMode.Instance.CurrentEditMode];
        //    mess = string.Format("export {0}: {1}", targetEditMode.Mode, _inputMapId.text);

        //    if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
        //    {
        //        UIManager.ShowMessage("", mess, UIMessageBox.MessageBoxType.OK_Cancel, (result) =>
        //        {
        //            if (result == UIMessageBox.MessageBoxAction.Accept)
        //            {
        //                var data = _toolExportData.Export(null);
        //                string path = GetExportPath();
        //                Debug.Log("path: " + path);
        //                Debug.Log("data: " + JsonWriter.Serialize(data));
        //                FileSaving.Save(path, JsonWriter.Serialize(data));
        //                Debug.LogError("Export new success");
        //            }

        //            return true;
        //        });
        //    }
        //    else
        //    {
        //        UIManager.ShowMessage("", mess, UIMessageBox.MessageBoxType.OK_Cancel, (result) =>
        //        {
        //            if (result == UIMessageBox.MessageBoxAction.Accept)
        //            {
        //                var data = _toolExportData.Export(null);
        //                string path = GetExportPath();
        //                Debug.Log("path: " + path);
        //                Debug.Log("data: " + JsonWriter.Serialize(data));
        //                FileSaving.Save(path, JsonWriter.Serialize(data));
        //                Debug.LogError("Export new success");

        //                //var data_2 = _toolExportData.Export(null, true);
        //                //string path_2 = GetExportPathforUnpacking();
        //                //Debug.Log("path_2: " + path_2);
        //                //Debug.Log("data_2: " + JsonWriter.Serialize(data_2));
        //                //FileSaving.Save(path_2, JsonWriter.Serialize(data_2));
        //                //Debug.LogError("Export new success 2");
        //            }

        //            return true;
        //        });
        //    }
        //}

        #endregion
    }
}