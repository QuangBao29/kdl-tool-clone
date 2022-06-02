#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Kawaii.ResourceManager
{
    public class CreateAtlasUtils : UnityEditor.Editor
    {
        public static KawaiiAtlasCloudSettings GetOrCreateAtlasSetting(string targetPath)
        {
            var cloudSetting = AssetDatabase.LoadAssetAtPath<KawaiiAtlasCloudSettings>(targetPath);
            if (cloudSetting != null)
                return cloudSetting;
            cloudSetting = new KawaiiAtlasCloudSettings();
            AssetDatabase.CreateAsset(cloudSetting, targetPath);
            AssetDatabase.SaveAssets();
            return cloudSetting;
        }

        public static void AddAtlasToCloudSetting(string settingPath, string atlasName)
        {
            var cloudSetting = GetOrCreateAtlasSetting(settingPath);
            cloudSetting.Add(atlasName);
            AssetDatabase.SaveAssets();
        }

        public static void CreateAtlas(KawaiiAtlas atlas, string targetPath, string settingPath)
        {
            AssetDatabase.CreateAsset(atlas, targetPath);
            if(!string.IsNullOrEmpty(settingPath))
                AddAtlasToCloudSetting(settingPath, atlas.name);
            else
                AssetDatabase.SaveAssets();
        }

        public static void CreateATextAtlasInFolder(string folderPath, string extension, string targetPath, string settingPath)
        {
            if (!Directory.Exists(folderPath))
                return;
            var folder = new DirectoryInfo(folderPath);
            FileInfo[] allFiles = folder.GetFiles("*"+extension, SearchOption.AllDirectories);
            KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
            atlas.LstTexts = new List<TextAsset>();
            foreach (var file in allFiles)
            {
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                if (textAsset != null)
                    atlas.LstTexts.Add(textAsset);
            }
            CreateAtlas(atlas, targetPath, settingPath);
        }

        public static void CreateListAtlasInFolder(string folderPath, string extension, string targetPath, string settingPath)
        {
            if (!Directory.Exists(folderPath))
                return;
            var folder = new DirectoryInfo(folderPath);
            FileInfo[] allFiles = folder.GetFiles("*" + extension, SearchOption.AllDirectories);

            foreach (var file in allFiles)
            {
                KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
                atlas.LstTexts = new List<TextAsset>();
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                if (textAsset != null)
                    atlas.LstTexts.Add(textAsset);
                CreateAtlas(atlas, targetPath + file.Name.Replace(extension, "") + ".asset", settingPath);
            }
        }

        public static void CreateAllPrefabAtlasInFolder(string folderPath, string targetPath, string settingPath)
        {
            if (!Directory.Exists(folderPath))
                return;

            var folder = new DirectoryInfo(folderPath);
            FileInfo[] allFiles = folder.GetFiles("*.prefab", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                atlas.LstPrefabs = new List<GameObject> { prefab };
                CreateAtlas(atlas, targetPath + file.Name.Replace(".prefab", "") + ".asset", settingPath);
            }
        }

        public static void CreateAtlasFromATexture(string texturePath, string targetPath, string settingPath)
        {
            KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
            atlas.LstSprites = new List<Sprite>();

            var objSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(texturePath);
            foreach (var obj in objSprites)
            {
                var sprite = (Sprite)obj;
                if (!atlas.LstSprites.Contains(sprite))
                    atlas.LstSprites.Add(sprite);
            }
            CreateAtlas(atlas, targetPath, settingPath);
        }

        public static void CreateListTexutreAtlasInFolder(string folderPath, string targetPath, string settingPath)
        {
            if (!Directory.Exists(folderPath))
                return;
            var folder = new DirectoryInfo(folderPath);
            FileInfo[] allFiles = folder.GetFiles("*.png", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
                atlas.LstSprites = new List<Sprite>();
                var objSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                foreach (var obj in objSprites)
                {
                    var sprite = (Sprite)obj;
                    if (!atlas.LstSprites.Contains(sprite))
                        atlas.LstSprites.Add(sprite);
                }
                CreateAtlas(atlas, targetPath + file.Name.Replace(".png", "") + ".asset", settingPath);
            }
        }

        public static void CreateATextureAtlasInFolder(string folderPath, string targetPath, string settingPath)
        {
            if (!Directory.Exists(folderPath))
                return;
            var folder = new DirectoryInfo(folderPath);
            FileInfo[] allFiles = folder.GetFiles("*.png", SearchOption.AllDirectories);
            KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
            atlas.LstSprites = new List<Sprite>();
            foreach (var file in allFiles)
            {
                var objSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                foreach (var obj in objSprites)
                {
                    var sprite = (Sprite)obj;
                    if (!atlas.LstSprites.Contains(sprite))
                        atlas.LstSprites.Add(sprite);
                }

            }
            CreateAtlas(atlas, targetPath, settingPath);
        }
    }
}

#endif