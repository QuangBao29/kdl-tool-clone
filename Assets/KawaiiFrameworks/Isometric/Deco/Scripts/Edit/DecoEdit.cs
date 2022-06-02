using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.IsoTools.DecoSystem
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Deco))]
    public class DecoEdit : MonoBehaviour
    {
        [SerializeField]
        protected AreaManager _areaManager;

        public Deco deco;
        protected List<Deco> _lstOverlaps; 

        private void Start()
        {
            if (deco == null)
                deco = GetComponent<Deco>();
        }

        public virtual void StartMove()
        {
            deco.Apply(null, null);
        }

        public virtual void Move(Vector3 worldPos)
        {
            deco.transform.position = worldPos;
        }

        public virtual void EndMove()
        {
            if (_areaManager == null)
                return;
            var isoPos = IsoWorld.WorldToIso(deco.transform.position, deco.Position.z);
            isoPos.x = Mathf.Round(isoPos.x);
            isoPos.y = Mathf.Round(isoPos.y);
            deco.Position = isoPos;
            var moveData = _areaManager.Move(deco);
            if (moveData != null)
            {
                _lstOverlaps = moveData.ListOverlaps;
                deco.Apply(moveData.piece, moveData.overlapPieces);
            }
        }

        public virtual void Rotate(int delta)
        {
            if (_areaManager == null)
                return;
            deco.WorldDirect = Calculator.Rotate(deco.WorldDirect, delta);
            deco.Apply(null, null);

            var moveData = _areaManager.Move(deco);
            if (moveData != null)
            {
                _lstOverlaps = moveData.ListOverlaps;
                deco.Apply(moveData.piece, moveData.overlapPieces);
            }
        }

        public List<Deco> ListOverlaps
        {
            get
            {
                return _lstOverlaps;
            }
        }
    }
}

