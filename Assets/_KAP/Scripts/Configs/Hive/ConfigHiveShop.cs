using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public enum HiveShopPriceType
    {
        Coin = 0,
        Gem = 1,
        Gacha = 2,
        Findout = 3
    }

    public class ConfigHiveShopRecord
    {
        public int Id;
        public int Coin;
        public int Gem;
        public HiveShopPriceType PriceType;
        public bool Enable;
        public bool Ad;
    }

    public class ConfigHiveShop : ConfigDataTable<ConfigHiveShopRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("Id");
            RebuildIndexByField<bool>("Enable", true);
        }

        public ConfigHiveShopRecord GetDecoById(int id)
        {
            return GetRecordByIndex<int>("Id", id);
        }

        public List<ConfigHiveShopRecord> GetEnableDecos(bool enable)
        {
            return GetRecordsByIndex<bool>("Enable", enable);
        }
    }

}
