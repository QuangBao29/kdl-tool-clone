﻿using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        private ToolCreateMapUnpackingSetting _toolUnpackingSetting = null;
        [SerializeField]
        private ToolCreateMapTransferKAPToKDL _toolTransfer = null;
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField] 
        private InputField _inputMapId = null;

        private readonly ConfigDeco _configDeco = new ConfigDeco();
        private readonly ConfigDecoTheme _configDecoTheme = new ConfigDecoTheme();
        private readonly ConfigDecoColor _configDecoColor = new ConfigDecoColor();
        private readonly ConfigDecoArea _configDecoArea = new ConfigDecoArea();

        private readonly List<ConfigRoomRecord> _lstConfigRoomRecords = new List<ConfigRoomRecord>();
        public ReadOnlyCollection<ConfigRoomRecord> ListConfigRoomRecords;
        private readonly List<ConfigWonderRecord> _lstConfigWonderRecords = new List<ConfigWonderRecord>();
        public ReadOnlyCollection<ConfigWonderRecord> ListConfigWonderRecords;
        private readonly List<ConfigHiveShopRecord> _lstConfigHiveShopRecords = new List<ConfigHiveShopRecord>();
        public ReadOnlyCollection<ConfigHiveShopRecord> ListConfigHiveRecords;
        private readonly List<ConfigRoomChallengeRecord> _lstConfigChallengeRecords = new List<ConfigRoomChallengeRecord>();
        public ReadOnlyCollection<ConfigRoomChallengeRecord> ListConfigChallengeRecords;

        private readonly List<ConfigBubbleHomeRecord> _lstConfigBubbleHomeRecords = new List<ConfigBubbleHomeRecord>();
        public ReadOnlyCollection<ConfigBubbleHomeRecord> ListConfigBubbleHomeRecords;
        private readonly List<ConfigBubblePlayRecord> _lstConfigBubblePlayRecords = new List<ConfigBubblePlayRecord>();
        public ReadOnlyCollection<ConfigBubblePlayRecord> ListConfigBubblePlayRecords;

        private readonly List<ConfigBubbleHomePositionRecord> _lstConfigBubbleHomePositionRecords = new List<ConfigBubbleHomePositionRecord>();
        public ReadOnlyCollection<ConfigBubbleHomePositionRecord> ListConfigBubbleHomePositionRecords;
        private readonly List<ConfigBubblePlayPositionRecord> _lstConfigBubblePlayPositionRecords = new List<ConfigBubblePlayPositionRecord>();
        public ReadOnlyCollection<ConfigBubblePlayPositionRecord> ListConfigBubblePlayPositionRecords;

        private void Awake()
        {
            //declare txtDeco to contain data loaded from path in local computer in string type
            var txtDeco = FileSaving.Load(Application.dataPath + _configDecoFilePath);
            //using _configDeco to load ConfigDeco from txtDeco
            _configDeco.LoadFromString(txtDeco);

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
            var _configBubbleHome = new ConfigBubbleHome();
            _configBubbleHome.LoadFromString(txtBubbleHome);
            _lstConfigBubbleHomeRecords.AddRange(_configBubbleHome.Records);
            ListConfigBubbleHomeRecords = _lstConfigBubbleHomeRecords.AsReadOnly();

            var txtBubbleHomePosition = FileSaving.Load(Application.dataPath + _configBubbleHomePositionFilePath);
            var _configBubbleHomePosition = new ConfigBubbleHomePosition();
            _configBubbleHomePosition.LoadFromString(txtBubbleHomePosition);
            _lstConfigBubbleHomePositionRecords.AddRange(_configBubbleHomePosition.Records);
            ListConfigBubbleHomePositionRecords = _lstConfigBubbleHomePositionRecords.AsReadOnly();

            var txtBubblePlay = FileSaving.Load(Application.dataPath + _configBubblePlayFilePath);
            var _configBubblePlay = new ConfigBubblePlay();
            _configBubblePlay.LoadFromString(txtBubblePlay);
            _lstConfigBubblePlayRecords.AddRange(_configBubblePlay.Records);
            ListConfigBubblePlayRecords = _lstConfigBubblePlayRecords.AsReadOnly();

            var txtBubblePlayPosition = FileSaving.Load(Application.dataPath + _configBubblePlayPositionFilePath);
            var _configBubblePlayPosition = new ConfigBubblePlayPosition();
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
        public void SaveBubbleToCsv()
        {
            string mess = "";
            mess = string.Format("--------EXPORT-------- \n Mode: {0}   roomId: {1}", ToolEditMode.Instance.CurrentEditMode.ToString(), _inputMapId.text);

            UIManager.ShowMessage("", mess, UIMessageBox.MessageBoxType.OK_Cancel, (result) =>
            {
                if (result == UIMessageBox.MessageBoxAction.Accept)
                {
                    if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                        SaveConfigBubbleHomeCsv();
                    else SaveConfigBubblePlayCsv();
                }
                return true;
            });
        }

        public void SaveConfigBubbleHomeCsv()
        {
            //get List variables: Id, Name, TypeId,...
            List<string> lstVariables = ConfigBubbleHomeRecord.GetLstVariables();
            List<string> lstVariablesPos = ConfigBubbleHomePositionRecord.GetLstVariables();
            string txt = "";
            string txtPos = "";
            Dictionary<int, string> dctBubbleHomePosition = GetDctBubblePosition();

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

            var listBubble = _toolBubbleSetting.GetLstBubble();
            for (var i = 0; i < listBubble.Count; i++)
            {
                string bubbleDecoIds = ConvertBubbleDecoIdToString(listBubble[i]);
                txt += listBubble[i].BubbleId + "\t" + bubbleDecoIds + "\t" + i + "\n";
            }

            //foreach (var bubble in _toolBubbleSetting.GetLstBubble())
            //{
            //    string bubbleDecoIds = ConvertBubbleDecoIdToString(bubble);
            //    txt += bubble.BubbleId + "\t" + bubbleDecoIds + "\n";
            //}

            foreach (var pair in dctBubbleHomePosition)
            {
                txtPos += pair.Key + "\t" + pair.Value + "\n";
            }

            //save txt to file .csv
            FileSaving.Save(Application.dataPath + _configBubbleHomeFilePath, txt);
            FileSaving.Save(Application.dataPath + _configBubbleHomePositionFilePath, txtPos);
            Debug.LogError("Export Bubble Home success");
        }
        public void SaveConfigBubblePlayCsv()
        {
            //get List variables: Id, Name, TypeId,...
            List<string> lstVariables = ConfigBubblePlayRecord.GetLstVariables();
            List<string> lstVariablesPos = ConfigBubblePlayPositionRecord.GetLstVariables();
            string txt = "";
            string txtPos = "";
            Dictionary<int, string> dctBubblePlayPosition = GetDctBubblePosition();
            var StringUnpackingDeco = ConvertListToString(_toolUnpackingSetting.LstUnpackDeco);
            if (_lstConfigBubblePlayRecords.Count != 0)
            {
                List<ConfigBubblePlayRecord> listRemovedRecord = new List<ConfigBubblePlayRecord>();
                for (var i = 0; i < _lstConfigBubblePlayRecords.Count; i++)
                {
                    var record = _lstConfigBubblePlayRecords[i];
                    var listRecord = SGUtils.ParseStringToList(record.BubbleId, '_');
                    if (listRecord[0] == _inputMapId.text)
                        listRemovedRecord.Add(record);
                }
                foreach (var rec in listRemovedRecord)
                {
                    _lstConfigBubblePlayRecords.Remove(rec);
                }

                foreach (var bubble in _toolBubbleSetting.GetLstBubble())
                {
                    ConfigBubblePlayRecord newRecord = new ConfigBubblePlayRecord();
                    newRecord.BubbleId = bubble.BubbleId;
                    newRecord.BubbleDecoIds = ConvertBubbleDecoIdToString(bubble);
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

                foreach (var bubble in _toolBubbleSetting.GetLstBubble())
                {
                    string bubbleDecoIds = ConvertBubbleDecoIdToString(bubble);
                    txt += bubble.BubbleId + "\t" + bubbleDecoIds + "\n";
                }

                FileSaving.Save(Application.dataPath + _configBubblePlayFilePath, txt);
            }

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
                if (dctBubblePlayPosition.Count != 0)
                {
                    ConfigBubblePlayPositionRecord newRecord = new ConfigBubblePlayPositionRecord();
                    newRecord.RoomId = dctBubblePlayPosition.ElementAt(0).Key.ToString();
                    newRecord.LstBubblePosition = dctBubblePlayPosition.ElementAt(0).Value;
                    newRecord.LstUnpackingDeco = StringUnpackingDeco;
                    _lstConfigBubblePlayPositionRecords.Add(newRecord);
                }
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
                if (dctBubblePlayPosition.Count != 0)
                {
                    txtPos += dctBubblePlayPosition.ElementAt(0).Key + "\t" + dctBubblePlayPosition.ElementAt(0).Value + "\t" + StringUnpackingDeco + "\n";
                }
                FileSaving.Save(Application.dataPath + _configBubblePlayPositionFilePath, txtPos);
            }
            Debug.LogError("Export Bubble Play success");
        }

        public void OnButtonImportBubbleCsv()
        {
            LoadFileCsv();
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                foreach (var record in _lstConfigBubbleHomePositionRecords)
                {
                    List<Vector3> listBubblePosition = record.GetLstBubblePositionVector3();
                    var idxList = FindIndexInListRoom(int.Parse(record.RoomId));
                    for ( var i = 0; i < listBubblePosition.Count; i++)
                    {
                        Vector3 temp = listBubblePosition[i];
                        temp.x += _areaManager.ListRooms[idxList].Position.x;
                        temp.y += _areaManager.ListRooms[idxList].Position.y;
                        listBubblePosition[i] = temp;
                    }
                    //add info to BubbleItem
                    for (var i = 0; i < listBubblePosition.Count; i++)
                    {
                        Debug.LogError("roomId + i: " + int.Parse(record.RoomId) + " " + i);
                        _toolBubbleSetting.OnImportAddBubble(int.Parse(record.RoomId), i, listBubblePosition[i]);
                    }
                }
                //add Deco&Color to BubbleItem
                var lstBubbleItem = _toolBubbleSetting.GetLstBubble();
                foreach (var item in lstBubbleItem)
                {
                    foreach (var rec in _lstConfigBubbleHomeRecords)
                    {
                        if (rec.BubbleId == item.BubbleId)
                        {
                            item.DctDecoIdColor = rec.GetDctBubbleIdColor();
                            break;
                        }
                    }
                    foreach (var pair in item.DctDecoIdColor)
                    {
                        foreach (var color in pair.Value)
                        {
                            _toolBubbleDecoSetting.ImportDecoItems(pair.Key, color, item);
                            Debug.LogError("color " + color + " pair.Key " + pair.Key + " item.Index " + item.Index);
                        }
                    }
                }
                foreach (var item in lstBubbleItem)
                {
                    foreach (var pair in _toolBubbleDecoSetting.DctRootDecoItems)
                    {
                        if (pair.Key.BubbleId == item.BubbleId)
                        {
                            pair.Value[0].OnImportSpawnDeco(pair.Key, item.BubblePosition);
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (var record in _lstConfigBubblePlayPositionRecords)
                {
                    if (record.RoomId == _inputMapId.text)
                    {
                        var listBubblePosition = record.GetLstBubblePositionVector3();
                        var listUnpacking = record.GetLstUnpackingDeco();
                        //add info to BubbleItem
                        for (var i = 0; i < listBubblePosition.Count; i++)
                        {
                            _toolBubbleSetting.OnImportAddBubble(int.Parse(record.RoomId), i, listBubblePosition[i]);
                        }
                        _toolUnpackingSetting.LstUnpackDeco = listUnpacking;
                        _toolUnpackingSetting.ImportUnpackingDeco();
                        break;
                    }
                }
                //add Deco&Color to BubbleItem
                var lstBubbleItem = _toolBubbleSetting.GetLstBubble();
                foreach (var item in lstBubbleItem)
                {
                    foreach (var rec in _lstConfigBubblePlayRecords)
                    {
                        if (rec.BubbleId == item.BubbleId)
                        {
                            item.DctDecoIdColor = rec.GetDctBubbleIdColor();
                            break;
                        }
                    }
                    foreach (var pair in item.DctDecoIdColor)
                    {
                        foreach (var color in pair.Value)
                        {
                            _toolBubbleDecoSetting.ImportDecoItems(pair.Key, color, item);
                            Debug.LogError("color " + color + " pair.Key " + pair.Key + " item.Index " + item.Index);
                        }
                    }
                }
                foreach (var item in lstBubbleItem)
                {
                    foreach (var pair in _toolBubbleDecoSetting.DctRootDecoItems)
                    {
                        if (pair.Key.BubbleId == item.BubbleId)
                        {
                            pair.Value[0].OnImportSpawnDeco(pair.Key, item.BubblePosition);
                            break;
                        }
                    }
                }
            }
            
        }

        public void LoadFileCsv()
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                var txtBubbleHome = FileSaving.Load(Application.dataPath + _configBubbleHomeFilePath);
                var _configBubbleHome = new ConfigBubbleHome();
                _configBubbleHome.LoadFromString(txtBubbleHome);
                _lstConfigBubbleHomeRecords.Clear();
                _lstConfigBubbleHomeRecords.AddRange(_configBubbleHome.Records);
                Debug.LogError("num of listconfigbubhomerec: " + _lstConfigBubbleHomeRecords.Count);
                ListConfigBubbleHomeRecords = _lstConfigBubbleHomeRecords.AsReadOnly();

                var txtBubbleHomePosition = FileSaving.Load(Application.dataPath + _configBubbleHomePositionFilePath);
                var _configBubbleHomePosition = new ConfigBubbleHomePosition();
                _configBubbleHomePosition.LoadFromString(txtBubbleHomePosition);
                _lstConfigBubbleHomePositionRecords.Clear();
                _lstConfigBubbleHomePositionRecords.AddRange(_configBubbleHomePosition.Records);
                ListConfigBubbleHomePositionRecords = _lstConfigBubbleHomePositionRecords.AsReadOnly();
            }
            else
            {
                var txtBubblePlay = FileSaving.Load(Application.dataPath + _configBubblePlayFilePath);
                var _configBubblePlay = new ConfigBubblePlay();
                _configBubblePlay.LoadFromString(txtBubblePlay);
                _lstConfigBubblePlayRecords.Clear();
                _lstConfigBubblePlayRecords.AddRange(_configBubblePlay.Records);
                Debug.LogError("num of listconfigbubplayrec: " + _lstConfigBubblePlayRecords.Count);
                ListConfigBubblePlayRecords = _lstConfigBubblePlayRecords.AsReadOnly();

                var txtBubblePlayPosition = FileSaving.Load(Application.dataPath + _configBubblePlayPositionFilePath);
                var _configBubblePlayPosition = new ConfigBubblePlayPosition();
                _lstConfigBubblePlayPositionRecords.Clear();
                _configBubblePlayPosition.LoadFromString(txtBubblePlayPosition);
                _lstConfigBubblePlayPositionRecords.AddRange(_configBubblePlayPosition.Records);
                ListConfigBubblePlayPositionRecords = _lstConfigBubblePlayPositionRecords.AsReadOnly();
            }
        }

        public Dictionary<int, string> GetDctBubblePosition()
        {
            Dictionary<int, string> dctBubblePosition = new Dictionary<int, string>();
            for (var i = 0; i < _toolBubbleSetting.GetLstBubble().Count; i++)
            {
                var bubble = _toolBubbleSetting.GetLstBubble()[i];
                var info = (DecoInfo)bubble.Info;
                //Debug.LogError("bubble.Id: " + info.Id);
                int idx = bubble.RoomIndex;
                var idxList = FindIndexInListRoom(idx);
                if (!dctBubblePosition.ContainsKey(idx))
                    dctBubblePosition.Add(idx, GetStringBubblePosition(bubble.BubblePosition, _areaManager.ListRooms[idxList].Position));
                else dctBubblePosition[idx] += GetStringBubblePosition(bubble.BubblePosition, _areaManager.ListRooms[idxList].Position);
            }
            return dctBubblePosition;
        }
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
                txt += configRecords[i].BubbleId + "\t" + configRecords[i].BubbleDecoIds + "\n";
            }
            return txt;
        }

        public string ConvertConfigBubblePlayPositionRecordToStringCsv(List<ConfigBubblePlayPositionRecord> configRecords)
        {
            string txt = "";
            for (var i = 0; i < configRecords.Count; i++)
            {
                txt += configRecords[i].RoomId + "\t" + configRecords[i].LstBubblePosition + "\t" + configRecords[i].LstUnpackingDeco + "\n";
            }
            return txt;
        }

        public string GetStringBubblePosition(Vector3 position, Vector3 roomPos)
        {
            return "[" + (position.x - roomPos.x) + "," + (position.y - roomPos.y) + "," + position.z + "];";
        }

        public string ConvertBubbleDecoIdToString(ToolCreateMapBubbleItem bubble)
        {
            string txt = "";
            foreach (var pair in bubble.DctDecoIdColor)
            {
                for (var i = 0; i < pair.Value.Count; i++)
                {
                    txt += pair.Key + "_" + pair.Value[i] + ";";
                }

            }
            return txt;
        }

        public string ConvertListToString(List<string> list)
        {
            string txt = "";
            for (var i = 0; i < list.Count; i++)
                txt += list[i] + ";";
            return txt;
        }

        public void SaveConfigPlayKAPToKDL()
        {
            List<string> lstVariables = ConfigBubblePlayRecord.GetLstVariables();
            List<string> lstVariablesPos = ConfigBubblePlayPositionRecord.GetLstVariables();
            string txt = "";
            string txtPos = "";
            //Save configPlay
            if (_lstConfigBubblePlayRecords.Count != 0)
            {
                List<ConfigBubblePlayRecord> listRemovedRecord = new List<ConfigBubblePlayRecord>();
                //delete all old BubbleId in config with same Room Id
                for (var i = 0; i < _lstConfigBubblePlayRecords.Count; i++)
                {
                    var record = _lstConfigBubblePlayRecords[i];
                    var listRecord = SGUtils.ParseStringToList(record.BubbleId, '_');
                    if (listRecord[0] == _inputMapId.text)
                        listRemovedRecord.Add(record);
                }
                foreach (var rec in listRemovedRecord)
                {
                    _lstConfigBubblePlayRecords.Remove(rec);
                }

                foreach (var pair in _toolTransfer.GetDctBubbleDecoIds())
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

                foreach (var pair in _toolTransfer.GetDctBubbleDecoIds())
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
                foreach (var pos in _toolTransfer.GetListPosition())
                {
                    strPos += GetStringBubblePosition(pos, Vector3.zero);
                }
                Debug.LogError("strPos: " + strPos);

                newRecord.RoomId = _inputMapId.text;
                newRecord.LstBubblePosition = strPos;
                newRecord.LstUnpackingDeco = ConvertListToString(_toolTransfer.GetListUnpackingDeco());
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
                foreach (var pos in _toolTransfer.GetListPosition())
                {
                    strPos += GetStringBubblePosition(pos, Vector3.zero);
                }
                txtPos += _inputMapId.text + "\t" + strPos + "\t" + ConvertListToString(_toolTransfer.GetListUnpackingDeco());
                FileSaving.Save(Application.dataPath + _configBubblePlayPositionFilePath, txtPos);
            }
            Debug.LogError("Export Bubble Play success");
        }
        #endregion

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
    }

}
