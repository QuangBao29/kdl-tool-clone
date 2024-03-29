﻿#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kawaii.ResourceManager;
using KAP.Config;
using Pathfinding.Serialization.JsonFx;
using Kawaii.IsoTools.DecoSystem;

namespace KAP
{
    public class ConfigAllThemeInRoomChallenge : Editor
    {
        const string _configDecoFilePath = "/_KAP/_GameResources/Configs/Deco/ConfigDeco.csv";
        const string _configChallengeFilePath = "/_KAP/_GameResources/Configs/Challenge/ConfigRoomChallenge.csv";
        const string _jsonRoomChallengeFolderPath = "/_KAP/_GameResources/Maps/RoomChallenge/";

        [MenuItem("Tools/KAP/Config Deco Themes Use In Room Challenge", false, 2)]
        public static void SetConfigAllThemeUseInRoomChallenge()
        {
            var txtConfigDeco = FileSaving.Load(Application.dataPath + _configDecoFilePath);
            var configDeco = new ConfigDeco();
            configDeco.LoadFromString(txtConfigDeco);

            var txtConfigRoomChallenge = FileSaving.Load(Application.dataPath + _configChallengeFilePath);
            var configChallenge = new ConfigRoomChallenge();
            configChallenge.LoadFromString(txtConfigRoomChallenge);

            foreach (var record in configChallenge.Records)
            {
                GetThemeUseInChallenge(record, configDeco);
            }

            List<string> lstVariables = ConfigRoomChallengeRecord.GetLstVariables();
            string txt = "";
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";

            foreach (var record in configChallenge.Records)
            {
                txt += record.GetTextRecord();
            }
            FileSaving.Save(Application.dataPath + _configChallengeFilePath, txt);
        }

        static void GetThemeUseInChallenge(ConfigRoomChallengeRecord record, ConfigDeco configDeco)
        {
            var jsonPath = Application.dataPath + _jsonRoomChallengeFolderPath + record.Id + ".json";
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