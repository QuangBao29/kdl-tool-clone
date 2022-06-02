using System;
using UnityEngine;
using UnityEngine.UI;
using Imba.Utils;

namespace KAP.Tools
{
    public enum EditMode
    {
        Room = 0,
        Theme = 1,
        Wonder = 2,
        RoomChallenge,
    }

    public class ToolEditMode : ManualSingletonMono<ToolEditMode>
    {
        public event Action<EditMode> OnChangeEditMode;
        [SerializeField] private Dropdown _ddEditMode = null;

        public override void Awake()
        {
            base.Awake();
            CurrentEditMode = EditMode.Room;
        }

        private EditMode _currentEditMode;
        public EditMode CurrentEditMode
        {
            get
            {
                return (EditMode)_ddEditMode.value;
            }
            set
            {
                _currentEditMode = value;
                OnChangeEditMode?.Invoke(_currentEditMode);
            }
        }

        public void OnDDSelectMode()
        {
            CurrentEditMode = (EditMode)_ddEditMode.value;
        }
    }
}