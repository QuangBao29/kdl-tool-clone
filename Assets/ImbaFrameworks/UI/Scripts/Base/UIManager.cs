// Copyright (c) 2015 - 2019 Imba
// Author: Kaka
// Created: 2019/08


using UnityEngine;

using UnityEngine.SceneManagement;

using Imba.Utils;



namespace Imba.UI
{
    /// <summary>
    /// UIManager: Manage all UI element include Popups, Views, Notices, Alert ...
    /// </summary>
	public class UIManager : ManualSingletonMono<UIManager>
    {
	    public bool showDebugLog = true;
	    
		public Camera UICamera;
		
		public RectTransform loadingObject;
        private UIAlertManager _alertManager;
        private UIPopupManager _popupManager;
        
		public bool IsShowingLoading {
			get {
				if (loadingObject) {
					return loadingObject.gameObject.activeSelf;
				}
				return false;
			}
		}

		#region Unity Methods

		public override void Awake()
		{
			base.Awake();
			
			if (!_popupManager)
			{
				_popupManager = GetComponentInChildren<UIPopupManager> ();
			}

			if(!_alertManager)
			{
                _alertManager = GetComponentInChildren<UIAlertManager>();
			}

			if (loadingObject) {
				loadingObject.gameObject.SetActive (false);
			}
			
			#if DISABLE_LOG
			Debug.unityLogger.logEnabled = false;
			#endif
		}
		
		void Start()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
		
		void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
	
		}
		
		
		#endregion

		#region  Public Methods

		public static UIPopup GetTopVisiblePopup()
		{
			return Instance._popupManager.GetTopVisiblePopup();
		}
		
		public static void ShowAlertMessage(string message, AlertType type = AlertType.Normal)
		{
			Instance._alertManager.ShowAlertMessage(message, type);
		}
		
		public static void ShowPopup(string popupName, object param = null)
		{
			Instance._popupManager.ShowPopup(popupName, param);
		}
		
		public static void ShowMessage(string title, string message, UIMessageBox.MessageBoxType type = UIMessageBox.MessageBoxType.OK, UIMessageBox.OnMessageBoxAction callback = null)
		{
			Instance._popupManager.ShowMessageDialog(title, message, type, callback);
		}
		
		public static void ShowLoading(float timeToHide = 0)
		{
			//Debug.Log ("ShowLoading");
			Instance.loadingObject.gameObject.SetActive(true);
			if(timeToHide <= 0) Instance.CancelInvoke("HideLoadingCallback");
			else Instance.Invoke("HideLoadingCallback", timeToHide);
		}

		public static void DebugLog<T>(string message, T com)
		{
#if UNITY_EDITOR
			if (!Instance || !Instance.showDebugLog) return;

			string msg = string.Format("[{0}] {1}", com != null ? com.GetType().ToString() : "", message);
			Debug.Log(string.Format("<color=blue>[UIManager][{0}] {1}</color>", com.GetType(), message));
#endif
		}

        public void DestroyAllPopups()
        {
            _popupManager.DestroyAllPopups();
        }

        public void HideLoading()
        {
            //Debug.Log ("HideLoading");
            Instance.CancelInvoke("HideLoadingCallback");
            Instance.loadingObject.gameObject.SetActive(false);
        }

        public UIPopup GetPopup(string popupName)
        {
            return _popupManager.GetPopup(popupName);
        }

        #endregion

        #region Private Methods

        void HideLoadingCallback()
		{
			loadingObject.gameObject.SetActive(false);
		}
		
		#endregion
	    
	 
    }
}

