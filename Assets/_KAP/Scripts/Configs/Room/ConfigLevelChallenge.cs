using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigLevelChallengeRecord
    {
        public int Level;
        public int EasyRoom;
        public int MediumRoom;
        public int HardRoom;
        public int EventRoom;
        public string LimitDecorMinMaxDecor;
        public string LimitDecorMinMaxShadow;
        public string LimitShadowMinMaxDecor;
        public string LimitShadowMinMaxShadow;
        public string Normal;
        public string Milestones;

        public static List<string> GetLstVariables()
        {
            return new List<string> { "Level"
                                    , "EasyRoom"
                                    , "MediumRoom"
                                    , "HardRoom"
                                    , "EventRoom"
                                    , "LimitDecorMinMaxDecor"
                                    , "LimitDecorMinMaxShadow"
                                    , "LimitShadowMinMaxDecor"
                                    , "LimitShadowMinMaxShadow"
                                    , "Normal"
                                    , "Milestones"
            };
        }

        public string GetTextRecord()
        {
            return Level
                + "\t" + EasyRoom
                + "\t" + MediumRoom
                + "\t" + HardRoom
                + "\t" + EventRoom
                + "\t" + LimitDecorMinMaxDecor
                + "\t" + LimitDecorMinMaxShadow
                + "\t" + LimitShadowMinMaxDecor
                + "\t" + LimitShadowMinMaxShadow
                + "\t" + Normal
                + "\t" + Milestones
                + "\n";
        }


        public class ConfigLevelChallenge : ConfigDataTable<ConfigLevelChallengeRecord>
        {
            protected override void RebuildIndex()
            {
                RebuildIndexByField<int>("Level");
            }

            public ConfigLevelChallengeRecord GetById(int id)
            {
                return GetRecordByIndex<int>("Level", id);
            }
        }

    }
}

