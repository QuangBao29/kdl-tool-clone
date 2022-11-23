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

        [Space]
        [SerializeField] private Transform _transGridRoom = null;
        [SerializeField] private ToolCreateMapListRoomItem _prefabRoomItem = null;

        [Space]
        [SerializeField] private InputField _inputMapId = null;
        [SerializeField] private Text _txtCurEditMode = null;

        [Space]
        [SerializeField] private ToolCreateMapExportBubbleData _toolExportData = null;

        private List<ToolCreateMapListRoomItem> _lstRoomItems = null;
        

        private Dictionary<EditMode, EditModeData> _dctEditModeData = new Dictionary<EditMode, EditModeData>();
        private Dictionary<EditMode, EditModeData> _dctEditModeDataPostion = new Dictionary<EditMode, EditModeData>();
        private Dictionary<EditMode, EditModeData> _dctEditModeDataBubble = new Dictionary<EditMode, EditModeData>();

        private string _exportJsonDecoUnpackingPath = "/_KAP/_GameResources/Maps/RoomUnpacking/";
        private string _importJsonDecoUnpackingPath = "Assets/_KAP/_GameResources/Maps/RoomUnpacking/";

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

            //_dctEditModeData.Add(EditMode.Room, new EditModeData(EditMode.Room, _importRoomPath, _exportRoomPath, _screenshotRoomPath, Color.black, KAPDefine.DefaultRoomId));
            //_dctEditModeData.Add(EditMode.Theme, new EditModeData(EditMode.Theme, _importThemePath, _exportThemePath, _screenshotThemePath, Color.red, KAPDefine.DefaultRoomThemeId));
            //_dctEditModeData.Add(EditMode.Wonder, new EditModeData(EditMode.Wonder, _importWonderPath, _exportWonderPath, _screenshotWonderPath, Color.blue, KAPDefine.DefaultWonderId));
            //_dctEditModeData.Add(EditMode.RoomChallenge, new EditModeData(EditMode.RoomChallenge, _importRoomChallengePath, _exportRoomChallengePath, _screenshotRoomChallengePath ,Color.yellow, KAPDefine.DefaultRoomChallengeID));
            _dctEditModeData.Add(EditMode.Home, new EditModeData(EditMode.Home, _importThemePath, _exportThemePath, _screenshotRoomHomePath, Color.black, KAPDefine.DefaultRoomThemeId));
            _dctEditModeData.Add(EditMode.Play, new EditModeData(EditMode.Play, _importRoomPath, _exportRoomPath, _screenshotRoomPlayPath, Color.red, KAPDefine.DefaultRoomId));
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

                    Debug.LogError("roomId: " + roomId);
                    room.name = roomId.ToString();
                    ToolCreateMapListRoomItem item = null;
                    if (roomItemIndex < roomItemCount)
                        item = _transGridRoom.GetChild(roomItemIndex++).GetComponent<ToolCreateMapListRoomItem>();
                    else
                        item = SGUtils.InstantiateObject<ToolCreateMapListRoomItem>(_prefabRoomItem, _transGridRoom);


                    item.gameObject.SetActive(true);
                    item.Setup(room, i);

                    //Add Item to List
                    _lstRoomItems.Add(item);
                    
                    ++i;
                }
            }




            for (; roomItemIndex < roomItemCount; ++roomItemIndex)
                _transGridRoom.GetChild(roomItemIndex).gameObject.SetActive(false);
        }

        public void OnButtonAddClick()
        {
            _importDecoController.CreateARoom(_areaManager.ListRooms.Count, Vector3.zero, Vector3.one);
            Setup();
            _toolBubbleSetting.UpdateListNumOfBubbleInARoom();
            _toolBubbleSetting.DebugForCheck();
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
        #endregion
        // ================================================================
        #region Import

        public string GetImportPath()
        {
            var targetEditMode = _dctEditModeData[ToolEditMode.Instance.CurrentEditMode];

            string mapName = _inputMapId.text + ".json";
            return Path.Combine(targetEditMode.ImportPath, mapName);
        }
        public string GetImportPathUnpacking()
        {
            string mapName = _inputMapId.text + ".json";
            return Path.Combine(_importJsonDecoUnpackingPath, mapName);
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
            string path = "";
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                path = GetImportPath();
            else
                path = GetImportPathUnpacking();
            var json = FileSaving.Load(path);
            if (!string.IsNullOrEmpty(json))
            {
                var lstRooms = JsonReader.Deserialize<Dictionary<string, DecoDataArray[]>>(json);
                _importDecoController.Import(lstRooms);
            }
            Setup();
            _toolBubbleSetting.ClearBubbles();
            _toolBubbleSetting.UpdateListNumOfBubbleInARoom();
            _configController.OnButtonImportBubbleCsv();
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
        public string GetExportPathforUnpacking()
        {
            string mapName = string.Format("{0}.json", _inputMapId.text);
            return Application.dataPath + Path.Combine(_exportJsonDecoUnpackingPath, mapName);
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

        public void OnButtonExportDecoKDLClick()
        {
            if (string.IsNullOrEmpty(_inputMapId.text))
                return;

            string mess = "";
            var targetEditMode = _dctEditModeData[ToolEditMode.Instance.CurrentEditMode];
            mess = string.Format("export {0}: {1}", targetEditMode.Mode, _inputMapId.text);

            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                UIManager.ShowMessage("", mess, UIMessageBox.MessageBoxType.OK_Cancel, (result) =>
                {
                    if (result == UIMessageBox.MessageBoxAction.Accept)
                    {
                        var data = _toolExportData.Export(null);
                        string path = GetExportPath();
                        Debug.Log("path: " + path);
                        Debug.Log("data: " + JsonWriter.Serialize(data));
                        FileSaving.Save(path, JsonWriter.Serialize(data));
                        Debug.LogError("Export new success");
                    }

                    return true;
                });
            }
            else
            {
                UIManager.ShowMessage("", mess, UIMessageBox.MessageBoxType.OK_Cancel, (result) =>
                {
                    if (result == UIMessageBox.MessageBoxAction.Accept)
                    {
                        var data = _toolExportData.Export(null);
                        string path = GetExportPath();
                        Debug.Log("path: " + path);
                        Debug.Log("data: " + JsonWriter.Serialize(data));
                        FileSaving.Save(path, JsonWriter.Serialize(data));
                        Debug.LogError("Export new success");

                        //var data_2 = _toolExportData.Export(null, true);
                        //string path_2 = GetExportPathforUnpacking();
                        //Debug.Log("path_2: " + path_2);
                        //Debug.Log("data_2: " + JsonWriter.Serialize(data_2));
                        //FileSaving.Save(path_2, JsonWriter.Serialize(data_2));
                        //Debug.LogError("Export new success 2");
                    }

                    return true;
                });
            }
            

        }

        #endregion
    }
}