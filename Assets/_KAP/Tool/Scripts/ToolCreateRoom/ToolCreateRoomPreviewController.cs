using System.Collections;
using System.Collections.Generic;
using Kawaii.IsoTools.DecoSystem;
using UnityEngine;
using UnityEngine.UI;
using KAP.Config;
using Imba.UI;
using Kawaii.ResourceManager;
using KAP.Tools;

namespace KAP.ToolCreateMap
{
    public class ToolCreateRoomPreviewController : MonoBehaviour
    {
        [SerializeField]
        private string atlasShadowPath = "Assets/_KAP/_GameResources/Atlas/Decos/99900.asset";
        [SerializeField]
        public int shadowThemeId = 99900;
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField]
        private ToolCreateMapConfigController _configController = null;
        [SerializeField]
        private ToolEditMode _editModeController = null;
        [SerializeField] private Text _txtTotalShadow = null;
        [SerializeField] private Text _txtGroups = null;
        [Space]
        [SerializeField]
        private InputField _inputMapId = null;
        [SerializeField]
        public FlexibleColorPicker _colorPicker = null;
        [SerializeField] private RectTransform _scroll = null;
        private Color _curShadowColor;

        private readonly List<KAPToolDecoShadow> _lstShadows = new List<KAPToolDecoShadow>();

        ConfigRoomRecord _configRoom = null;
        ConfigWonderRecord _configWonder = null;
        ConfigRoomChallengeRecord _configRoomChallenge = null;
        public void SetupPreview()
        {
            KawaiiAtlas shadowAtlas = null;
#if UNITY_EDITOR
            shadowAtlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(atlasShadowPath, shadowThemeId.ToString());
#endif
            _lstShadows.Clear();
            List<KAPToolDecoShadow> lstAllShadows = new List<KAPToolDecoShadow>();
            _areaManager.ForeachDecos((deco) => {
                
                var shadowComp = deco.GetComponent<KAPToolDecoShadow>();
                if (shadowComp == null)
                    return;

                Sprite sprShadow = null;
                if (shadowAtlas != null)
                    sprShadow = shadowAtlas.GetSprite(GetShadowSpriteName(deco.FLIsoSize, deco.IsWallHang));

                if(sprShadow == null)
                {
                    Debug.LogError(string.Format("Can't find sprShadow, isoSize: {0}, is wallHang {1}", deco.FLIsoSize, deco.IsWallHang));
                    return;
                }

                shadowComp.Setup(sprShadow);
                shadowComp.IsShowShadow = true;
                var type = shadowComp.GetShowType();
                if (type == KAPToolDecoShadow.ShowType.Shadow)
                {
                    _lstShadows.Add(shadowComp);
                }

                var info = shadowComp.Deco.ParseInfo<DecoInfo>();
                if (!info.IsStatic
                    && (shadowComp.Deco.IsWallHang || shadowComp.Deco.TreeLevel > 0))
                {
                    lstAllShadows.Add(shadowComp);
                }
            });
            Invoke("RefreshShadowColor", 0.2f);
            Debug.LogError(string.Format("Total shadows: <color=yellow>{0}</color>", lstAllShadows.Count));
            _txtTotalShadow.gameObject.SetActive(true);
            _txtTotalShadow.text = "Total shadow: " + lstAllShadows.Count;
            ShowGroupShadow(lstAllShadows);

        }

        public void SetUpReviewShadow(InputField inputGroup)
        {
            if (string.IsNullOrEmpty(inputGroup.text))
                SetupPreview();
            else
                SetupPreviewByGroup(SplitString(inputGroup.text));
        }

        private void SetupPreviewByGroup(List<int> groups)
        {
            
            KawaiiAtlas shadowAtlas = null;
#if UNITY_EDITOR
            shadowAtlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(atlasShadowPath, shadowThemeId.ToString());
#endif
            _lstShadows.Clear();
            List<KAPToolDecoShadow> lstAllShadows = new List<KAPToolDecoShadow>();
            _areaManager.ForeachDecos((deco) => {

                var shadowComp = deco.GetComponent<KAPToolDecoShadow>();
                if (shadowComp == null)
                    return;
                Sprite sprShadow = null;
                if (shadowAtlas != null)
                    sprShadow = shadowAtlas.GetSprite(GetShadowSpriteName(deco.FLIsoSize, deco.IsWallHang));
                if (!groups.Contains(deco.Group)) return;
                if (sprShadow == null)
                {
                    Debug.LogError(string.Format("Can't find sprShadow, isoSize: {0}, is wallHang {1}", deco.FLIsoSize, deco.IsWallHang));
                    return;
                }

                shadowComp.Setup(sprShadow);
                shadowComp.IsShowShadow = true;
                var type = shadowComp.GetShowType();
                if (type == KAPToolDecoShadow.ShowType.Shadow)
                {
                    _lstShadows.Add(shadowComp);
                }

                var info = shadowComp.Deco.ParseInfo<DecoInfo>();
                if (!info.IsStatic
                    && (shadowComp.Deco.IsWallHang || shadowComp.Deco.TreeLevel > 0))
                {
                    lstAllShadows.Add(shadowComp);
                }
            });
            Invoke("RefreshShadowColor", 0.2f);
            Debug.LogError(string.Format("Total shadows: <color=yellow>{0}</color>", lstAllShadows.Count));
            _txtTotalShadow.gameObject.SetActive(false);
            ShowGroupShadow(lstAllShadows);
        }

