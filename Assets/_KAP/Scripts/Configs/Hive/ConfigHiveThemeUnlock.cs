using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public enum HiveThemeUnlockType
    {
        None = 0,
        RoomTheme = 1,
        Event = 2,
        ComingSoon = 3,
        Gacha = 4,
        Findout = 5
    }

    public class ConfigHiveThemeUnlockRecord
    {
        public int Id;
        public float DecoPriceFactor;
        public bool Enable;
        public HiveThemeUnlockType UnlockType;
        public string UnlockRequire;
    }

    public class ConfigHiveThemeUnlock : ConfigDataTable<ConfigHiveThemeUnlockRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("Id");
            RebuildIndexByField<bool>("Enable", true);
        }

        public ConfigHiveThemeUnlockRecord GetThemeById(int id)
        {
            return GetRecordByIndex<int>("Id", id);
        }

        public List<ConfigHiveThemeUnlockRecord> GetEnableThemes(bool enable)
        {
            return GetRecordsByIndex<bool>("Enable", enable);
        }
    }
}

