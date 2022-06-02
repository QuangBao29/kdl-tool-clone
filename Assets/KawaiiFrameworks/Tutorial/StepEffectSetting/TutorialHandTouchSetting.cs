using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.Tutorial
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TutorialStep))]
    public class TutorialHandTouchSetting : TutorialStepSetting
    {
        public TutorialHand Hand;
        public RectTransform Container;
        public float EulerAngles;
        public Vector2 DeltaPosition;
        public Vector3 LocalScale = Vector3.one;
        public int DelayMilisecond;

        public override void Show()
        {
            Hand.ShowTouch(Container, EulerAngles, DeltaPosition, LocalScale, DelayMilisecond);
        }

        public override void Hide()
        {
            Hand.Hide();
        }

#if UNITY_EDITOR
        private void Awake()
        {
            if (Hand == null)
                Hand = FindObjectOfType<TutorialHand>();
        }

        public override void SetupInspector()
        {
            Hand = FindObjectOfType<TutorialHand>();
        }
#endif
    }
}
