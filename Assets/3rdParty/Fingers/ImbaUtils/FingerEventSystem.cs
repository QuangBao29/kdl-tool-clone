using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DigitalRubyShared;
namespace Fingers
{
    public class FingerEventSystem : MonoBehaviour
    {
        private TapGestureRecognizer tapGesture;
        private TapGestureRecognizer doubleTapGesture;
        private PanGestureRecognizer panGesture;
        private ScaleGestureRecognizer scaleGesture;
        private LongPressGestureRecognizer longPressGesture;
        private SwipeGestureRecognizer swipeGesture;
        private RotateGestureRecognizer rotateGesture;

        public Action<GestureRecognizer> onTap;
        public Action<GestureRecognizer> onDoubleTap;
        public Action<GestureRecognizer> onPan;
        public Action<GestureRecognizer> onScale;
        public Action<GestureRecognizer> onLongPress;
        public Action<GestureRecognizer> onSwipe;
        public Action<GestureRecognizer> onRotate;

        public bool tap = true;
        public bool doubleTap;
        public bool pan = true;
        public bool scale = true;
        public bool longPress;
        public bool swipe;
        public bool rotate;

        public bool showTouch = false;

        private void Start()
        {
            FingersScript.Instance.ShowTouches = showTouch;
            if(tap)
            {
                tapGesture = new TapGestureRecognizer();
                //tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
                tapGesture.StateUpdated += TapGestureCallback;
                FingersScript.Instance.AddGesture(tapGesture);
            }

            if(doubleTap)
            {
                doubleTapGesture = new TapGestureRecognizer();
                doubleTapGesture.MinimumNumberOfTouchesToTrack = 2;
                doubleTapGesture.StateUpdated += DoubleTapGestureCallback;
                FingersScript.Instance.AddGesture(doubleTapGesture);
            }

            if(pan)
            {
                panGesture = new PanGestureRecognizer();
                panGesture.ThresholdUnits = 0.05f;
                //panGesture.MinimumNumberOfTouchesToTrack = 2;
                panGesture.StateUpdated += PanGestureCallback;
                FingersScript.Instance.AddGesture(panGesture);
            }

            if(scale)
            {
                scaleGesture = new ScaleGestureRecognizer();
                scaleGesture.StateUpdated += ScaleGestureCallback;
                FingersScript.Instance.AddGesture(scaleGesture);
            }

            if(longPress)
            {
                longPressGesture = new LongPressGestureRecognizer();
                longPressGesture.StateUpdated += LongPressGestureCallback;
                FingersScript.Instance.AddGesture(longPressGesture);
            }

            if(swipe)
            {
                swipeGesture = new SwipeGestureRecognizer();
                swipeGesture.StateUpdated += SwipeGestureCallback;
                FingersScript.Instance.AddGesture(swipeGesture);
            }

            if(rotate)
            {
                rotateGesture = new RotateGestureRecognizer();
                rotateGesture.StateUpdated += RotateGestureCallback;
                FingersScript.Instance.AddGesture(rotateGesture);
            }
        }

        #region Callback
        void TapGestureCallback(GestureRecognizer gesture)
        {
            onTap?.Invoke(gesture);
        }

        void DoubleTapGestureCallback(GestureRecognizer gesture)
        {
            onDoubleTap?.Invoke(gesture);
        }

        void PanGestureCallback(GestureRecognizer gesture)
        {
            onPan?.Invoke(gesture);
        }

        void ScaleGestureCallback(GestureRecognizer gesture)
        {
            onScale?.Invoke(gesture);
        }

        void LongPressGestureCallback(GestureRecognizer gesture)
        {
            onLongPress?.Invoke(gesture);
        }

        void SwipeGestureCallback(GestureRecognizer gesture)
        {
            onSwipe?.Invoke(gesture);
        }

        void RotateGestureCallback(GestureRecognizer gesture)
        {
            onRotate?.Invoke(gesture);
        }


        #endregion

        public static Vector3 GetWorldPos(Vector2 screenPos)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            float t = -ray.origin.z / ray.direction.z;

            return ray.GetPoint(t);
        }

        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

    }
}
