using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fingers;
using Kawaii.IsoTools.DecoSystem;

namespace KAP.ToolCreateMap
{
    public class DemoColliderLayerItem : MonoBehaviour
    {
        [SerializeField] private AreaManager _areaManager = null;
        [SerializeField] private FingerEventRaycaster2D fingerRaycast = null;
        [SerializeField] private Toggle _toggle = null;
        [SerializeField] private Text _textName = null;
        [SerializeField] private Toggle _toggleShowSprites = null;
        private DemoColliderLayer _layer;

        public void Setup(DemoColliderLayer colLayer)
        {
            _layer = colLayer;
            _textName.text = colLayer.ToString();
        }

        public void OnButtonChangeToggleValue()
        {
            _toggle.isOn = !_toggle.isOn;
            Refresh();
        }

        public void Refresh()
        {
            if (!_toggle.isOn)
                fingerRaycast.IgnoreLayerMask |= (1 << _layer.GetHashCode());
            else
                fingerRaycast.IgnoreLayerMask &= ~(1 << _layer.GetHashCode());

            var lstDecos = _areaManager.GetDecos((deco) => {
                return deco.gameObject.layer == _layer.GetHashCode();
            });

            foreach (var deco in lstDecos)
                deco.Spr.enabled = _toggle.isOn || _toggleShowSprites.isOn;
        }
    }
}

