#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kawaii.ResourceManager;
using Imba.Utils;
using System.IO;

namespace KAP.Tools
{
    public class CreateAtlasHelper : Editor
    {
        #region PATH
        const string _atlasFolderPath = "Assets/_KAP/_GameResources/Atlas/";
        const string _textureFolderPath = "Assets/_KAP/_GameResources/Textures/";
        const string _prefabFolderPath = "Assets/_KAP/_GameResources/Prefabs/";
        const string _cloudSettingPath = "Assets/_KAP/_GameResources/Atlas/CloudBundleSetting.asset";
        #endregion

        [MenuItem("Tools/KAP/Create Game Atlas", false, 2)]
        public static void CreateAllAtlas()
        {
            if (Directory.Exists(_atlasFolderPath))
            {
                var atlasFolder = new DirectoryInfo(_atlasFolderPath);
                foreach (FileInfo file in atlasFolder.GetFiles())
                {
                    if (file.Name == "CloudBundleSetting.asset")
                        continue;
                    file.Delete();
                }
                foreach (DirectoryInfo dir in atlasFolder.GetDirectories())
                {
                    dir.Delete(true);
                }
            }

            CreateDecorAtlas();

            // sprite
            CreateAtlasUtils.CreateAtlasFromATexture(_textureFolderPath + @"CommonIcons/common.png", _atlasFolderPath + "commonicons.asset", _cloudSettingPath);
            CreateAtlasUtils.CreateAtlasFromATexture(_textureFolderPath + @"CommonIcons/deco_theme.png", _atlasFolderPath + "deco_theme.asset", _cloudSettingPath);
            CreateAtlasUtils.CreateAtlasFromATexture(_textureFolderPath + @"Shop/shop.png", _atlasFolderPath + "shop.asset", _cloudSettingPath);
            CreateRoomAtlas();
            CreateRoomThemeAtlas();
            CreateWonderAtlas();
            CreateHiveDefaultAtlas();
            CreateRoomChallengeAtlas();
            CreateSeparatedRoomAtlas();
            if (!Directory.Exists(_atlasFolderPath + "RoomTypes/"))
                Directory.CreateDirectory(_atlasFolderPath + "RoomTypes/");
            CreateAtlasUtils.CreateAtlasFromATexture(_textureFolderPath + @"RoomTypes/roomtypeicons.png", _atlasFolderPath + @"RoomTypes/roomtypeicons.asset", _cloudSettingPath);

            // config
            CreateAtlasUtils.CreateATextAtlasInFolder(Application.dataPath + @"/_KAP/_GameResources/Configs/", "*.csv", _atlasFolderPath + "configs.asset", _cloudSettingPath);

        }

        /// <summary>
        /// Create and merge decors same theme in multi textures
        /// </summary>
        static void CreateDecorAtlas()
        {
            string targetFolderPath = _atlasFolderPath + "Decos/";
            if (!Directory.Exists(targetFolderPath))
                Directory.CreateDirectory(targetFolderPath);

            var decosFolderPath = Application.dataPath + @"/_KAP/_GameResources/Textures/Decos/";
            if (!Directory.Exists(decosFolderPath))
                return;
            DirectoryInfo decosFolder = new DirectoryInfo(decosFolderPath);
            FileInfo[] allTextures = decosFolder.GetFiles("*.png", SearchOption.AllDirectories);

            Dictionary<string, KawaiiAtlas> dicAtlas = new Dictionary<string, KawaiiAtlas>();
            foreach (var texture in allTextures)
            {
                var path = FileUtil.GetProjectRelativePath(texture.FullName.Replace('\\', '/'));
                var nameParts = SGUtils.ParseStringToList(texture.Name.Replace(".png", ""), '_');

                var themeId = nameParts[0];

                KawaiiAtlas atlas = null;
                if (!dicAtlas.TryGetValue(themeId, out atlas))
                {
                    atlas = CreateInstance<KawaiiAtlas>();
                    atlas.LstSprites = new List<Sprite>();
                    dicAtlas[themeId] = atlas;
                }

                var objSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                foreach (var obj in objSprites)
                {
                    var sprite = (Sprite)obj;
                    if (!atlas.LstSprites.Contains(sprite))
                        atlas.LstSprites.Add(sprite);
                }
            }

            foreach (var iter in dicAtlas)
            {
                CreateAtlasUtils.CreateAtlas(iter.Value, targetFolderPath + iter.Key + ".asset", _cloudSettingPath);
            }
        }

