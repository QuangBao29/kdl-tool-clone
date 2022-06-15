using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.config
{
    public class ConfigUnpackingRoomRecord
    {
        public int RoomId;
        public int RoomSizeX;
        public int RoomSizeY;
        public string AllBubblePosition;
        public string AllUnpackingDeco;

        private List<int> _bubblePosition;
        private List<int> _lstUnpackingDeco;
        public List<int> GetBubblePostion()
        {
            if (_bubblePosition == null)
            {
                _bubblePosition = SGUtils.ParseStringToListInt(AllBubblePosition, ';');
            }
            return _bubblePosition;
        }

        public List<int> GetLstUnpackingDeco()
        {
            if (_lstUnpackingDeco == null)
            {
                _lstUnpackingDeco = SGUtils.ParseStringToListInt(AllUnpackingDeco, ';');
            }
            return _lstUnpackingDeco;
        }

        public List<string> GetLstVariables()
        {
            return new List<string> { "RoomId"
                                    , "RoomSizeX"
                                    , "RoomSizeY"
                                    , "AllBubblePosition"
                                    , "AllUnpackingDeco"
            };
        }
        public string GetTextRecord()
        {
            return RoomId
                + "\t" + RoomSizeX
                + "\t" + RoomSizeY
                + "\t" + AllBubblePosition
                + "\t" + AllUnpackingDeco
                + "\n";
        }
    }

    public class ConfigUnpackingRoom : ConfigDataTable<ConfigUnpackingRoomRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("RoomId");
        }

        public ConfigUnpackingRoomRecord GetById(int id)
        {
            return GetRecordByIndex<int>("Id", id);
        }
    }
}

