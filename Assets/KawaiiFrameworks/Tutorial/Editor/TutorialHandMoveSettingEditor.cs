#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Kawaii.Tutorial
{
    [CustomEditor(typeof(TutorialHandMoveSetting))]
    public class TutorialHandMoveSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var myscript = (TutorialHandMoveSetting)target;

            if (GUILayout.Button("Run Test"))
            {
                myscript.RunTest();
            }
        }
    }
}
#endif
