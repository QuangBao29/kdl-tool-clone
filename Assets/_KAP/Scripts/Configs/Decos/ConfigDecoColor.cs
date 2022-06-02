using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigDecoColorRecord
    {
        public string Id;
        public int DecoId;
        public int ColorId;
    }
    public class ConfigDecoColor : ConfigDataTable<ConfigDecoColorRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<string>("Id");
            RebuildIndexByField<int>("DecoId", true);
        }

        public ConfigDecoColorRecord GetDecoColorById(string id)
        {
            return GetRecordByIndex<string>("Id", id);
        }

        public List<ConfigDecoColorRecord> GetListDecoColorsByDecoId(int decoId)
        {
            return GetRecordsByIndex<int>("DecoId", decoId);
        }
    }
}