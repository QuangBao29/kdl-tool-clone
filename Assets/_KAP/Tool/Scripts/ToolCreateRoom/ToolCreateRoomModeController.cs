using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Imba.Utils;

public enum ToolRoomMode
{
    CreateMap = 0,
    Camera = 1,
}

namespace KAP.ToolCreateMap
{
    public class ToolCreateRoomModeController : ManualSingletonMono<ToolCreateRoomModeController>
    {
        [SerializeField] private ToolCreateRoomPreviewController _previewController = null;
        [SerializeField] private Dropdown _drdSelectMode = null;

        [Space]
        [SerializeField] private SGToggle _sgTglCreateRoom = null;
        [SerializeField] private SGToggle _sgTglCamera = null;

        [Space]
        [SerializeField] private SGToggle _sgTglUnPreview = null;
        [SerializeField] private SGToggle _sgTglPreview = null;

        [SerializeField] private InputField _inputGroup = null;

        private ToolRoomMode _mode;
        public ToolRoomMode Mode
        {
            get
            {
                return _mode;
            }
            private set
            {
                _mode = value;
                switch (_mode)
                {
                    case ToolRoomMode.CreateMap:
                        _sgTglCreateRoom.Show();
                        break;
                    case ToolRoomMode.Camera:
                        _sgTglCamera.Show();
                        break;
                    default:
                        break;
                }
            }
        }

        private bool _isPreview;
        public bool IsPreview
        {
            get
            {
                return _isPreview;
            }
            private set
            {
                _isPreview = value;
                if (_isPreview)
                {
                    _sgTglPreview.Show();
                    _previewController.SetUpReviewShadow(_inputGroup);
                }
                else
                {
                    _sgTglUnPreview.Show();
                    _previewController.SetupUnPreview();
                }
            }
        }

        private void Start()
        {
            Mode = 0;
            OnUnPreviewClick();
        }

        public void OnDropdownSelectModeChange()
        {
            Mode = (ToolRoomMode)_drdSelectMode.value;
        }

        public void OnPreviewClick()
        {
            IsPreview = true;
        }

        public void OnUnPreviewClick()
        {
            IsPreview = false;
        }
    }
}