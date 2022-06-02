#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pathfinding.Serialization.JsonFx;
using System.IO;
using Kawaii.IsoTools.DecoSystem;
using Kawaii.ResourceManager;
namespace KAP
{
    public class ConvertOldJsonToNewJsonMap : Editor
    {
        const string _jsonFolderPath = "Assets/_KAP/_GameResources/Maps/";
        [MenuItem("Tools/KAP/Convert Old Map Json", false, 2)]
        public static void ConverOldMapJson()
        {
            if (!Directory.Exists(_jsonFolderPath))
                return;
            var folder = new DirectoryInfo(_jsonFolderPath);
            var allFiles = folder.GetFiles("*.json", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(FileUtil.GetProjectRelativePath(file.FullName));
                var json = textAsset.text;
               
                try
                {
                    var newData = new Dictionary<int, List<Dictionary<string, object>>>();
                    var data = JsonReader.Deserialize<DecoDataTree[]>(json);
                    foreach(var deco in data)
                    {
                        ScanOldDecoRecrusive(newData, deco, 0);
                    }

                    FileSaving.Save(file.FullName, JsonWriter.Serialize(newData));
                }
                catch
                {
                    var newData = new Dictionary<string, List<Dictionary<string, object>>>();
                    var data = JsonReader.Deserialize<Dictionary<string, DecoDataArray[]>>(json);
                    int roomId = 0;
                    foreach(var iter in data)
                    {
                        List<Dictionary<string, object>> lst = null;
                        if (!newData.TryGetValue(iter.Key, out lst))
                        {
                            lst = new List<Dictionary<string, object>>();
                            newData[iter.Key] = lst;
                        }
                        foreach (var decoData in iter.Value)
                        {
                            DecoInfo info = null;
                            if (decoData.Size != null)
                            {
                                Debug.LogError(roomId);
                                info = new DecoInfo
                                {
                                    Id = roomId
                                };
                                roomId++;
                            }
                            else
                                info = JsonReader.Deserialize<DecoInfo>(decoData.Info);
                            decoData.Info = info.ExportToJson(null);
                            lst.Add(decoData.ToJsonObject);
                        }
                    }
                    FileSaving.Save(file.FullName, JsonWriter.Serialize(newData));
                }
            }
        }

        static void ScanOldDecoRecrusive(Dictionary<int, List<Dictionary<string, object>>> result, DecoDataTree deco, int level)
        {
            DecoDataArray newDeco = new DecoDataArray
            {
                Position = deco.Position,
                WorldDirect = deco.WorldDirect,
                Group = deco.Group,
                Size = deco.Size
            };

            if(newDeco.Size != null)
            {
                var info = new DecoInfo { Id = deco.RoomId };
                newDeco.Info = info.ExportToJson(null);
            }
            else if(!string.IsNullOrEmpty(deco.Info))
            {
                var info = JsonReader.Deserialize<DecoInfo>(deco.Info);
                newDeco.Info = info.ExportToJson(null);
            }

            List<Dictionary<string, object>> lst = null;
            if(!result.TryGetValue(level, out lst))
            {
                lst = new List<Dictionary<string, object>>();
                result[level] = lst;
            }
            lst.Add(newDeco.ToJsonObject);

            if(deco.LstChilds != null)
            {
                var childLevel = level + 1;
                foreach(var child in deco.LstChilds)
                {
                    ScanOldDecoRecrusive(result, child, childLevel);
                }
            }
        }
    }
}
#endif