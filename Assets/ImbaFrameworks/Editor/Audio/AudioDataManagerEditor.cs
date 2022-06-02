
using UnityEditor;
using UnityEngine;


using Imba.Audio;

namespace Imba.Editor.Audio
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudioDataManager))]
    public class AudioDataManagerEditor : UnityEditor.Editor
    {
        

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            AudioDataManager myScript = (AudioDataManager)target;
            if (GUILayout.Button("Reload"))
            {
                myScript.Reload();
            }
        }
    }
}