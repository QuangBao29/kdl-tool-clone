using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kawaii.IsoTools.DecoSystem;
using Imba.Utils;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapManager : ManualSingletonMono<ToolCreateMapManager>
    {
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField]
        private EditManager _editManager = null;

        [Header("- Collider Layers:")]
        [SerializeField] private Transform _transGridColLayer = null;
        [SerializeField] private DemoColliderLayerItem _prefabColLayer = null;
        [SerializeField]
        private Toggle _toggleShowSpriteByLayers = null;
        [Header("- Rooms:")]
        [SerializeField] private ToolCreateMapListRooms _listRoomsController = null;
        [SerializeField] private ToolCreateMapListDecos _listDecoController = null;

        [SerializeField] private GameObject[] _decoEditObjects = null;

        [Space]
        [SerializeField] private Material _matOutline = null;
        [SerializeField] private Material _matDefault = null;

        private void Start()
        {
            _listDecoController.Setup();
            SetupCollider();
            _listRoomsController.OnButtonAddClick();

            OnChangeCurrentEditDeco();
        }

        private void SetupCollider()
        {
            var iLayer = 0;
            var layerItemCount = _transGridColLayer.childCount;
            foreach (var l in Enum.GetValues(typeof(DemoColliderLayer)))
            {
                var colLayer = (DemoColliderLayer)l;
                if (colLayer == DemoColliderLayer.None)
                    continue;
                DemoColliderLayerItem item = null;
                if (iLayer < layerItemCount)
                    item = _transGridColLayer.GetChild(iLayer++).GetComponent<DemoColliderLayerItem>();
                else
                    item = SGUtils.InstantiateObject(_prefabColLayer, _transGridColLayer);
                item.gameObject.SetActive(true);
                item.Setup(colLayer);
            }

            for (; iLayer < layerItemCount; ++iLayer)
                _transGridColLayer.GetChild(iLayer).gameObject.SetActive(false);
        }

   
        public void OnShowTileOutlineClick()
        {
            var lst = _areaManager.GetDecos((deco) => {
                var layer = deco.gameObject.layer;
                return layer == (int)DemoColliderLayer.Tile;
            });
            if (lst == null)
                return;
            foreach (var deco in lst)
            {
                if (deco.Spr.material == _matOutline)
                    deco.Spr.material = _matDefault;
                else
                    deco.Spr.material = _matOutline;
            }
        }



        public void Log()
        {
            _areaManager.Log();
        }
       
        // ========================================================================

        public void OnChangeCurrentEditDeco()
        {
            var cur = _editManager.Current;
            foreach (var obj in _decoEditObjects)
            {
                obj.SetActive(cur != null);
            }
        }

        public void OnToggleShowSpriteByLayerChange()
        {
            _toggleShowSpriteByLayers.isOn = !_toggleShowSpriteByLayers.isOn;
            var childCount = _transGridColLayer.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var item = _transGridColLayer.GetChild(i).GetComponent<DemoColliderLayerItem>();
                if (item.gameObject.activeSelf)
                {
                    item.Refresh();
                }
            }
        }
    }
}