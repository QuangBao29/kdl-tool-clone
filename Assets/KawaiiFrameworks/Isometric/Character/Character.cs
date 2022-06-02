using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;
using UnityEngine.Rendering;

namespace Kawaii.IsoTools.CharacterSystem
{
    public class Character : IsoObject
    {
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField]
        private SortingGroup _sortingGroup = null;
        [SerializeField]
        private Transform _transFL = null;
        [SerializeField]
        private Transform _transBR = null;

        private int _worldDirect;
        private AreaPiece _areaPiece = null;
        private DecoInteraction.Point _interactPoint = null;

        bool IsInRoom(DecoRoot room)
        {
            var roomPos = room.Position;
            roomPos.z += 1;
            return IsoUtils.IsRect3DContainsAPoint(_position, roomPos, room.Size);
        }

        public override Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                base.Position = value;
                if (_areaManager == null)
                    return;
                if (_areaPiece != null)
                {
                    if (IsInRoom(_areaPiece.Root))
                    {
                        _areaPiece.Sort();
                        return;
                    }
                    _areaPiece.RemoveObject(this);
                    _areaPiece = null;
                }

                Transform transRoom = null;
                if(_areaPiece == null)
                {
                    var lstRoom = _areaManager.ListRooms;
                    foreach(var room in lstRoom)
                    {
                        if (IsInRoom(room))
                        {

                            foreach(var piece in room.LstAreaPieces)
                            {
                                if(piece.SortingLayerName == SortingLayerName)
                                {
                                    _areaPiece = piece;
                                    piece.AddObject(this);
                                    piece.Sort();
                                    transRoom = piece.Container;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                transform.SetParent(transRoom);
            }
        }

        public int WorldDirect
        {
            get
            {
                return _worldDirect;
            }
            set
            {
                _worldDirect = value;
                switch(_worldDirect)
                {
                    case IsoDirect.FL:
                        _transFL.gameObject.SetActive(true);
                        _transBR.gameObject.SetActive(false);
                        _transFL.localScale = new Vector3(1, 1, 1);
                        break;
                    case IsoDirect.FR:
                        _transFL.gameObject.SetActive(true);
                        _transBR.gameObject.SetActive(false);
                        _transFL.localScale = new Vector3(-1, 1, 1);
                        break;
                    case IsoDirect.BL:
                        _transFL.gameObject.SetActive(false);
                        _transBR.gameObject.SetActive(true);
                        _transBR.localScale = new Vector3(-1, 1, 1);
                        break;
                    case IsoDirect.BR:
                        _transFL.gameObject.SetActive(false);
                        _transBR.gameObject.SetActive(true);
                        _transBR.localScale = new Vector3(1, 1, 1);
                        break;
                }
            }
        }

        public override int SortingLayerOrder
        {
            get
            {
                return _sortingGroup.sortingOrder;
            }
            set
            {
                _sortingGroup.sortingOrder = value;
                OnSortingOrderChange?.Invoke(value);
            }
        }

        public override string SortingLayerName
        {
            get
            {
                return _sortingGroup.sortingLayerName;
            }
            set
            {
                _sortingGroup.sortingLayerName = value;
                OnSortingLayerChange?.Invoke(value);
            }
        }

        public void RotateToPos(Vector2 goalPos)
        {
            float x = goalPos.x - _position.x;
            float y = goalPos.y - _position.y;

            if (x > 0.05f)
            {
                if (y >= 0.05f)
                    WorldDirect = IsoDirect.BL;
                else
                    WorldDirect = IsoDirect.BR;

            }
            else if (x < -0.05f)
            {
                if (y <= -0.05f)
                    WorldDirect = IsoDirect.FR;
                else
                    WorldDirect = IsoDirect.FL;
            }
            else if (y > 0.05f)
            {
                WorldDirect = IsoDirect.BL;
            }
            else
                WorldDirect = IsoDirect.FR;
        }

        public DecoInteraction.Point InteractPoint
        {
            get
            {
                return _interactPoint;
            }
            set
            {
                if (_interactPoint != null)
                    _interactPoint.UnInteract();
                _interactPoint = value;
                if (_interactPoint != null)
                    _interactPoint.Interact(this);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(Input.GetKey(KeyCode.UpArrow))
            {
                var pos = Position;
                pos.x += 0.1f;
                RotateToPos(pos);
                Position = pos;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                var pos = Position;
                pos.x -= 0.1f;
                RotateToPos(pos);
                Position = pos;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                var pos = Position;
                pos.y += 0.1f;
                RotateToPos(pos);
                Position = pos;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                var pos = Position;
                pos.y -= 0.1f;
                RotateToPos(pos);
                Position = pos;
            }
        }
#endif
    }
}

