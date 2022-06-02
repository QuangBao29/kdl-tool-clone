using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Imba.UI;

public class UISamplePopup : UIPopup
{
    public void OnHidePopupClick()
    {
        UIManager.ShowMessage("Confirm", "Are you sure to buy this item?", UIMessageBox.MessageBoxType.Yes_No,
            (ret) =>
            {
                if (ret == UIMessageBox.MessageBoxAction.Accept)
                {
                    this.Hide();
                    return true;
                }
                else
                {
                    return true;
                }
            });
    }

    public void OnCancelClick()
    {
        this.Hide();
        //UIManager.Instance.PopupManager.ShowPopup(UIPopupName.UpgradeCar);
    }
}