#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Kawaii.Tutorial
{
    [CustomEditor(typeof(TutorialHandTouchSetting))]
    public class TutorialHandTouchSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var myscript = (TutorialHandTouchSetting)target;

            if(GUILayout.Button("Run Test"))
            {
                myscript.Show();
            }
        }
    }
}


#endif