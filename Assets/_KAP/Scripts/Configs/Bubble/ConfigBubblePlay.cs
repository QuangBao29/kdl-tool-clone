using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;
using KAP.ToolCreateMap;
namespace KAP.Config
{
    public class ConfigBubblePlayRecord
    {
        public string BubbleId;
        public string BubbleDecoIds;

        private List<string> _lstBubbleDecoId;
        private Dictionary<int, List<int>> _dctBubbleIdColor = new Dictionary<int, List<int>>();

        public List<string> GetLstBubbleDeco()
        {
            if (_lstBubbleDecoId == null || _lstBubbleDecoId.Count <= 0)
            {
                _lstBubbleDecoId = SGUtils.ParseStringToList(KDLUtils.RemoveHiddenFromString(BubbleDecoIds), ';');
            }
            return _lstBubbleDecoId;
        }

        public Dictionary<int, List<int>> GetDctBubbleIdColor()
        {
            GetLstBubbleDeco();
            for (var i = 0; i < _lstBubbleDecoId.Count; i++)
            {
                var lstBubbleIdColor = SGUtils.ParseStringToListInt(_lstBubbleDecoId[i], '_');
                var id = lstBubbleIdColor[0];
                var color = lstBubbleIdColor[1];
                if (_dctBubbleIdColor.ContainsKey(id))
                    _dctBubbleIdColor[id].Add(color);
                else _dctBubbleIdColor.Add(id, new List<int> { color });
            }
            return _dctBubbleIdColor;
        }

        public static List<string> GetLstVariables()
        {
            return new List<string> { "BubbleId"
                                    , "BubbleDecoIds"
            };
        }
    }

    public class ConfigBubblePlay : ConfigDataTable<ConfigBubblePlayRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<string>("BubbleId");
        }
        public ConfigBubblePlayRecord GetById(string id)
        {
            return GetRecordByIndex<string>("BubbleId", id);
        }
        public IndexField<string> GetIndexField()
        {
            return GetIndexField<string>("BubbleId");
        }
    }
}
