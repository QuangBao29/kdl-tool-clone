using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;
using KAP;
using KAP.ToolCreateMap;
using Kawaii.ResourceManager;
//using Kawaii.ResourceManager.Editor;
using UnityEngine.UI;

public class ToolCreateMapUnpackingSetting : MonoBehaviour
{
    [SerializeField] private AreaManager _areaManager = null;
    [SerializeField] private ToolCreateMapConfigController _configController = null;
    [SerializeField] private ToolCreateMapBubbleDecoItems _prefabDecoItems = null;
    [SerializeField] private EditManager _editManager = null;
    [SerializeField] private GameObject _content = null;
    [SerializeField] private GameObject _btnAddDeco = null;
    [SerializeField] private Image _imgButton;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _defaultColor;

    [HideInInspector]
    public List<string> LstUnpackDeco = new List<string>();
    [HideInInspector]
    public List<ToolCreateMapBubbleDecoItems> LstDecoItem = new List<ToolCreateMapBubbleDecoItems>();

    private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";
    private bool IsShow = false;
    public void OnClickShowPanel()
    {
        if (!IsShow)
        {
            _imgButton.color = _selectedColor;
            IsShow = true;
            _btnAddDeco.SetActive(true);
            _content.SetActive(true);
        }
        else
        {
            _imgButton.color = _defaultColor;
            IsShow = false;
            _btnAddDeco.SetActive(false);
            _content.SetActive(false);
        }
    }
    public void HidePanelUnpacking()
    {
        gameObject.SetActive(false);
    }
    public void ShowPanelUnpacking()
    {
        gameObject.SetActive(true);
    }
    public void ButtonAddDecoToListUnpack()
    {
        var current = _editManager.Current;
        if (current == null)
        {
            Debug.LogError("current null");
            return;
        }
        var decoInfo = current.deco.ParseInfo<DecoInfo>();
        var decoId = decoInfo.Id + "_" + decoInfo.Color;
        if (LstUnpackDeco.Contains(decoId))
        {
            Debug.LogError("List contained");
            return;
        }
        LstUnpackDeco.Add(decoId);
        Debug.LogError("deco moi dc add: " + decoId);
        var decoItem = CreateDecoItems(decoInfo.Id, decoInfo.Color);
        decoItem.SetActiveImgCheck();
        if (decoItem != null)
            LstDecoItem.Add(decoItem);
        else Debug.LogError("decoItem null");
        decoItem.Deco = _editManager.Current.deco;
    }

    public ToolCreateMapBubbleDecoItems CreateDecoItems(int id, int colorId)
    {
        var colorPath = colorId > 0 ? "_" + colorId : "";
        string idPath = id.ToString() + colorPath;
        var config = _configController.ConfigDeco.GetDecoById(id);
        if (config == null)
            return null;

        KawaiiAtlas atlas = null;
#if UNITY_EDITOR
        atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + config.ThemeId + ".asset", config.ThemeId.ToString());
#endif
        var FLSprite = atlas != null ? atlas.GetSprite(idPath) : null;
        ToolCreateMapBubbleDecoItems decoItem = SGUtils.InstantiateObject<ToolCreateMapBubbleDecoItems>(_prefabDecoItems, _content.transform);
        decoItem.Image.sprite = FLSprite;
        decoItem.Name.text = idPath;
        decoItem.Info = new DecoInfo { Id = id, Color = colorId, IsBubble = false, IsUnpacking = true };
        decoItem.gameObject.SetActive(true);
        return decoItem;
    }
    public void ImportUnpackingDeco()
    {
        Debug.LogError("num: " + LstUnpackDeco.Count);
        foreach (var decoId in LstUnpackDeco)
        {
            Debug.LogError("decoId: " + decoId);
            var lstIdColor = SGUtils.ParseStringToListInt(decoId, '_');
            var id = lstIdColor[0];
            var color = lstIdColor[1];
            var decoItem = CreateDecoItems(id, color);
            if (decoItem != null)
                LstDecoItem.Add(decoItem);
            else Debug.LogError("decoItem null");
            decoItem.Deco = null;
        }
        var tempList = new List<Deco>();
        _areaManager.ForeachDecos((deco) => {
            var info = deco.ParseInfo<DecoInfo>();
            if (info.IsUnpacking)
                tempList.Add(deco);
        });
        Debug.LogError("number of list temp: " + tempList.Count);
        foreach (var clone in LstDecoItem)
        {
            var cloneInfo = clone.ParseInfo<DecoInfo>();
            foreach (var deco in tempList)
            {
                var info = deco.ParseInfo<DecoInfo>();
                if (info.Id == cloneInfo.Id && info.Color == cloneInfo.Color)
                {
                    Debug.LogError(string.Format("Id: {0}, Color: {1}", info.Id, info.Color));
                    clone.Deco = deco;
                    clone.SetActiveImgCheck();
                }
            }
        }
    }
    public void ClearAll()
    {
        var listTemp = LstDecoItem;
        foreach (var deco in listTemp)
        {
            Destroy(deco.gameObject);
        }
        LstDecoItem.Clear();
    }
}
