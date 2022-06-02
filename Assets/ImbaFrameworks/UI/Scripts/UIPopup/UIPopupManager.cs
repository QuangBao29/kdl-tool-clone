// Copyright (c) 2015 - 2019 Imba
// Author: Kaka
// Created: 2019/08
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Imba.Utils;

namespace Imba.UI
{
    /// <summary>
    /// Manage all popups
    /// </summary>
	public class UIPopupManager : MonoBehaviour
	{
     
        #region Constants
        //public const string POPUP_PREFAB_LOCATION = "Prefabs/Popups/";
        
        #endregion

        #region Static Fields
        #endregion

        #region Events
        #endregion

        #region Public Vars
        
        //database here
        public UIPopupDatabase database;
        public Transform popupsContainer;//should attack to main canvas
        
        #endregion

        #region Private Vars
      
        private Dictionary<string, UIPopup> _dictPopup = new Dictionary<string, UIPopup>();
        private Dictionary<string, UIPopupController> _dictPopupControllers = new Dictionary<string, UIPopupController>();
	    private List<UIPopup> _visiblePopups = new List<UIPopup>();
        
      
        #endregion

        #region Properties
//        public Dictionary<string, UIPopup> PopupPrefabs
//        {
//            get
//            {
//                return _dictPopup;
//            }
//        }



//        public Dictionary<string, UIPopupController> PopupControllers
//        {
//            get
//            {
//                return _dictPopupControllers;
//            }
//        }

	    public List<UIPopup> VisiblePopups
	    {
	        get { return _visiblePopups; }
	    }

	    #endregion

        #region Constructors
        #endregion

        #region Unity Methods

        void Awake()
        {
            
            Initialize();
        }


      
        
        #endregion


        #region Public Methods
        
        public int GetLowestAlwaysOnTopPopupOrder()
        {
            int order = 0;
            foreach (Transform c in popupsContainer)
            {
                if (!c.gameObject.activeSelf)
                    continue;
                UIPopup popup = c.GetComponent<UIPopup>();
                if (!popup || !popup.Controller.AlwaysOnTop || order > popup.OrderInParent)
                    continue;
                order = popup.OrderInParent;
            }
            return order;

        }

        public int GetHighestAlwaysOnTopPopupOrder()
        {

            int order = GetLowestAlwaysOnTopPopupOrder();
            foreach (Transform c in popupsContainer)
            {
                if (!c.gameObject.activeSelf)
                    continue;
                UIPopup popup = c.GetComponent<UIPopup>();
                if (!popup || !popup.Controller.AlwaysOnTop || order < popup.OrderInParent)
                    continue;
                order = popup.OrderInParent;
            }
            return order;

        }

        public int GetActiveTopPopupOrder()
        {

            int order = 0;
            foreach (Transform ch in popupsContainer)
            {
                if (!ch.gameObject.activeSelf)
                    continue;
                UIPopup popup = ch.GetComponent<UIPopup>();
                if (popup && popup.Controller.AlwaysOnTop)
                    continue;
                if (order > ch.GetSiblingIndex())
                    continue;
                order = ch.GetSiblingIndex();
            }
            return order;

        }


        private UIPopupController GetPopupController(string popupName)
        {
            if(!_dictPopupControllers.TryGetValue(popupName, out var popupController)){
                UIManager.DebugLog("No PopupController with name " + popupName, this);
            }
            return popupController;
        }

        public UIPopup GetPopup(string popupName)
        {
            if (!_dictPopup.TryGetValue(popupName, out var popup))
            {
                UIManager.DebugLog("No Popup with name " + popupName, this);
            }
            return popup;
        }

        public void ShowPopup(string popupName, object ps = null)
        {
            UIManager.DebugLog("ShowPopup " + popupName, this);
            UIPopupController controller = GetPopupFromCacheOrCreate(popupName, ps);
            if (controller == null)
            {
                Debug.LogError("Cannot get or create popup " + popupName);
                return;
            }

            if (controller.IsHidding()) StartCoroutine(WaitToShow(controller, ps));
            else
            {
                controller.Show(ps);    
            }
            
        }

        public void HidePopup(string name, bool instantHide = false)
        {
            Debug.Log("HidePopup " + name);
            UIPopupController controller = GetPopupController(name);
            if (controller == null)
            {
                return;
            }

            controller.Hide(instantHide);
        }

        //force hide all dialog
        public void HideAllDialog()
        {
            foreach (var dlg in _dictPopupControllers)
            {
                UIPopupController controller = dlg.Value;
                if (controller != null)
                    controller.Hide(true);
            }
        }

        public void DestroyAllPopups()
        {
            Invoke("WaitDestroyAllDialog", 0.01f);
        }

