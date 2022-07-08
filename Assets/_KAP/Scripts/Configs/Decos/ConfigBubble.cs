using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;

namespace KAP.Config
{
    public class ConfigBubbleRecord
    {
        public string BubbleId;
        public string BubbleDecoIds;

        private List<int> _lstBubbleDecoId;

        public List<int> GetLstBubbleDeco()
        {
            if (_lstBubbleDecoId == null || _lstBubbleDecoId.Count <= 0)
            {
                _lstBubbleDecoId = SGUtils.ParseStringToListInt(BubbleDecoIds, ';');
            }
            return _lstBubbleDecoId;
        }
    }

    public class ConfigBubble : ConfigDataTable<ConfigBubbleRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<string>("BubbleId");
        }
        public ConfigBubbleRecord GetById(string id)
        {
            return GetRecordByIndex<string>("BubbleId", id);
        }
        public IndexField<string> GetIndexField()
        {
            return GetIndexField<string>("BubbleId");
        }
    }
}

