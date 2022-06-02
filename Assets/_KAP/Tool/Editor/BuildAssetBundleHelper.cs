#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Kawaii.ResourceManager;

namespace KAP.Tools
{
    public class BuildAssetBundleHelper : Editor
    {
        const string _cloudSettingPath = "Assets/_KAP/_GameResources/Atlas/CloudBundleSetting.asset";

        [MenuItem("Tools/KAP/Create all Atlas & Build AssetBundle For Streaming Asset", false, 2)]
        public static void BuildAssetbundleForStreamingAsset()
        {
            CreateConfigHiveShop.Create();
            ConfigAllThemeInRoom.ConfigAllThemeUseInRoom();
            ConfigAllThemeInRoomTheme.ConfigAllThemeUseInRoomTheme();
            ConfigAllThemeInWonder.SetConfigAllThemeUseInWonder();
            ConfigAllThemeInRoomChallenge.SetConfigAllThemeUseInRoomChallenge();
            CreateAtlasHelper.CreateAllAtlas();
            BuildAssetbundle(KawaiiAtlasCloudSettings.AtlasType.StreamingAsset);
        }

        [MenuItem("Tools/KAP/Build AssetBundle For Cloud", false, 2)]
        public static void BuildAssetbundleForCloud()
        {
            BuildAssetbundle(KawaiiAtlasCloudSettings.AtlasType.Cloud);
        }

        static void BuildAssetbundle(KawaiiAtlasCloudSettings.AtlasType atlasType)
        {
            string path = EditorUtility.SaveFolderPanel("Save Bundle", EditorPrefs.GetString("BundleManagerLastDir", ""), "");
            if (path.Length != 0)
            {
                KawaiiAtlasCloudSettings cloudSetting = AssetDatabase.LoadAssetAtPath<KawaiiAtlasCloudSettings>(_cloudSettingPath);
                if(cloudSetting == null)
                {
                    Debug.LogError("Cloud Setting Is Missing!");
                    return;
                }

                EditorPrefs.SetString("BundleManagerLastDir", path);
                var atlasPath = Application.dataPath + @"/_KAP/_GameResources/Atlas/";
                var d = new DirectoryInfo(atlasPath);
                var Files = d.GetFiles("*.asset", SearchOption.AllDirectories);
                if (Files != null && Files.Length > 0)
                {
                    foreach (var file in Files)
                    {
                        var aPath = FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/'));
                        var atlas = AssetDatabase.LoadAssetAtPath<KawaiiAtlas>(aPath);
                        if(atlas != null )
                        {
                            AssetImporter assetImporter = AssetImporter.GetAtPath(aPath);
                            var type = cloudSetting.GetTypeOfAtlas(atlas.name);
                            if (type == KawaiiAtlasCloudSettings.AtlasType.All || type == atlasType)
                                assetImporter.assetBundleName = file.Name.Replace(".asset", "") + ".bundle";
                            else
                                assetImporter.assetBundleName = null;
                            assetImporter.SaveAndReimport();
                        }
                    }
                }

#if UNITY_IOS
            BuildBundle (path + "/iOS", BuildTarget.iOS, BuildAssetBundleOptions.None);
#endif
#if UNITY_ANDROID
                BuildBundle(path + "/Android", BuildTarget.Android, BuildAssetBundleOptions.None);

#endif
            }
        }

        static void BuildBundle(string path, BuildTarget target, BuildAssetBundleOptions opts = BuildAssetBundleOptions.None)
        {
            Debug.Log("BuildBundle: " + path);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //BuildAssetBundleOptions opts = BuildAssetBundleOptions.DeterministicAssetBundle;
            //BuildAssetBundleOptions opts = BuildAssetBundleOptions.ChunkBasedCompression;
            BuildPipeline.BuildAssetBundles(path, opts, target);
        }
    }
}
#endif