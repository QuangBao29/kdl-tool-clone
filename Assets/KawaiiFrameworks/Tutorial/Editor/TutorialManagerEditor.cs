#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Kawaii.Tutorial
{
    [CustomEditor(typeof(TutorialManager))]
    public class TutorialManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var myScript = (TutorialManager)target;

            if (GUILayout.Button("Setup"))
            {
                myScript.ListSteps.Clear();
                var allSteps = myScript.GetComponentsInChildren<TutorialStep>(true);
                foreach (var step in allSteps)
                {
                    step.Setup();
                    myScript.ListSteps.Add(step);
                    step.gameObject.SetActive(false);
                }
                var i = 0;
                foreach(var step in myScript.ListSteps)
                {
                    if (step.AutoNext)
                    {
                        var nextStep = (i + 1) < myScript.ListSteps.Count ? myScript.ListSteps[i + 1] : null;
                        step.NextStep = nextStep != null ? nextStep.Step : myScript.NoneStepTypeKey;
                    }
                    else
                        step.NextStep = myScript.NoneStepTypeKey;
                    i++;
                }
                myScript.gameObject.SetActive(true);
            }
        }
    }
}

#endif