        void RefreshShadowColor()
        {
            int mapId = 0;
            int.TryParse(_inputMapId.text, out mapId);

            switch (_editModeController.CurrentEditMode)
            {
                case EditMode.Room:
                    _configRoom = _configController.GetConfigRoomById(mapId);
                    if (_configRoom == null)
                    {
                        UIManager.ShowAlertMessage("Not found config room: " + mapId, AlertType.Error);
                    }
                    else
                    {
                        _curShadowColor = _colorPicker.color = SGUtils.HexToColor(_configRoom.ShadowHexColor);
                    }
                    break;
                case EditMode.Wonder:
                    _configWonder = _configController.GetConfigWonderById(mapId);
                    if (_configWonder == null)
                    {
                        UIManager.ShowAlertMessage("Not found config Wonder: " + mapId, AlertType.Error);
                    }
                    else
                    {
                        _curShadowColor = _colorPicker.color = SGUtils.HexToColor(_configWonder.ShadowHexColor);
                    }
                    break;
                case EditMode.RoomChallenge:
                    _configRoomChallenge = _configController.GetConfigRoomChallenge(mapId);
                    if(_configRoomChallenge == null)
                    {
                        UIManager.ShowAlertMessage($"Not found config room challenge: {mapId}", AlertType.Error);
                    }
                    else
                    {
                        _curShadowColor = _colorPicker.color = SGUtils.HexToColor(_configRoomChallenge.ShadowHexColor);
                    }
                    break;
                default:
                    Debug.LogError(string.Format("other mode: {0}", _editModeController.CurrentEditMode));
                    break;
            }

            foreach (var shadow in _lstShadows)
            {
                var info = shadow.Deco.ParseInfo<DecoInfo>();
                if (string.IsNullOrEmpty(info.ShadowColor))
                    shadow.SetColor(_curShadowColor);
                else
                    shadow.SetColor(SGUtils.HexToColor(info.ShadowColor));
            }
        }

        public void SetupUnPreview()
        {
            int count = 0;
            _areaManager.ForeachDecos((deco) =>
            {
                var shadowComp = deco.GetComponent<KAPToolDecoShadow>();
                if (shadowComp != null)
                    shadowComp.IsShowShadow = false;
                count++;
            });
        }

        public static string GetShadowSpriteName(Vector3 flIsoSize, bool isWall)
        {
            if (isWall)
            {
                return string.Format("w_{0}_{1}", flIsoSize.y, flIsoSize.z);
            }
            else // deco indoor
            {
                if (flIsoSize.x >= flIsoSize.y) // flip shadow x_y -> y_x to save resource
                    return string.Format("{0}_{1}", flIsoSize.x, flIsoSize.y);
                else
                    return string.Format("{0}_{1}", flIsoSize.y, flIsoSize.x);
            }
        }

        private void LateUpdate()
        {
            if(_curShadowColor != _colorPicker.color)
            {
                _curShadowColor = _colorPicker.color;
                foreach (var shadow in _lstShadows)
                {
                    var info = shadow.Deco.ParseInfo<DecoInfo>();
                    if (string.IsNullOrEmpty(info.ShadowColor))
                        shadow.SetColor(_curShadowColor);
                    else
                        shadow.SetColor(SGUtils.HexToColor(info.ShadowColor));
                }
            }
        }

        public void OnButtonSaveClick()
        {
            switch (_editModeController.CurrentEditMode)
            {
                case EditMode.Room:
                    if (_configRoom == null)
                    {
                        UIManager.ShowAlertMessage("Not found config room!", AlertType.Error);
                        return;
                    }

                    _configRoom.ShadowHexColor = _colorPicker.hexInput.text;
                    _configController.SaveConfigRoomCsv();
                    UIManager.ShowAlertMessage(string.Format("Save color Room {0} completed.", _configRoom.Id));
                    break;
                case EditMode.Wonder:
                    if (_configWonder == null)
                    {
                        UIManager.ShowAlertMessage("Not found config wonder!", AlertType.Error);
                        return;
                    }

                    _configWonder.ShadowHexColor = _colorPicker.hexInput.text;
                    _configController.SaveConfigWonderCsv();
                    UIManager.ShowAlertMessage(string.Format("Save color Wonder {0} completed.", _configWonder.Id));
                    break;
                case EditMode.RoomChallenge:
                    if(_configRoomChallenge == null)
                    {
                        UIManager.ShowAlertMessage($"Not found config wonder!", AlertType.Error);
                        return;
                    }
                    _configRoomChallenge.ShadowHexColor = _colorPicker.hexInput.text;
                    _configController.SaveConfigRoomChallengeCSV();
                    UIManager.ShowAlertMessage($"Save color room challenge {_configRoomChallenge.Id} completed");
                    break;
                default:
                    Debug.LogError(string.Format("other mode: {0}", _editModeController.CurrentEditMode));
                    break;
            }
        }

        private void ShowGroupShadow(List<KAPToolDecoShadow> lstShadown)
        {
            Dictionary<int, int> dictGroup = new Dictionary<int, int>();
            foreach(var item in lstShadown)
            {
                int group = item.Deco.Group;
                if(dictGroup.ContainsKey(group))
                {
                    int value = dictGroup[group];
                    value++;
                    dictGroup[group] = value;
                }
                else
                {
                    dictGroup[group] = 1;
                }
            }
            _txtGroups.text = string.Empty;
            foreach (var item in dictGroup)
            {
                _txtGroups.text += "Group " + item.Key + ": " + item.Value + " shadows\n";
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_txtGroups.rectTransform);
            _scroll.SetHeight(_txtGroups.rectTransform.GetHeight());
        }

        private List<int> SplitString(string input)
        {
            List<int> result = new List<int>();
            string[] arrString =  input.Split(';');
            foreach (var item in arrString)
                result.Add(int.Parse(item));
            return result;
                
        }
    }
}