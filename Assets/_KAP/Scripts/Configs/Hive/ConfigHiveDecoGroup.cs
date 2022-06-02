using System;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;
using Pathfinding.Serialization.JsonFx;
using Kawaii.IsoTools.DecoSystem;

namespace KAP.Config
{
    public class ConfigHiveDecoGroupRecord
    {
        public string Id;
        public int DecoId;
        public int ColorId;
        public string Json;

        private Dictionary<string, DecoDataArray[]> _data = null;
        public Dictionary<string, DecoDataArray[]> Data
        {
            get
            {
                try
                {
                    if (_data == null && !string.IsNullOrEmpty(Json))
                        _data = JsonReader.Deserialize<Dictionary<string, DecoDataArray[]>>(Json);
                    return _data;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    return null;
                }

            }
        }
    }

    public class ConfigHiveDecoGroup : ConfigDataTable<ConfigHiveDecoGroupRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<string>("Id");
            RebuildIndexByField<int>("DecoId", true);
        }

        public ConfigHiveDecoGroupRecord GetDecoGroupById(string id)
        {
            return GetRecordByIndex<string>("Id", id);
        }

        public List<ConfigHiveDecoGroupRecord> GetListDecoGroupsByDecoId(int decoId)
        {
            return GetRecordsByIndex<int>("DecoId", decoId);
        }
    }
}

