using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
namespace Fingers
{
    public class FingerEventObjectTrigger : MonoBehaviour
    {
        [Serializable]
        public class FingerEventCallback : UnityEvent<GestureRecognizer> { };
        public FingerEventCallback onTap;
        public FingerEventCallback onDoubleTap;
        public FingerEventCallback onPan;
        public FingerEventCallback onScale;
        public FingerEventCallback onLongPress;
        public FingerEventCallback onSwipe;
        public FingerEventCallback onRotate;

        private void Start()
        {

        }

        //public void PlayPunchClick()
        //{
        //    if (isLockPunch || punchTrans == null)
        //        return;
        //    isLockPunch = true;
        //    punchOriginScale = punchTrans.localScale;
        //    punchTrans.DOPunchScale(punchParam, 0.5f, 1).SetEase(Ease.Linear).OnComplete(() => {
        //        punchTrans.localScale = punchOriginScale;
        //        isLockPunch = false;
        //    });
        //}
    }
}
