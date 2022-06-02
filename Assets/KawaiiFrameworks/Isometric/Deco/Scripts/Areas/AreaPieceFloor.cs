using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.IsoTools.DecoSystem
{
    public class AreaPieceFloor : AreaPiece
    {
        public AreaPieceFloor(string name):base(name)
        {

        }

        public override void Setup(Deco parentDeco, Transform container, AreaParameters parameters)
        {
            if (parentDeco == null)
                return;
            base.Setup(parentDeco, container, parameters);
            _worldDirect = IsoDirect.FL;
            LocalDirectMultiply = 1;
            FLIsoSize = parameters.PieceSize;
        }

        public override void AddChild(Deco child)
        {
            if (child == null)
                return;
            _lstChilds.Add(child);
            child.transform.SetParent(transform);
            var childNoZPos = child.Position;
            if (System.Math.Abs(childNoZPos.z) > 0.01f)
                childNoZPos = IsoUtils.RemoveZOfPosition(childNoZPos);
            child.Position = IsoUtils.AddZToIsoPosition(childNoZPos, Z);

            if (UseRootSorting)
            {
                var root = Root;
                if(root != null)
                {
                    var decoRoot = (DecoRoot)root;
                    decoRoot.AddSortingObject(child);
                    decoRoot.SortIsoGroup(SortingLayerName);
                }
            }
            else
            {
                AddObject(child);
                Sort();
            }
        }

        public override bool RemoveChild(Deco child)
        {
            if (child == null)
                return false;
            if (_lstChilds.Remove(child))
            {
                child.transform.SetParent(null);
                child.Position = IsoUtils.RemoveZOfPosition(child.Position);
                if (UseRootSorting)
                {
                    var root = Root;
                    if (root != null)
                        root.RemoveSortingObject(child);
                }
                else
                    RemoveObject(child);
            }
            return false;
        }

        public override List<Deco> GetOverlapChilds(Deco deco)
        {
            List<Deco> lst = new List<Deco>();
            var lstDecoRect = deco.ListColliderRects;

            foreach (var child in _lstChilds)
            {
                var lstChildRect = child.ListColliderRects;
                if (IsoUtils.Collision(lstDecoRect, lstChildRect))
                    lst.Add(child);
            }

            foreach (var child in _lstOverlapChilds)
            {
                var lstChildRect = child.ListColliderRects;
                if (IsoUtils.Collision(lstDecoRect, lstChildRect))
                    lst.Add(child);
            }
            return lst;
        }

        public override void Log()
        {
            Debug.LogError(string.Format("Layer Name: {0} - Z: {1} - Face: {6} \n FLLocalIsoPos: {2} - WorldIsoPos: {3} - NoZPosition: {4} \n Size: {5}",
            _name, Z,FlLocalIsoPos, WorldIsoPos, NoZIsoPos, PieceSize, Face));
        }
    }
}

