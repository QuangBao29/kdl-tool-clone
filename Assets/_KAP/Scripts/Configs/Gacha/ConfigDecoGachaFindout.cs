using System.Collections;
using System.Collections.Generic;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigDecoGachaFindoutRecord
    {
        public int Id;
        public string Name;
        public string ThemeId;
        public float Rate;
        public int Star;
    }

    public class ConfigDecoGachaFindout : ConfigDataTable<ConfigDecoGachaFindoutRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("Id");
            RebuildIndexByField<string>("ThemeId", true);
            RebuildIndexByField<int>("Star", true);
        }

        public ConfigDecoGachaFindoutRecord GetById(int id)
        {
            return GetRecordByIndex<int>("Id", id);
        }

        public List<ConfigDecoGachaFindoutRecord> GetListByStar(int star)
        {
            return GetRecordsByIndex<int>("Star", star);
        }

        public List<ConfigDecoGachaFindoutRecord> GetListByThemeId(string id)
        {
            return GetRecordsByIndex<string>("ThemeId", id);
        }
    }
    
}
