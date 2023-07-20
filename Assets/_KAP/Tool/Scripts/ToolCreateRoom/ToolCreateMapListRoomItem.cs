using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kawaii.IsoTools.DecoSystem;
using Kawaii.IsoTools;
using KAP.Tools;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapListRoomItem : MonoBehaviour
    {
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField]
        private ToolCreateMapListRooms _parentController = null;
        [SerializeField]
        private Toggle _toggleEnable = null;
        [SerializeField]
        private Toggle _toggleEnableDebug = null;
        [SerializeField]
        private InputField _inputRoomId = null;
        [SerializeField]
        private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField]
        private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;

        [Space]
        [SerializeField]
        private InputField _inputPosX = null;
        [SerializeField]
        private InputField _inputPosY = null;
        [SerializeField]
        private InputField _inputPosZ = null;

        [Space]
        [SerializeField]
        private InputField _inputSizeX = null;
        [SerializeField]
        private InputField _inputSizeY = null;
        [SerializeField]
        private InputField _roomOrder = null;

        [Space]
        [SerializeField]
        private Image _img = null;
        [SerializeField]
        private Color _selectColor = Color.white;

        private DecoRoot _room;
        private int _index;
        private bool _isSelect = false;

        public bool IsSelect
        {
            get => _isSelect;
            set => _isSelect = value;
        }

        public void Setup(DecoRoot room, int index, int roomOrder)
        {
            _index = index;
            _room = room;
            var roomInfo = (DecoInfo)room.Info;
            var roomId = roomInfo != null ? roomInfo.Id : 0;
            _inputRoomId.text = (roomId > 1000) ? roomId.ToString() : index.ToString(); // real roomId always > 1000

            _roomOrder.text = roomOrder.ToString();

            _inputPosX.text = room.Position.x.ToString();
            _inputPosY.text = room.Position.y.ToString();
            _inputPosZ.text = room.Position.z.ToString();

            _inputSizeX.text = room.Size.x.ToString();
            _inputSizeY.text = room.Size.y.ToString();
        }
        public void OnClickRoomItem()
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.OldRoom)
            {
                return;
            }
            if (IsSelect)
            {
                UnSelectRoomItem();
                _parentController.SetSelectedItem(null);
                _toolBubbleDecoSetting.OnUnselectedItems(_inputRoomId.text);
            }
            else
            {
                _parentController.OnUnselectAllItems();
                _img.color = _selectColor;
                IsSelect = true;
                _parentController.SetSelectedItem(this);
                _toolBubbleDecoSetting.OnAddToScrollRect(_inputRoomId.text);
            }
            _toolBubbleDecoSetting.OnSelectRootDecoItems();
        }
        public void UnSelectRoomItem()
        {
            IsSelect = false;
            _img.color = Color.white;
        }
        public string GetRoomOrder()
        {
            return _roomOrder.text;
        }
        public int GetRoomId()
        {
            var roomInfo = (DecoInfo)_room.Info;
            return roomInfo.Id;

        }

        public DecoInfo GetDecoInfo()
        {
            var roomInfo = (DecoInfo)_room.Info;
            return roomInfo;

        }

        public bool IsOn()
        {
            return _toggleEnable.isOn;
        }

        // ===============================================================
        #region On Button Click

        public void OnButtonXClick()
        {
            _parentController.RemoveARoom(_room);
        }

        public void OnButtonClearClick()
        {
            _room.RemoveChilds();
        }

        #endregion
        // ===============================================================
        #region UI Change Event

        public void OnButtonToogleDebugClick()
        {
            _toggleEnableDebug.isOn = !_toggleEnableDebug.isOn;
            _room.Spr.enabled = _toggleEnableDebug.isOn; 
        }

        public void OnButtonChangeToogleClick()
        {
            _toggleEnable.isOn = !_toggleEnable.isOn;
            if (_room == null)
                return;
            var lstPieceInRooms = _room.LstAreaPieces;
            foreach(var piece in lstPieceInRooms)
            {
                if (_toggleEnable.isOn)
                    piece.Unlock();
                else
                    piece.Lock();
            }
            _room.gameObject.SetActive(_toggleEnable.isOn);
        }



        public void OnInputPosChangeValue()
        {
            if (_room == null)
                return;
            int x = 0;
            int y = 0;
            int z = 0;
            int.TryParse(_inputPosX.text, out x);
            int.TryParse(_inputPosY.text, out y);
            int.TryParse(_inputPosZ.text, out z);

            _room.Position = new Vector3(x, y, z);
            _room.SetFLIsoPos(_room.Position);
            _room.Apply(null, null);
            _areaManager.SortRoom();
        }
        
        public void OnInputSizeChangeValue()
        {
            if (_room == null)
            {
                Debug.LogError(string.Format("Error! room is null, size: {0}-{1}", _inputSizeX.text, _inputSizeY.text));
                return;
            }
            
            int sizeX = 0;
            int sizeY = 0;
            int.TryParse(_inputSizeX.text, out sizeX);
            int.TryParse(_inputSizeY.text, out sizeY);
            _room.SetFLIsoSize(new Vector3(sizeX, sizeY, 0));
            _room.WorldDirect = IsoDirect.FL;
            var lstPieces = _room.LstAreaPieces;
            foreach (var piece in lstPieces)
            {
                var parameters = new AreaParameters
                {
                    Face = piece.Face,
                    SortingLayerName = piece.SortingLayerName,
                    FLLocalPos = piece.FlLocalIsoPos,
                    PieceSize = new Vector2(sizeX, sizeY),
                    UseRootSorting = false,
                    Alone = false,
                };
                piece.Setup(_room, piece.Container, parameters);
            }
            //Debug.LogError("check");
            _room.Apply(null, null);
            _areaManager.SortRoom();
        }

        public void OnInputRoomIdChange()
        {
            var room = _areaManager.ListRooms[_index];
            if (room != _room)
            {
                Debug.LogError("error");
            }
            var roomInfo = (DecoInfo)room.Info;
            var preRoomId = roomInfo.Id;
            roomInfo.Id = int.Parse(_inputRoomId.text);
            room.gameObject.name = roomInfo.Id.ToString();
            //Debug.LogError("pre vs cur " + preRoomId + " " + roomInfo.Id);
            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                if (root.Key.RoomId == preRoomId)
                {
                    root.Key.RoomId = roomInfo.Id;
                    root.Key.gameObject.name = roomInfo.Id.ToString();

                    foreach (var item in root.Value)
                    {
                        item.RoomId = root.Key.RoomId;
                        item.BubbleId = item.RoomId + "_" + item.GetIndex();
                    }
                    break;
                }
            }
            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                Debug.LogError("id: " + root.Key.RoomId);
            }
        }
     
        #endregion
    }
}

