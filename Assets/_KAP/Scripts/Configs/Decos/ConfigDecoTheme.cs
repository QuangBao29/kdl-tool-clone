using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigDecoThemeRecord
    {
        public int Id;
        public string Name;
    }

    public class ConfigDecoTheme : ConfigDataTable<ConfigDecoThemeRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("Id");
        }
        public ConfigDecoThemeRecord GetDecoThemeById(int themeId)
        {
            return GetRecordByIndex<int>("Id", themeId);
        }
    }
}