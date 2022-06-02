using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Kawaii.Tutorial
{
    [RequireComponent(typeof(TutorialStep))]
    public class TutorialAutoNextSetting : TutorialStepSetting
    {
        [SerializeField]
        public TutorialStep _tutStep = null;
        [SerializeField]
        public int Milisecond = 0;

        public async override void Show()
        {
            if (Milisecond <= 0)
                return;
            await Task.Delay(Milisecond);
            if(gameObject.activeSelf)
                _tutStep.Next();
        }
#if UNITY_EDITOR
        public override void SetupInspector()
        {
            base.SetupInspector();
            _tutStep = GetComponent<TutorialStep>();
        }
#endif

    }

}
