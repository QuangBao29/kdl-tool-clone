using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fingers;

namespace KAP.Tools
{
    [RequireComponent(typeof(SGPanZoom))]
    public class ToolCamera : MonoBehaviour
    {
        private SGPanZoom _panZoom;

        [Header("Full View")]
        [SerializeField] private Toggle tglFullView = null;
        [SerializeField] private GameObject[] _blockPanels = null;
        private bool _isFullView;
        public bool IsFullView
        {
            get
            {
                return _isFullView;
            }
            set
            {
                _isFullView = value;
                foreach (var blockPanel in _blockPanels)
                {
                    blockPanel.SetActive(_isFullView);
                }
            }
        }

        [Header("Camera Size")]
        [SerializeField] private Slider _sldCameraSize = null;
        [SerializeField] private Text _txtCameraSizeValue = null;
        [SerializeField] private float _zoomMin = 3;
        [SerializeField] private float _zoomMax = 10;
        private float _cameraSizeValue;
        public float CameraSizeValue
        {
            get
            {
                return _cameraSizeValue;
            }
            set
            {
                _cameraSizeValue = value;
                _panZoom.SetZoom(_cameraSizeValue);
                _txtCameraSizeValue.text = _cameraSizeValue.ToString();
            }
        }

        private void Awake()
        {
            _panZoom = GetComponent<SGPanZoom>();
        }

        private void Start()
        {
            SetupCameraSize();
        }

        public void OnTglFullViewChange()
        {
            IsFullView = tglFullView.isOn;
        }

        public void OnSliderCameraSizeChange()
        {
            float value = _sldCameraSize.value;
            CameraSizeValue = value;
        }

        private void SetupCameraSize()
        {
            _panZoom.ZoomMin = _zoomMin;
            _panZoom.ZoomMax = _zoomMax;

            _sldCameraSize.minValue = _zoomMin;
            _sldCameraSize.maxValue = _zoomMax;

            float averange = (_zoomMin + _zoomMax) / 2;
            _sldCameraSize.value = averange;
            CameraSizeValue = averange;
        }
    }
}