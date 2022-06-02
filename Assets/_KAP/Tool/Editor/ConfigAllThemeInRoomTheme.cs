#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kawaii.ResourceManager;
using KAP.Config;
using Pathfinding.Serialization.JsonFx;
using Kawaii.IsoTools.DecoSystem;

namespace KAP.Tools
{
    public class ConfigAllThemeInRoomTheme : Editor
    {
        const string _configDecoFilePath = "/_KAP/_GameResources/Configs/Deco/ConfigDeco.csv";
        const string _configRoomThemeFilePath = "/_KAP/_GameResources/Configs/Room/ConfigRoomTheme.csv";
        const string _jsonRoomThemeFolderPath = "/_KAP/_GameResources/Maps/RoomThemes/";

        [MenuItem("Tools/KAP/Config Theme Use In Room Theme", false, 2)]
        public static void ConfigAllThemeUseInRoomTheme()
        {
            var txtConfigDeco = FileSaving.Load(Application.dataPath + _configDecoFilePath);
            var configDeco = new ConfigDeco();
            configDeco.LoadFromString(txtConfigDeco);

            var txtConfigRoomTheme = FileSaving.Load(Application.dataPath + _configRoomThemeFilePath);
            var configRoomTheme = new ConfigRoomTheme();
            configRoomTheme.LoadFromString(txtConfigRoomTheme);

            if(configRoomTheme.Records.Count <= 0)
            {
                Debug.LogError(string.Format("Error!! Not any record"));
                return;
            }

            foreach (var record in configRoomTheme.Records)
            {
                GetThemeUseInRoomTheme(record, configDeco);
            }

            List<string> lstVariables = ConfigRoomThemeRecord.GetLstVariables();
            string txt = "";
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";

            foreach (var record in configRoomTheme.Records)
            {
                txt += record.GetTextRecord();
            }
            FileSaving.Save(Application.dataPath + _configRoomThemeFilePath, txt);
        }

        static void GetThemeUseInRoomTheme(ConfigRoomThemeRecord record, ConfigDeco configDeco)
        {
            var jsonPath = Application.dataPath + _jsonRoomThemeFolderPath + record.Id + ".json";
            var json = FileSaving.Load(jsonPath);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("Json is null " + record.Id);
                return;
            }

            var data = JsonReader.Deserialize<Dictionary<string, DecoDataArray[]>>(json);
            Dictionary<int, bool> allThemes = new Dictionary<int, bool>();
            foreach (var iter in data)
            {
                foreach (var deco in iter.Value)
                {
                    if (string.IsNullOrEmpty(deco.Info))
                        continue;
                    var info = JsonReader.Deserialize<DecoInfo>(deco.Info);
                    var configDecoRecord = configDeco.GetDecoById(info.Id);
                    if (configDecoRecord == null)
                        continue;
                    allThemes[configDecoRecord.ThemeId] = true;
                }
            }
            string txtThemes = "";
            foreach (var iter in allThemes)
            {
                txtThemes += iter.Key + ";";
            }

            record.AllThemes = txtThemes;
            Debug.LogError(string.Format("get decoTheme using {0}: {1}", record.Id, record.AllThemes));
        }
    }
}
#endif