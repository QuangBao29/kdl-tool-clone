using System;
using UnityEngine;
using UnityEngine.UI;
using Imba.Utils;
using KAP.ToolCreateMap;

namespace KAP.Tools
{
    public enum EditMode
    {
        Room = 0,
        Theme = 1,
        Wonder = 2,
        RoomChallenge = 6,
        Home = 3,
        Play = 4,
        SeparatedRoom = 5,
        OldRoom = 7,
        Event = 8,
        PoolDeco = 9
    }

    public class ToolEditMode : ManualSingletonMono<ToolEditMode>
    {
        public event Action<EditMode> OnChangeEditMode;
        [SerializeField] private Dropdown _ddEditMode = null;
        [SerializeField] private Dropdown _ddPhaseMode = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField] private ToolCreateMapDecoSetting _toolMapDecoSetting = null;

        public override void Awake()
        {
            base.Awake();
            CurrentEditMode = EditMode.Home;
            OnChangeEditMode += OnShowBaseGemInput;
        }

        private EditMode _currentEditMode;
        public EditMode CurrentEditMode
        {
            get
            {
                return KDLUtils.ParseEnum<EditMode>(_ddEditMode.captionText.text);
            }
            set
            {
                _currentEditMode = value;
                OnChangeEditMode?.Invoke(_currentEditMode);
            }
        }
        public void OnDDSelectMode()
        {
            CurrentEditMode = KDLUtils.ParseEnum<EditMode>(_ddEditMode.captionText.text);
            //_toolPhaseController.OnChangeModeUI();
        }

        private void OnShowBaseGemInput(EditMode mode)
        {
            if (mode == EditMode.Play || mode == EditMode.Event)
                _toolBubbleDecoSetting.BaseGem.gameObject.SetActive(true);
            else _toolBubbleDecoSetting.BaseGem.gameObject.SetActive(false);
            if (mode == EditMode.Event)
            {
                _toolMapDecoSetting.GetToggleIsBubble().gameObject.SetActive(false);
                _toolMapDecoSetting.GetToggleIsStatic().gameObject.SetActive(true);
            }
        }
    }
}