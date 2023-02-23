using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KAP.Tools;

namespace KAP.ToolCreateMap
{
    
    public class ToolCreateMapPhaseController : MonoBehaviour
    {
        [SerializeField]
        private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField]
        private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField]
        private ToolCreateMapUnpackingSetting _toolUnpackingSetting = null;

        //private void Awake()
        //{
        //    //_toolBubbleSetting.ShowPanelListBubble();
        //    //_toolUnpackingSetting.ShowPanelUnpacking();
        //}
        public void OnChangePhaseUI()
        {
            switch (ToolEditMode.Instance.CurrentPhaseMode)
            {
                case PhaseMode.All:
                    {
                        _toolBubbleSetting.ShowPanelListBubble();
                        if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                            _toolUnpackingSetting.ShowPanelUnpacking();
                        else _toolUnpackingSetting.HidePanelUnpacking();
                        break;
                    }
                case PhaseMode.StaticDeco:
                    {
                        _toolBubbleSetting.HidePanelListBubble();
                        _toolUnpackingSetting.HidePanelUnpacking();

                        break;
                    }
                case PhaseMode.Bubble:
                    {
                        _toolBubbleSetting.ShowPanelListBubble();
                        _toolUnpackingSetting.HidePanelUnpacking();

                        break;
                    }
                case PhaseMode.Unpacking:
                    {
                        if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
                            _toolUnpackingSetting.ShowPanelUnpacking();
                        _toolBubbleSetting.HidePanelListBubble();
                        break;
                    }
                default:
                    break;
            }
        }
        public void OnChangeModeUI()
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
            {
                _toolUnpackingSetting.ShowPanelUnpacking();
            }
            else
            {
                _toolUnpackingSetting.HidePanelUnpacking();
            }
            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                foreach (var item in root.Value)
                    item.UnActiveImgCheck();
            }
        }
    }
}

