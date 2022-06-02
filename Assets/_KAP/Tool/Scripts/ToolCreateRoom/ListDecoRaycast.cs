using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;
using OneP.InfinityScrollView;

namespace KAP.ToolCreateMap
{
    public class ListDecoRaycast : MonoBehaviour
    {
        [SerializeField]
        private ToolIsoFingerRaycaster _fingerRaycaster = null;
        [SerializeField]
        private EditManager _editManager = null;
        [SerializeField]
        private InfinityScrollView _scrollView = null;

        private readonly List<Deco> _lstDecos = new List<Deco>();

        public Deco GetDecoByIndex(int index)
        {
            if (index < 0 || index >= _lstDecos.Count)
                return null;
            return _lstDecos[index];
        }

        public void Show()
        {
            if (_editManager.Current == null)
            {
                Hide();
                return;
            }
            gameObject.SetActive(true);
            var lstCatchs = _fingerRaycaster.ListLastCatchs;
            _lstDecos.Clear();
            foreach(var trans in lstCatchs)
            {
                if (trans == null)
                    continue;
                var deco = trans.GetComponent<Deco>();
                if (deco != null)
                    _lstDecos.Add(deco);
            }
            _scrollView.Setup(_lstDecos.Count);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
        }
        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}


