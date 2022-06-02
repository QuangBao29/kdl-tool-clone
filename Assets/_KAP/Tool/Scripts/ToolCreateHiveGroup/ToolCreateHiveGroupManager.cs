using System;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;
using Pathfinding.Serialization.JsonFx;

namespace KAP.ToolCreateMap
{
    public class ToolCreateHiveGroupManager : MonoBehaviour
    {
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField]
        private ToolCreateMapImportDeco _importDecoController = null;
        [SerializeField]
        private ToolCreateMapListDecos _lstDecosController = null;

        [SerializeField]
        private int _hiveRoomSize = 18;

        private void Start()
        {
            _importDecoController.CreateARoom(0, Vector3.zero, new Vector3(_hiveRoomSize, _hiveRoomSize));
            _lstDecosController.Setup();
        }

        public void OnButtonCopyClick()
        {
            var data = _areaManager.Export(null, (deco)=> {
                return deco.PieceParent != null;
            });

            GUIUtility.systemCopyBuffer = JsonWriter.Serialize(data);
            Debug.LogError(GUIUtility.systemCopyBuffer);
        }

        public void OnButtonPasteClick()
        {
            OnButtonClearClick();
            var json = GUIUtility.systemCopyBuffer;
            if (string.IsNullOrEmpty(json))
                return;
            try
            {
                var data = JsonReader.Deserialize<Dictionary<string, DecoDataArray[]>>(json);
                _importDecoController.Import(data);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        public void OnButtonClearClick()
        {
            _areaManager.ClearAllRooms();
            _importDecoController.CreateARoom(0, Vector3.zero, new Vector3(_hiveRoomSize, _hiveRoomSize));
        }
    }

}
