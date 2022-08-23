using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KAP;
using KAP.Config;
using Kawaii.IsoTools.DecoSystem;
using Kawaii.IsoTools;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapBubbleDecoItems : Deco
    {
        [SerializeField] private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField] private ToolCreateMapUnpackingSetting _toolUnpackingSetting = null;
        [SerializeField] private EditManager _editManager = null;
        [SerializeField] private GameObject _imgCheck = null;
        public Image Image = null;
        public Text Name = null;

        private int _bubbleIndex;
        private string _bubbleId;
        private int _roomIdx;
        private Vector3 _bubblePosition;

        private ToolCreateMapBubbleItem _prefab = null;
        private Bubble _bubble = null;

        public Bubble BubbleDeco
        {
            set => _bubble = value;
            get => _bubble;
        }
        public ToolCreateMapBubbleItem Prefab
        {
            set => _prefab = value;
            get => _prefab;
        }
        public int BubbleIndex
        {
            set
            {
                _bubbleIndex = value;
            }
            get
            {
                return _bubbleIndex;
            }
        }
        public string BubbleId
        {
            get => _bubbleId;
            set => _bubbleId = value;
        }
        public int RoomIndex
        {
            get => _roomIdx;
            set => _roomIdx = value;
        }
        public Vector3 BubblePosition
        {
            get => _bubblePosition;
            set => _bubblePosition = value;
        }

        public void SetupBubbleId()
        {
            _bubbleId = _roomIdx + "_" + _bubbleIndex;
        }

        #region Bubble Deco

        public void UpDateInfo(int RoomIndex, Vector3 BubblePosition, int BubbleIndex)
        {
            _roomIdx = _toolBubbleSetting.GetRoomId(RoomIndex);
            _bubblePosition = BubblePosition;
            _bubbleIndex = BubbleIndex;
            SetupBubbleId();
        }
        public void UpdateName()
        {
            var info = this.ParseInfo<DecoInfo>();
            var colorPath = info.Color > 0 ? "_" + info.Color : "";
            Name.text = info.Id.ToString() + colorPath;
        }
        public void OnButtonRemoveItemClick()
        {
            var decoInfo = this.ParseInfo<DecoInfo>();
            var id = decoInfo.Id;
            var colorId = decoInfo.Color;

            _toolBubbleSetting.CurrentBubble.DctDecoIdColor[id].Remove(colorId);
            if (_toolBubbleSetting.CurrentBubble.DctDecoIdColor[id].Count == 0)
                _toolBubbleSetting.CurrentBubble.DctDecoIdColor.Remove(id);
            Debug.LogError("removed from DctDecoIdColor");

            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                if (root.Key.BubbleId == _toolBubbleSetting.CurrentBubble.BubbleId)
                {
                    foreach (var deco in root.Value)
                    {
                        var decoinfo = deco.ParseInfo<DecoInfo>();
                        if (decoinfo.Id == id && decoinfo.Color == colorId)
                        {
                            root.Value.Remove(deco);
                            Destroy(deco.gameObject);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        public void OnClickSpawnDeco()
        {
            var decoInfo = this.ParseInfo<DecoInfo>();
            var bubble = _toolBubbleSetting.CreateDecoBubble(decoInfo.Id, decoInfo.Color);
            bubble.Info = new DecoInfo { Id = decoInfo.Id, Color = decoInfo.Color, IsBubble = true };
            bubble.BubbleIndex = _bubbleIndex;
            bubble.RoomIndex = _roomIdx;
            bubble.BubbleId = bubble.RoomIndex + "_" + bubble.BubbleIndex;
            bubble.Prefab = this;
            bubble.Position = IsoWorld.WorldToIso(Camera.main.transform.position, 0);
            var decoEdit = bubble.GetComponent<DecoEditDemo>();
            if (_editManager.Current != null)
            {
                var current = _editManager.Current;
                _editManager.SetCurrent(null);
                current.deco.Remove();
            }
            if (_editManager.SetCurrent(decoEdit))
            {
                decoEdit.StartMove();
                decoEdit.EndMove();
                _editManager.editTool.SetValid(decoEdit.EditStatus);
            }

            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                if (root.Key.BubbleId == _toolBubbleSetting.CurrentBubble.BubbleId)
                {
                    if (root.Key.BubbleDeco != null)
                    {
                        var temp = root.Key.BubbleDeco;
                        temp.Remove();
                        root.Key.BubbleDeco = null;
                    }
                    //root.Key.BubbleDeco = bubble;
                    foreach (var item in root.Value)
                    {
                        item.UnActiveImgCheck();
                    }
                    break;
                }
            }
            SetActiveImgCheck();
        }

        public void UnActiveImgCheck()
        {
            _imgCheck.SetActive(false);
        }
        public void SetActiveImgCheck()
        {
            _imgCheck.SetActive(true);
        }

        #endregion

        #region Deco Unpacking

        public void OnButtonRemoveDecoUnpackClick()
        {
            var decoInfo = this.ParseInfo<DecoInfo>();
            var id = decoInfo.Id;
            var colorId = decoInfo.Color;
            var decoId = (id + "_" + colorId).ToString();
            if (_toolUnpackingSetting.LstUnpackDeco.Contains(decoId))
            {
                foreach (var item in _toolUnpackingSetting.LstDecoItem)
                {
                    var info = item.ParseInfo<DecoInfo>();
                    var IdItem = info.Id + "_" + info.Color;
                    if (decoId == IdItem)
                    {
                        Destroy(item.gameObject);
                        _toolUnpackingSetting.LstDecoItem.Remove(item);
                        break;
                    }                   
                }
                _toolUnpackingSetting.LstUnpackDeco.Remove(decoId);
                Debug.LogError("Id removed: " + decoId + " count unpacking: " + _toolUnpackingSetting.LstUnpackDeco.Count);
            }
        }
        #endregion
    }

}
