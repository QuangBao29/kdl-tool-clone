using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using KAP.Config;
namespace KAP
{
    public class DecoThemeOptions : MonoBehaviour
    {
        [SerializeField]
        private Dropdown _dropdownTheme = null;
        [SerializeField]
        private GameObject _objPanelAdd = null;
        [SerializeField]
        private bool _addZeroTheme = false;

        [SerializeField]
        private UnityEvent _onValueChangeEvent = null;
        private int _mValue = int.MinValue;
    
        public void OnDropDownChangeValue()
        {
            if (_mValue == int.MinValue)
                return;
            var txtValue = _dropdownTheme.captionText.text;
            int.TryParse(txtValue, out _mValue);
            _onValueChangeEvent?.Invoke();
        }

        public int Value
        {
            get
            {
                return _mValue;
            }
            set
            {
                _mValue = value;
                if (_dropdownTheme.options.Count == 0)
                    return;
               
                string txtValue = value.ToString();
                var index = _dropdownTheme.options.FindIndex(option => { return option.text == txtValue; });
                _dropdownTheme.value = index;

            }
        }

        public void Setup(ReadOnlyCollection<ConfigDecoThemeRecord> lstRecords)
        {
            _dropdownTheme.options.Clear();
            var lstOptions = new List<Dropdown.OptionData>();
            if (_addZeroTheme)
                lstOptions.Add(new Dropdown.OptionData("0"));
            foreach (var record in lstRecords)
            {
                lstOptions.Add(new Dropdown.OptionData(record.Id.ToString()));
            }
            _dropdownTheme.AddOptions(lstOptions);
            var index = _dropdownTheme.options.FindIndex(option => { return option.text == _mValue.ToString(); });
            _dropdownTheme.value = index;
        }

        public void OnButtonPlusClick()
        {
            _objPanelAdd.SetActive(!_objPanelAdd.activeSelf);
        }
    }
}

