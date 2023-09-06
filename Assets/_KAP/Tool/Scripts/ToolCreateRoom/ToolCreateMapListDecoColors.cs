using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneP.InfinityScrollView;
using KAP.Config;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapListDecoColors : MonoBehaviour
    {
        [SerializeField]
        private EditManager _editManager = null;
        [SerializeField]
        private ToolCreateMapImportDeco _importDecoController = null;
        [SerializeField]
        private ToolCreateMapConfigController _configController = null;
        [SerializeField]
        private InfinityScrollView _scrollItems = null;
        [SerializeField]
        private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;

        private List<ConfigDecoColorRecord> _lstRecords;
        public int CurrentId { get; private set; }
        public int CurrentColor { get; private set; }
        public int CurrentThemeId { get; private set; }

        public ConfigDecoColorRecord GetRecordByIndex(int index)
        {
            if (_lstRecords == null || index < 0 || index >= _lstRecords.Count)
                return null;
            return _lstRecords[index];
        }

        private void Start()
        {
            Hide();
        }

        void Show()
        {
            var cur = _editManager.Current;
            if (cur == null)
                return;
            var info = (DecoInfo)cur.deco.Info;
            if (info == null) return;

            var decoRecord = _configController.ConfigDeco.GetDecoById(info.Id);
            if (decoRecord == null)
                return;
            gameObject.SetActive(true);
            CurrentId = info.Id;
            CurrentThemeId = decoRecord.ThemeId;
            _lstRecords = _configController.ConfigDecoColor.GetListDecoColorsByDecoId(info.Id);
            CurrentColor = info.Color;
            _scrollItems.Setup(_lstRecords != null ? _lstRecords.Count : 0);
        }

        void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnChangeCurrentEditDeco()
        {
            var cur = _editManager.Current;
            if (cur != null)
                Show();
            else
                Hide();
        }

        public void ChangeColor(int colorId)
        {
            if (colorId == CurrentColor)
                return;
            var cur = _editManager.Current;
            if (cur == null)
                return;

            int oldColor = CurrentColor;
            int newColor = colorId;
            var deco = cur.deco;
            var info = (DecoInfo)deco.Info;
            info.Color = colorId;
            var colorPath = colorId > 0 ? "_" + colorId : "";
            var flSprite = _importDecoController.GetSprite(info.Id + colorPath, CurrentThemeId);
            var brSprite = _importDecoController.GetSprite(info.Id +"_b" + colorPath, CurrentThemeId);
            deco.SetupSprite(flSprite, brSprite != null ? brSprite : flSprite);
            deco.WorldDirect = deco.WorldDirect;
            CurrentColor = colorId;
            _scrollItems.Setup(_lstRecords != null ? _lstRecords.Count : 0);

            _toolBubbleDecoSetting.OnChangeColorDecos(oldColor, newColor);
        }

    }

}
