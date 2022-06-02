using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;

namespace Kawaii.IsoTools.CharacterSystem
{
    [RequireComponent(typeof(Deco))]
    public class DecoInteraction : MonoBehaviour
    {
        [Serializable]
        public class Point
        {
            public string Id;
            public string AnimTrigger;
            public int Direct;
            public Vector3 LocalPos;
            public Vector3 WorldPos;
            public Transform Container;
            public int SortingOrder;
            public Character Char { get; private set; }
            public Vector3 WorldStandingPos { get; private set; }

            public bool Interact(Character character)
            {
                if (Char != null || character == null)
                    return false;
                Char = character;
                WorldStandingPos = character.Position;
                character.Position = WorldPos;
                character.SortingLayerOrder = SortingOrder;
                character.WorldDirect = Direct;
                character.transform.SetParent(Container);
                return true;
            }

            public bool UnInteract()
            {
                if (Char == null)
                    return false;
                Char.Position = WorldStandingPos;
                Char = null;
                return true;
            }
        }

        [SerializeField]
        private Deco _deco = null;
        [SerializeField]
        private readonly List<Point> _lstPoints = new List<Point>();
        public ReadOnlyCollection<Point> ListPoints;

        private SpriteRenderer _sprEx = null;

        private int _index; 

        private void Awake()
        {
            _deco = GetComponent<Deco>();
            ListPoints = _lstPoints.AsReadOnly();
        }

        public void Setup(List<DecoInteractParam> lstParams, Sprite ex)
        {
            if (lstParams == null)
            {
                if (_sprEx != null)
                    _sprEx.gameObject.SetActive(false);
                return;
            }
            _lstPoints.Clear();
            _index = 0;
            var decoPos = _deco.Position;
            var maxSize = (int)(_deco.Size.x + _deco.Size.y); 
            foreach(var p in lstParams)
            {
                var wPos = decoPos + p.LocalPos;
                _lstPoints.Add(new Point
                {
                    Id = wPos.x + "_" + wPos.y + "_" + wPos.z,
                    AnimTrigger = p.AnimTrigger,
                    Direct = p.Direct,
                    LocalPos = p.LocalPos,
                    WorldPos = wPos,
                    Container = _deco.Spr.transform,
                    SortingOrder = maxSize - (int)Mathf.Min(p.LocalPos.x, p.LocalPos.y)
                });
            }

            //sprite ex
            if (ex != null)
            {
                if (_sprEx != null)
                    _sprEx.gameObject.SetActive(true);
                else
                {
                    var objEx = new GameObject();
                    objEx.name = _deco.name + "_ex";
                    var transEx = objEx.transform;
                    transEx.SetParent(_deco.Spr.transform);
                    transEx.localPosition = Vector3.zero;
                    transEx.localEulerAngles = Vector3.zero;
                    transEx.localScale = Vector3.one;
                    _sprEx = objEx.AddComponent<SpriteRenderer>();
                    _sprEx.sortingOrder = maxSize + 1;
                }
                _sprEx.sprite = ex;
            }
            else if (_sprEx != null)
                _sprEx.gameObject.SetActive(false);
        }

        public Point GetEmptyPoint()
        {
            if (_index >= _lstPoints.Count)
                return null;
            var i = 0;

            while(i < _lstPoints.Count)
            {
                var point = _lstPoints[_index];
                if (point.Char == null)
                    return point;
                _index++;
                if (_index >= _lstPoints.Count)
                    _index = 0;
                i++;
            }
            return null;
        }
    }

}
