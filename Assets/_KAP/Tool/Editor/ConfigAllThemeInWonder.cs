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
    public class ConfigAllThemeInWonder : Editor
    {
        const string _configDecoFilePath = "/_KAP/_GameResources/Configs/Deco/ConfigDeco.csv";
        const string _configWonderFilePath = "/_KAP/_GameResources/Configs/Wonder/ConfigWonder.csv";
        const string _jsonWonderFolderPath = "/_KAP/_GameResources/Maps/Wonders/";

        [MenuItem("Tools/KAP/Config Deco Themes Use In Wonder", false, 2)]
        public static void SetConfigAllThemeUseInWonder()
        {
            var txtConfigDeco = FileSaving.Load(Application.dataPath + _configDecoFilePath);
            var configDeco = new ConfigDeco();
            configDeco.LoadFromString(txtConfigDeco);

            var txtConfigWonder = FileSaving.Load(Application.dataPath + _configWonderFilePath);
            var configWonder = new ConfigWonder();
            configWonder.LoadFromString(txtConfigWonder);

            foreach (var record in configWonder.Records)
            {
                GetThemeUseInWonder(record, configDeco);
            }

            List<string> lstVariables = ConfigWonderRecord.GetLstVariables();
            string txt = "";
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";

            foreach (var record in configWonder.Records)
            {
                txt += record.GetTextRecord();
            }
            FileSaving.Save(Application.dataPath + _configWonderFilePath, txt);
        }

        static void GetThemeUseInWonder(ConfigWonderRecord record, ConfigDeco configDeco)
        {
            var jsonPath = Application.dataPath + _jsonWonderFolderPath + record.Id + ".json";
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
            Debug.LogError(record.AllThemes);
        }
    }

}
#endif