        static void CreateRoomAtlas()
        {
            string targetFolderPath = _atlasFolderPath + "Rooms/";
            if (!Directory.Exists(targetFolderPath))
                Directory.CreateDirectory(targetFolderPath);
            var textureFolderPath = Application.dataPath + @"/_KAP/_GameResources/Textures/Rooms/";
            var jsonFolderPath = Application.dataPath + @"/_KAP/_GameResources/Maps/Rooms/";
            if (!Directory.Exists(textureFolderPath))
                return;
            if (!Directory.Exists(jsonFolderPath))
                return;
            var textureFolder = new DirectoryInfo(textureFolderPath);
            FileInfo[] allTextures = textureFolder.GetFiles("*.png", SearchOption.AllDirectories);
            foreach (var file in allTextures)
            {
                KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
                //texture
                atlas.LstSprites = new List<Sprite>();
                var objSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                foreach (var obj in objSprites)
                {
                    var sprite = (Sprite)obj;
                    if (sprite.rect.width > 512 || sprite.rect.height > 512)
                    {
                        Debug.LogError(string.Format("width or height so long: {0}", file.Name));
                        continue;
                    }
                    if (!atlas.LstSprites.Contains(sprite))
                        atlas.LstSprites.Add(sprite);
                }
                //json
                var jsonFilePath = jsonFolderPath + file.Name.Replace(".png", ".json");

                atlas.LstTexts = new List<TextAsset>();
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(FileUtil.GetProjectRelativePath(jsonFilePath.Replace('\\', '/')));
                if (textAsset != null)
                    atlas.LstTexts.Add(textAsset);
                else
                {
                    Debug.LogError("Not found Json File: " + file.Name);
                    continue;
                }
                CreateAtlasUtils.CreateAtlas(atlas, targetFolderPath + file.Name.Replace(".png", "") + ".asset", _cloudSettingPath);
            }
        }

        static void CreateRoomThemeAtlas()
        {
            string targetFolderPath = _atlasFolderPath + "RoomThemes/";
            if (!Directory.Exists(targetFolderPath))
                Directory.CreateDirectory(targetFolderPath);

            var textureFolderPath = Application.dataPath + @"/_KAP/_GameResources/Textures/RoomThemes/";
            var jsonFolderPath = Application.dataPath + @"/_KAP/_GameResources/Maps/RoomThemes/";
            if (!Directory.Exists(textureFolderPath))
                return;
            if (!Directory.Exists(jsonFolderPath))
                return;

            var textureFolder = new DirectoryInfo(textureFolderPath);
            FileInfo[] allTextures = textureFolder.GetFiles("*.png", SearchOption.AllDirectories);
            foreach (var file in allTextures)
            {
                KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
                //texture
                atlas.LstSprites = new List<Sprite>();
                var objSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                foreach (var obj in objSprites)
                {
                    var sprite = (Sprite)obj;
                    if (!atlas.LstSprites.Contains(sprite))
                        atlas.LstSprites.Add(sprite);
                }
                //json
                var jsonFilePath = jsonFolderPath + file.Name.Replace(".png", ".json");

                atlas.LstTexts = new List<TextAsset>();
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(FileUtil.GetProjectRelativePath(jsonFilePath.Replace('\\', '/')));
                if (textAsset != null)
                    atlas.LstTexts.Add(textAsset);
                else
                {
                    Debug.LogError("Not found Json File: " + file.Name);
                    continue;
                }
                CreateAtlasUtils.CreateAtlas(atlas, targetFolderPath + file.Name.Replace(".png", "") + ".asset", _cloudSettingPath);
            }
        }

        static void CreateWonderAtlas()
        {
            string targetFolderPath = _atlasFolderPath + "Wonders/";
            if (!Directory.Exists(targetFolderPath))
                Directory.CreateDirectory(targetFolderPath);

            var textureFolderPath = Application.dataPath + @"/_KAP/_GameResources/Textures/Wonders/";
            var jsonFolderPath = Application.dataPath + @"/_KAP/_GameResources/Maps/Wonders/";
            if (!Directory.Exists(textureFolderPath))
                return;
            if (!Directory.Exists(jsonFolderPath))
                return;

            var textureFolder = new DirectoryInfo(textureFolderPath);
            FileInfo[] allTextures = textureFolder.GetFiles("*.png", SearchOption.AllDirectories);
            foreach (var file in allTextures)
            {
                KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
                //texture
                atlas.LstSprites = new List<Sprite>();
                var objSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                foreach (var obj in objSprites)
                {
                    var sprite = (Sprite)obj;
                    if (!atlas.LstSprites.Contains(sprite))
                        atlas.LstSprites.Add(sprite);
                }
                //json
                var jsonFilePath = jsonFolderPath + file.Name.Replace(".png", ".json");

                atlas.LstTexts = new List<TextAsset>();
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(FileUtil.GetProjectRelativePath(jsonFilePath.Replace('\\', '/')));
                if (textAsset != null)
                    atlas.LstTexts.Add(textAsset);
                else
                {
                    Debug.LogError("Not found Json File: " + file.Name);
                    continue;
                }
                CreateAtlasUtils.CreateAtlas(atlas, targetFolderPath + file.Name.Replace(".png", "") + ".asset", _cloudSettingPath);
            }
        }

