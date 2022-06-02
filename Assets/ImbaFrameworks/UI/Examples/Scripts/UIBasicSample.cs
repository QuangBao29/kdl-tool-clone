using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Imba.UI;

public class UIBasicSample : MonoBehaviour
{
    public void OnOpenPopupClick()
    {
        Debug.Log("Click open popup");
        UIManager.ShowPopup(UIPopupName.MessageBox.ToString());
    }
    
    public void OnShowLoadingClick()
    {
        Debug.Log("Click Show Loading");
        UIManager.ShowLoading(2f);
    }
    
    public void OnShowAlertClick()
    {
        Debug.Log("Click Show Alert");
        UIManager.ShowAlertMessage("Fire in the hole!", AlertType.Error);
    }
}
