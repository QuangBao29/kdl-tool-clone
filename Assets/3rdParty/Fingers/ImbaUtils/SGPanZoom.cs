using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DigitalRubyShared;
namespace Fingers
{
    public class SGPanZoom : MonoBehaviour
    {
        [SerializeField]
        private FingerEventSystem _fingerSystem = null;

        const float _dpiBase = 258;
        const float _panSmoothBase = 0.0075f;

        [SerializeField]
        private Camera _cam = null;
        [SerializeField]
        private List<Camera> _lstSubCam = null;
        [SerializeField]
        private ScreenBounding _moveArea = null;

        [SerializeField]
        private float _panSmooth = 0.015f;
        [SerializeField]
        private float _zoomSmooth = 3f;

        [SerializeField]
        private float _zoomOutMin = 2;
        [SerializeField]
        private float _zoomOutMax = 4;

        [SerializeField]
        private bool _autoFixZoomMax = true;

        [SerializeField]
        private bool _autoSetZoomMax = false;
        [SerializeField]
        private bool _autoSetPanSmooth = true;

        [HideInInspector]
        public bool IsLockedPan = false;

        private void Start()
        {
            if (_cam == null)
                _cam = Camera.main;

            if(_autoSetPanSmooth)
            {
                _panSmooth = _panSmoothBase * (_dpiBase / Screen.dpi);
            }

            if (_autoFixZoomMax && _moveArea != null)
            {
                var vertical = _moveArea.roomSize.y / 2;
                var horiz = _moveArea.roomSize.x * Screen.height / Screen.width * 0.5f;
                _zoomOutMax = Mathf.Min(vertical, horiz);
            }

            if (_autoSetZoomMax)
            {
                _cam.orthographicSize = _zoomOutMax;
                UpdateSubCams();
            }

            if(_fingerSystem != null)
            {
                _fingerSystem.onPan += PanGestureCallback;
                _fingerSystem.onScale += ScaleGestureCallback;
            }
        }

#if UNITY_EDITOR

        private void Update()
        {
            Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
#endif

        public void ScaleGestureCallback(GestureRecognizer gesture)
        {
#if !UNITY_EDITOR
            if (gesture.State == GestureRecognizerState.Executing)
            {
                var scaleGesture = (ScaleGestureRecognizer)gesture;
                Zoom((scaleGesture.ScaleMultiplier-1)*_zoomSmooth);
            }
#endif
        }

        void Zoom(float increment)
        {
            var orthoSize = Mathf.Clamp(_cam.orthographicSize - increment, _zoomOutMin, _zoomOutMax);
            _cam.orthographicSize = orthoSize;
            ConstrainToMoveArea(_cam);
            UpdateSubCams();
        }

        public void PanGestureCallback(GestureRecognizer gesture)
        {
            if (IsLockedPan)
                return;
            switch (gesture.State)
            {
                case GestureRecognizerState.Executing:
                    var panGesture = (PanGestureRecognizer)gesture;
                    float deltaX = panGesture.DeltaX * _panSmooth;
                    float deltaY = panGesture.DeltaY * _panSmooth;
                    var pos = _cam.transform.position;
                    pos.x -= deltaX;
                    pos.y -= deltaY;
                    _cam.transform.position = pos;
                    ConstrainToMoveArea(_cam);
                    UpdateSubCams();
                    break;
            }
        }

        void ConstrainToMoveArea(Camera cam)
        {
            if (_moveArea)
            {
                Bounds camBound = GetOrthographicCameraBounds(cam);
                Vector3 min = transform.InverseTransformDirection(_moveArea.Min) + camBound.extents;
                Vector3 max = transform.InverseTransformDirection(_moveArea.Max) - camBound.extents;
                Vector3 p = cam.transform.position;
                if(min.x < max.x)
                    p.x = Mathf.Clamp(p.x, min.x, max.x);
                else
                    p.x = Mathf.Clamp(p.x, max.x, min.x);
                if(min.y < max.y)
                    p.y = Mathf.Clamp(p.y, min.y, max.y);
                else
                    p.y = Mathf.Clamp(p.y, max.y, min.y);
                p.z = 0;
                cam.transform.position = p;
            }
        }

        public void SetZoom(float value)
        {
            _cam.orthographicSize = value;
            UpdateSubCams();
        }

        public void ZoomSmooth(float from, float to, float time, System.Action cb)
        {
            DOTween.To((pNewValue) => {
                _cam.orthographicSize = pNewValue;
                UpdateSubCams();
            }, from, to, time)
                   .SetEase(Ease.Linear)
                   .OnComplete(() => {
                       if (cb != null)
                           cb();
                   });
        }

        public void FlyTo(Vector3 targetWorld, bool isSmooth = false, float offset = 0)
        {
            if (offset > 0)
            {
                var delta = Vector3.Distance(targetWorld, _cam.transform.localPosition);
                if (delta < offset)
                    return;
            }
            if (!isSmooth)
            {
                _cam.transform.position = targetWorld;
                ConstrainToMoveArea(_cam);
            }
            else
            {
                _cam.transform.DOMove(targetWorld, 0.5f);
            }

            UpdateSubCams();
        }

        void UpdateSubCams()
        {
            if (_lstSubCam == null)
                return;
            var orthographicSize = _cam.orthographicSize;
            var pos = _cam.transform.position;
            foreach (var c in _lstSubCam)
            {
                c.orthographicSize = orthographicSize;
                c.transform.position = pos;
                ConstrainToMoveArea(c);
            }
        }

        public float ZoomMax
        {
            get
            {
                return _zoomOutMax;
            }
            set
            {
                _zoomOutMax = value;
            }
        }

        public float ZoomMin
        {
            get
            {
                return _zoomOutMin;
            }
            set
            {
                _zoomOutMin = value;
            }
        }

        public static Bounds GetOrthographicCameraBounds(Camera camera)
        {
            float screenAspect = Screen.width * 1.0f / (float)Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            var viewportRect = camera.rect;
            Bounds bounds = new Bounds(camera.transform.localPosition, new Vector3(cameraHeight * screenAspect * viewportRect.width, cameraHeight * viewportRect.height, 0));
            return bounds;
        }
    }
}

