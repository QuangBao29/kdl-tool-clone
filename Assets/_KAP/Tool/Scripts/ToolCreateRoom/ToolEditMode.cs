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
        RoomChallenge,
        Home = 3,
        Play = 4
    }
    //public enum EditModeKDL
    //{
    //    Home = 0,
    //    Play = 1,
    //}
    public class ToolEditMode : ManualSingletonMono<ToolEditMode>
    {
        public event Action<EditMode> OnChangeEditMode;
        [SerializeField] private Dropdown _ddEditMode = null;
        [SerializeField] private GameObject _panelUnpack = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;

        public override void Awake()
        {
            base.Awake();
            CurrentEditMode = EditMode.Home;
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
            if (CurrentEditMode == EditMode.Play)
                _panelUnpack.SetActive(true);
            else _panelUnpack.SetActive(false);
            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                foreach (var item in root.Value)
                    item.UnActiveImgCheck();
            }
            
        }
    }
}