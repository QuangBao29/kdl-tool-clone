using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.IsoTools.DecoSystem
{
    public class AreaManager : MonoBehaviour
    {
        private List<Area> _lstFloorArea = new List<Area>();
        private List<Area> _lstWallArea = new List<Area>();

        private IsoGroupSorting _roomSorting = new IsoGroupSorting("AreaManager");

        private readonly List<DecoRoot> _lstRooms = new List<DecoRoot>();
        public ReadOnlyCollection<DecoRoot> ListRooms;

        private Vector3 _worldPos;
        private Vector2 _worldSize;
        private int _worldDirect;
        private bool _isCalculateWorldIso;

        private void Awake()
        {
            ListRooms = _lstRooms.AsReadOnly();
        }

        public virtual void AddPiece(AreaPiece piece)
        {
            if (piece == null)
                return;

            if(piece.IsWall)
            {
                if (!piece.Alone)
                {
                    foreach (var area in _lstWallArea)
                    {
                        if (area.Face != piece.Face)
                            continue;
                        if (area.X != piece.X || area.Y != piece.Y)
                            continue;
                        area.AddPiece(piece);
                        piece.AreaParent = area;
                        return;
                    }
                }
                   
                var newWallArea = new Area() { IsWall = true, Face = piece.Face, X = piece.X, Y = piece.Y };
                newWallArea.AddPiece(piece);
                piece.AreaParent = newWallArea;
                _lstWallArea.Add(newWallArea);
            }
            else
            {
                if (!piece.Alone)
                {
                    foreach (var area in _lstFloorArea)
                    {
                        if (area.Face != piece.Face)
                            continue;
                        if (area.Z != piece.Z)
                            continue;
                        area.AddPiece(piece);
                        piece.AreaParent = area;
                        return;
                    }
                }
                var newArea = new Area() { IsWall = false, Face = piece.Face, Z = piece.Z };
                newArea.AddPiece(piece);
                piece.AreaParent = newArea;
                _lstFloorArea.Add(newArea);
            }
        }

        public virtual bool RemovePiece(AreaPiece piece)
        {
            if (piece == null)
                return false;
            if (piece.AreaParent == null)
                return false;
            if (piece.AreaParent.RemovePiece(piece))
            {
                if (piece.AreaParent.LstPieces.Count == 0)
                    _lstFloorArea.Remove(piece.AreaParent);
                piece.AreaParent = null;
                return true;
            }
            return false;
        }

        public class MoveData
        {
            public AreaPiece piece;
            public List<AreaPiece> overlapPieces = new List<AreaPiece>();
            public List<Deco> ListOverlaps;
        }

        public virtual MoveData Move(Deco deco)
        {
            if (deco == null)
                return null;
           
            MoveData nearest = null;
            if(deco.IsWallHang)
            {
                foreach (var area in _lstWallArea)
                {
                    if (deco.CanInFaces != (deco.CanInFaces | (1 << area.Face)))
                        continue;
                    var moveData = GetValidPieceInAArea(area, deco, true);

                    if (moveData == null)
                        continue;
                    if (nearest == null)
                    {
                        nearest = moveData;
                        continue;
                    }
                 
                    if (nearest.piece == null)
                    {
                        if (moveData.piece != null)
                            nearest = moveData;
                        else if (nearest.ListOverlaps == null && moveData.ListOverlaps != null)
                            nearest = moveData;
                        continue;
                    }
                    if (moveData.piece == null)
                        continue;
                    var nearestValue = nearest.piece.X != int.MinValue ? nearest.piece.X : nearest.piece.Y;
                    var moveDataValue = moveData.piece.X != int.MinValue ? moveData.piece.X : moveData.piece.Y;
                    if (moveDataValue < nearestValue)
                    {
                        nearest = moveData;
                        continue;
                    }
                }
            }
            else
            {
                foreach (var area in _lstFloorArea)
                {
                    if (deco.CanInFaces != (deco.CanInFaces | (1 << area.Face)))
                        continue;
                    var moveData = GetValidPieceInAArea(area, deco, false);
                    if (moveData == null)
                        continue;

                    if (nearest == null)
                    {
                        nearest = moveData;
                        continue;
                    }

                    if(nearest.piece == null)
                    {
                        if(moveData.piece != null)
                            nearest = moveData;
                        else if (nearest.ListOverlaps == null && moveData.ListOverlaps != null)
                            nearest = moveData;
                        continue;
                    }
                    if (moveData.piece == null)
                        continue;

                    int nearestZ = nearest.piece.Z;
                    int z = moveData.piece.Z;

                    if (z > nearestZ)
                    {
                        nearest = moveData;
                        continue;
                    }
                }
            }
            return nearest;
        }

        protected virtual MoveData GetValidPieceInAArea(Area area, Deco deco, bool needSameDirect)
        {
            if (area.LstPieces.Count == 0)
                return new MoveData();

            var pos = IsoUtils.RemoveZOfPosition(deco.Position);
            var lstCheckPoints = new List<Vector2>();
            var topPos = pos;
            var size = deco.Size;
            if (!deco.IsWallHang)
            {
                if (size.x > 1)
                {
                    topPos.x += size.x - 1;
                    lstCheckPoints.Add(new Vector2(topPos.x, pos.y));
                }
                if (size.y > 1)
                {
                    topPos.y += size.y - 1;
                    lstCheckPoints.Add(new Vector2(pos.x, topPos.y));
                }
                if (lstCheckPoints.Count == 2)
                    lstCheckPoints.Add(topPos);
            }
            else
            {
                if (size.x > 1)
                {
                    topPos.x = pos.x + size.x - 1; 
                    lstCheckPoints.Add(new Vector2(topPos.x, pos.y));
                }
                else
                {
                    topPos.y = pos.y + size.y - 1;
                    lstCheckPoints.Add(new Vector2(pos.x, topPos.y));
                }

                if(size.z > 1)
                {
                    topPos.z = pos.z + size.z - 1;
                    lstCheckPoints.Add(new Vector3(pos.x, pos.y, topPos.z));
                    lstCheckPoints.Add(IsoUtils.RemoveZOfPosition(topPos));
                }
            }
            AreaPiece nearestPiece = null;
            var lstOverlaps = new List<Deco>();
            var lstOverlapPieces = new List<AreaPiece>();
            foreach (var piece in area.LstPieces)
            {
                if (piece.IsLock)
                    continue;
                if (piece.DecoParent == deco)
                    continue;
                if (needSameDirect && piece.WorldDirect != deco.WorldDirect)
                    continue;
                if (piece.IsCollideWithDeco(pos, size))
                {
                    if(nearestPiece == null)
                    {
                        if (piece.ContainsPoint(pos))
                            nearestPiece = piece;
                    }
                    for (var i = 0; i < lstCheckPoints.Count;)
                    {
                        var point = lstCheckPoints[i];
                        if (piece.ContainsPoint(point))
                        {
                            lstCheckPoints.RemoveAt(i);
                        }
                        else
                            i++;
                    }
                    var decoOverlaps = piece.GetOverlapChilds(deco);
                    if (decoOverlaps != null && decoOverlaps.Count > 0)
                    {
                        foreach (var overlap in decoOverlaps)
                            if (!lstOverlaps.Contains(overlap))
                                lstOverlaps.Add(overlap);
                    }
                    lstOverlapPieces.Add(piece);
                }
            }

            if (lstCheckPoints.Count > 0)
                return new MoveData();
            if (lstOverlaps.Count > 0)
                return new MoveData { ListOverlaps = lstOverlaps };
            return new MoveData
            {
                piece = nearestPiece,
                overlapPieces = lstOverlapPieces
            };
        }

        public virtual Dictionary<int, List<Dictionary<string, object>>> Export(object param, Deco.FindDecoMatch match = null)
        {
            var data = new  Dictionary<int, List<Dictionary<string, object>>>();
            foreach (var r in _lstRooms)
            {
                var rData = r.ExportData(param, match);
                foreach(var iter in rData)
                {
                    List<Dictionary<string, object>> lst = null;
                    if (data.TryGetValue(iter.Key, out lst))
                        lst.AddRange(iter.Value);
                    else
                    {
                        data[iter.Key] = iter.Value;
                    }
                }
            }
            return data;
        }
        #region Rooms
        public void AddRoom(DecoRoot room)
        {
            _lstRooms.Add(room);
            _roomSorting.AddObject(room);
        }

        public void RemoveRoom(DecoRoot room)
        {
            if(_lstRooms.Remove(room))
            {
                room.Remove();
            }
            _roomSorting.RemoveObject(room);
        }

        public void ClearAllRooms()
        {
            foreach (var r in _lstRooms)
                r.Remove();
            _lstRooms.Clear();
            _roomSorting.ClearAll();
        }

        public void SortRoom()
        {
            _roomSorting.Sort();
        }

        public void LockARoom(int index)
        {
            if (index < 0 || index >= _lstRooms.Count)
                return;
            var room = _lstRooms[index];
            room.Lock();
        }

        public void UnLockARoom(int index)
        {
            if (index < 0 || index >= _lstRooms.Count)
                return;
            var room = _lstRooms[index];
            room.UnLock();
        }

        #endregion

        #region World Rotate
        void CalculateWorldIso()
        {
            if (_isCalculateWorldIso)
                return;
            float minX = int.MaxValue;
            float maxX = int.MinValue;
            float minY = int.MaxValue;
            float maxY = int.MinValue;
            foreach (var room in _lstRooms)
            {
                var pos = room.Position;
                var topPos = pos + room.Size;
                if (pos.x < minX)
                    minX = pos.x;
                if (pos.y < minY)
                    minY = pos.y;
                if (topPos.x > maxX)
                    maxX = topPos.x;
                if (topPos.y > maxY)
                    maxY = topPos.y;
            }
            _worldPos = new Vector2(minX, minY);
            _worldSize = new Vector2(maxX - minX, maxY - minY);
            foreach(var room in _lstRooms)
            {
                room.SetFLIsoPos(room.Position - _worldPos);
            }
            _isCalculateWorldIso = true;
        }

        public int WorldDirect
        {
            get {
                return _worldDirect;
            }
            set
            {
                CalculateWorldIso();
                _worldDirect = value;
              
                foreach (var room in _lstRooms)
                {
                    room.Position = Calculator.GetChildWorldPositionFollowParent(_worldDirect, room.FLLocalIsoPos, room.FLIsoSize, _worldDirect, _worldPos, _worldSize);
                    room.WorldDirect = _worldDirect;
                    room.Apply(null, null);
                }
            }
        }

        #endregion
        public void Log()
        {
            foreach (var area in _lstFloorArea)
            {
                foreach (var piece in area.LstPieces)
                {
                    piece.Log();
                }
            }

            foreach (var area in _lstWallArea)
            {
                foreach (var piece in area.LstPieces)
                {
                    piece.Log();
                }
            }
        }

        #region Find Deco

        public List<Deco> GetDecos(Deco.FindDecoMatch match)
        {
            List<Deco> result = new List<Deco>();
            foreach(var room in _lstRooms)
            {
                room.GetDecosInChilds(match, result);
            }
            return result;
        }

        public void ForeachDecos(Action<Deco> func)
        {
            foreach (var room in _lstRooms)
            {
                room.Foreach(func);
            }
        }
        #endregion
    }
}
