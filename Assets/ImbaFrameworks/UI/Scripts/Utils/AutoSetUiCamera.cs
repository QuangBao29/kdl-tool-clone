using UnityEngine;
using System.Collections;
using Imba.UI;

public class AutoSetUiCamera : MonoBehaviour
{

    public Canvas canvas;
    // Use this for initialization
    void Awake()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();
        Setup();
    }
    public void Setup()
    {
        var uiCamera = UIManager.Instance.UICamera;
        if (canvas.worldCamera == null && uiCamera != null)
        {
            canvas.worldCamera = uiCamera;
            //canvas.sortingLayerName = "UI";
        }
    }

}
