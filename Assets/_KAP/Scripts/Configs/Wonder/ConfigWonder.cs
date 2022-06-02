using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigWonderRecord
    {
        public int Id;
        public string Name;
        public string InsprieName;
        public MainColorType MainColor;
        public string ShadowHexColor;
        public string AllThemes;
        public int TurnTime;
        public int MinCoin;
        public int MaxCoin;
        public int SubCoinPerSecond;
        public string RewardCompleted;
        public int BackgroundID;
        public int GemRewardEachTurn;

        private List<int> _allThemes;
        public List<int> GetAllThemes()
        {
            if (_allThemes == null)
            {
                _allThemes = SGUtils.ParseStringToListInt(AllThemes, ';');
            }
            return _allThemes;
        }

        public static List<string> GetLstVariables()
        {
            return new List<string> { "Id"
                                    , "Name"
                                    , "InsprieName"
                                    , "MainColor"
                                    , "ShadowHexColor"
                                    , "AllThemes"
                                    , "TurnTime"
                                    , "MinCoin"
                                    , "MaxCoin"
                                    , "SubCoinPerSecond"
                                    , "RewardCompleted"
                                    , "BackgroundID"
                                    , "GemRewardEachTurn"
                                    };
        }

        public string GetTextRecord()
        {
            return Id
                + "\t" + Name
                + "\t" + InsprieName
                + "\t" + MainColor
                + "\t" + ShadowHexColor
                + "\t" + AllThemes
                + "\t" + TurnTime
                + "\t" + MinCoin
                + "\t" + MaxCoin
                + "\t" + SubCoinPerSecond
                + "\t" + RewardCompleted
                + "\t" + BackgroundID
                + "\t" + GemRewardEachTurn

            + "\n";
        }
    }

    public class ConfigWonder : ConfigDataTable<ConfigWonderRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("Id");
        }

        public ConfigWonderRecord GetById(int id)
        {
            return GetRecordByIndex<int>("Id", id);
        }
    }
}