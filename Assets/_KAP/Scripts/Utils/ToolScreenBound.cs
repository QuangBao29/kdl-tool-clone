using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;
using Kawaii.IsoTools;
using Fingers;

namespace KAP
{
    public class ToolScreenBound : MonoBehaviour
    {
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField]
        private SGPanZoom _sgPanZoom = null;
        [SerializeField]
        private ScreenBounding _screenBounding = null;

        public float FitCameraSize { get; private set; }
        private Vector2 _position;
        private Vector2 _size;
        private Vector2 _worldCenterPos;

        public void InitRoomPlay(int roomId, bool flySmooth = false)
        {
            Vector3 minPos = new Vector3(int.MaxValue, int.MaxValue);
            Vector3 maxPos = new Vector3(int.MinValue, int.MinValue);

            var allRooms = _areaManager.ListRooms;
            foreach (var room in allRooms)
            {
                var roomPos = room.Position;
                var roomMaxPos = roomPos + room.Size;
                if (roomPos.x < minPos.x)
                    minPos.x = roomPos.x;
                if (roomPos.y < minPos.y)
                    minPos.y = roomPos.y;
                if (roomMaxPos.x > maxPos.x)
                    maxPos.x = roomMaxPos.x;
                if (roomMaxPos.y > maxPos.y)
                    maxPos.y = roomMaxPos.y;
                break;
            }
            _position = minPos;
            _size = maxPos - minPos;

            var resolution = (float)Screen.height / Screen.width;
            var padding = 1f;
            var cell = 0.32f;

            float maxSizeX = Mathf.Max(_size.x, _size.y) * cell;
            float maxSizeY = maxSizeX * resolution;

            var boundCenterPos1 = IsoWorld.IsoToWorld(new Vector3(_position.x + maxSizeX / 2, _position.y + maxSizeX / 2, 3));
            _worldCenterPos = IsoWorld.IsoToWorld(new Vector2(_position.x + _size.x / 2, _position.y + _size.y / 2));
            _screenBounding.SetCenter(_worldCenterPos);
            var boundCenterPosDeltaX = boundCenterPos1.x - _worldCenterPos.x + padding;
            var maxBoundSize = new Vector2(maxSizeX + padding + boundCenterPosDeltaX, maxSizeY + padding * 2);

            _screenBounding.roomSize = maxBoundSize;
            _screenBounding.CalcMinMax();
            _sgPanZoom.ZoomMax = _sgPanZoom.GetZoomFromBoudSize(_screenBounding.roomSize) + 3f;

            FitCameraSize = _sgPanZoom.GetZoomFromBoudSize(_screenBounding.roomSize) + 1f;
            Debug.LogError("how many time su run?");
            _sgPanZoom.Zoom = FitCameraSize;
            _sgPanZoom.FlyTo(_worldCenterPos + new Vector2(0, 3f), flySmooth);
        }

        public void Init(bool flySmooth = false)
        {
            _screenBounding.roomSize = new Vector2(50, 50);
            _screenBounding.CalcMinMax();
        }

        public void Rotate()
        {
            var direct = _areaManager.WorldDirect;
            switch (direct)
            {
                case IsoDirect.FL:
                    _worldCenterPos = IsoWorld.IsoToWorld(new Vector2(_position.x + _size.x / 2, _position.y + _size.y / 2));
                    break;
                default:
                    _worldCenterPos = IsoWorld.IsoToWorld(new Vector2(_position.x + _size.y / 2, _position.y + _size.x / 2));
                    break;
            }
            _screenBounding.SetCenter(_worldCenterPos);
            _screenBounding.CalcMinMax();
        }

        public Vector2 WorldCenterPosition
        {
            get
            {
                return _worldCenterPos;
            }
        }
    }
}

