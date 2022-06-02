using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kawaii.IsoTools.DecoSystem
{
    public class Area
    {
        public bool IsWall;
        public int Face;
        public int X = int.MinValue;
        public int Y = int.MinValue;
        public int Z;
        protected List<AreaPiece> _lstPieces = new List<AreaPiece>();
        public ReadOnlyCollection<AreaPiece> LstPieces ;

        public Area()
        {
            LstPieces = _lstPieces.AsReadOnly();
        }

        public bool AddPiece(AreaPiece piece)
        {
            if (piece == null)
                return false;
            _lstPieces.Add(piece);
            return true;
        }

        public bool RemovePiece(AreaPiece piece)
        {
            if (piece == null)
                return false;
            return _lstPieces.Remove(piece);
        }
    }
}

