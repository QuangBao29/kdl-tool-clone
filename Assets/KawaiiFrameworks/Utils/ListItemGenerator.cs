using Imba.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kawaii.Utils
{
    public class ListItemGenerator : MonoBehaviour
    {
        [SerializeField]
        private Transform _container = null;

        [Tooltip("The first item is Prefab")]
        [SerializeField]
        private List<GameObject> _lstAllItems = null;

        private List<GameObject> _lstActiveItems = new List<GameObject>();

        public List<T> Setup<T>(int totalItem)
        {
            if (_lstAllItems.Count <= 0)
            {
                Debug.LogError(string.Format("[ListItemGenerator]: need at least 1 item for prefab!"));
                return new List<T>();
            }

            // instance more item if not enough
            if (_lstAllItems.Count < totalItem)
            {
                int needMore = totalItem - _lstAllItems.Count;
                for (int i = 0; i < needMore; i++)
                {
                    GameObject newItem = Instantiate<GameObject>(_lstAllItems[0], _container, false);
                    _lstAllItems.Add(newItem);
                }
            }

            // setup Items
            _lstActiveItems.Clear();
            List<T> lstItem = new List<T>();
            for (int i = 0; i < _lstAllItems.Count; i++)
            {
                _lstAllItems[i].SetActive(false);
                if (i < totalItem)
                {
                    _lstAllItems[i].SetActive(true);
                    _lstActiveItems.Add(_lstAllItems[i]);
                    lstItem.Add(_lstAllItems[i].GetComponent<T>());
                }
            }

            return lstItem;
        }

        public List<GameObject> Setup(int totalItem)
        {
            if (_lstAllItems.Count <= 0)
            {
                Debug.LogError(string.Format("[ListItemGenerator]: need at least 1 item for prefab!"));
                return new List<GameObject>();
            }

            // instance more item if not enough
            if (_lstAllItems.Count < totalItem)
            {
                int needMore = totalItem - _lstAllItems.Count;

                //var btn = _lstAllItems[0].GetComponent<Button>();
                //if (btn != null)
                //    btn.interactable = true;
                var effectGroup = _lstAllItems[0].GetComponent<UISGTab>();
                if (effectGroup != null)
                    effectGroup.Interaction = true;

                for (int i = 0; i < needMore; i++)
                {
                    GameObject newItem = Instantiate<GameObject>(_lstAllItems[0], _container, false);
                    _lstAllItems.Add(newItem);
                }
            }

            // setup Items
            _lstActiveItems.Clear();
            for (int i = 0; i < _lstAllItems.Count; i++)
            {
                _lstAllItems[i].SetActive(false);
                if (i < totalItem)
                {
                    _lstAllItems[i].SetActive(true);
                    _lstActiveItems.Add(_lstAllItems[i]);
                }
            }

            return _lstActiveItems;
        }
    }
}