        static void CreateRoomChallengeAtlas()
        {
            string targetFolderPath = _atlasFolderPath + "RoomChallenge/";
            if (!Directory.Exists(targetFolderPath))
                Directory.CreateDirectory(targetFolderPath);

            var textureFolderPath = Application.dataPath + @"/_KAP/_GameResources/Textures/RoomChallenge/";
            var jsonFolderPath = Application.dataPath + @"/_KAP/_GameResources/Maps/RoomChallenge/";
            if (!Directory.Exists(textureFolderPath))
                return;
            if (!Directory.Exists(jsonFolderPath))
                return;

            var textureFolder = new DirectoryInfo(textureFolderPath);
            FileInfo[] allTextures = textureFolder.GetFiles("*.png", SearchOption.AllDirectories);
            foreach (var file in allTextures)
            {
                KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
                //texture
                atlas.LstSprites = new List<Sprite>();
                var objSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                foreach (var obj in objSprites)
                {
                    var sprite = (Sprite)obj;
                    if (!atlas.LstSprites.Contains(sprite))
                        atlas.LstSprites.Add(sprite);
                }
                //json
                var jsonFilePath = jsonFolderPath + file.Name.Replace(".png", ".json");

                atlas.LstTexts = new List<TextAsset>();
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(FileUtil.GetProjectRelativePath(jsonFilePath.Replace('\\', '/')));
                if (textAsset != null)
                    atlas.LstTexts.Add(textAsset);
                else
                {
                    Debug.LogError("Not found Json File: " + file.Name);
                    continue;
                }
                CreateAtlasUtils.CreateAtlas(atlas, targetFolderPath + file.Name.Replace(".png", "") + ".asset", _cloudSettingPath);
            }
        }

        static void CreateHiveDefaultAtlas()
        {
            string targetFolderPath = _atlasFolderPath + "HiveDefaultRoom/";
            if (!Directory.Exists(targetFolderPath))
                Directory.CreateDirectory(targetFolderPath);
            var textureFolderPath = Application.dataPath + @"/_KAP/_GameResources/Textures/HiveDefaultRoom/";
            var jsonFolderPath = Application.dataPath + @"/_KAP/_GameResources/Maps/HiveDefaultRoom/";
            if (!Directory.Exists(textureFolderPath))
                return;
            if (!Directory.Exists(jsonFolderPath))
                return;
            var textureFolder = new DirectoryInfo(textureFolderPath);
            FileInfo[] allTextures = textureFolder.GetFiles("*.png", SearchOption.AllDirectories);
            foreach (var file in allTextures)
            {
                KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
                //texture
                atlas.LstSprites = new List<Sprite>();
                var objSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(FileUtil.GetProjectRelativePath(file.FullName.Replace('\\', '/')));
                foreach (var obj in objSprites)
                {
                    var sprite = (Sprite)obj;
                    if (sprite.rect.width > 512 || sprite.rect.height > 512)
                    {
                        Debug.LogError(string.Format("width or height so long: {0}", file.Name));
                        continue;
                    }
                    if (!atlas.LstSprites.Contains(sprite))
                        atlas.LstSprites.Add(sprite);
                }
                //json
                var jsonFilePath = jsonFolderPath + file.Name.Replace(".png", ".json");

                atlas.LstTexts = new List<TextAsset>();
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(FileUtil.GetProjectRelativePath(jsonFilePath.Replace('\\', '/')));
                if (textAsset != null)
                    atlas.LstTexts.Add(textAsset);
                else
                {
                    Debug.LogError("Not found Json File: " + file.Name);
                    continue;
                }
                CreateAtlasUtils.CreateAtlas(atlas, targetFolderPath + file.Name.Replace(".png", "") + ".asset", _cloudSettingPath);
            }
        }

        static void CreateSeparatedRoomAtlas()
        {
            string targetFolderPath = _atlasFolderPath + "SeparatedRooms/";
            if (!Directory.Exists(targetFolderPath))
                Directory.CreateDirectory(targetFolderPath);

            var jsonFolderPath = Application.dataPath + @"/_KAP/_GameResources/Maps/SeparatedRooms/";
            if (!Directory.Exists(jsonFolderPath))
                return;
            var jsonsFolder = new DirectoryInfo(jsonFolderPath);
            FileInfo[] allJsons = jsonsFolder.GetFiles("*.json", SearchOption.AllDirectories);
            foreach (var file in allJsons)
            {
                KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
                //json
                var jsonFilePath = jsonFolderPath + file.Name;

                atlas.LstTexts = new List<TextAsset>();
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(FileUtil.GetProjectRelativePath(jsonFilePath.Replace('\\', '/')));
                if (textAsset != null)
                    atlas.LstTexts.Add(textAsset);
                else
                {
                    Debug.LogError("Not found Json File: " + file.Name);
                    continue;
                }
                CreateAtlasUtils.CreateAtlas(atlas, targetFolderPath + file.Name.Replace(".json", "") + ".asset", _cloudSettingPath);
            }
        }
    }
}
#endif