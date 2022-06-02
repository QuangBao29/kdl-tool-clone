using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigHiveRoomRecord
    {
        public int Index;
        public int Coin;
        public int Wood;
    }

    public class ConfigHiveRoom : ConfigDataTable<ConfigHiveRoomRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("Index");
        }

        public ConfigHiveRoomRecord GetRoomByIndex(int index)
        {
            return GetRecordByIndex<int>("Index", index);
        }
    }
}

