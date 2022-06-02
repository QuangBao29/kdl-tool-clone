using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public enum RoomTypeUnlockType
    {
        None = 0,
		PreviousType,
		TotalCompletedRoom,
    }

    public enum IconType
    {
        Chandelier,
        Salon,
        Plant,
        Bathtube,
        Lavabo,
        Desk,
        Bat,
        Bench,
        Fence,
        Stove,
        Bed,
        Door,
        Table,
        Cover,
        Bookshelf,
        Swing,
    }

    public class ConfigRoomTypeRecord
    {
        public int Id;
        public string Name;
        public RoomTypeUnlockType UnlockType;
        public string UnlockRequire; // preType -> parse to dic<int, int> || totalroom -> parse int
        public string CompletedReward;
        public IconType IconType;
        public MainColorType MainColor;
    }

	public class ConfigRoomType : ConfigDataTable<ConfigRoomTypeRecord>
	{
		protected override void RebuildIndex()
		{
			RebuildIndexByField<int>("Id");
		}

		public ConfigRoomTypeRecord GetById(int id)
		{
			return GetRecordByIndex<int>("Id", id);
		}
	}
}