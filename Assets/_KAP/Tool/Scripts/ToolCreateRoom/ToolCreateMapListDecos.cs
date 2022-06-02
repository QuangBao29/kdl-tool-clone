using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneP.InfinityScrollView;
using KAP.Config;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapListDecos : MonoBehaviour
    {
        [SerializeField]
        private EditManager _editManager = null;
        [SerializeField]
        private ToolCreateMapConfigController _configController = null;
        [SerializeField]
        private InfinityScrollView _scrollDecoList = null;
        [SerializeField]
        private ListDecoFilter _filterCenter = null;

        private List<ConfigDecoRecord> _lstRecords = new List<ConfigDecoRecord>();
        public void Setup()
        {
            var configDeco = _configController.ConfigDeco;
            _filterCenter.SetupThemeOptions(_configController.ConfigDecoTheme.Records);
            _lstRecords = _filterCenter.Filter(configDeco.Records);
            _scrollDecoList.Setup(_lstRecords.Count);
            _scrollDecoList.InternalReload();
        }

        public ConfigDecoRecord GetConfigByIndex(int index)
        {
            if (_lstRecords == null || index < 0 || index >= _lstRecords.Count)
                return null;
            return _lstRecords[index];
        }

        public void OnChangeCurrentEditDeco()
        {
            var cur = _editManager.Current;
            gameObject.SetActive(cur == null);
        }
    }
}

