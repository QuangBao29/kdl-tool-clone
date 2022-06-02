using System;
using System.Collections.Generic;
using UnityEngine;

namespace Imba.UI
{
    [Serializable]
    [CreateAssetMenu(fileName = "PopupDatabase", menuName = "Imba/Popup Database", order = 1)]
    public class UIPopupDatabase: ScriptableObject
    {
        [SerializeField]
        private List<UIPopupLink> database;
 
        public List<UIPopupLink> Database => database;
        
        public void Reset()
        {
            database = new List<UIPopupLink>();
        }
    }
}