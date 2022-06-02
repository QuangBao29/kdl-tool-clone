using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Imba.Utils;

namespace Kawaii.Tutorial
{
    public class TutorialStep : MonoBehaviour
    {
        public TutorialManager Manager;
        public string BigStep;
        public string Step;
        public string NextStep;
        public bool AutoNext = true;

        public SGToggle SGToggle = null;
        public List<TutorialStepSetting> ListSettings = null;
        [Serializable]
        public class StepEventCallback : UnityEvent<object> { };

        public StepEventCallback OnShowing;
        public StepEventCallback OnShown;
        public UnityEvent OnHiding;
        public UnityEvent OnHidden;

        public virtual void Show(object param = null)
        {
            OnShowing?.Invoke(param);
            SGToggle.Show();
            if(ListSettings != null)
                foreach (var setting in ListSettings)
                    setting.Show();
            OnShown?.Invoke(param);
        }

        public virtual void Hide()
        {
            OnHiding?.Invoke();
            if (ListSettings != null)
                foreach (var setting in ListSettings)
                    setting.Hide();
            SGToggle.Visible = false;
            OnHidden?.Invoke();
        }

        public virtual void Next(object param = null)
        {
            Manager.ShowStep(NextStep, param);
        }

        //#region First Room Event
       
        

       

        //#endregion

        public void Setup()
        {
            Manager = FindObjectOfType<TutorialManager>();
            BigStep = transform.parent.name;
            Step = gameObject.name;
            SGToggle = gameObject.GetComponent<SGToggle>();
            ListSettings = new List<TutorialStepSetting>(gameObject.GetComponents<TutorialStepSetting>());
        }
    }
}