        void WaitDestroyAllDialog()
        {
            foreach (var dlg in _dictPopupControllers)
            {
                UIPopupController controller = dlg.Value;
                if (controller != null)
                {
                    if (controller.DestroyOnLoadScene)
                        controller.DoDestroy();
                    else
                        controller.Hide(true);
                }
            }
        }

	    public void AddVisiblePopup(UIPopup popup)
	    {
	        if (!_visiblePopups.Contains(popup)) _visiblePopups.Add(popup);
	    }
	    
	    public void RemoveVisiblePopup(UIPopup popup)
	    {
	        if (_visiblePopups.Contains(popup)) _visiblePopups.Remove(popup);
	    }
	    
	    public void RemoveHiddenFromVisiblePopups()
	    {
	        RemoveNullsFromVisiblePopups();
	        for (int i = VisiblePopups.Count - 1; i >= 0; i--)
	            if (VisiblePopups[i].VisibilityState == VisibilityState.Hidden)
	                VisiblePopups.RemoveAt(i);
	    }

	    /// <summary> Removes any null entries from the VisiblePopups list </summary>
	    public void RemoveNullsFromVisiblePopups()
	    {
	        for (int i = VisiblePopups.Count - 1; i >= 0; i--)
	            if (VisiblePopups[i] == null)
	                VisiblePopups.RemoveAt(i);
	    }


        public UIPopup GetTopVisiblePopup()
        {
            if (VisiblePopups.Count > 0) return VisiblePopups.Last();

            return null;
        }
	    
        private UIPopupController GetPopupFromCacheOrCreate(string popupName, object ps = null)
        {

            UIPopupController popupController = GetPopupController(popupName);
            if (popupController)
            {
                UIManager.DebugLog("Load dialog from cache " + popupName, this);
                return popupController;
            }

            UIManager.DebugLog("Create new dialog " + popupName, this);
            popupController = CreateDefaultPopup(popupName);

            if (popupController)
            {
                popupController.gameObject.name = popupName;
                
                if (_dictPopupControllers.ContainsKey(popupName))
                    _dictPopupControllers[popupName] = popupController;
                else
                    _dictPopupControllers.Add(popupName, popupController);
                
                UIPopup popup = popupController.GetComponent<UIPopup>();
                popup.Initialize(ps);
                if (_dictPopup.ContainsKey(popupName))
                    _dictPopup[popupName] = popup;
                else
                    _dictPopup.Add(popupName, popup);
            }
           
            return popupController;
        }

        private UIPopupController CreateDefaultPopup(string name)
        {
//            GameObject go = Resources.Load<GameObject>(POPUP_PREFAB_LOCATION + name.ToString());
//            if (!go)
//            {
//                Debug.LogError("Cannot load resource " + name);
//                throw new System.Exception(POPUP_PREFAB_LOCATION + name.ToString() + " not found!");
//            }
//
            var popupData = database.Database.Find(p => p.name == name);
            var go = GameObject.Instantiate(popupData.prefab) as GameObject;
            UIPopupController popup = go.GetComponent<UIPopupController>();
            if (popup == null)
            {
                Destroy(go);
                Debug.LogError("Cannot instance " + name);
                throw new System.Exception( name.ToString() + " doesn't have component UIPopupController!");
            }
            else
            {
                popup.Initialize(this);
                popup.transform.SetParent(popupsContainer, false);
            }
            return popup;

            
        }
	    
	    public void ShowMessageDialog(string title,
	        string message,
	        UIMessageBox.MessageBoxType style = UIMessageBox.MessageBoxType.OK,
	        UIMessageBox.OnMessageBoxAction callback = null)
	    {
	        //HideLoading ();//hide loading anytime call show message box
	        
	        UIMessageBox.MessageBoxParam ps = new UIMessageBox.MessageBoxParam();
	        ps.MessageTitle = title;
	        ps.MessageBody = message;
	        ps.MessageBoxType = style;
	        ps.OnMessageBoxActionCallback = callback;
	        ShowPopup("MessageBox", ps);
	    }


        #endregion

        #region Private Methods



        private void Initialize()
        {
         
            
//            _dictPopupControllers = new Dictionary<string, UIPopupController>();
//            
//            //UIPopupController[] dialogControllers = transform.GetComponentsInChildren<UIPopupController>(true);
//            foreach (var dlg in database.Database)
//            {
//                //dlg.Initialize(this);
//                _dictPopupControllers.Add(dlg.name, dlg);
//            }
        }
        
        IEnumerator WaitToShow(UIPopupController controller, object ps = null)
        {
            while (controller.IsHidding())
            {
                yield return null;
            }
            controller.Show(ps);
        }
        
        
       
        #endregion

        #region Virtual Methods
        #endregion

        #region Static Methods
        #endregion

	


	}
}