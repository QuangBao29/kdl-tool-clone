using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigDecoRewardRecord
    {
        public string RoomId;
        public string LstDecoIds;

        public List<string> GetLstDecoIds()
        {
            List<string> lstID = new List<string>();
            lstID = SGUtils.ParseStringToList(LstDecoIds, ';');
            return lstID;
        }

        public static List<string> GetLstVariables()
        {
            return new List<string>
            {
                "RoomId",
                "LstDecoIds"
            };
        }
    }

    public class ConfigDecoReward : ConfigDataTable<ConfigDecoRewardRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<string>("RoomId");
        }
        public ConfigDecoRewardRecord GetById(string id)
        {
            return GetRecordByIndex<string>("RoomId", id);
        }
        public IndexField<string> GetIndexField()
        {
            return GetIndexField<string>("RoomId");
        }
    }

}
