using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.IsoTools.DecoSystem
{
    [Serializable]
    public class AreaParameters
    {
        public bool IsWall;
        public int Face;
        public string SortingLayerName;
        public Vector3 FLLocalPos;
        public Vector2 PieceSize;
        public bool UseRootSorting;
        public bool Alone;
    }

    [Serializable]
    public class DecoParameters
    {
        public bool IsWallHang;
        public string SortingLayerName;
        public Vector3 FLSize;
        public Sprite FLSprite;
        public Sprite BRSprite;
        public int CanInFaces;
        public int ColliderLayer;
        public List<AreaParameters> ListAreas = new List<AreaParameters>();
        public List<IsoRect> ListColliderRects = new List<IsoRect>();
    }
}

