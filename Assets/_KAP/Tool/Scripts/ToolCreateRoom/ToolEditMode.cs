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
        Test = 5,
        DecoReward = 6
    }
    public enum PhaseMode
    {
        All = 0,
        StaticDeco = 1,
        Bubble = 2,
        Unpacking = 3,
    }
    public class ToolEditMode : ManualSingletonMono<ToolEditMode>
    {
        public event Action<EditMode> OnChangeEditMode;
        [SerializeField] private Dropdown _ddEditMode = null;
        [SerializeField] private Dropdown _ddPhaseMode = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField] private ToolCreateMapPhaseController _toolPhaseController = null;

        public override void Awake()
        {
            base.Awake();
            CurrentEditMode = EditMode.Home;
            CurrentPhaseMode = PhaseMode.All;
            _toolPhaseController.OnChangeModeUI();
        }

        private EditMode _currentEditMode;
        private PhaseMode _currentPhaseMode;
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
        public PhaseMode CurrentPhaseMode
        {
            get
            {
                return KDLUtils.ParseEnum<PhaseMode>(_ddPhaseMode.captionText.text);
            }
            set
            {
                _currentPhaseMode = value;
            }
        }
        public void OnDDSelectMode()
        {
            CurrentEditMode = KDLUtils.ParseEnum<EditMode>(_ddEditMode.captionText.text);
            _toolPhaseController.OnChangeModeUI();
        }
        public void OnDDSelectPhaseMode()
        {
            CurrentPhaseMode = KDLUtils.ParseEnum<PhaseMode>(_ddPhaseMode.captionText.text);
            _toolPhaseController.OnChangePhaseUI();
        }
    }
}