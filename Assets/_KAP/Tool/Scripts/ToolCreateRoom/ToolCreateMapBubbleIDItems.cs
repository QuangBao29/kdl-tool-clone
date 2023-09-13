using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KAP.Tools;
using Kawaii.IsoTools;
using Kawaii.IsoTools.DecoSystem;
using System.Linq;
using Kawaii.ResourceManager;
using Kawaii.Utils;
using KAP.ToolCreateMap;
using TMPro;

public class ToolCreateMapBubbleIDItems : MonoBehaviour
{
    [SerializeField]
    private ToolCreateMapConfigController _configController = null;
    [SerializeField]
    private ToolCreateMapBubbleSetting _bubbleSetting = null;
    [SerializeField]
    private ToolCreateMapBubbleDecoSetting _bubbleDecoSetting = null;
    [SerializeField]
    private TextMeshProUGUI _textBubbleID = null;
    [HideInInspector]
    public Deco CurrentDeco = null;
    private string _bubbleId = "";
    [HideInInspector]
    public bool IsInit = false;

    private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";
    public void OnClickGenerateDecoItem()
    {
        if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
        {

        }
        else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
        {

        }
        _bubbleSetting.OnShowViewDecoBubble();
        var record = _configController.ConfigBubbleHome.GetById(_bubbleId);
        _bubbleSetting.CurrentBubbleID = _bubbleId;
        _bubbleDecoSetting.OnGenerateItem(_bubbleId, this);
    }
    public void SetBubbleID(string bubbleID)
    {
        _bubbleId = bubbleID;
        _textBubbleID.text = bubbleID;
    }
    public string GetBubbleID()
    {
        return _bubbleId;
    }
}
