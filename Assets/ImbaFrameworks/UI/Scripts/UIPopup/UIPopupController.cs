// Copyright (c) 2015 - 2019 Imba
// Author: Kaka
// Created: 2019/08


using UnityEngine;

using Imba.UI.Animation;

namespace Imba.UI
{
    public enum AfterHideBehaviour
    {
        Disable,
        Destroy
    }

    //[Serializable]
    //[CreateAssetMenu(fileName = "PopupController", menuName = "Imba/PopupController", order = 1)]
    /// <summary>
    /// Control Popup behaviour include: Save, Load, Properties ...
    /// </summary>
	public class UIPopupController : MonoBehaviour
	{
        #region Constants
        //public static readonly string DEFAULT_RESOURCES_LOCATION = "Assets/_Overleague/Resources/";
       
        #endregion

        #region Static Fields
        #endregion

        #region Events
        #endregion

        #region Public Vars

        //public string PopupName;

        public AfterHideBehaviour AfterHideBehaviour;

        public bool AlwaysOnTop;
	    
	    public bool DeactiveGameObjectWhenHide;//Use enable canvas or set active object?

        public bool ShowOverlay = true;

	    public bool CloseByClickOutside = true;
	    
	    public bool CloseByBackButton = true;
	    
	    public bool DestroyOnLoadScene = true;
	    
	    public bool FadeContent = true;
	    
	    public UIPopupBehavior ShowBehavior = new UIPopupBehavior(AnimationType.Show);
	    
	    public UIPopupBehavior HideBehavior = new UIPopupBehavior(AnimationType.Hide);

        //public string CustomResourcesLocation = DEFAULT_RESOURCES_LOCATION;

        #endregion

        #region Private Vars
	    [SerializeField]
        private UIPopupManager _popupManager;//referrence to manager
        //private UIPopup Popup;//referrence to view
        #endregion

        #region Properties

        public UIPopup Popup
        {
            get
            {
                return GetComponent<UIPopup>();
            }
        }

        public UIPopupManager PopupManager { get => _popupManager;}
        #endregion

        #region Constructors
        #endregion

        #region Unity Methods
        #endregion

        #region Public Methods



        public void Initialize(UIPopupManager popupManager)
        {
            _popupManager = popupManager;
        }

        public bool IsHidding()
        {
            return (Popup && Popup.VisibilityState == VisibilityState.Hiding);
        }

        public void Show(object ps = null)
        {
            // Neu popup dang trong animation hiding
            if (IsHidding())
            {
                Debug.Log("Popup is hiding, pls wait");
                return;
            }

            //neu da co popup -> show
            if (Popup )
            {
                Popup.Show(ps);
            }
        }


        public void Hide(bool instantHide)
        {
            if (!Popup || Popup.VisibilityState != VisibilityState.Shown)
                return;

            Popup.Hide(instantHide);
        }

        public void DoDestroy()
        {
            if (Popup != null)
            {
                Debug.LogWarning("Destroy " + name);
                Destroy(Popup.gameObject);
            }
        }

        #endregion

        #region Private Methods

     
        #endregion

        #region Virtual Methods
        #endregion

        #region Static Methods
        #endregion


	}
}