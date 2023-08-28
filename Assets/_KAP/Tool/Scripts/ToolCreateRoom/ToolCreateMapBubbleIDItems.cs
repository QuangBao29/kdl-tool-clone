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

    private string _bubbleId = "";

    private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";
    public void OnGenerateDecoItem()
    {
        _bubbleSetting.OnShowViewDecoBubble();
        var config = _configController.ConfigBubbleHome.GetById(_bubbleId);
        var lstID = config.GetLstBubbleDeco();
        _bubbleDecoSetting.OnGenerateItem(lstID, _bubbleId);
    }
    public void SetBubbleID(string bubbleID)
    {
        _bubbleId = bubbleID;
        _textBubbleID.text = bubbleID;
    }
}
