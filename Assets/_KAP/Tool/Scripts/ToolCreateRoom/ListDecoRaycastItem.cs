using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneP.InfinityScrollView;
using UnityEngine.UI;
using Kawaii.IsoTools.DecoSystem;

namespace KAP.ToolCreateMap
{
    public class ListDecoRaycastItem : InfinityBaseItem
    {
        [SerializeField]
        private EditManager _editManager = null;
        [SerializeField]
        private ListDecoRaycast _uiParent = null;
        [SerializeField]
        private Image _imgIcon = null;
        [SerializeField]
        private GameObject _objCheck = null;

        private Deco _deco = null;
        public override void Reload(InfinityScrollView infinity, int _index)
        {
            base.Reload(infinity, _index);
            _deco = _uiParent.GetDecoByIndex(_index);
            if(_deco == null)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            _imgIcon.sprite = _deco.Spr.sprite;
            _imgIcon.transform.localScale = new Vector3(_deco.Spr.flipX?-1:1, 1, 1);
            var curSelected = _editManager.Current != null ? _editManager.Current.deco : null;
            _objCheck.SetActive(_deco == curSelected);
        }

        public void OnButtonItemClick()
        {
            if (_deco == null)
                return;
            if (_editManager.SetCurrent(_deco.GetComponent<DecoEditDemo>()))
                _uiParent.Show();
        }
        
    }
}

