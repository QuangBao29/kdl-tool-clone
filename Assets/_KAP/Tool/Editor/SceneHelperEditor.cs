#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneHelperEditor : Editor
{
    [MenuItem("Tools/SceneEditor/scene_room", false, 2)]
    static public void OpenSceneRoom()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/_KAP/Tool/Scenes/scene_create_map.unity");
    }

    [MenuItem("Tools/SceneEditor/scene_hive_group", false, 2)]
    static public void OpenSceneHiveShop()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/_KAP/Tool/Scenes/scene_create_hive_group.unity");
    }
}
#endif