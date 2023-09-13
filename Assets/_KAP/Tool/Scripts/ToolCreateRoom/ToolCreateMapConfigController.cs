using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using KAP.Config;
using Kawaii.ResourceManager;
using KAP.Tools;
using System.Linq;
using Kawaii.IsoTools.DecoSystem;
using Imba.UI;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapConfigController : MonoBehaviour
    {
        [SerializeField]
        private string _configDecoFilePath = "/_KAP/_GameResources/Configs/Deco/ConfigDeco.csv";
        [SerializeField]
        private string _configDecoThemeFilePath = "/_KAP/_GameResources/Configs/Deco/ConfigDecoTheme.csv";
        [SerializeField]
        private string _configDecoColorFilePath = "/_KAP/_GameResources/Configs/Deco/ConfigDecoColor.csv";
        [SerializeField]
        private string _configAreaFilePath = "/_KAP/_GameResources/Configs/Deco/ConfigDecoArea.csv";
        [SerializeField]
        private string _configRoomFilePath = "/_KAP/_GameResources/Configs/Room/ConfigRoom.csv";
        [SerializeField]
        private string _configWonderFilePath = "/_KAP/_GameResources/Configs/Wonder/ConfigWonder.csv";
        [SerializeField] 
        private string _configRoomChallengePath = "/_KAP/_GameResources/Configs/Challenge/ConfigRoomChallenge.csv";
        [SerializeField]
        private string _configHiveShop = "/_KAP/_GameResources/Configs/Hive/ConfigHiveShop.csv";

        private string _configBubbleHomeFilePath = "/_KDL/_GameResources/Configs/Bubble/ConfigBubbleHome.csv";

        private string _configBubblePlayFilePath = "/_KDL/_GameResources/Configs/Bubble/ConfigBubblePlay.csv";

        private string _configBubbleHomePositionFilePath = "/_KDL/_GameResources/Configs/Bubble/ConfigBubbleHomePosition.csv";

        private string _configBubblePlayPositionFilePath = "/_KDL/_GameResources/Configs/Bubble/ConfigBubblePlayPosition.csv";

        [SerializeField]
        private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField] 
        private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField]
        private ToolCreateMapTransferKAPToKDL _toolTransfer = null;
        [SerializeField]
        private ToolCreateMapListRooms _toolLstRooms = null;
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField] 
        private InputField _inputMapId = null;
        //[SerializeField]
        //private InputField _inputRoomIdReward = null;

        private readonly ConfigDeco _configDeco = new ConfigDeco();
        private readonly ConfigDecoTheme _configDecoTheme = new ConfigDecoTheme();
        private readonly ConfigDecoColor _configDecoColor = new ConfigDecoColor();
        private readonly ConfigDecoArea _configDecoArea = new ConfigDecoArea();
        private readonly ConfigBubbleHome _configBubbleHome = new ConfigBubbleHome();
        private readonly ConfigBubbleHomePosition _configBubbleHomePosition = new ConfigBubbleHomePosition();
        private readonly ConfigBubblePlay _configBubblePlay = new ConfigBubblePlay();
        private readonly ConfigBubblePlayPosition _configBubblePlayPosition = new ConfigBubblePlayPosition();

        private readonly List<ConfigRoomRecord> _lstConfigRoomRecords = new List<ConfigRoomRecord>();
        public ReadOnlyCollection<ConfigRoomRecord> ListConfigRoomRecords;
        private readonly List<ConfigWonderRecord> _lstConfigWonderRecords = new List<ConfigWonderRecord>();
        public ReadOnlyCollection<ConfigWonderRecord> ListConfigWonderRecords;
        private readonly List<ConfigHiveShopRecord> _lstConfigHiveShopRecords = new List<ConfigHiveShopRecord>();
        public ReadOnlyCollection<ConfigHiveShopRecord> ListConfigHiveRecords;
        private readonly List<ConfigRoomChallengeRecord> _lstConfigChallengeRecords = new List<ConfigRoomChallengeRecord>();
        public ReadOnlyCollection<ConfigRoomChallengeRecord> ListConfigChallengeRecords;

        private readonly List<ConfigDecoRecord> _lstConfigDecoRecords = new List<ConfigDecoRecord>();
        public ReadOnlyCollection<ConfigDecoRecord> ListConfigDecoRecords;

        private readonly List<ConfigBubbleHomeRecord> _lstConfigBubbleHomeRecords = new List<ConfigBubbleHomeRecord>();
        public ReadOnlyCollection<ConfigBubbleHomeRecord> ListConfigBubbleHomeRecords;
        private readonly List<ConfigBubblePlayRecord> _lstConfigBubblePlayRecords = new List<ConfigBubblePlayRecord>();
        public ReadOnlyCollection<ConfigBubblePlayRecord> ListConfigBubblePlayRecords;

        private readonly List<ConfigBubbleHomePositionRecord> _lstConfigBubbleHomePositionRecords = new List<ConfigBubbleHomePositionRecord>();
        public ReadOnlyCollection<ConfigBubbleHomePositionRecord> ListConfigBubbleHomePositionRecords;
        private readonly List<ConfigBubblePlayPositionRecord> _lstConfigBubblePlayPositionRecords = new List<ConfigBubblePlayPositionRecord>();
        public ReadOnlyCollection<ConfigBubblePlayPositionRecord> ListConfigBubblePlayPositionRecords;

        private Dictionary<int, List<Vector3>> _dctRoomIdPosition = new Dictionary<int, List<Vector3>>();    //roomId - position
        private Dictionary<string, string> _dctBubbleIdStar = new Dictionary<string, string>();     //bubbleId - star
        private Dictionary<string, string> _dctBubbleIdWD = new Dictionary<string, string>();    //BubbleId - world direct
        private Dictionary<string, List<int>> _dctBubbleIdPrice = new Dictionary<string, List<int>>();  //bubbleId - price
        public Dictionary<int, List<Vector3>> DctRoomIdPosition { get => _dctRoomIdPosition; set => _dctRoomIdPosition = value; }
        public Dictionary<string, string> DctBubbleIdStar { get => _dctBubbleIdStar; set => _dctBubbleIdStar = value; }
        public Dictionary<string, string> DctBubbleIdWD { get => _dctBubbleIdWD; set => _dctBubbleIdWD = value; }
        public Dictionary<string, List<int>> DctBubbleIdPrice { get => _dctBubbleIdPrice; set => _dctBubbleIdPrice = value; }

        private Dictionary<string, string> _dctRoomIdIndex = new Dictionary<string, string>();       //roomId - Index

        //private Dictionary<string, int> dctRoomIdNumBubble = new Dictionary<string, int>();         //RoomId - num of bubble
        private Dictionary<string, string> _dctRoomIdStrPos = new Dictionary<string, string>();    //roomId - position
        
        private Dictionary<string, string> dctRoomIdUnpackDeco = new Dictionary<string, string>();  //roomId - unpack Deco
        private Dictionary<string, string> dctBubbleIdDecoIds = new Dictionary<string, string>();   //bubbleid - decoids
        private Dictionary<string, string> dctBubbleIdStar = new Dictionary<string, string>();      //bubbleId - star
        private Dictionary<string, int> dctBubbleIdDeco = new Dictionary<string, int>();            //bubbleId - decoId
        private Dictionary<string, string> dctBubbleIdWD = new Dictionary<string, string>();        //BubbleId - world direct
        private Dictionary<string, string> dctBubbleIdPrice = new Dictionary<string, string>();     //bubbleId - price

        private void Awake()
        {
            var txtDeco = FileSaving.Load(Application.dataPath + _configDecoFilePath);
            _configDeco.LoadFromString(txtDeco);
            _lstConfigDecoRecords.AddRange(_configDeco.Records);
            ListConfigDecoRecords = _lstConfigDecoRecords.AsReadOnly();

            var txtDecoTheme = FileSaving.Load(Application.dataPath + _configDecoThemeFilePath);
            _configDecoTheme.LoadFromString(txtDecoTheme);

            var txtColors = FileSaving.Load(Application.dataPath + _configDecoColorFilePath);
            _configDecoColor.LoadFromString(txtColors);

            var txtArea = FileSaving.Load(Application.dataPath + _configAreaFilePath);
            _configDecoArea.LoadFromString(txtArea);

            var txtRoom = FileSaving.Load(Application.dataPath + _configRoomFilePath);
            var _configRoom = new ConfigRoom();
            _configRoom.LoadFromString(txtRoom);
            _lstConfigRoomRecords.AddRange(_configRoom.Records);
            ListConfigRoomRecords = _lstConfigRoomRecords.AsReadOnly();

            var txtWonder = FileSaving.Load(Application.dataPath + _configWonderFilePath);
            var _configWonder = new ConfigWonder();
            _configWonder.LoadFromString(txtWonder);
            _lstConfigWonderRecords.AddRange(_configWonder.Records);
            ListConfigWonderRecords = _lstConfigWonderRecords.AsReadOnly();

            var txtRoomChallenge = FileSaving.Load(Application.dataPath + _configRoomChallengePath);
            var _configRoomChallenge = new ConfigRoomChallenge();
            _configRoomChallenge.LoadFromString(txtRoomChallenge);
            _lstConfigChallengeRecords.AddRange(_configRoomChallenge.Records);
            ListConfigChallengeRecords = _lstConfigChallengeRecords.AsReadOnly();

            var txtBubbleHome = FileSaving.Load(Application.dataPath + _configBubbleHomeFilePath);
            _configBubbleHome.LoadFromString(txtBubbleHome);
            _lstConfigBubbleHomeRecords.AddRange(_configBubbleHome.Records);
            ListConfigBubbleHomeRecords = _lstConfigBubbleHomeRecords.AsReadOnly();

            var txtBubbleHomePosition = FileSaving.Load(Application.dataPath + _configBubbleHomePositionFilePath);
            _configBubbleHomePosition.LoadFromString(txtBubbleHomePosition);
            _lstConfigBubbleHomePositionRecords.AddRange(_configBubbleHomePosition.Records);
            ListConfigBubbleHomePositionRecords = _lstConfigBubbleHomePositionRecords.AsReadOnly();

            var txtBubblePlay = FileSaving.Load(Application.dataPath + _configBubblePlayFilePath);
            _configBubblePlay.LoadFromString(txtBubblePlay);
            _lstConfigBubblePlayRecords.AddRange(_configBubblePlay.Records);
            ListConfigBubblePlayRecords = _lstConfigBubblePlayRecords.AsReadOnly();

            var txtBubblePlayPosition = FileSaving.Load(Application.dataPath + _configBubblePlayPositionFilePath);
            _configBubblePlayPosition.LoadFromString(txtBubblePlayPosition);
            _lstConfigBubblePlayPositionRecords.AddRange(_configBubblePlayPosition.Records);
            ListConfigBubblePlayPositionRecords = _lstConfigBubblePlayPositionRecords.AsReadOnly();
        }


        #region ConfigRoom
        public ConfigRoomRecord GetConfigRoomById(int roomId)
        {
            return _lstConfigRoomRecords.Find((record) => {
                return record.Id == roomId;
            });
        }

        public void SaveConfigRoomCsv()
        {
            //get List variables: Id, Name, TypeId,...
            List<string> lstVariables = ConfigRoomRecord.GetLstVariables();
            string txt = "";
            //save to txt in string type
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";

            //encode data from list configRoom to txt
            foreach (var record in _lstConfigRoomRecords)
            {
                txt += record.GetTextRecord();
            }
            //save txt to file .csv
            FileSaving.Save(Application.dataPath + _configRoomFilePath, txt);
        }

        #endregion

        #region Config Wonder

        public ConfigWonderRecord GetConfigWonderById(int wonderId)
        {
            return _lstConfigWonderRecords.Find((record) => {
                return record.Id == wonderId;
            });
        }

        public void SaveConfigWonderCsv()
        {
            List<string> lstVariables = ConfigWonderRecord.GetLstVariables();
            string txt = "";
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";

            foreach (var record in _lstConfigWonderRecords)
            {
                txt += record.GetTextRecord();
            }
            FileSaving.Save(Application.dataPath + _configWonderFilePath, txt);
        }

        #endregion

        #region ConfigHiveShop
        public ConfigHiveShopRecord GetConfigHiveShopById(int decoId)
        {
            return _lstConfigHiveShopRecords.Find((record) => {
                return record.Id == decoId;
            });
        }

        public void SaveConfigHiveShopCsv()
        {
            List<string> lstVariables = new List<string> { "Id", "Coin", "Gem", "Enable" };
            string txt = "";
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";

            foreach (var record in _lstConfigHiveShopRecords)
            {
                txt += record.Id + "\t" + record.Coin + "\t" + record.Gem + "\t" + record.Enable + "\n";
            }
            FileSaving.Save(Application.dataPath + _configHiveShop, txt);
        }

        #endregion


        #region Config Room Challenge
        public ConfigRoomChallengeRecord GetConfigRoomChallenge(int id)
        {
            return _lstConfigChallengeRecords.Find((record) =>
            {
                return record.Id == id;
            });
        }

        public void SaveConfigRoomChallengeCSV()
        {
            List<string> lstVariables = ConfigRoomChallengeRecord.GetLstVariables();
            string txt = "";
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";

            foreach (var record in _lstConfigChallengeRecords)
            {
                txt += record.GetTextRecord();
            }
            FileSaving.Save(Application.dataPath + _configRoomChallengePath, txt);
        }
        #endregion

        #region Config KDL
        public void InitDataConfig()
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                foreach (var item in ConfigBubbleHomePosition.GetIndexField())
                {
                    if (!DctRoomIdPosition.ContainsKey(int.Parse(item.Key)))
                    {
                        DctRoomIdPosition.Add(int.Parse(item.Key), item.Value[0].GetLstBubblePositionVector3());
                    }
                }
                foreach (var item in ConfigBubbleHome.GetIndexField())
                {
                    if (!DctBubbleIdStar.ContainsKey(item.Key))
                    {
                        DctBubbleIdStar.Add(item.Key, item.Value[0].Star.ToString());
                    }
                    if (!DctBubbleIdPrice.ContainsKey(item.Key))
                    {
                        DctBubbleIdPrice.Add(item.Key, item.Value[0].GetLstPrice());
                    }
                    if (!DctBubbleIdWD.ContainsKey(item.Key))
                    {
                        DctBubbleIdWD.Add(item.Key, item.Value[0].WorldDirect);
                    }
                }
            }
            else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
            {
                foreach (var item in ConfigBubblePlayPosition.GetIndexField())
                {
                    if (!DctRoomIdPosition.ContainsKey(int.Parse(item.Key)))
                    {
                        DctRoomIdPosition.Add(int.Parse(item.Key), item.Value[0].GetLstBubblePositionVector3());
                    }
                }
            }
            
        }
        public void OnClickBuildConfig()
        {
            _toolLstRooms.ExportData();
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                var mess = string.Format("build config dua tren mansion hien tai?");

                UIManager.ShowMessage("", mess, UIMessageBox.MessageBoxType.OK_Cancel, (result) =>
                {
                    if (result == UIMessageBox.MessageBoxAction.Accept)
                    {
                        BuildConfigFromCurrentMansion();
                    }
                    return true;
                });
            }
            else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
            {
                var info = (DecoInfo)_areaManager.ListRooms[0].Info;
                var mess = string.Format("build config cho room play: " + info.Id);

                UIManager.ShowMessage("", mess, UIMessageBox.MessageBoxType.OK_Cancel, (result) =>
                {
                    if (result == UIMessageBox.MessageBoxAction.Accept)
                    {
                        BuildCurrentRoomPlay();
                        SortConfigPlayRecords();
                    }
                    return true;
                });
            }
        }
        
        private void BuildCurrentRoomPlay()
        {
            //clear cache data
            //dctRoomIdNumBubble.Clear();
            dctRoomIdUnpackDeco.Clear();
            _dctRoomIdStrPos.Clear();
            _dctRoomIdIndex.Clear();
            dctBubbleIdDecoIds.Clear();
            dctBubbleIdStar.Clear();
            dctBubbleIdDeco.Clear();
            dctBubbleIdWD.Clear();
            dctBubbleIdPrice.Clear();

            List<string> lstVariables = ConfigBubblePlayRecord.GetLstVariables();
            List<string> lstVariablesPos = ConfigBubblePlayPositionRecord.GetLstVariables();
            string txt = "";
            string txtPos = "";

            foreach (var r in _toolBubbleDecoSetting.DctBubbleDecoItems)
            {
                if (!dctBubbleIdDeco.ContainsKey(r.Key))
                {
                    //var info = (DecoInfo)item.Deco.Info;
                    //dctBubbleIdDeco.Add(r.Key, info.Id);
                }
                else Debug.LogError("error logic play 1");
            }

            //get bubbleDecoIds
            foreach (var pair in dctBubbleIdDeco)
            {
                var listDecoColor = ConfigDecoColor.GetListDecoColorsByDecoId(pair.Value);
                if (!dctBubbleIdDecoIds.ContainsKey(pair.Key))
                {
                    string bubbleDecoIds = "";
                    int result = listDecoColor.Count >= 3 ? 3 : listDecoColor.Count;
                    for (var j = 0; j < result; j++)
                    {
                        bubbleDecoIds += listDecoColor[j].Id + ";";
                    }
                    dctBubbleIdDecoIds.Add(pair.Key, bubbleDecoIds);
                }
            }

            //foreach (var r in _toolBubbleDecoSetting.DctRootDecoItems)
            //{
            //    if (!_dctRoomIdStrPos.ContainsKey(r.Key.RoomId.ToString()))
            //    {
            //        var strPos = "";
            //        Vector3 pos;
            //        foreach (var root in _areaManager.ListRooms)
            //        {
            //            var rootInfo = (DecoInfo)root.Info;
            //            //Debug.LogError("root pos: " + root.Position);
            //            if (rootInfo.Id == r.Key.RoomId)
            //            {
            //                //foreach (var item in r.Value)
            //                //{
            //                //    pos = item.Deco.Position - root.Position;
            //                //    strPos += "[" + pos.x + "," + pos.y + "," + pos.z + "];";
            //                //}
            //                for (var i = 0; i < r.Value.Count; i++)
            //                {
            //                    var item = _toolBubbleDecoSetting.OnGetBubbleDecoItemWithIndex(r.Key.RoomId, i);
            //                    pos = item.Deco.Position - root.Position;
            //                    strPos += "[" + pos.x + "," + pos.y + "," + pos.z + "];";
            //                }
            //                break;
            //            }
            //        }
            //        _dctRoomIdStrPos.Add(r.Key.RoomId.ToString(), strPos);
            //    }
            //}

            //get list unpacking deco
            List<Deco> listBubble = new List<Deco>();
            string unpackingDeco = "";
            foreach (var r in _toolBubbleDecoSetting.DctBubbleDecoItems)
            {
                foreach (var item in r.Value)
                {
                    //if (!listBubble.Contains(item.Deco))
                    //    listBubble.Add(item.Deco);
                }
            }
            var Root = _areaManager.ListRooms[0];
            var infoRoot = (DecoInfo)Root.Info;
            Root.Foreach((deco) =>
            {
                if (!listBubble.Contains(deco))
                {
                    var info = (DecoInfo)deco.Info;
                    if (info.Id != infoRoot.Id && info.Id / 100000 < 22)
                        unpackingDeco += info.Id + "_" + info.Color + ";";
                }
            });
            if (!dctRoomIdUnpackDeco.ContainsKey(infoRoot.Id.ToString()))
                dctRoomIdUnpackDeco.Add(infoRoot.Id.ToString(), unpackingDeco);

            //Build ConfigBubblePlay
            foreach (var pair in dctBubbleIdDecoIds)
            {
                var record = ConfigBubblePlay.GetById(pair.Key);
                if (record != null)
                {
                    foreach (var rec in _lstConfigBubblePlayRecords)
                    {
                        if (rec.BubbleId == pair.Key)
                            rec.BubbleDecoIds = pair.Value;
                    }
                }
                else
                {
                    ConfigBubblePlayRecord newConfig = new ConfigBubblePlayRecord();
                    newConfig.BubbleId = pair.Key;
                    newConfig.BubbleDecoIds = pair.Value;
                    _lstConfigBubblePlayRecords.Add(newConfig);
                }
            }
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n" + ConvertConfigBubblePlayRecordToStringCsv(_lstConfigBubblePlayRecords);

            //Build ConfigBubblePlayPosition
            foreach (var pair in _dctRoomIdStrPos)
            {
                var record = ConfigBubblePlayPosition.GetByRoomId(pair.Key);
                if (record != null)
                {
                    foreach (var rec in _lstConfigBubblePlayPositionRecords)
                    {
                        if (rec.RoomId == pair.Key)
                        {
                            rec.LstBubblePosition = pair.Value;
                            rec.LstUnpackingDeco = dctRoomIdUnpackDeco[pair.Key];
                            rec.BaseGem = _toolBubbleDecoSetting.BaseGem.text;
                        }
                    }
                }
                else
                {
                    ConfigBubblePlayPositionRecord newConfig = new ConfigBubblePlayPositionRecord();
                    newConfig.RoomId = pair.Key;
                    newConfig.LstBubblePosition = pair.Value;
                    newConfig.LstUnpackingDeco = dctRoomIdUnpackDeco[pair.Key];
                    newConfig.LstDecoReward = dctRoomIdUnpackDeco[pair.Key];
                    newConfig.BaseGem = _toolBubbleDecoSetting.BaseGem.text;
                    _lstConfigBubblePlayPositionRecords.Add(newConfig);
                }
            }
            for (var i = 0; i < lstVariablesPos.Count - 1; i++)
            {
                txtPos += lstVariablesPos[i] + "\t";
            }
            txtPos += lstVariablesPos[lstVariablesPos.Count - 1] + "\n" + ConvertConfigBubblePlayPositionRecordToStringCsv(_lstConfigBubblePlayPositionRecords);

            FileSaving.Save(Application.dataPath + _configBubblePlayFilePath, txt);
            FileSaving.Save(Application.dataPath + _configBubblePlayPositionFilePath, txtPos);
            Debug.LogError("Export Bubble Play success");
        }
        private void SortConfigPlayRecords()
        {
            LoadFileCsv();
            List<string> lstVariables = ConfigBubblePlayRecord.GetLstVariables();
            string txt = "";
            List<ConfigBubblePlayRecord> _newlstConfigPlay = new List<ConfigBubblePlayRecord>();
            var txtBubblePlay = FileSaving.Load(Application.dataPath + _configBubblePlayFilePath);
            var _configBubblePlay = new ConfigBubblePlay();
            _configBubblePlay.LoadFromString(txtBubblePlay);
            for (var i = 0; i < _lstConfigBubblePlayPositionRecords.Count; i++)
            {
                var roomId = _lstConfigBubblePlayPositionRecords[i].RoomId;
                var count = _lstConfigBubblePlayPositionRecords[i].GetLstBubblePositionVector3().Count;
                for (var j = 0; j < count; j++)
                {
                    _newlstConfigPlay.Add(_configBubblePlay.GetById(roomId + "_" + j));
                }
            }

            _lstConfigBubblePlayRecords.Clear();
            _lstConfigBubblePlayRecords.AddRange(_newlstConfigPlay);
            ListConfigBubblePlayRecords = _lstConfigBubblePlayRecords.AsReadOnly();

            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n" + ConvertConfigBubblePlayRecordToStringCsv(_newlstConfigPlay);
            FileSaving.Save(Application.dataPath + _configBubblePlayFilePath, txt);
        }
        private void BuildConfigFromCurrentMansion()
        {
            //clear cache data
            //dctRoomIdNumBubble.Clear();
            _dctRoomIdStrPos.Clear();
            _dctRoomIdIndex.Clear();
            dctBubbleIdDecoIds.Clear();
            dctBubbleIdStar.Clear();
            dctBubbleIdDeco.Clear();
            dctBubbleIdWD.Clear();
            dctBubbleIdPrice.Clear();

            List<string> lstVariables = ConfigBubbleHomeRecord.GetLstVariables();
            List<string> lstVariablesPos = ConfigBubbleHomePositionRecord.GetLstVariables();
            string txt = "";
            string txtPos = "";
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";

            for (var i = 0; i < lstVariablesPos.Count - 1; i++)
            {
                txtPos += lstVariablesPos[i] + "\t";
            }
            txtPos += lstVariablesPos[lstVariablesPos.Count - 1] + "\n";

            //get position bubble in room
            foreach (var pair in _toolBubbleSetting.DctDecoInRoom)
            {
                var bubbleId = pair.Key;
                var roomId = SGUtils.ParseStringToListInt(bubbleId, '_')[0];
                var bubbleIndex = SGUtils.ParseStringToListInt(bubbleId, '_')[1];
                var deco = pair.Value;
                var strPos = "";
                Vector3 pos = Vector3.one;
                foreach (var root in _areaManager.ListRooms)
                {
                    var rootInfo = (DecoInfo)root.Info;
                    if (rootInfo.Id == roomId)
                    {
                        pos = deco.Position - root.Position;
                        strPos += "[" + pos.x + "," + pos.y + "," + pos.z + "];";
                        if (!_dctRoomIdStrPos.ContainsKey(roomId.ToString()))
                        {
                            _dctRoomIdStrPos.Add(roomId.ToString(), strPos);
                        }
                        else _dctRoomIdStrPos[roomId.ToString()] += strPos;
                        break;
                    }
                }
                DctRoomIdPosition[roomId][bubbleIndex] = pos;
            }
            foreach (var pair in _dctRoomIdStrPos)
            {
                Debug.LogError("roomId strPos: " + pair.Key + " " + pair.Value);
            }

            //get Indx Room
            foreach (var room in _toolLstRooms.GetLstRoomItem())
            {
                //Debug.LogError("check room: " + room.GetRoomId());
                if (!_dctRoomIdIndex.ContainsKey(room.GetRoomId().ToString()))
                {
                    _dctRoomIdIndex.Add(room.GetRoomId().ToString(), room.GetRoomOrder());
                }
            }

            //Build ConfigBubbleHome
            var idx = 0;
            List<string> lstRoomId = new List<string>();
            for (var i = 1; i <= _dctRoomIdIndex.Count; i++)
            {
                foreach (var pair in _dctRoomIdIndex)
                {
                    if (pair.Value == i.ToString())
                    {
                        lstRoomId.Add(pair.Key);
                        break;
                    }
                }
            }
            for (var i = 0; i < lstRoomId.Count; i++)
            {
                var roomId = lstRoomId[i];
                int bubbleCount = DctRoomIdPosition[int.Parse(roomId)].Count;

                for (var j = 0; j < bubbleCount; j++)
                {
                    var bubbleId = roomId + "_" + j;
                    SortListBubbleDecoAndPrice(bubbleId);
                    string bubbledecoids = "";
                    string prices = "";
                    foreach (var decoid in _toolBubbleDecoSetting.DctBubbleDecoItems[bubbleId])
                    {
                        bubbledecoids += decoid + ";";
                    }
                    foreach (var price in DctBubbleIdPrice[bubbleId])
                    {
                        prices += price + ";";
                    }
                    txt += bubbleId + "\t" + bubbledecoids + "\t" + idx + "\t" + prices + "\t" +
                        DctBubbleIdWD[bubbleId] + "\t" + DctBubbleIdStar[bubbleId] + "\n";
                    idx++;
                }
            }

            //Build ConfigBubbleHomePosition
            int exp = 0;
            for (var i = 1; i <= _dctRoomIdIndex.Count; i++)
            {
                foreach (var pair in _dctRoomIdIndex)
                {
                    if (pair.Value == i.ToString())
                    {
                        txtPos += pair.Key + "\t" + _dctRoomIdStrPos[pair.Key] + "\t" + pair.Value + "\t" + exp + "\n";
                        exp += DctRoomIdPosition[int.Parse(pair.Key)].Count;
                        break;
                    }
                }
            }

            FileSaving.Save(Application.dataPath + _configBubbleHomeFilePath, txt);
            FileSaving.Save(Application.dataPath + _configBubbleHomePositionFilePath, txtPos);
            Debug.LogError("Export Bubble Home success");
        }
        private void SortListBubbleDecoAndPrice(string bubbleId)
        {
            List<int> numbers = new List<int> { 5, 2, 9, 3, 6, 1, 8, 4, 7 };
            List<string> strings = new List<string> { "Five", "Two", "Nine", "Three", "Six", "One", "Eight", "Four", "Seven" };

            List<Tuple<int, string>> pairs = DctBubbleIdPrice[bubbleId].Zip(_toolBubbleDecoSetting.DctBubbleDecoItems[bubbleId], Tuple.Create).ToList();

            pairs.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            DctBubbleIdPrice[bubbleId] = pairs.Select(pair => pair.Item1).ToList();
            _toolBubbleDecoSetting.DctBubbleDecoItems[bubbleId] = pairs.Select(pair => pair.Item2).ToList();

            //Debug.LogError("price: " + string.Join(", ", DctBubbleIdPrice[bubbleId]));
            //Debug.LogError("bubble deco: " + string.Join(", ", _toolBubbleDecoSetting.DctBubbleDecoItems[bubbleId]));
        }
        public void LoadFileCsv()
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                var txtBubbleHome = FileSaving.Load(Application.dataPath + _configBubbleHomeFilePath);
                _configBubbleHome.LoadFromString(txtBubbleHome);
                _lstConfigBubbleHomeRecords.Clear();
                _lstConfigBubbleHomeRecords.AddRange(_configBubbleHome.Records);
                //Debug.LogError("num of listconfigbubhomerec: " + _lstConfigBubbleHomeRecords.Count);
                ListConfigBubbleHomeRecords = _lstConfigBubbleHomeRecords.AsReadOnly();

                var txtBubbleHomePosition = FileSaving.Load(Application.dataPath + _configBubbleHomePositionFilePath);
                _configBubbleHomePosition.LoadFromString(txtBubbleHomePosition);
                _lstConfigBubbleHomePositionRecords.Clear();
                _lstConfigBubbleHomePositionRecords.AddRange(_configBubbleHomePosition.Records);
                ListConfigBubbleHomePositionRecords = _lstConfigBubbleHomePositionRecords.AsReadOnly();
            }
            else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
            {
                var txtBubblePlay = FileSaving.Load(Application.dataPath + _configBubblePlayFilePath);
                var _configBubblePlay = new ConfigBubblePlay();
                _configBubblePlay.LoadFromString(txtBubblePlay);
                _lstConfigBubblePlayRecords.Clear();
                _lstConfigBubblePlayRecords.AddRange(_configBubblePlay.Records);
                ListConfigBubblePlayRecords = _lstConfigBubblePlayRecords.AsReadOnly();

                var txtBubblePlayPosition = FileSaving.Load(Application.dataPath + _configBubblePlayPositionFilePath);
                _lstConfigBubblePlayPositionRecords.Clear();
                var _configBubblePlayPosition = new ConfigBubblePlayPosition();
                _configBubblePlayPosition.LoadFromString(txtBubblePlayPosition);
                _lstConfigBubblePlayPositionRecords.AddRange(_configBubblePlayPosition.Records);
                ListConfigBubblePlayPositionRecords = _lstConfigBubblePlayPositionRecords.AsReadOnly();
            }
        }
        #endregion

        #region Utils

        public int FindIndexInListRoom(int roomIndex)
        {
            for (var i = 0; i < _areaManager.ListRooms.Count; i++)
            {
                var info = _areaManager.ListRooms[i].ParseInfo<DecoInfo>();
                if (info.Id == roomIndex)
                    return i;
            }
            return -1;
        }
        public string ConvertConfigBubblePlayRecordToStringCsv(List<ConfigBubblePlayRecord> configRecords)
        {
            string txt = "";
            for (var i = 0; i < configRecords.Count; i++)
            {
                //Debug.LogError("check: " + configRecords[i].BubbleId);
                if (configRecords[i].BubbleId == null)
                {
                    Debug.LogError("bubbleId null");
                    return txt;
                }
                if (configRecords[i].BubbleDecoIds == null)
                {
                    Debug.LogError("bubbledecosId null");
                    return txt;
                }
                    
                txt += configRecords[i].BubbleId + "\t" + configRecords[i].BubbleDecoIds + "\n";
            }
            return txt;
        }

        public string ConvertConfigBubblePlayPositionRecordToStringCsv(List<ConfigBubblePlayPositionRecord> configRecords)
        {
            string txt = "";
            for (var i = 0; i < configRecords.Count; i++)
            {
                txt += (i + 1).ToString() + "\t" + configRecords[i].RoomId + "\t" + configRecords[i].LstBubblePosition + "\t" + 
                    configRecords[i].LstUnpackingDeco + "\t" + configRecords[i].LstDecoReward + "\t" + configRecords[i].BaseGem + "\n";
            }
            return txt;
        }

        public string GetStringBubblePosition(Vector3 position, Vector3 roomPos)
        {
            return "[" + (position.x - roomPos.x) + "," + (position.y - roomPos.y) + "," + position.z + "];";
        }

        public string ConvertListToString(List<string> list)
        {
            string txt = "";
            for (var i = 0; i < list.Count; i++)
                txt += list[i] + ";";
            return txt;
        }

        public void SaveConfigHomeKAPToKDL()
        {
            LoadFileCsv();
            List<string> lstVariables = ConfigBubbleHomeRecord.GetLstVariables();
            List<string> lstVariablesPos = ConfigBubbleHomePositionRecord.GetLstVariables();
            string txt = "";
            string txtPos = "";
            Dictionary<int, string> dctRoomOrder = new Dictionary<int, string>();
            foreach (var room in _toolLstRooms.GetLstRoomItem())
            {
                if (dctRoomOrder.ContainsKey(room.GetRoomId()))
                {
                    Debug.LogError("loi trung room");
                }
                else
                {
                    dctRoomOrder.Add(room.GetRoomId(), room.GetRoomOrder());
                }
            }
            //save to txt in string type
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";

            for (var i = 0; i < lstVariablesPos.Count - 1; i++)
            {
                txtPos += lstVariablesPos[i] + "\t";
            }
            txtPos += lstVariablesPos[lstVariablesPos.Count - 1] + "\n";

            for (var i = 0; i < _toolTransfer.DctBubble.Count; i++)
            {
                txt += _toolTransfer.DctBubble.ElementAt(i).Key + "\t" + _toolTransfer.DctBubble.ElementAt(i).Value[0] + "\t" + i + "\t" 
                    + _toolTransfer.DctBubble.ElementAt(i).Value[1] + "\t" + _toolTransfer.DctBubble.ElementAt(i).Value[2] + "\t" + KAPDefine.DefaultStar.ToString() + "\n";
            }
            foreach (var pair in _toolTransfer.DctBubblePos)
            {
                foreach (var room in dctRoomOrder)
                {
                    if (room.Key == pair.Key)
                    {
                        string pos = "";
                        foreach (var vec in pair.Value)
                        {
                            pos += GetStringBubblePosition(vec, Vector3.zero);
                        }
                        txtPos += pair.Key + "\t" + pos + "\t" + room.Value + "\n";
                        break;
                    }
                }
            }

            //save txt to file .csv
            FileSaving.Save(Application.dataPath + _configBubbleHomeFilePath, txt);
            FileSaving.Save(Application.dataPath + _configBubbleHomePositionFilePath, txtPos);
            Debug.LogError("Export Bubble Home success");
        }

        public void SaveConfigPlayKAPToKDL()
        {
            LoadFileCsv();
            List<string> lstVariables = ConfigBubblePlayRecord.GetLstVariables();
            List<string> lstVariablesPos = ConfigBubblePlayPositionRecord.GetLstVariables();
            string txt = "";
            string txtPos = "";
            //Save configPlay
            if (_lstConfigBubblePlayRecords.Count != 0)
            {
                List<ConfigBubblePlayRecord> listRemovedRecord = new List<ConfigBubblePlayRecord>();
                for (var i = 0; i < _lstConfigBubblePlayRecords.Count; i++)
                {
                    var record = _lstConfigBubblePlayRecords[i];
                    var listRecord = SGUtils.ParseStringToList(record.BubbleId, '_');
                    if (listRecord[0] == _inputMapId.text)
                    {
                        listRemovedRecord.Add(record);
                        //Debug.LogError("remove");
                    }
                        
                }
                foreach (var rec in listRemovedRecord)
                {
                    _lstConfigBubblePlayRecords.Remove(rec);
                }

                foreach (var pair in _toolTransfer.DctBubbleDecoIds)
                {
                    ConfigBubblePlayRecord newRecord = new ConfigBubblePlayRecord();
                    newRecord.BubbleId = pair.Key;
                    newRecord.BubbleDecoIds = pair.Value;
                    _lstConfigBubblePlayRecords.Add(newRecord);
                }
                string newtxt = "";
                for (var i = 0; i < lstVariables.Count - 1; i++)
                {
                    newtxt += lstVariables[i] + "\t";
                }
                newtxt += lstVariables[lstVariables.Count - 1] + "\n" + ConvertConfigBubblePlayRecordToStringCsv(_lstConfigBubblePlayRecords);
                FileSaving.Save(Application.dataPath + _configBubblePlayFilePath, newtxt);
            }
            else
            {
                for (var i = 0; i < lstVariables.Count - 1; i++)
                {
                    txt += lstVariables[i] + "\t";
                }
                txt += lstVariables[lstVariables.Count - 1] + "\n";

                foreach (var pair in _toolTransfer.DctBubbleDecoIds)
                {
                    txt += pair.Key + "\t" + pair.Value + "\n";
                }
                FileSaving.Save(Application.dataPath + _configBubblePlayFilePath, txt);
            }
            

            //Save ConfigPlayPosition
            if (_lstConfigBubblePlayPositionRecords.Count != 0)
            {
                List<ConfigBubblePlayPositionRecord> listRemovedRecord = new List<ConfigBubblePlayPositionRecord>();
                for (var i = 0; i < _lstConfigBubblePlayPositionRecords.Count; i++)
                {
                    var record = _lstConfigBubblePlayPositionRecords[i];
                    if (record.RoomId == _inputMapId.text)
                        listRemovedRecord.Add(record);
                }
                foreach (var rec in listRemovedRecord)
                {
                    _lstConfigBubblePlayPositionRecords.Remove(rec);
                }

                ConfigBubblePlayPositionRecord newRecord = new ConfigBubblePlayPositionRecord();
                var strPos = "";
                foreach (var pos in _toolTransfer.LstPosBubble)
                {
                    strPos += GetStringBubblePosition(pos, Vector3.zero);
                }

                newRecord.RoomId = _inputMapId.text;
                newRecord.LstBubblePosition = strPos;
                newRecord.LstUnpackingDeco = ConvertListToString(_toolTransfer.LstUnpackingDeco);
                _lstConfigBubblePlayPositionRecords.Add(newRecord);

                string newtxt = "";
                for (var i = 0; i < lstVariablesPos.Count - 1; i++)
                {
                    newtxt += lstVariablesPos[i] + "\t";
                }
                newtxt += lstVariablesPos[lstVariablesPos.Count - 1] + "\n" + ConvertConfigBubblePlayPositionRecordToStringCsv(_lstConfigBubblePlayPositionRecords);
                FileSaving.Save(Application.dataPath + _configBubblePlayPositionFilePath, newtxt);
            }
            else
            {
                for (var i = 0; i < lstVariablesPos.Count - 1; i++)
                {
                    txtPos += lstVariablesPos[i] + "\t";
                }
                txtPos += lstVariablesPos[lstVariablesPos.Count - 1] + "\n";

                var strPos = "";
                foreach (var pos in _toolTransfer.LstPosBubble)
                {
                    strPos += GetStringBubblePosition(pos, Vector3.zero);
                }
                txtPos += _inputMapId.text + "\t" + strPos + "\t" + ConvertListToString(_toolTransfer.LstUnpackingDeco);
                FileSaving.Save(Application.dataPath + _configBubblePlayPositionFilePath, txtPos);
            }
            Debug.LogError("Export Bubble Play success");
        }

        public void ClearConfigHome()
        {
            List<string> lstVariables = ConfigBubbleHomeRecord.GetLstVariables();
            List<string> lstVariablesPos = ConfigBubbleHomePositionRecord.GetLstVariables();
            string txt = "";
            string txtPos = "";
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";
            for (var i = 0; i < lstVariablesPos.Count - 1; i++)
            {
                txtPos += lstVariablesPos[i] + "\t";
            }
            txtPos += lstVariablesPos[lstVariablesPos.Count - 1] + "\n";
            FileSaving.Save(Application.dataPath + _configBubbleHomeFilePath, txt);
            FileSaving.Save(Application.dataPath + _configBubbleHomePositionFilePath, txtPos);
            Debug.LogError("Clear Data Home Success");
        }
        public void ClearConfigPlay()
        {
            List<string> lstVariables = ConfigBubblePlayRecord.GetLstVariables();
            List<string> lstVariablesPos = ConfigBubblePlayPositionRecord.GetLstVariables();
            string txt = "";
            string txtPos = "";
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";
            for (var i = 0; i < lstVariablesPos.Count - 1; i++)
            {
                txtPos += lstVariablesPos[i] + "\t";
            }
            txtPos += lstVariablesPos[lstVariablesPos.Count - 1] + "\n";
            FileSaving.Save(Application.dataPath + _configBubblePlayFilePath, txt);
            FileSaving.Save(Application.dataPath + _configBubblePlayPositionFilePath, txtPos);
            Debug.LogError("Clear Data Play Success");
        }
        #endregion
        public void OnClickDecoReward()
        {
            List<string> lstVariablesPos = ConfigBubblePlayPositionRecord.GetLstVariables();
            string txtPos = "";
            for (var i = 0; i < lstVariablesPos.Count; i++)
            {
                txtPos += lstVariablesPos[i] + "\t";
            }
            txtPos += "LstDecoReward" + "\n";
            for (var i = 0; i < _lstConfigBubblePlayPositionRecords.Count; i++)
            {
                var rec = _lstConfigBubblePlayPositionRecords[i];
                txtPos += rec.RoomId + "\t";
                txtPos += rec.LstBubblePosition + "\t";
                txtPos += rec.LstUnpackingDeco + "\t";
                txtPos += rec.LstUnpackingDeco + "\n";
            }

            FileSaving.Save(Application.dataPath + _configBubblePlayPositionFilePath, txtPos);
            Debug.LogError("Export Bubble Play success");
        }
        public void OnClickBaseGem()
        {
            List<string> lstVariablesPos = ConfigBubblePlayPositionRecord.GetLstVariables();
            string txtPos = "";
            for (var i = 0; i < lstVariablesPos.Count - 1; i++)
            {
                txtPos += lstVariablesPos[i] + "\t";
            }
            txtPos += lstVariablesPos[lstVariablesPos.Count - 1] + "\n";
            for (var i = 0; i < _lstConfigBubblePlayPositionRecords.Count; i++)
            {
                var rec = _lstConfigBubblePlayPositionRecords[i];
                txtPos += rec.RoomId + "\t";
                txtPos += rec.LstBubblePosition + "\t";
                txtPos += rec.LstUnpackingDeco + "\t";
                txtPos += rec.LstDecoReward + "\t";
                txtPos += "100" + "\n";
            }

            FileSaving.Save(Application.dataPath + _configBubblePlayPositionFilePath, txtPos);
            Debug.LogError("Export Bubble Play success");
        }
        //public void AddIdColumn()
        //{
        //    LoadFileCsv();
        //    List<string> lstVariablesPos = ConfigBubblePlayPositionRecord.GetLstVariables();
        //    string txtPos = "Id" + "\t";
        //    int j = 1;
        //    for (var i = 0; i < lstVariablesPos.Count - 1; i++)
        //    {
        //        txtPos += lstVariablesPos[i] + "\t";
        //    }
        //    txtPos += lstVariablesPos[lstVariablesPos.Count - 1] + "\n";
        //    foreach (var config in _lstConfigBubblePlayPositionRecords)
        //    {
        //        //ConfigBubblePlayPositionRecord newRec = new ConfigBubblePlayPositionRecord();
        //        txtPos += j.ToString() + "\t" + config.RoomId + "\t" + config.LstBubblePosition + "\t" +
        //            config.LstUnpackingDeco + "\t" + config.LstDecoReward + "\t" + config.BaseGem + "\n";
        //        j++;
        //    }
        //    FileSaving.Save(Application.dataPath + _configBubblePlayPositionFilePath, txtPos);
        //    Debug.LogError("Export Bubble Play success");
        //}
        public ConfigDeco ConfigDeco
        {
            get
            {
                return _configDeco;
            }
        }

        public ConfigDecoTheme ConfigDecoTheme
        {
            get
            {
                return _configDecoTheme;
            }
        }

        public ConfigDecoColor ConfigDecoColor
        {
            get
            {
                return _configDecoColor;
            }
        }

        public ConfigDecoArea ConfigDecoArea
        {
            get
            {
                return _configDecoArea;
            }
        }
        public ConfigBubbleHome ConfigBubbleHome
        {
            get => _configBubbleHome;
        }
        public ConfigBubbleHomePosition ConfigBubbleHomePosition
        {
            get => _configBubbleHomePosition;
        }
        public ConfigBubblePlay ConfigBubblePlay
        {
            get => _configBubblePlay;
        }
        public ConfigBubblePlayPosition ConfigBubblePlayPosition
        {
            get => _configBubblePlayPosition;
        }
    }

}
