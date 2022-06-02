using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;
using UnityEngine.Rendering;
namespace Fingers
{
    //[RequireComponent(typeof(FingerEventSystem))]
    public class FingerEventRaycaster2D : MonoBehaviour
    {
        [SerializeField]
        private FingerEventSystem _fingerSystem = null;
        public LayerMask IgnoreLayerMask;

        private FingerEventObjectTrigger _dragSelection;
        private FingerEventObjectTrigger _longPressSelection;

        protected void Start()
        {
            if (_fingerSystem == null)
                return;
            if(_fingerSystem.tap)
            {
                _fingerSystem.onTap += TapGestureCallback;
            }
            if(_fingerSystem.doubleTap)
            {
                _fingerSystem.onDoubleTap += DoubleTapGestureCallback;
            }
            if(_fingerSystem.pan)
            {
                _fingerSystem.onPan += PanGestureCallback;
            }
            if(_fingerSystem.longPress)
            {
                _fingerSystem.onLongPress += LongPressGestureCallback;
            }
        }

        void TapGestureCallback(GestureRecognizer gesture)
        {
            switch(gesture.State)
            {
                case GestureRecognizerState.Ended:
                    var trigger = Raycast(gesture.FocusX, gesture.FocusY);
                    if (trigger != null)
                    {
                        trigger.onTap?.Invoke(gesture);
                    }
                    break;
            }
        }

        void DoubleTapGestureCallback(GestureRecognizer gesture)
        {
            switch(gesture.State)
            {
                case GestureRecognizerState.Ended:
                    var trigger = Raycast(gesture.FocusX, gesture.FocusY);
                    if (trigger != null)
                        trigger.onDoubleTap?.Invoke(gesture);
                    break;
            }
        }

        void LongPressGestureCallback(GestureRecognizer gesture)
        {
            switch(gesture.State)
            {
                case GestureRecognizerState.Began:
                    var trigger = Raycast(gesture.FocusX, gesture.FocusY);
                    if (trigger != null)
                    {
                        _longPressSelection = trigger;
                        trigger.onLongPress?.Invoke(gesture);
                    }
                    break;
                case GestureRecognizerState.Executing:
                    break;
                case GestureRecognizerState.Ended:
                    var trigger2 = Raycast(gesture.FocusX, gesture.FocusY);
                    if (trigger2 != null && trigger2 == _longPressSelection)
                    {
                        trigger2.onLongPress?.Invoke(gesture);
                    }
                    break;
            }
        }

        void PanGestureCallback(GestureRecognizer gesture)
        {
            switch (gesture.State)
            {
                case GestureRecognizerState.Began:
                    _dragSelection = Raycast(gesture.FocusX, gesture.FocusY);
                    if (_dragSelection != null)
                        _dragSelection.onPan?.Invoke(gesture);
                    break;
                case GestureRecognizerState.Executing:
                    if (_dragSelection != null)
                        _dragSelection.onPan?.Invoke(gesture);
                    break;
                case GestureRecognizerState.Ended:
                    if (_dragSelection != null)
                        _dragSelection.onPan?.Invoke(gesture);
                    _dragSelection = null;
                    break;
            }
        }


        protected virtual FingerEventObjectTrigger Raycast(float screenX, float screenY)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, 0));
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero, Mathf.Infinity, ~IgnoreLayerMask);
            FingerEventObjectTrigger nearest = null;
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {

                    var trans = hit.transform;
                    if (trans == null)
                        continue;
                    var fingerTrigger = trans.GetComponent<FingerEventObjectTrigger>();
                    if (fingerTrigger == null)
                        continue;
                    if (nearest == null)
                    {
                        nearest = fingerTrigger;
                        continue;
                    }

                    var curLayer = trans.gameObject.layer;
                    var nearestLayer = nearest.gameObject.layer;
                    if (curLayer != nearestLayer)
                    {
                        if (curLayer > nearestLayer)
                        {
                            nearest = fingerTrigger;
                        }
                        continue;
                    }
                }
            }
                   
            return nearest;
        }
    }

}
