#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Imba.UI;
using Imba;

namespace KAP.Tools
{
    public class DatabaseHelperEditor : Editor
    {
        [MenuItem("Tools/KAP/Create Popup Database", false, 100)]
        static public void LoadPopupPrefabToDatabase()
        {
            string popupDatabasePath =  "Assets/_KAP/_GameResources/Databases/PopupDatabase.asset";
            string popupPrefabsFolderPath = Application.dataPath + "/_KAP/_GameResources/Resources/Prefabs/UI/Popups/";
            
            // Get all popup prefabs
            var folder = new DirectoryInfo(popupPrefabsFolderPath);
            var allFiles = folder.GetFiles("*.prefab", SearchOption.AllDirectories);

            // Create popup database
            UIPopupDatabase popupDatabase = new UIPopupDatabase();
            popupDatabase.Reset();
            foreach (var file in allFiles)
            {
                var popupPrefab = AssetDatabase.LoadAssetAtPath<Object>(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                popupDatabase.Database.Add(new UIPopupLink
                {
                    prefab = (GameObject)popupPrefab,
                    name = popupPrefab.name,
                });
            }
            AssetDatabase.CreateAsset(popupDatabase, popupDatabasePath);

            AssetDatabase.SaveAssets();
        }
    }
}

#endif