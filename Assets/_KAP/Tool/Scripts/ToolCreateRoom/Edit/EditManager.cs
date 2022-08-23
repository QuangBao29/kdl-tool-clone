using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KAP.ToolCreateMap;

namespace KAP
{
    public class EditManager :MonoBehaviour
    {
        public string EditingTag;
        public DecoEditTool editTool;
        public DecoEditDemo Current { get; protected set; }
        [SerializeField] private GameObject _panelBubbleDecoSetting = null;
        public UnityEvent OnChangeCurrentEvent;

        public bool SetCurrent(DecoEditDemo decoEdit)
        {
            if (!CanSetCurrent())
                return false;

            if (Current == decoEdit)
                return true;

            if (Current != null && Current.EditStatus != KHHEditStatus.Valid && decoEdit != null)
                return false;
            Current = decoEdit;
            if (Current != null)
            {
                editTool.Show();
                _panelBubbleDecoSetting.SetActive(true);
            }
            else
            {
                _panelBubbleDecoSetting.SetActive(false);
                editTool.gameObject.SetActive(false);
            }
            OnChangeCurrentEvent?.Invoke();
            return true;
        }

        private bool CanSetCurrent()
        {
            if(ToolCreateRoomModeController.IsInstanceValid())
            {
                var modeController = ToolCreateRoomModeController.Instance;
                if (modeController.IsPreview
                    || modeController.Mode == ToolRoomMode.Camera)
                    return false;
            }
            
            return true;
        }
    }
}
