using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.IsoTools.DecoSystem
{
    public class AreaPieceWall : AreaPiece
    {
        public AreaPieceWall(string name) :base(name)
        {
            IsWall = true;
        }

        public override void Setup(Deco parentDeco, Transform container, AreaParameters parameters)
        {
            if (parentDeco == null)
                return;
            base.Setup(parentDeco, container, parameters);
            FLIsoSize = new Vector2(-FlLocalIsoPos.x, _flPieceSize.x);
            _worldDirect = IsoDirect.FL;
            LocalDirectMultiply = 1;
        }

        public override Vector2 PieceSize
        {
            get
            {
                return _flPieceSize;
            }
        }

        public Vector2 GetReflectPosFromNoZWorldIsoPos(Vector3 noZPos)
        {
            switch (_worldDirect)
            {
                case IsoDirect.FL:
                case IsoDirect.BR:
                    var deltaX = X - noZPos.x;
                    return new Vector3(noZPos.y + deltaX, -deltaX);
                default:
                    var deltaY = Y - noZPos.y;
                    return new Vector3(noZPos.x + deltaY, -deltaY);
            }
        }

        public Vector2 GetReflectSizeFromWorldSize(Vector3 size)
        {
            switch (_worldDirect)
            {
                case IsoDirect.FL:
                case IsoDirect.BR:
                    return new Vector2(size.y, size.z);
                default:
                    return new Vector2(size.x, size.z);
            }
        }

        public override bool ContainsPoint(Vector2 noZPos)
        {
            return IsoUtils.IsRect2DContainsAPoint(GetReflectPosFromNoZWorldIsoPos(noZPos), GetReflectPosFromNoZWorldIsoPos(NoZIsoPos), _flPieceSize);
        }

        public override bool IsCollideWithDeco(Vector3 noZDecoPos, Vector3 decoSize)
        {
            return IsoUtils.Collision(GetReflectPosFromNoZWorldIsoPos(noZDecoPos), GetReflectSizeFromWorldSize(decoSize), GetReflectPosFromNoZWorldIsoPos(NoZIsoPos), _flPieceSize);
        }

        public override void AddChild(Deco child)
        {
            if (child == null)
                return;
            child.transform.SetParent(transform);
            _lstChilds.Add(child);

            var childNoZPos = child.Position;
            if (System.Math.Abs(childNoZPos.z) > 0.01f)
                childNoZPos = IsoUtils.RemoveZOfPosition(childNoZPos);
            if(X != int.MinValue)
            {
                var deltaX = X - childNoZPos.x;
                var parsePos = new Vector3(X, childNoZPos.y + deltaX, -deltaX);
                child.Position = parsePos;
            }
            else
            {
                var deltaY = Y - childNoZPos.y;
                var parsePos2 = new Vector3(childNoZPos.x + deltaY, Y, -deltaY);
                child.Position = parsePos2;
            }

            if (UseRootSorting)
            {
                var root = Root;
                if (root != null)
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
            if(child == null)
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
            var noZDecoPos = IsoUtils.RemoveZOfPosition(deco.Position);
            Vector2 decoReflectPos = GetReflectPosFromNoZWorldIsoPos(noZDecoPos);
            Vector2 decoReflectSize = GetReflectSizeFromWorldSize(deco.Size);

            foreach (var child in _lstChilds)
            {
                var childReflectPos = GetReflectPosFromNoZWorldIsoPos(IsoUtils.RemoveZOfPosition(child.Position));
                var childReflectSize = GetReflectSizeFromWorldSize(child.Size);
                if (IsoUtils.Collision(decoReflectPos, decoReflectSize, childReflectPos, childReflectSize))
                    lst.Add(child);
            }
            foreach (var child in _lstOverlapChilds)
            {
                var childReflectPos = GetReflectPosFromNoZWorldIsoPos(IsoUtils.RemoveZOfPosition(child.Position));
                var childReflectSize = GetReflectSizeFromWorldSize(child.Size);
                if (IsoUtils.Collision(decoReflectPos, decoReflectSize, childReflectPos, childReflectSize))
                    lst.Add(child);
            }
            return lst;
        }

        public override void Log()
        {
            Debug.LogError(string.Format(" Parent name: {0} - Direct: {9} - Layer: {8} - X: {1} - Y: {2} - Size: {6} - Piece Size: {7} \n FLLocalPos: {3} - NoZPosition: {4} - WorldPos: {5}", 
                DecoParent.name, X, Y, FlLocalIsoPos, NoZIsoPos, WorldIsoPos, WorldIsoSize, PieceSize, Face, _worldDirect));
        }
    }
}
