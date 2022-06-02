using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public enum RoomThemeStickerType
    {
        None,
        Special,
        Event,
    }

    public class ConfigRoomThemeRecord
	{
		public int Id;
		public string Name;
        public string CompletedReward;
        public RoomThemeStickerType StickerType;
        public MainColorType MainColor;
        public string AllThemes;

        private List<int> _allThemesUsed;
        public List<int> GetAllThemeUsed()
        {
            if (_allThemesUsed == null)
            {
                _allThemesUsed = SGUtils.ParseStringToListInt(AllThemes, ';');
            }
            return _allThemesUsed;
        }

        public static List<string> GetLstVariables()
        {
            return new List<string>
            {
                  "Id"
                , "Name"
                , "CompletedReward"
                , "StickerType"
                , "MainColor"
                , "AllThemes"
            };
        }

        public string GetTextRecord()
        {
            return Id
                + "\t" + Name
                + "\t" + CompletedReward
                + "\t" + StickerType
                + "\t" + MainColor
                + "\t" + AllThemes
                + "\n";
        }
    }

	public class ConfigRoomTheme : ConfigDataTable<ConfigRoomThemeRecord>
	{
		protected override void RebuildIndex()
		{
			RebuildIndexByField<int>("Id");
		}

		public ConfigRoomThemeRecord GetById(int id)
		{
			return GetRecordByIndex<int>("Id", id);
		}
	}
}