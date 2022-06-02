using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.IsoTools.DecoSystem
{
    public class DecoRoot : Deco
    {
        #region Iso Sorting

        private Dictionary<string, IsoGroupSorting> _isoSortByLayerNames = new Dictionary<string, IsoGroupSorting>();

        public void AddSortingObject(IsoObject isoObject)
        {
            var sortingLayerName = isoObject.SortingLayerName;
            IsoGroupSorting isoGroupSorting = null;
            if(!_isoSortByLayerNames.TryGetValue(sortingLayerName, out isoGroupSorting))
            {
                isoGroupSorting = new IsoGroupSorting(sortingLayerName);
                _isoSortByLayerNames[sortingLayerName] = isoGroupSorting;
            }
            isoGroupSorting.AddObject(isoObject);
        }

        public bool RemoveSortingObject(IsoObject isoObject)
        {
            var sortingLayerName = isoObject.SortingLayerName;
            IsoGroupSorting isoGroupSorting = null;
            if (!_isoSortByLayerNames.TryGetValue(sortingLayerName, out isoGroupSorting))
                return false;
            return isoGroupSorting.RemoveObject(isoObject);
        }

        public void SortIsoGroup(string sortingLayerName)
        {
            IsoGroupSorting isoGroupSorting = null;
            if (!_isoSortByLayerNames.TryGetValue(sortingLayerName, out isoGroupSorting) || isoGroupSorting == null)
                return;
            isoGroupSorting.Sort();
        }
        #endregion

        #region Override

        public override int WorldDirect
        {
            get
            {
                return _worldDirect;
            }
            set
            {
                _worldDirect = value;
                Refresh();
            }
        }

        protected override void Refresh()
        {
            switch (_worldDirect)
            {
                case IsoDirect.FL:
                    Size = FLIsoSize;
                    break;
                case IsoDirect.FR:
                    Size = new Vector3(FLIsoSize.y, FLIsoSize.x, FLIsoSize.z);
                    break;
                case IsoDirect.BL:
                    Size = new Vector3(FLIsoSize.y, FLIsoSize.x, FLIsoSize.z);
                    break;
                case IsoDirect.BR:
                    Size = FLIsoSize;
                    break;
            }
            Spr.size = new Vector2(0.227f * Size.x, 0.227f * Size.y);
        }

        //public override DecoDataTree ExportDataTree(object param)
        //{
        //    var data = new DecoDataTree();
        //    if(Info != null)
        //        data.Info = Info.ExportToJson(param);
        //    data.Position = new DecoVector3 { X = _position.x, Y = _position.y, Z = _position.z };
        //    data.WorldDirect = _worldDirect;
        //    data.Size = new DecoVector3(_size);

        //    foreach (var piece in LstAreaPieces)
        //    {
        //        foreach (var child in piece.LstChilds)
        //        {
        //            var childData = child.ExportDataTree(param);
        //            if (childData == null)
        //            {
        //                Debug.LogError(string.Format("Export Data Fail: Name: {0} - Uid: {1} - Position: {2} - WorldDirect: {3}", child.name, child.Info.ExportToJson(param), child.Position, child.WorldDirect));
        //                return null;
        //            }
        //            data.LstChilds.Add(childData);
        //        }
        //    }
        //    return data;
        //}

        public override Dictionary<string, object> ExportDataSelf(object param)
        {
            var data = new DecoDataArray
            {
                Position = new DecoVector3 { X = _position.x, Y = _position.y, Z = _position.z },
                WorldDirect = _worldDirect,
                Group = _group,
                Size = new DecoVector3(_size)
            };

            if (Info != null)
                data.Info = Info.ExportToJson(param);
            return data.ToJsonObject;
        }

        public void Lock()
        {
            foreach (var piece in LstAreaPieces)
            {
                piece.Lock();
            }
        }

        public void UnLock()
        {
            foreach (var piece in LstAreaPieces)
            {
                piece.Unlock();
            }
        }

        #endregion

        public void SetFLIsoSize(Vector3 size)
        {
            FLIsoSize = size;
        }

        public void SetFLIsoPos(Vector3 pos)
        {
            FLLocalIsoPos = pos;
        }
    }
}
