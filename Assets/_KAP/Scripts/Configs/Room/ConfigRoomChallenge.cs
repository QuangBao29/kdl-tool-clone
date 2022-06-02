using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigRoomChallengeRecord
    {
        public int Id;
        public string Name;
        public string ShadowHexColor;
        public string AllThemes;
        public DifficultyLevelType DifficultyLevel;
        public int Energy;
        public int TimeoutInSec;
        public int MinCoinReward;
        public int MaxCoinReward;
        public float LostCoinPerSecond;
        public int BackgroundID;

        private List<int> _allThemesUseInRoom;
        public List<int> GetAllThemeUseInRoom()
        {
            if (_allThemesUseInRoom == null)
            {
                _allThemesUseInRoom = SGUtils.ParseStringToListInt(AllThemes, ';');
            }
            return _allThemesUseInRoom;
        }

        public static List<string> GetLstVariables()
        {
            return new List<string> { "Id"
                                    , "Name"
                                    , "ShadowHexColor"
                                    , "AllThemes"
                                    , "DifficultyLevel"
                                    , "Energy"
                                    , "TimeoutInSec"
                                    , "MinCoinReward"
                                    , "MaxCoinReward"
                                    , "LostCoinPerSecond"
                                    , "BackgroundID"
            };
        }

        public string GetTextRecord()
        {
            return Id
                + "\t" + Name
                + "\t" + ShadowHexColor
                + "\t" + AllThemes
                + "\t" + DifficultyLevel
                + "\t" + Energy
                + "\t" + TimeoutInSec
                + "\t" + MinCoinReward
                + "\t" + MaxCoinReward
                + "\t" + LostCoinPerSecond
                + "\t" + BackgroundID
                + "\n";
        }
    }

    public class ConfigRoomChallenge : ConfigDataTable<ConfigRoomChallengeRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("Id");
        }

        public ConfigRoomChallengeRecord GetById(int id)
        {
            return GetRecordByIndex<int>("Id", id);
        }
    }

}

