using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

namespace Kawaii.Tutorial
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TutorialStep))]
    public class TutorialHandMoveSetting : TutorialStepSetting
    {
        public TutorialHand Hand;
        public RectTransform Container;
        public float EulerAngles;
        public Vector2 DeltaFromPosition;
        public Vector2 DeltaToPosition;
        public Vector3 LocalScale = Vector3.one;
        public int DelayMilisecond;
        public float Duration = 0.5f;
        public int Loops = -1;
        public Ease Ease = Ease.Linear;

        public override void Show()
        {
            Hand.ShowMove(Container, EulerAngles, DeltaFromPosition, DeltaToPosition, LocalScale, Duration, DelayMilisecond, Loops, Ease);
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

        public async void RunTest()
        {
            Hand.ShowMove(Container, EulerAngles, DeltaFromPosition, DeltaToPosition, LocalScale, Duration, DelayMilisecond, Loops, Ease);
            await Task.Delay(500);
            Hand.ShowMove(Container, EulerAngles, DeltaToPosition, DeltaFromPosition, LocalScale, Duration, DelayMilisecond, Loops, Ease);
        }

        public override void SetupInspector()
        {
            Hand = FindObjectOfType<TutorialHand>();
        }
#endif
    }

}
