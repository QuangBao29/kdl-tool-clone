using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigRoomCloserBetterRecord
    {
        public int Id;
        public string RoomId;
        public string LstUnpackingDeco;
        public string BaseGem;

        public static List<string> GetLstVariables()
        {
            return new List<string> { "Id"
                                    , "RoomId"
                                    , "LstUnpackingDeco"
                                    , "BaseGem"
            };
        }
    }
    public class ConfigRoomCloserBetter : ConfigDataTable<ConfigRoomCloserBetterRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<string>("RoomId");
        }
        public ConfigRoomCloserBetterRecord GetByRoomId(string roomId)
        {
            return GetRecordByIndex<string>("RoomId", roomId);
        }
        public IndexField<string> GetIndexField()
        {
            return GetIndexField<string>("RoomId");
        }
    }

}
