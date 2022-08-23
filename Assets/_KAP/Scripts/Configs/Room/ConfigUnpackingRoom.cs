using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;
using Kawaii.IsoTools.DecoSystem;

namespace KAP.Config
{
    public class ConfigUnpackingRoomRecord
    {
        public string Id;
        public string SizeX;
        public string SizeY;
        public string ListBubblePosX;
        public string ListBubblePosY;
        public string ListBubblePosZ;
        public string AllUnpackingDeco;

        //private List<int> _lstBubblePosition;
        private List<int> _lstUnpackingDeco;

        //public List<int> GetListBubblePostion()
        //{
        //    if (_lstBubblePosition == null)
        //    {
        //        _lstBubblePosition = SGUtils.ParseStringToListInt(AllBubblePosition, ';');
        //    }
        //    return _lstBubblePosition;
        //}

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
            return new List<string> { "Id"
                                    , "RoomSizeX"
                                    , "RoomSizeY"
                                    , "ListBubblePosX"
                                    , "ListBubblePosY"
                                    , "ListBubblePosZ"
                                    , "AllUnpackingDeco"
            };
        }
        public string GetTextRecord()
        {
            return Id
                + "\t" + SizeX
                + "\t" + SizeY
                + "\t" + ListBubblePosX
                + "\t" + ListBubblePosY
                + "\t" + ListBubblePosZ
                + "\t" + AllUnpackingDeco
                + "\n";
        }
    }

    public class ConfigUnpackingRoom : ConfigDataTable<ConfigUnpackingRoomRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<string>("Id");
        }

        public ConfigUnpackingRoomRecord GetById(string id)
        {
            return GetRecordByIndex<string>("Id", id);
        }
    }
}

