using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kawaii.IsoTools.DecoSystem
{
    public class AreaPiece:IsoGroupSorting
    {
        public Area AreaParent;

        public bool IsWall { get; protected set; }
        public int Face { get; protected set; }
        public Deco DecoParent { get; protected set; }
        public Vector3 FlLocalIsoPos { get; protected set; }
        public Vector2 FLIsoSize { get; protected set; }

        public bool UseRootSorting { get; protected set; }
        public bool Alone { get; protected set; }
        public int LocalDirectMultiply { get; protected set; } = 1;

        public Transform Container { get; protected set; }
        public SortingGroup SortGroup { get; protected set; }
        public string SortingLayerName { get; protected set; }

        protected List<Deco> _lstChilds = new List<Deco>();
        protected List<Deco> _lstOverlapChilds = new List<Deco>();

        public ReadOnlyCollection<Deco> LstChilds;
        public ReadOnlyCollection<Deco> LstOverlapChilds;

        protected bool _isLock;
        protected Vector2 _flPieceSize;
        protected Vector3 _worldIsoPos;
        protected int _worldDirect;

        public AreaPiece(string name) : base(name)
        {
            LstChilds = _lstChilds.AsReadOnly();
            LstOverlapChilds = _lstOverlapChilds.AsReadOnly();
        }

        public virtual void Setup(Deco parentDeco, Transform container, AreaParameters parameters)
        {
            Face = parameters.Face;
            SortingLayerName = parameters.SortingLayerName;
            DecoParent = parentDeco;
            Container = container;
            UseRootSorting = parameters.UseRootSorting;
            Alone = parameters.Alone;
            if (!parameters.UseRootSorting)
                SortGroup = container.GetComponent<SortingGroup>();
            FlLocalIsoPos = parameters.FLLocalPos;
            _flPieceSize = parameters.PieceSize;
        }

        

        public Transform transform
        {
            get
            {
                return Container;
            }
        }

        public void Lock()
        {
            _isLock = true;
        }

        public void Unlock()
        {
            _isLock = false;
        }

        public bool IsLock
        {
            get
            {
                if (DecoParent.PieceParent != null)
                    return DecoParent.PieceParent.IsLock;
                return _isLock;
            }
        }

        #region Need Override

        public virtual DecoRoot Root
        {
            get
            {
                if (DecoParent != null)
                    return DecoParent.Root;
                return null;
            }
        }

        public virtual Vector3 WorldIsoPos
        {
            get
            {
                return _worldIsoPos;
            }
            set
            {
                _worldIsoPos = value;
            }
        }
        public virtual int WorldDirect
        {
            get
            {
                return _worldDirect;
            }
            set
            {
                _worldDirect = value;
            }
        }

        public virtual void Apply(int deltaDirect)
        {
            foreach (var child in _lstChilds)
            {
                if(deltaDirect != 0)
                    child.WorldDirect = Calculator.Rotate(child.WorldDirect, child.LocalDirectMultiply * deltaDirect);
                child.Position = Calculator.GetChildWorldPositionFollowParent(child.WorldDirect, child.FLLocalIsoPos, child.FLIsoSize, _worldDirect, _worldIsoPos, WorldIsoSize);
                child.Apply(this, null);
            }
            if (UseRootSorting)
            {
                var root = Root;
                if (root != null)
                    ((DecoRoot)root).SortIsoGroup(SortingLayerName);

            }
            else if(deltaDirect != 0)
                Sort();
        }

        public virtual Vector2 WorldIsoSize
        {
            get
            {
                switch (_worldDirect)
                {
                    case IsoDirect.FL:
                    case IsoDirect.BR:
                        return FLIsoSize;
                }
                return new Vector2(FLIsoSize.y, FLIsoSize.x);
            }
        }

        public virtual Vector2 PieceSize
        {
            get {
                switch (WorldDirect)
                {
                    case IsoDirect.FL:
                    case IsoDirect.BR:
                        return _flPieceSize;
                }
                return new Vector2(_flPieceSize.y, _flPieceSize.x);
            }
        }

        public virtual bool ContainsPoint(Vector2 noZPos)
        {
            return IsoUtils.IsRect2DContainsAPoint(noZPos, NoZIsoPos, PieceSize);
        }

        public virtual void AddChild(Deco child)
        {

        }

        public virtual bool RemoveChild(Deco child)
        {
            return false;
        }

        public virtual void AddOverlapChild(Deco child)
        {
            if (child == null)
                return;
            _lstOverlapChilds.Add(child);
        }

        public virtual bool RemoveOverlapChild(Deco child)
        {
            if (child == null)
                return false;
            return _lstOverlapChilds.Remove(child);
        }

        public virtual List<Deco> GetOverlapChilds(Deco deco)
        {
            return null;
        }

        public virtual bool IsCollideWithDeco(Vector3 noZDecoPos, Vector3 decoSize)
        {
            return IsoUtils.Collision(NoZIsoPos, PieceSize, noZDecoPos, decoSize);
        }

        public Vector3 NoZIsoPos
        {
            get
            {
                return IsoUtils.RemoveZOfPosition(_worldIsoPos);
            }
        }

        public int X
        {
            get
            {
                switch (_worldDirect)
                {
                    case IsoDirect.FL:
                    case IsoDirect.BR:
                        return (int)_worldIsoPos.x;
                }
                return int.MinValue;
            }
        }

        public int Y
        {
            get
            {
                switch (_worldDirect)
                {
                    case IsoDirect.FR:
                    case IsoDirect.BL:
                       return (int)_worldIsoPos.y;
                }
                return int.MinValue;
            }
        }

        public int Z
        {
            get
            {
                return (int)_worldIsoPos.z;
            }
        }

        public virtual void Log()
        {

        }


        #endregion
    }
}

