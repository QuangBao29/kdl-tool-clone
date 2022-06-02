using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;

namespace KAP.ToolCreateMap
{
    [RequireComponent(typeof(Deco))]
    public class KAPToolDecoShadow : MonoBehaviour
    {
        public enum ShowType
        {
            None,
            Shadow,
            Disable
        }
        [SerializeField]
        private ToolCreateMapDecoSetting _decoSetting = null;
        [SerializeField]
        private Deco _deco = null;
        private Sprite _sprShadow = null;
        private bool _isShowShadow = false;

        public void Setup(Sprite shadow)
        {
            _sprShadow = shadow;
        }

        public ShowType GetShowType()
        {
            if (_decoSetting.CurrentShadow == this)
                return ShowType.Shadow;
            var info = (DecoInfo)_deco.Info;
            if (info.IsStatic)
                return ShowType.None;
            if (_deco.IsWallHang)
                return ShowType.Shadow;
            if (_deco.TreeLevel == 1)
                return ShowType.Shadow;
            if(_deco.PieceParent.DecoParent.ParseInfo<DecoInfo>().IsStatic)
                return ShowType.Shadow;

            return ShowType.Disable;
        }

        public bool IsShowShadow
        {
            get
            {
                return _isShowShadow;
            }
            set
            {
                var showType = GetShowType();
                if(value)
                {
                    switch (showType)
                    {
                        case ShowType.Shadow:
                            var flIsoSize = _deco.FLIsoSize;
                            _deco.Spr.transform.localScale = new Vector3((!_deco.IsWallHang && (flIsoSize.x < flIsoSize.y)) ? -1 : 1, 1, 1);
                            _deco.Spr.sprite = _sprShadow;
                            _isShowShadow = value;
                            break;
                        case ShowType.Disable:
                            _deco.Spr.enabled = false;
                            _isShowShadow = value;
                            break;
                    }
                }
                else
                {
                    _isShowShadow = false;
                    switch (showType)
                    {
                        case ShowType.Shadow:
                            _deco.Spr.transform.localScale = Vector3.one;
                            _deco.Spr.color = Color.white;
                            _deco.WorldDirect = _deco.WorldDirect;
                            break;
                        case ShowType.Disable:
                            _deco.Spr.enabled = true;
                            break;
                    }
                }
            }
        }

        public void SetColor(Color color)
        {
            if(_isShowShadow)
                _deco.Spr.color = color;
        }

        public Deco Deco
        {
            get
            {
                return _deco;
            }
        }
    }
}
