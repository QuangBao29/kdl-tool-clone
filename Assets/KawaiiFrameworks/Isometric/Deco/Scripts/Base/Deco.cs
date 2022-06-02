using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kawaii.IsoTools.DecoSystem
{
    [ExecuteInEditMode]
    public class Deco : IsoObject
    {
        [SerializeField]
        protected AreaManager _areaManager = null;
        [SerializeField]
        protected SpriteRenderer _spr = null;
        [SerializeField]
        protected BoxCollider2D _boxCol = null;
        [SerializeField]
        protected int _group;
        protected int _worldDirect;
        protected DecoRoot _root;
        public IDecoInfo Info;

        #region
        public bool IsWallHang { get; protected set; }
        public int CanInFaces { get; protected set; }
        public Vector3 FLLocalIsoPos { get; protected set; }
        public Vector3 FLIsoSize { get; protected set; }
        public int LocalDirectMultiply { get; protected set; } = 1;

        public AreaPiece PieceParent { get; protected set; }
        protected List<AreaPiece> _lstOverlapPieces = new List<AreaPiece>();
        public List<AreaPiece> LstAreaPieces { get; private set; } = new List<AreaPiece>();
        protected int _oldDirect = IsoDirect.FL;

        protected Sprite _sprFL;
        protected Sprite _sprBR;

        protected List<IsoRect> _lstFLLocalColliderRects = new List<IsoRect>();
        #endregion

        public virtual void SetupParameters(DecoParameters parameters)
        {
            IsWallHang = parameters.IsWallHang;
            FLIsoSize = parameters.FLSize;
            SortingLayerName = parameters.SortingLayerName;
            SetupSprite(parameters.FLSprite, parameters.BRSprite);
            CanInFaces = parameters.CanInFaces;
            BoxCol.gameObject.layer = parameters.ColliderLayer;
            _lstFLLocalColliderRects = parameters.ListColliderRects;
            if (parameters.ListAreas != null)
                foreach (var pieceData in parameters.ListAreas)
                {
                    AddAreaPiece(pieceData);
                }
            Refresh();
        }

        protected virtual void Refresh()
        {
            switch (_worldDirect)
            {
                case IsoDirect.FL:
                    _spr.sprite = _sprFL;
                    _spr.flipX = false;
                    Size = FLIsoSize;
                    break;
                case IsoDirect.FR:
                    _spr.sprite = _sprFL;
                    _spr.flipX = true;
                    Size = new Vector3(FLIsoSize.y, FLIsoSize.x, FLIsoSize.z);
                    break;
                case IsoDirect.BL:
                    _spr.sprite = _sprBR;
                    _spr.flipX = true;
                    Size = new Vector3(FLIsoSize.y, FLIsoSize.x, FLIsoSize.z);
                    break;
                case IsoDirect.BR:
                    _spr.sprite = _sprBR;
                    _spr.flipX = false;
                    Size = FLIsoSize;
                    break;
            }
        
            _boxCol.size = _spr.bounds.size;
            Vector3 pivotOffsetWorldSpace = _spr.bounds.center - _spr.transform.position;
            pivotOffsetWorldSpace.x *= _spr.transform.localScale.x;
            _boxCol.offset = pivotOffsetWorldSpace;
        }

        #region Direction
        public virtual int WorldDirect
        {
            get
            {
                return _worldDirect;
            }
            set
            {
                if(_sprFL != _sprBR)
                    _worldDirect = value;
                else
                {
                    if (value == IsoDirect.BR)
                        _worldDirect = IsoDirect.FL;
                    else if (value == IsoDirect.BL)
                        _worldDirect = IsoDirect.FR;
                    else
                        _worldDirect = value;
                }
                Refresh();
            }
        }

        #endregion

        #region Sprite

        public virtual void SetupSprite(Sprite fl, Sprite br)
        {
            _sprFL = fl;
            _sprBR = br;
        }

        #endregion

        #region Area
       
        public void AddAreaPiece(AreaParameters pieceParams)
        {
            AreaPiece piece = null;
            GameObject obj = new GameObject();
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.name = pieceParams.SortingLayerName;
            if (!pieceParams.UseRootSorting)
            {
                var pieceContainer = obj.AddComponent<SortingGroup>();
                pieceContainer.sortingLayerName = pieceParams.SortingLayerName;
                SortStep++;
            }
            if (pieceParams.IsWall)
                piece = new AreaPieceWall(obj.name);
            else
                piece = new AreaPieceFloor(obj.name);

            piece.Setup(this, obj.transform, pieceParams);
            LstAreaPieces.Add(piece);
        }
        #endregion

        #region Sorting Order
        public override int SortingLayerOrder
        {
            get
            {
                return _spr.sortingOrder;
            }
            set
            {
                _spr.sortingOrder = value + _spr.transform.GetSiblingIndex();
                foreach(var piece in LstAreaPieces)
                {
                    if (!piece.UseRootSorting)
                    {
                        piece.SortGroup.sortingOrder = value + piece.Container.GetSiblingIndex();
                    }
                }
                OnSortingOrderChange?.Invoke(_spr.sortingOrder);
            }
        }

        public override string SortingLayerName
        {
            get
            {
                return _spr.sortingLayerName;
            }
            set
            {
                _spr.sortingLayerName = value;
                OnSortingLayerChange?.Invoke(value);
            }
        }

        void SortAreaPiece()
        {
            List<AreaPiece> lstFloors = new List<AreaPiece>();
            List<AreaPiece> lstWallBack = new List<AreaPiece>();
            List<AreaPiece> lstWallFront = new List<AreaPiece>();

            foreach (var piece in LstAreaPieces)
            {
                if (piece.IsWall)
                {
                    if (piece.WorldDirect < IsoDirect.BR)
                        lstWallFront.Add(piece);
                    else
                        lstWallBack.Add(piece);
                }
                else
                {
                    lstFloors.Add(piece);
                }
            }
            if (lstWallBack.Count > 0)
            {
                for (var i = 0; i < lstWallBack.Count; ++i)
                {
                    lstWallBack[i].Container.SetAsFirstSibling();
                }
            }

            if (lstWallFront.Count > 0)
            {
                for (var i = 0; i < lstWallFront.Count; ++i)
                {
                    lstWallFront[i].Container.SetAsLastSibling();
                }
            }

            if (lstFloors.Count > 0)
            {
                IsoUtils.BubbleSort<AreaPiece>(lstFloors, (p1, p2) => {
                    if (p2.Z == p1.Z)
                    {
                        return IsoUtils.IsFront(p2.WorldIsoPos, p2.WorldIsoSize, p1.WorldIsoPos, p1.WorldIsoSize);
                    }
                    return p2.Z > p1.Z;
                });


                for (var i = 0; i < lstFloors.Count; ++i)
                {
                    lstFloors[i].Container.SetAsLastSibling();
                }
            }
        }
        #endregion

        #region Apply
        public virtual void Apply(AreaPiece parent, List<AreaPiece> overlapPieces)
        {
            if (parent == null)
            {
                if (PieceParent != null)
                {
                    PieceParent.RemoveChild(this);
                    PieceParent = null;
                    foreach (var piece in _lstOverlapPieces)
                        piece.RemoveOverlapChild(this);
                    _lstOverlapPieces.Clear();
                }
            }
            else if (PieceParent != parent)
            {
                parent.AddChild(this);
                if (PieceParent == null)
                {
                    LocalDirectMultiply = Calculator.GetChildDirectMultiply(parent.WorldDirect, _worldDirect);
                    FLLocalIsoPos = Calculator.GetChildFLLocalPositionFollowParent(_worldDirect, Position, FLIsoSize, parent.WorldDirect, parent.WorldIsoPos, parent.WorldIsoSize);
                }
                PieceParent = parent;
            }

            if (overlapPieces != null)
            {
                foreach (var piece in _lstOverlapPieces)
                    piece.RemoveOverlapChild(this);
                _lstOverlapPieces = overlapPieces;
                foreach (var piece in _lstOverlapPieces)
                    piece.AddOverlapChild(this);
            }

            int deltaDirect = _worldDirect - _oldDirect;
            _oldDirect = _worldDirect;
            if (_areaManager != null)
                foreach (var areaPiece in LstAreaPieces)
                {
                    _areaManager.RemovePiece(areaPiece);
                    if (deltaDirect != 0)
                        areaPiece.WorldDirect = Calculator.Rotate(areaPiece.WorldDirect, areaPiece.LocalDirectMultiply * deltaDirect);
                    areaPiece.WorldIsoPos = Calculator.GetChildWorldPositionFollowParent(areaPiece.WorldDirect, areaPiece.FlLocalIsoPos, areaPiece.FLIsoSize, _worldDirect, _position, _size);
                    _areaManager.AddPiece(areaPiece);
                    areaPiece.Apply(deltaDirect);
                }

            SortAreaPiece();
        }
        #endregion

        #region Remove
        public void Remove()
        {
            if (_areaManager != null)
                foreach (var areaPiece in LstAreaPieces)
                {
                    _areaManager.RemovePiece(areaPiece);
                    while (areaPiece.LstChilds.Count > 0)
                        areaPiece.LstChilds[0].Remove();
                }
            if (PieceParent != null)
            {
                PieceParent.RemoveChild(this);
                PieceParent = null;
                foreach (var piece in _lstOverlapPieces)
                    piece.RemoveOverlapChild(this);
                _lstOverlapPieces.Clear();
            }

            Destroy(gameObject);
        }

        public List<Deco> RemoveOnlySelf()
        {
            List<Deco> lstChild = new List<Deco>();
            if (_areaManager != null)
                foreach (var areaPiece in LstAreaPieces)
                {
                    var lstChildInPiece = areaPiece.LstChilds;
                    while(lstChildInPiece.Count > 0)
                    {
                        var child = lstChildInPiece[0];
                        lstChild.Add(child);
                        child.Apply(null, null);
                    }
                    _areaManager.RemovePiece(areaPiece);
                }

            if (PieceParent != null)
            {
                PieceParent.RemoveChild(this);
                PieceParent = null;
                foreach (var piece in _lstOverlapPieces)
                    piece.RemoveOverlapChild(this);
                _lstOverlapPieces.Clear();
            }

            Destroy(gameObject);
            return lstChild;
        }

        public void RemoveChilds()
        {
            foreach (var areaPiece in LstAreaPieces)
            {
                if(areaPiece.LstChilds.Count > 0)
                {
                    while(areaPiece.LstChilds.Count > 0)
                    {
                        areaPiece.LstChilds[0].Remove();
                    }
                }
            }
        }
        #endregion

       

        public DecoRoot Root
        {
            get
            {
                if (PieceParent != null)
                    return PieceParent.Root;
                return _root;
            }
            set
            {
                _root = value;
            }
        }

        public int TreeLevel
        {
            get
            {
                int level = 0;
                var parent = PieceParent;
                while(parent != null)
                {
                    level++;
                    parent = parent.DecoParent.PieceParent;
                }
                return level;
            }
        }


        public SpriteRenderer Spr
        {
            get
            {
                return _spr;
            }
        }

        public BoxCollider2D BoxCol
        {
            get
            {
                return _boxCol;
            }
        }

        public int Group
        {
            get => _group;
            set
            {
                _group = value;
            }
        }

        #region Collider with other deco

        public virtual List<IsoRect> ListColliderRects
        {
            get
            {
                var lst = new List<IsoRect>();
                var noZPos = IsoUtils.RemoveZOfPosition(_position);
                if (_lstFLLocalColliderRects.Count == 0)
                {
                    lst.Add(new IsoRect(noZPos.x, noZPos.y, _size.x, _size.y));
                }
                else
                {
                    switch(_worldDirect)
                    {
                        case IsoDirect.FL:
                        case IsoDirect.BR:
                            foreach (var localRect in _lstFLLocalColliderRects)
                            {
                                lst.Add(new IsoRect(noZPos.x + localRect.PosX, noZPos.y + localRect.PosY, localRect.SizeX, localRect.SizeY));
                            }
                            break;
                        default:
                            foreach (var localRect in _lstFLLocalColliderRects)
                            {
                                lst.Add(new IsoRect(noZPos.x + localRect.PosY, noZPos.y + localRect.PosX, localRect.SizeY, localRect.SizeX));
                            }
                            break;
                    }
                }
                return lst;
            }
        }


        #endregion

        #region Export Data
        //public virtual DecoDataTree ExportDataTree(object param)
        //{
        //    var data = new DecoDataTree
        //    {
        //        Info = Info.ExportToJson(param),
        //        Position = new DecoVector3 { X = _position.x, Y = _position.y, Z = _position.z },
        //        WorldDirect = _worldDirect
        //    };
        //    foreach (var piece in LstAreaPieces)
        //    {
        //        foreach(var child in piece.LstChilds)
        //        {
        //            var childData = child.ExportDataTree(param);
        //            if(childData == null)
        //            {
        //                Debug.LogError(string.Format("Export Data Fail: Name: {0} - Uid: {1} - Position: {2} - WorldDirect: {3}", child.name, child.Info.ExportToJson(param), child.Position, child.WorldDirect));
        //                return null;
        //            }
        //            data.LstChilds.Add(childData);
        //        }
        //    }
        //    return data;
        //}
        public delegate bool FindDecoMatch(Deco deco);

        public virtual Dictionary<string, object> ExportDataSelf(object param)
        {
            var data = new DecoDataArray
            {
                Position = new DecoVector3 { X = _position.x, Y = _position.y, Z = _position.z },
                WorldDirect = _worldDirect,
                Group = _group,
            };

            if (Info != null)
                data.Info = Info.ExportToJson(param);
            return data.ToJsonObject;
        }

        public Dictionary<int, List<Dictionary<string, object>>> ExportData(object param, FindDecoMatch match)
        {
            var dataResult = new Dictionary<int, List<Dictionary<string, object>>>();
            ExportDataRecrusive(dataResult, this, param, match);
            return dataResult;
        }

        void ExportDataRecrusive(Dictionary<int, List<Dictionary<string, object>>> dataResult, Deco deco, object param, FindDecoMatch match)
        {
            if (deco == null)
                return;
            List<Dictionary<string, object>> lstByTreeLevel = null;
            var treeLevel = deco.TreeLevel;
            if(match == null || match(deco))
            {
                if (!dataResult.TryGetValue(treeLevel, out lstByTreeLevel))
                {
                    lstByTreeLevel = new List<Dictionary<string, object>>();
                    dataResult[treeLevel] = lstByTreeLevel;
                }
                lstByTreeLevel.Add(deco.ExportDataSelf(param));
            }
          
            foreach (var piece in deco.LstAreaPieces)
            {
                foreach (var child in piece.LstChilds)
                    ExportDataRecrusive(dataResult, child, param, match);
            }
        }

        #endregion

        #region Foreach Child

        public List<Deco> GetAllChilds()
        {
            List<Deco> lstResults = new List<Deco>();
            GetChildRecrusive(lstResults, this);
            return lstResults;
        }

        void GetChildRecrusive(List<Deco> lstResult, Deco deco)
        {
            if (deco == null)
                return;

            foreach (var piece in deco.LstAreaPieces)
            {
                foreach (var child in piece.LstChilds)
                {
                    lstResult.Add(child);
                    GetChildRecrusive(lstResult, child);
                }
            }
        }

       

        public void GetDecosInChilds(FindDecoMatch match, List<Deco> lstResult = null)
        {
            if(lstResult == null)
                lstResult = new List<Deco>();
            if (match == null)
                return;
            GetDecoInChildRecrusive(lstResult, this, match);
        }

        void GetDecoInChildRecrusive(List<Deco> lstResult, Deco deco, FindDecoMatch match)
        {
            if (deco == null)
                return;
            if (match(deco))
                lstResult.Add(deco);
            foreach (var piece in deco.LstAreaPieces)
            {
                foreach (var child in piece.LstChilds)
                    GetDecoInChildRecrusive(lstResult, child, match);
            }
        }

        public Deco GetFirstDecoInChilds(FindDecoMatch match)
        {
            Deco result = null;
            if (match == null)
                return result;
            GetFirstDecoInChildRecrusive(ref result, this, match);
            return result;
        }

        void GetFirstDecoInChildRecrusive(ref Deco result, Deco deco, FindDecoMatch match)
        {
            if (result != null)
                return;
            if (deco == null)
                return;
            if(match(deco))
            {
                result = deco;
                return;
            }
            foreach (var piece in deco.LstAreaPieces)
            {
                foreach (var child in piece.LstChilds)
                    GetFirstDecoInChildRecrusive(ref result, child, match);
            }
        }

        public void Foreach(Action<Deco> func)
        {
            if (func == null)
                return;
            ForeachRecrusive(this, func);
        }

        void ForeachRecrusive(Deco deco, Action<Deco> func)
        {
            if (deco == null)
                return;
            func(deco);
            foreach (var piece in deco.LstAreaPieces)
            {
                foreach (var child in piece.LstChilds)
                    ForeachRecrusive(child, func);
            }
        }
        #endregion

        public T ParseInfo<T>()where T:IDecoInfo
        {
            return (T)Info;
        }
    }
}

