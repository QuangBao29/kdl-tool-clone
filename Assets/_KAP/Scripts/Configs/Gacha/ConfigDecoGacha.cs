using System.Collections;
using System.Collections.Generic;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigDecoGachaRecord
    {
        public int Id;
        public string Name;
        public string ThemeId;
        public float Rate;
        public int Star;
    }

    public class ConfigDecoGacha : ConfigDataTable<ConfigDecoGachaRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("Id");
            RebuildIndexByField<string>("ThemeId", true);
            RebuildIndexByField<int>("Star", true);
        }

        public ConfigDecoGachaRecord GetById(int id)
        {
            return GetRecordByIndex<int>("Id", id);
        }

        public List<ConfigDecoGachaRecord> GetListByStar(int star)
        {
            return GetRecordsByIndex<int>("Star", star);
        }

        public List<ConfigDecoGachaRecord> GetListByThemeId(string id)
        {
            return GetRecordsByIndex<string>("ThemeId", id);
        }
    }
    
}
