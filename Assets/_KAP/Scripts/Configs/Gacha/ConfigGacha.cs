using System.Collections;
using System.Collections.Generic;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public enum GachaType
    {
        AllInOne = 0,
        AdorableBox = 1,
        EventGacha = 2
    }

    public enum ItemType
    {
        Coin = 0,
        Gem = 1,
        Gacha = 2
    }

    public class ConfigGachaRecord
    {
        public int Id;
        public string Name;
        public GachaType Type;
        public string ThemeId;
        public bool HasAds;
        public int GemPriceOnce;
        public int GemPriceTen;
        public int TicketPriceOnce;
        public int TicketPriceTen;
        public int CoinPriceOnce;
        public int CoinPriceTen;
        public ItemType ItemType;
        public int Priority;
        public MainColorType bgColor;
    }

    public class ConfigGacha : ConfigDataTable<ConfigGachaRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("Id");
            RebuildIndexByField<GachaType>("Type", true);
            RebuildIndexByField<ItemType>("ItemType", true);
            RebuildIndexByField<string>("ThemeId", true);
            RebuildIndexByField<MainColorType>("bgColor", true);
        }

        public ConfigGachaRecord GetById(int id)
        {
            return GetRecordByIndex<int>("Id", id);
        }


        public List<ConfigGachaRecord> GetListByType(GachaType type)
        {
            return GetRecordsByIndex<GachaType>("Type", type);
        }

    }
}
