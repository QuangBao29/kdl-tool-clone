using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Imba.UI
{
    public class UIViewManager : MonoBehaviour
    {
        private Dictionary<string, UIViewController> _dictUiView = new Dictionary<string, UIViewController>();
        // public Dictionary<string, UIViewController> DicUiView
        // {
        //     get { return _dictUiView; }
        // }

        private Stack<string> _lastShownView;
        private Stack<string> _lastHiddenView;
        
        public Stack<string> LastShownView
        {
            get
            {
                if (_lastShownView == null) _lastShownView = new Stack<string>();

                // Debug.Log("In stack ");
                // foreach (var a in _lastShownView)
                // {
                //     Debug.Log("" + a);
                // }
                // Debug.Log("==========");
                
                return _lastShownView;
            }
        }
        
        private void Awake() {
            Initialize();
        }

        #region Public Method

        public void InitAllViews()
        {
            foreach (var view in _dictUiView)
            {
                view.Value.View.Initialize();
            }
        }
        
        public UIViewController ShowView(string viewName, object ps = null, bool isBack = false)
        {
            UIViewController viewController = GetViewControllerByName(viewName);
            if(viewController == null)
            {
                return null;
            }

            HideOthersView(viewName);
            viewController.Show(ps, isBack);

            return viewController;
        }

        public void HideOthersView(string viewName)
        {
            foreach(var view in _dictUiView)
            {
                if (view.Key != viewName && view.Value.View.IsVisible())
                {
                    view.Value.Hide();
                }
            }
        }

        public void HideView(string viewName, bool instantHide = false)
        {
            UIViewController viewController = GetViewControllerByName(viewName);
            if(viewController == null)
            {
                return;
            }
            viewController.Hide(instantHide);
        }
        #endregion
        public UIViewController GetViewControllerByName(string viewName)
        {
            UIViewController viewController;
            if(!_dictUiView.TryGetValue(viewName,out viewController))
            {
                Debug.LogError("No ViewController with name " + viewName);
            }
            return viewController;
        }
        
        public T GetViewByName<T>(string viewName) where T:UIView
        {
            return GetViewControllerByName(viewName).View as T;
        }

        private void Initialize()
        {
            _dictUiView = new Dictionary<string, UIViewController>();
            UIViewController[] viewController = GetComponentsInChildren<UIViewController>(true);
            foreach(var view in viewController)
            {
                view.Initialize(this);
                _dictUiView.Add(view.ViewName, view);
            }
        }    
    }
}
