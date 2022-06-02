using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigDecoAreaRecord
    {
        public int DecoId;
        public bool IsWall;
        public DemoAreaFace Face;
        public DemoSortingLayerName SortingLayerName;
        public int FLLocalPosX;
        public int FLLocalPosY;
        public int FLLocalPosZ;
        public int PieceSizeX;
        public int PieceSizeY;
        public bool UseRootSorting;
        public bool Alone;
    }
    public class ConfigDecoArea : ConfigDataTable<ConfigDecoAreaRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("DecoId", true);
        }

        public List<ConfigDecoAreaRecord> GetListDecoAreaByDecoId(int decoId)
        {
            return GetRecordsByIndex<int>("DecoId", decoId);
        }
    }
}

