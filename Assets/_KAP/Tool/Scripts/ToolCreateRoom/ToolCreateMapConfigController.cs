using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using KAP.Config;
using Kawaii.ResourceManager;

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
        private string _configHiveShop = "/_KAP/_GameResources/Configs/Hive/ConfigHiveShop.csv";
        [SerializeField] private string _configRoomChallengePath = "/_KAP/_GameResources/Configs/Challenge/ConfigRoomChallenge.csv";

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
            //save data from txtRoom to _configRoom in Records (attribute of ConfigRoom)
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
