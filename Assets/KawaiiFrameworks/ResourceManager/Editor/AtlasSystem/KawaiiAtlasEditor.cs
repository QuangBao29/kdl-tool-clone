#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
namespace Kawaii.ResourceManager
{
    [CustomEditor(typeof(KawaiiAtlas))]
    public class ImbaAtlasEditor : UnityEditor.Editor
    {
        [MenuItem("Imba/Create A Imba Atlas")]
        static void CreateImbaAtlas()
        {
            string path = EditorUtility.SaveFilePanel("Create ImbaAtals", "Assets/", "default_atlas.asset", "asset");
            if (path == "")
                return;
            path = FileUtil.GetProjectRelativePath(path);
            KawaiiAtlas atlas = CreateInstance<KawaiiAtlas>();
            Debug.LogError(path);
            AssetDatabase.CreateAsset(atlas, path);
            AssetDatabase.SaveAssets();
        }

        //public static void AtlasUpdate(ImbaAtlas myScript)
        //{
        //    if (myScript.texturePaths != null)
        //    {
        //        myScript.LstSprites.Clear();
        //        foreach (var path in myScript.texturePaths)
        //        {
        //            var objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        //            foreach (var iter in objs)
        //            {
        //                var spr = (Sprite)iter;
        //                if (!myScript.LstSprites.Contains(spr))
        //                {
        //                    myScript.LstSprites.Add(spr);
        //                }
        //            }
        //            //Debug.LogError(path + ":" + myScript.sprites.Count);
        //        }
        //    }

        //    if (myScript.textFilePaths != null)
        //    {
        //        myScript.LstTexts.Clear();
        //        foreach (var path in myScript.textFilePaths)
        //        {
        //            DirectoryInfo d = new DirectoryInfo(path);
        //            FileInfo[] Files = d.GetFiles("*.csv", SearchOption.AllDirectories);

        //            foreach (FileInfo file in Files)
        //            {
        //                //if (file.Name == "ConfigCreationFeature.csv" || file.Name == "ConfigCreationBackup.csv"
        //                //|| file.Name == "ConfigChallengeBot.csv" || file.Name == "ConfigDecoRepairId.csv" || file.Name == "ConfigDecoType.csv"
        //                //|| file.Name == "ConfigChallengeStatic.csv")
        //                //continue;
        //                var fullPath = "";
        //                var subs = file.DirectoryName.Split('/');
        //                bool isStartReadPath = false;
        //                foreach (var folder in subs)
        //                {
        //                    if (folder == "Assets")
        //                    {
        //                        isStartReadPath = true;
        //                    }
        //                    if (isStartReadPath)
        //                        fullPath += folder + "/";
        //                }
        //                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(fullPath + file.Name);
        //                if (textAsset != null)
        //                    myScript.LstTexts.Add(textAsset);
        //            }
        //        }
        //    }


