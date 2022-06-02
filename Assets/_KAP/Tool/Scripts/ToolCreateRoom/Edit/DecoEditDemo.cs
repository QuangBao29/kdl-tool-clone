using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;
using Kawaii.IsoTools;
using Fingers;
using Kawaii.IsoTools.DecoSystem;

namespace KAP
{
    public enum KHHEditStatus
    {
        Valid = 0,
        Invalid = 1,
        CanSwap = 2
    }

    public class DecoEditDemo : DecoEdit
    {
        [SerializeField]
        private EditManager _editManager = null;
        [SerializeField]
        private SGPanZoom _panZoomSystem = null;
        public void OnTapEvent(GestureRecognizer gesture)
        {
            switch (gesture.State)
            {
                case GestureRecognizerState.Ended:
                    _editManager.SetCurrent(this);
                    break;
            }
        }

        public void OnPanEvent(GestureRecognizer gesture)
        {
            switch (gesture.State)
            {
                case GestureRecognizerState.Began:
                    if (_editManager.Current == this)
                    {
                        StartMove();
                        _editManager.editTool.StartMove();
                    }
                    break;
                case GestureRecognizerState.Executing:
                    if (_editManager.Current == this)
                        Move(Camera.main.ScreenToWorldPoint(new Vector3(gesture.FocusX, gesture.FocusY)));
                    break;
                case GestureRecognizerState.Ended:
                    if (_editManager.Current == this)
                    {
                        EndMove();
                        _editManager.editTool.SetValid(EditStatus);
                        _editManager.editTool.EndMove();
                    }
                    break;
            }
        }

        public override void StartMove()
        {
            _panZoomSystem.IsLockedPan = true;
            deco.Apply(null, null);
            deco.transform.SetParent(_editManager.transform);
        }

        public override void Move(Vector3 worldPos)
        {
            var position = worldPos;
            position.y += 1;
            deco.transform.position = position;
        }

        public override void EndMove()
        {
            var isoPos = IsoWorld.WorldToIso(deco.transform.position, deco.Position.z);
            isoPos.x = Mathf.Round(isoPos.x);
            isoPos.y = Mathf.Round(isoPos.y);
            deco.Position = isoPos;
            _panZoomSystem.IsLockedPan = false;
            var moveData = _areaManager.Move(deco);
            if (moveData != null)
            {
                _lstOverlaps = moveData.ListOverlaps;
                deco.Apply(moveData.piece, moveData.overlapPieces);
            }
        }

        public override void Rotate(int delta)
        {
            deco.WorldDirect = Calculator.Rotate(deco.WorldDirect, delta);
            deco.Apply(null, null);
            var moveData = _areaManager.Move(deco);
            if (moveData != null)
            {
                _lstOverlaps = moveData.ListOverlaps;
                deco.Apply(moveData.piece, moveData.overlapPieces);
            }

            _editManager.editTool.Show();
            _editManager.editTool.SetValid(EditStatus);
        }

        public KHHEditStatus EditStatus
        {
            get
            {
                if (_lstOverlaps != null)
                {
                    if (_lstOverlaps.Count > 1)
                        return KHHEditStatus.Invalid;
                    else if (_lstOverlaps.Count == 1)
                        return KHHEditStatus.CanSwap;
                }
                if (deco.PieceParent == null)
                    return KHHEditStatus.Invalid;
                return KHHEditStatus.Valid;
            }
        }
    }
}

