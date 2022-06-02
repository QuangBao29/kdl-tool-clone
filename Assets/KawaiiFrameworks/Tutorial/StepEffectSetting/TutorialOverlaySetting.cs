using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kawaii.Tutorial
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TutorialStep))]
    public class TutorialOverlaySetting : TutorialStepSetting
    {
        public TutorialOverlay Overlay;
        [Header("Show")]
        public int Alpha = 168;
        public float Delay = 0;
        public bool RaycastTarget = true;
        public bool Instant = false;
        [Header("Hide")]
        public bool IsHide = false;
        public bool HideInstant = true;


        public override void Show()
        {
            Overlay.Show(Alpha/255f, Delay, RaycastTarget, Instant);
        }

        public override void Hide()
        {
            if(IsHide)
            Overlay.Hide(HideInstant);
        }


#if UNITY_EDITOR
        private void Awake()
        {
            if (Overlay == null)
                Overlay = FindObjectOfType<TutorialOverlay>();
        }

        public override void SetupInspector()
        {
            base.SetupInspector();
            Overlay = FindObjectOfType<TutorialOverlay>();
        }
#endif
    }

}
