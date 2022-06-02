
using System.Collections.Generic;

using Imba.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ImbaEditorHelper : Editor
{

    /*[MenuItem("Imba/TestExtension")]
    static void TestExtension()
    {
        GameObject[] rootObjects =  SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject g in rootObjects)
        {
            Debug.Log("Child GO count of " + g.name + "=" + g.GetAllChilds().Count);
            Debug.Log("Child Trans count of " + g.name + "=" + g.transform.GetAllChilds().Count);
        }
    }*/
    
    [MenuItem("Imba/FindMissingScriptsInScene")]
    static void SelectMissingScript()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();
        List<Object> objectWithDeadLink = new List<Object>();

        foreach (GameObject g in rootObjects)
        {
            CheckRecursive(objectWithDeadLink, g.transform);
        }

        if (objectWithDeadLink.Count > 0)
        {
            Debug.LogError("Num of missing object " + objectWithDeadLink.Count);
            Selection.objects = objectWithDeadLink.ToArray();
        }
        else
        {
            Debug.Log("No GameObjects in '" + currentScene.name + "' have missing script");
        }
    }

    [MenuItem("Imba/RemoveMissingScriptsInScene")]
    static void RemoveMissingScript()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();
        List<Object> objectWithDeadLink = new List<Object>();

        foreach (GameObject g in rootObjects)
        {
            CheckRecursive(objectWithDeadLink, g.transform, true);
        }

        if (objectWithDeadLink.Count > 0)
        {
            Debug.LogError("Num of missing object " + objectWithDeadLink.Count);
        }
        else
        {
            Debug.Log("No GameObjects in '" + currentScene.name + "' have missing script");
        }
    }
    
    static void CheckRecursive(List<Object> objectWithDeadLink, Transform rootObject, bool remove = false)
    {
        Component[] components = rootObject.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            Component currentComponent = components[i];
            if (currentComponent == null)
            {
                objectWithDeadLink.Add(rootObject.gameObject);

                if (remove)
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(rootObject.gameObject);
                    //GameObject.DestroyImmediate(components[i]);
                    Debug.LogWarning("Remove missing obj in " + rootObject);
                }
                else
                {
                    //Selection.activeGameObject = rootObject.gameObject;
                    Debug.LogWarning(rootObject + "has a missing script");
                    break;
                }
              
            }
            else
            {
                //Debug.Log("Checked " + currentComponent.name);
                
            }
        }
        
        List<Transform> childs = rootObject.GetAllChilds();
        foreach (var c in childs)
        {
            CheckRecursive(objectWithDeadLink, c, remove);
        }
      
        
    }


    [MenuItem("Imba/ClearData")]
    static void ClearData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Cleared data");
    }
}

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Imba/FindMissingScriptsInPrefab")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FindMissingScripts));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Script in selected prefabs"))
        {
            FindInSelected();
        }
    }

    private static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        int go_count = 0, components_count = 0, missing_count = 0;
        foreach (GameObject g in go)
        {
            go_count++;
            Component[] components = g.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        var parent = t.parent;
                        s = parent.name + "/" + s;
                        t = parent;
                    }

                    Debug.Log(s + " has an empty script attached in position: " + i, g);
                }
            }
        }

        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count,
            components_count, missing_count));
    }
}