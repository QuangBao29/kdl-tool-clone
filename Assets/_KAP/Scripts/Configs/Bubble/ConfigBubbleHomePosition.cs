using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigBubbleHomePositionRecord
    {
        public string RoomId;
        public string LstBubblePosition;
        public int Index;
        public int Exp;

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
                //Debug.LogError("bubblePos: " + ListBubblePos[i].x + ListBubblePos[i].y + ListBubblePos[i].z);
            }
            return ListBubblePos;
        }

        public static List<string> GetLstVariables()
        {
            return new List<string> { "RoomId"
                                    , "LstBubblePosition"
                                    , "Index"
                                    ,"Exp"
            };
        }
    }
    public class ConfigBubbleHomePosition : ConfigDataTable<ConfigBubbleHomePositionRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<string>("RoomId");
        }
        public ConfigBubbleHomePositionRecord GetByRoomId(string roomId)
        {
            return GetRecordByIndex<string>("RoomId", roomId);
        }
        public ConfigBubbleHomePositionRecord GetByRoomIndex(int index)
        {
            return GetRecordByIndex<int>("Index", index);
        }
        public IndexField<string> GetIndexField()
        {
            return GetIndexField<string>("RoomId");
        }
    }

}
