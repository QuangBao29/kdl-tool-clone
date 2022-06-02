using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OneP.InfinityScrollView;
using KAP.Config;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapListDecoColorItem : InfinityBaseItem
    {
        [SerializeField]
        private ToolCreateMapImportDeco _importDecoController = null;
        [SerializeField]
        private ToolCreateMapListDecoColors _uiParent = null;

        [SerializeField]
        private Image _imgIcon = null;
        [SerializeField]
        private Text txt_Name = null;
        [SerializeField]
        private GameObject _objCheckIcon = null;

        private ConfigDecoColorRecord _record;

        public override void Reload(InfinityScrollView infinity, int _index)
        {
            base.Reload(infinity, _index);
            _record = _uiParent.GetRecordByIndex(_index);
            if(_record == null)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            var colorPath = _record.ColorId > 0 ? "_" + _record.ColorId : "";
            _imgIcon.sprite = _importDecoController.GetSprite(_record.DecoId + colorPath, _uiParent.CurrentThemeId);
            txt_Name.text = _record.ColorId.ToString();
            _objCheckIcon.SetActive(_record.ColorId == _uiParent.CurrentColor);
        }

        public void OnButtonItemClick()
        {
            if (_record == null)
                return;
            _uiParent.ChangeColor(_record.ColorId);
        }
    }
}

