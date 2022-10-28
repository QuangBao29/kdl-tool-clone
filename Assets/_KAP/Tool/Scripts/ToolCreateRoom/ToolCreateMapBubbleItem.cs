using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KAP.Tools;
using DigitalRubyShared;
using UnityEngine.UI;
using Kawaii.ResourceManager;
using KAP;
using Kawaii.IsoTools.DecoSystem;
using System.Linq;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapBubbleItem : Bubble
    {
        [Space]
        [SerializeField] private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField] private Image _img = null;
        [SerializeField] private Text _name = null;
        [SerializeField] private Color _color;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Image _swapImg = null;
        [SerializeField] private Color _selectedSwapBtnColor;
        [SerializeField] private Color _defaultSwapBtnColor;

        private int _index;
        private Vector3 _bubblePosition;

        public Vector3 BubblePosition
        {
            set => _bubblePosition = value;
            get => _bubblePosition;
        }
        public int Index
        {
            get => _index;
            set => _index = value;
        }
        public void OnTapBubbleItem()
        {
            _toolBubbleSetting.ResetColorAllBubbleItems();
            _img.color = _color;
            _swapImg.color = _defaultSwapBtnColor;
            _toolBubbleSetting.CurrentBubble = this;
            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                if (root.Key.BubbleId != this.BubbleId) root.Key.gameObject.SetActive(false);
                else root.Key.gameObject.SetActive(true);
            }
        }
        public void OnClickSwapBtn()
        {
            if (_swapImg.color == _defaultSwapBtnColor)
            {
                _swapImg.color = _selectedSwapBtnColor;
                _toolBubbleSetting.ListSwapBubble.Add(this);
                if (_toolBubbleSetting.ListSwapBubble.Count == 2)
                {
                    _toolBubbleSetting.SwapBubbleInSameRoom();
                }
            }
            else
            {
                _swapImg.color = _defaultSwapBtnColor;
                _toolBubbleSetting.ListSwapBubble.Remove(this);
            }
            
        }
        public void ResetColor()
        {
            _img.color = _defaultColor;
        }
        public void ResetColorImgSwap()
        {
            _swapImg.color = _defaultSwapBtnColor;
        }
        public void UpdateBubbleId()
        {
            this.BubbleId = this.RoomIndex + "_" + _index;
            _name.text = this.BubbleId;
            this.gameObject.name = _name.text;
        }
        public void UpdateInfo(Vector3 BubblePosition, int roomIndex, int BubbleIndex)
        {
            _bubblePosition = BubblePosition;
            this.RoomIndex = _toolBubbleSetting.GetRoomId(roomIndex);
            _index = BubbleIndex;
            UpdateBubbleId();
        }
        public void Setup(int index)
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                _index = index;
            else _index = _toolBubbleSetting.DctNumBubbleInRoom.ElementAt(0).Value;
            var info = (DecoInfo)_areaManager.ListRooms[_areaManager.ListRooms.Count - 1].Info;
            this.RoomIndex = info.Id;
            UpdateBubbleId();
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
            {
                _toolBubbleSetting.DctNumBubbleInRoom[_toolBubbleSetting.DctNumBubbleInRoom.ElementAt(0).Key]++;
                Debug.LogError("0++");
            }
            else
            {
                _toolBubbleSetting.DctNumBubbleInRoom[_toolBubbleSetting.DctNumBubbleInRoom.ElementAt(_toolBubbleSetting.DctNumBubbleInRoom.Count - 1).Key]++;
            }
        }
        public void UpdateIndexAfterDeleteBubble(int deletedIndex)
        {
            if (_index > deletedIndex)
            {
                _index--;
                UpdateBubbleId();
            }
        }
    }
}