        //    //var decoAnimPath = Application.dataPath + @"/_KDH/AssetBundle/DecorAnim/" + myScript.name;
        //    //if (Directory.Exists(decoAnimPath))
        //    //{
        //    //    myScript.prefabs = new List<GameObject>();
        //    //    DirectoryInfo dDecoAnim = new DirectoryInfo(decoAnimPath);
        //    //    if (dDecoAnim != null)
        //    //    {
        //    //        FileInfo[] Files = dDecoAnim.GetFiles("*.prefab", SearchOption.AllDirectories);
        //    //        var fullPath = "Assets/_KDH/AssetBundle/DecorAnim/" + myScript.name + "/";
        //    //        //Debug.LogError(Files.Length);
        //    //        foreach (FileInfo file in Files)
        //    //        {
        //    //            //Debug.LogError(fullPath + file.Name);
        //    //            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath + file.Name);
        //    //            if (prefab != null)
        //    //            {
        //    //                var comp = prefab.GetComponent<DecoAnim>();
        //    //                if (comp != null && comp.transFront != null)
        //    //                    myScript.prefabs.Add(prefab);
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    //var fashionAnimPath = Application.dataPath + @"/_KDH/AssetBundle/FashionAnim/" + myScript.name;
        //    //if (Directory.Exists(fashionAnimPath))
        //    //{
        //    //    myScript.prefabs = new List<GameObject>();
        //    //    DirectoryInfo dFashionAnim = new DirectoryInfo(fashionAnimPath);
        //    //    if (dFashionAnim != null)
        //    //    {
        //    //        var fullPath = "Assets/_KDH/AssetBundle/FashionAnim/" + myScript.name + "/";
        //    //        var subDirectories = dFashionAnim.GetDirectories();
        //    //        foreach (var subDir in subDirectories)
        //    //        {
        //    //            var dirName = subDir.Name;
        //    //            Object[] allSpr = AssetDatabase.LoadAllAssetRepresentationsAtPath(fullPath + dirName + "/" + dirName + ".png");
        //    //            if (allSpr == null)
        //    //            {
        //    //                Debug.LogError("Not have sprites " + dirName);
        //    //            }
        //    //            else
        //    //            {
        //    //                foreach (var iter in allSpr)
        //    //                {
        //    //                    var spr = (Sprite)iter;
        //    //                    if (!myScript.sprites.Contains(spr))
        //    //                        myScript.sprites.Add(spr);
        //    //                }
        //    //            }
        //    //            FileInfo[] Files = subDir.GetFiles("*.prefab", SearchOption.AllDirectories);

        //    //            foreach (FileInfo file in Files)
        //    //            {
        //    //                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath + dirName + "/" + file.Name); ;

        //    //                if (prefab != null)
        //    //                {
        //    //                    var comp = prefab.GetComponent<FashionByType>();
        //    //                    if (comp != null && comp.canUse)
        //    //                    {
        //    //                        myScript.prefabs.Add(prefab);
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}

        //public override void OnInspectorGUI()
        //{
        //    var myScript = (ImbaAtlas)target;
        //    DrawDefaultInspector();
        //    EditorGUILayout.Space();
        //    DropAreaGUI();
        //    if (GUILayout.Button("Update"))
        //    {
        //        AtlasUpdate(myScript);
        //    }
        //}

        //public void DropAreaGUI()
        //{
        //    Event evt = Event.current;
        //    Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        //    GUI.Box(drop_area, "Add Resource");

        //    switch (evt.type)
        //    {
        //        case EventType.DragUpdated:
        //        case EventType.DragPerform:
        //            if (!drop_area.Contains(evt.mousePosition))
        //                return;

        //            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        //            if (evt.type == EventType.DragPerform)
        //            {
        //                DragAndDrop.AcceptDrag();
        //                var myScript = (ImbaAtlas)target;
        //                foreach (Object dragged_object in DragAndDrop.objectReferences)
        //                {
        //                    var type = dragged_object.GetType();

        //                    if (type == typeof(Texture2D))
        //                    {
        //                        var path = AssetDatabase.GetAssetPath(dragged_object);
        //                        if (myScript.LstSprites == null)
        //                            myScript.LstSprites = new List<Sprite>();

        //                        var objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        //                        foreach (var iter in objs)
        //                        {
        //                            var spr = (Sprite)iter;
        //                            if (!myScript.LstSprites.Contains(spr))
        //                                myScript.LstSprites.Add(spr);
        //                        }
        //                        if (myScript.texturePaths == null)
        //                            myScript.texturePaths = new List<string>();
        //                        if (!myScript.texturePaths.Contains(path))
        //                            myScript.texturePaths.Add(path);

        //                        Debug.LogError(path + ":" + myScript.LstSprites.Count);
        //                    }
        //                    else
        //                    {
        //                        var path = AssetDatabase.GetAssetPath(dragged_object);
        //                        myScript.textFilePaths.Add(path);
        //                    }
        //                }
        //            }
        //            break;
        //    }
        //}
    }
}
#endif

