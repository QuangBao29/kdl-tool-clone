using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Imba.Utils;
namespace Kawaii.Tutorial
{
    public class TutorialManager : ManualSingletonMono<TutorialManager>
    {
        [SerializeField]
        private TutorialOverlay _overlay = null;
        [SerializeField]
        private TutorialHand _hand = null;

        public string NoneStepTypeKey = "None";
        public string NoneBigStepTypeKey = "None";

        [Serializable]
        public class FinishBigStepEvent : UnityEvent<string> { };
        public FinishBigStepEvent OnFinishBigStep;

        public List<TutorialStep> ListSteps = null;

        public object Linker = null;

        public TutorialStep CurrentStep { get; private set; }

        public override void Awake()
        {
            base.Awake();
            _overlay.Hide(true);
            _hand.Hide();
        }

        private void FinishTutorial()
        {
            Destroy(gameObject);
        }

        // ========================================================================
        #region BigStep
        
        public TutorialStep GetStep(string stepType)
        {
            foreach (var step in ListSteps)
            {
                if (step.Step == stepType)
                    return step;
            }
            return null;
        }

        #endregion
       
        public void ShowStep(string stepType, object param = null)
        {
            var oldBigStep = NoneBigStepTypeKey;
            if (CurrentStep != null)
            {
                oldBigStep = CurrentStep.BigStep;
                CurrentStep.Hide();
            }
            TutorialStep step = GetStep(stepType);
            CurrentStep = step;
            if (step == null)
            {
                if(oldBigStep != NoneBigStepTypeKey)
                    OnFinishBigStep?.Invoke(oldBigStep.ToString());
                return;
            }
            if(oldBigStep != NoneBigStepTypeKey && step.BigStep != oldBigStep) //finish big step
            {
                OnFinishBigStep?.Invoke(oldBigStep.ToString());
            }
            step.Show(param);
        }
    }
}