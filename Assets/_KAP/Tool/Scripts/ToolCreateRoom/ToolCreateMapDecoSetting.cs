using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kawaii.ResourceManager;
using Kawaii.IsoTools.DecoSystem;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapDecoSetting : MonoBehaviour
    {
        const float _bgWight = 372;
        const float _staticHeight = 60;
        const float _noColorHeight = 120;
        const float _fullHeight = 200;

        [SerializeField]
        private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField]
        private EditManager _editManager = null;
        [SerializeField]
        private ToolCreateMapConfigController _configController = null;
        [SerializeField]
        private InputField _inputMapId = null;
        [SerializeField]
        private RectTransform _rTransBg = null;
        [SerializeField]
        private Toggle _toggleIsStatic = null;
        [SerializeField]
        private Toggle _toggleIsBubble = null;
        [SerializeField]
        private GameObject _objNonStatic = null;
        [SerializeField]
        private Toggle _toggleShadowColor = null;
        [SerializeField]
        private FlexibleColorPicker _colorPicker = null;

        [SerializeField]
        private string atlasShadowPath = "Assets/_KAP/_GameResources/Atlas/Decos/99900.asset";
        [SerializeField]
        public int shadowThemeId = 99900;

        private KawaiiAtlas shadowAtlas = null;

        private KAPToolDecoShadow _curShadow = null;
        private Color _defaultColor = Color.white;

        public KAPToolDecoShadow CurrentShadow
        {
            get
            {
                return _curShadow;
            }
        }

        private void Start()
        {
#if UNITY_EDITOR
            shadowAtlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(atlasShadowPath, shadowThemeId.ToString());
#endif
            Hide();
        }

        public void Show()
        {
            var cur = _editManager.Current;
            if (cur == null)
            {
                Hide();
                return;
            }
            int roomId = 0;
            int.TryParse(_inputMapId.text, out roomId);
            var configRoom = _configController.GetConfigRoomById(roomId);
            if (configRoom != null)
                _defaultColor = SGUtils.HexToColor(configRoom.ShadowHexColor);
            else
                _defaultColor = Color.white;

            _curShadow = cur.GetComponent<KAPToolDecoShadow>();

            gameObject.SetActive(true);
            var info = (DecoInfo)cur.deco.Info;
            if (info == null)
            {
                return;
            }
            _toggleIsStatic.isOn = info.IsStatic;
            _toggleIsBubble.isOn = info.IsBubble;
            return;
            if (info.IsStatic)
            {
                _objNonStatic.SetActive(false);
                _rTransBg.SetSize(new Vector2(_bgWight, _staticHeight));
                return;
            }
            _objNonStatic.SetActive(true);
            _toggleShadowColor.isOn = !string.IsNullOrEmpty(info.ShadowColor);
            if (!_toggleShadowColor.isOn)
            {
                _colorPicker.gameObject.SetActive(false);
                _rTransBg.SetSize(new Vector2(_bgWight, _noColorHeight));
                return;
            }

            _colorPicker.gameObject.SetActive(true);
            _colorPicker.color = SGUtils.HexToColor(info.ShadowColor);
            _rTransBg.SetSize(new Vector2(_bgWight, _fullHeight));
        }
        public void ShowBubbleKDL()
        {
            var cur = _editManager.Current;
            if (cur == null)
            {
                HideKDL();
                return;
            }
            gameObject.SetActive(true);
            var info = (DecoInfo)cur.deco.Info;
            if (info == null)
            {
                return;
            }
            _toggleIsBubble.isOn = info.IsBubble;
        }
        public void OnToggleBubbleChange()
        {
            var cur = _editManager.Current;
            _toggleIsBubble.isOn = !_toggleIsBubble.isOn;
            var info = (DecoInfo)cur.deco.Info;
            var rootInfo = (DecoInfo)cur.deco.Root.Info;
            int numOfBubble = 0;
            //foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            //{
            //    if (root.Key.RoomId == rootInfo.Id)
            //    {
            //        numOfBubble = root.Value.Count;
            //        Debug.LogError("num of bubble " + numOfBubble);
            //        break;
            //    }
            //}
            info.IsBubble = _toggleIsBubble.isOn;
            if (info.IsBubble)
            {
                _toolBubbleDecoSetting.CreateBubbleDecoItems(info.Id, info.Color, rootInfo.Id, rootInfo.Id + "_" + numOfBubble, cur.deco);
            }
            else
            {
                foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
                {
                    if (root.Key.RoomId == rootInfo.Id)
                    {
                        ToolCreateMapBubbleDecoItems temp = null;
                        foreach (var child in root.Value)
                        {
                            if (child.Deco == cur.deco)
                            {
                                //Debug.LogError("dung deco");
                                temp = child;
                                Destroy(child.gameObject);
                                _editManager.SetCurrent(null);
                                break;
                            }
                        }
                        root.Value.Remove(temp);
                        //Debug.LogError("count: " + root.Value.Count);
                        break;
                    }
                }
            }
            ShowBubbleKDL();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
            if (_curShadow == null)
                return;
            if(_curShadow.IsShowShadow)
            {
                _curShadow.IsShowShadow = false;
                foreach (var piece in _curShadow.Deco.LstAreaPieces)
                {
                    foreach (var c in piece.LstChilds)
                    {
                        ShowShadowChildRecrusive(c, _curShadow.IsShowShadow);
                    }
                }
            }
            _curShadow = null;
        }
        public void HideKDL()
        {
            gameObject.SetActive(false);
        }

        public void OnToggleStaticChange()
        {
            if (_curShadow == null)
                return;
            _toggleIsStatic.isOn = !_toggleIsStatic.isOn;
            var info = (DecoInfo)_curShadow.Deco.Info;
            info.IsStatic = _toggleIsStatic.isOn;
            if (info.IsStatic)
            {
                info.ShadowColor = null;
                if(_curShadow.IsShowShadow)
                {
                    _curShadow.IsShowShadow = false;
                    foreach (var piece in _curShadow.Deco.LstAreaPieces)
                    {
                        foreach (var c in piece.LstChilds)
                        {
                            ShowShadowChildRecrusive(c, _curShadow.IsShowShadow);
                        }
                    }
                }
            }
            Show();
        }
        
        public void OnToggleColorChange()
        {
            if (_curShadow == null)
                return;
            var info = (DecoInfo)_curShadow.Deco.Info;
            if (info.IsStatic)
                return;
            _toggleShadowColor.isOn = !_toggleShadowColor.isOn;
            if (_toggleShadowColor.isOn)
            {
                info.ShadowColor = _colorPicker.hexInput.text;
                if (_curShadow.IsShowShadow)
                    _curShadow.SetColor(_colorPicker.color);
            }
            else
            {
                info.ShadowColor = null;
                if (_curShadow.IsShowShadow)
                    _curShadow.SetColor(_defaultColor);
            }
            Show();
        }

        private void Update()
        {
            //if (!_toggleShadowColor.isOn)
            //    return;
            //if (_curShadow == null)
            //    return;
            //var info = (DecoInfo)_curShadow.Deco.Info;
            //if(info.ShadowColor != _colorPicker.hexInput.text)
            //{
            //    info.ShadowColor = _colorPicker.hexInput.text;
            //    _curShadow.SetColor(_colorPicker.color);
            //}
        }

        public void OnButtonPreviewClick()
        {
            if (_curShadow == null)
                return;
            var deco = _curShadow.Deco;
            _curShadow.Setup(shadowAtlas.GetSprite(ToolCreateRoomPreviewController.GetShadowSpriteName(deco.FLIsoSize, deco.IsWallHang)));
            _curShadow.IsShowShadow = !_curShadow.IsShowShadow;
            var info = (DecoInfo)_curShadow.Deco.Info;
            if (!string.IsNullOrEmpty(info.ShadowColor))
                _curShadow.SetColor(SGUtils.HexToColor(info.ShadowColor));
            else
                _curShadow.SetColor(_defaultColor);
            foreach (var piece in deco.LstAreaPieces)
            {
                foreach (var c in piece.LstChilds)
                {
                    ShowShadowChildRecrusive(c, _curShadow.IsShowShadow);
                }
            }
        }

        void ShowShadowChildRecrusive(Deco child, bool isShowShadow)
        {
            child.Spr.enabled = !isShowShadow;
            foreach(var piece in child.LstAreaPieces)
            {
                foreach(var c in piece.LstChilds)
                {
                    ShowShadowChildRecrusive(c, isShowShadow);
                }
            }
        }
    }

}
