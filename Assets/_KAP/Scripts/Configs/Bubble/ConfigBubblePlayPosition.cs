using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;
using KAP.ToolCreateMap;
namespace KAP.Config
{
    public class ConfigBubblePlayPositionRecord
    {
        public string Id;
        public string RoomId;
        public string LstBubblePosition;
        public string LstUnpackingDeco;
        public string BaseGem;

        private List<string> _lstUnpackingDeco;

        public List<Vector3> GetLstBubblePositionVector3()
        {
            List<string> ListVector3 = SGUtils.ParseStringToList(LstBubblePosition, ';');
            List<Vector3> ListBubblePos = new List<Vector3>();
            for (var i = 0; i < ListVector3.Count; i++)
            {
                if (ListVector3[i].Length < 7)
                    continue;
                ListVector3[i] = ListVector3[i].Substring(1, ListVector3[i].Length - 2);
                List<int> temp = SGUtils.ParseStringToListInt(ListVector3[i], ',');
                ListBubblePos.Add(new Vector3((float)temp[0], (float)temp[1], (float)temp[2]));
            }
            return ListBubblePos;
        }

        public List<string> GetLstUnpackingDeco()
        {
            if (_lstUnpackingDeco == null || _lstUnpackingDeco.Count <= 0)
            {
                _lstUnpackingDeco = SGUtils.ParseStringToList(KDLUtils.RemoveHiddenFromString(LstUnpackingDeco), ';');
            }
            return _lstUnpackingDeco;
        }

        public static List<string> GetLstVariables()
        {
            return new List<string> { "Id"
                                    , "RoomId"
                                    , "LstBubblePosition"
                                    , "LstUnpackingDeco"
                                    , "BaseGem"
            };
        }
    }

    public class ConfigBubblePlayPosition : ConfigDataTable<ConfigBubblePlayPositionRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<string>("RoomId");
        }
        public ConfigBubblePlayPositionRecord GetByRoomId(string roomId)
        {
            return GetRecordByIndex<string>("RoomId", roomId);
        }
        public IndexField<string> GetIndexField()
        {
            return GetIndexField<string>("RoomId");
        }
    }
}

