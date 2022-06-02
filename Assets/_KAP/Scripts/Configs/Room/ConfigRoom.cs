using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public enum DifficultyLevelType
    {
        Easy,
        Medium,
        Hard,
    }

    public enum FillShadowPlayMode
    {
        Normal,
        DecoTurn,
        ShadowTurn,
        ShadowTurnLimitDeco,
    }

    public enum RoomLockType
    {
        None,
        RewardedAd
    }

    public class ConfigRoomRecord
	{
        public int Id;
        public string Name;
        public int TypeId;
        public int ThemeId;
        public string ShadowHexColor;
        public string AllThemes;
        public DifficultyLevelType DifficultyLevel;
        public int Energy;
        public int TimeoutInSec;
        public int MinCoinReward;
        public int MaxCoinReward;
        public float LostCoinPerSecond;
        public FillShadowPlayMode PlayMode;
        public int DecoEachTurn;
        public int ShadowEachTurn;
        public int BackgroundID;
        public RoomLockType LockType;

        private List<int> _allThemesUseInRoom;
        public List<int> GetAllThemeUseInRoom()
        {
            if(_allThemesUseInRoom == null)
            {
                _allThemesUseInRoom = SGUtils.ParseStringToListInt(AllThemes, ';');
            }
            return _allThemesUseInRoom;
        }

        public static List<string> GetLstVariables()
        {
            return new List<string> { "Id"
                                    , "Name"
                                    , "TypeId"
                                    , "ThemeId"
                                    , "ShadowHexColor"
                                    , "AllThemes"
                                    , "DifficultyLevel"
                                    , "Energy"
                                    , "TimeoutInSec"
                                    , "MinCoinReward"
                                    , "MaxCoinReward"
                                    , "LostCoinPerSecond"
                                    , "PlayMode"
                                    , "DecoEachTurn"
                                    , "ShadowEachTurn"
                                    , "BackgroundID"
                                    , "RoomLockType"
            };
        }

        public string GetTextRecord()
        {
            return Id
                + "\t" + Name
                + "\t" + TypeId
                + "\t" + ThemeId
                + "\t" + ShadowHexColor
                + "\t" + AllThemes
                + "\t" + DifficultyLevel
                + "\t" + Energy
                + "\t" + TimeoutInSec
                + "\t" + MinCoinReward
                + "\t" + MaxCoinReward
                + "\t" + LostCoinPerSecond
                + "\t" + PlayMode
                + "\t" + DecoEachTurn
                + "\t" + ShadowEachTurn
                + "\t" + BackgroundID
                + "\t" + LockType
                + "\n";
        }
    }

    public class ConfigRoom : ConfigDataTable<ConfigRoomRecord>
	{
		protected override void RebuildIndex()
		{
			RebuildIndexByField<int>("Id");
            RebuildIndexByField<int>("TypeId", true);
            RebuildIndexByField<int>("ThemeId", true);
        }

		public ConfigRoomRecord GetById(int id)
		{
			return GetRecordByIndex<int>("Id", id);
		}

		public List<ConfigRoomRecord> GetListByTypeId(int typeId)
		{
            return GetRecordsByIndex<int>("TypeId", typeId);
		}

		public List<ConfigRoomRecord> GetListByThemeId(int themeId)
		{
			return GetRecordsByIndex<int>("ThemeId", themeId);
		}
	}
}