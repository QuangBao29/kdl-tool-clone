﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kawaii.ResourceManager;
using Kawaii.IsoTools.DecoSystem;
using KAP.Tools;

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
        private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField]
        private ToolCreateMapListRooms _lstRooms = null;
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
        public Toggle GetToggleIsStatic()
        {
            return _toggleIsStatic;
        }
        public Toggle GetToggleIsBubble()
        {
            return _toggleIsBubble;
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
        public void ShowStaticKDL()
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Event)
            {
                var cur = _editManager.Current;
                if (cur == null)
                {
                    return;
                }
                gameObject.SetActive(true);
                var info = (DecoInfo)cur.deco.Info;
                if (info == null)
                {
                    return;
                }
                _toggleIsStatic.isOn = info.IsStatic;
            }
        }
        public void OnToggleBubbleChange()
        {
            var cur = _editManager.Current;
            _toggleIsBubble.isOn = !_toggleIsBubble.isOn;
            var info = (DecoInfo)cur.deco.Info;
            var rootInfo = (DecoInfo)cur.deco.Root.Info;
            int numOfBubble = 0;
            info.IsBubble = _toggleIsBubble.isOn;
            if (info.IsBubble)
            {
                foreach (var item in _lstRooms.GetLstRoomItem())
                {
                    if(item.GetRoomId() == rootInfo.Id && !item.IsSelect)
                    {
                        item.OnClickRoomItem();
                        break;
                    }
                }
                numOfBubble = _toolBubbleSetting.LstCurBubbleIDItem.Count;
                //Debug.LogError("check count of bubble: " + numOfBubble);
                string newBubbleID = rootInfo.Id + "_" + numOfBubble;
                _toolBubbleDecoSetting.DctBubbleDecoItems.Add(newBubbleID, new List<string>());
                _toolBubbleDecoSetting.DctBubbleDecoItems[newBubbleID].Add(info.Id + "_" + info.Color);
                _toolBubbleSetting.OnGenerateItem(rootInfo.Id.ToString());
                if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                {
                    _configController.DctBubbleIdPrice.Add(newBubbleID, new List<string>());
                    _configController.DctBubbleIdPrice[newBubbleID].Add(0.ToString());
                    _configController.DctBubbleIdStar.Add(newBubbleID, "0");
                    _configController.DctBubbleIdWD.Add(newBubbleID, cur.deco.WorldDirect.ToString());
                }
                if (!_toolBubbleSetting.DctDecoInRoom.ContainsKey(newBubbleID))
                {
                    //Debug.LogError("check");
                    _toolBubbleSetting.DctDecoInRoom.Add(newBubbleID, cur.deco);
                }
                _configController.DctRoomIdPosition[rootInfo.Id].Add(cur.deco.Position);
            }
            else
            {
                _toolBubbleSetting.OnRemoveBubbleId(cur, cur.deco.Root);
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
            _toggleIsStatic.isOn = !_toggleIsStatic.isOn;
            var cur = _editManager.Current;
            var info = (DecoInfo)cur.deco.Info;
            info.IsStatic = _toggleIsStatic.isOn;
            
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
