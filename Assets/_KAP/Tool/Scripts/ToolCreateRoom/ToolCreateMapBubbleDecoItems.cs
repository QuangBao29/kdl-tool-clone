using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KAP.Tools;
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
        [SerializeField] private ToolCreateMapImportDeco _importDecoController = null;
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
        private Deco _deco = null;

        public Bubble BubbleDeco
        {
            set => _bubble = value;
            get => _bubble;
        }
        public Deco Deco
        {
            set => _deco = value;
            get => _deco;
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
            Debug.LogError("current BubbleId: " + _toolBubbleSetting.CurrentBubble.BubbleId);

            _toolBubbleSetting.CurrentBubble.DctDecoIdColor[id].Remove(colorId);
            if (_toolBubbleSetting.CurrentBubble.DctDecoIdColor[id].Count == 0)
                _toolBubbleSetting.CurrentBubble.DctDecoIdColor.Remove(id);

            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                if (root.Key.BubbleId == _toolBubbleSetting.CurrentBubble.BubbleId)
                {
                    foreach (var deco in root.Value)
                    {
                        var decoinfo = deco.ParseInfo<DecoInfo>();
                        if (decoinfo.Id == id && decoinfo.Color == colorId)
                        {
                            if (root.Key.BubbleDeco != null)
                            {
                                var info = (DecoInfo)root.Key.BubbleDeco.Info;
                                if (info.Id == id && info.Color == colorId)
                                {
                                    var temp = root.Key.BubbleDeco;
                                    temp.Remove();
                                    root.Key.BubbleDeco = null;
                                }
                            }
                            else
                            {
                                var current = _editManager.Current;
                                if (current != null)
                                {
                                    var currentBubble = current.gameObject.GetComponent<Bubble>();
                                    if (currentBubble != null)
                                    {
                                        if (currentBubble.Prefab == this)
                                        {
                                            _editManager.SetCurrent(null);
                                            current.deco.Remove();
                                        }
                                    }
                                }
                            }
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
        
        public void OnImportSpawnDeco(ToolCreateMapBubbleDecoItems root, Vector3 position)
        {
            var decoInfo = this.ParseInfo<DecoInfo>();
            var bubble = _toolBubbleSetting.CreateDecoBubble(decoInfo.Id, decoInfo.Color);
            bubble.Info = new DecoInfo { Id = decoInfo.Id, Color = decoInfo.Color, IsBubble = true };
            bubble.BubbleIndex = _bubbleIndex;
            bubble.RoomIndex = _roomIdx;
            bubble.BubbleId = bubble.RoomIndex + "_" + bubble.BubbleIndex;
            bubble.Prefab = this;
            bubble.Position = position;
            OnOkCLick(bubble);
            if (root.BubbleDeco != null)
            {
                var temp = root.BubbleDeco;
                temp.Remove();
                root.BubbleDeco = null;
            }
            root.BubbleDeco = bubble;
            foreach (var item in _toolBubbleDecoSetting.DctRootDecoItems[root])
            {
                item.UnActiveImgCheck();
            }
            SetActiveImgCheck();
        }
        public void OnOkCLick(Bubble bubble)
        {
            Debug.LogError("kiem tra: " + bubble.BubbleId);
            var current = bubble.GetComponent<DecoEditDemo>();
            if (_editManager.SetCurrent(current))
            {
                current.StartMove();
                current.EndMove();
                _editManager.editTool.SetValid(current.EditStatus);
            }
            if (current.deco.IsWallHang)
            {
                Debug.LogError("deco nay wall hang: " + bubble.BubbleId);
                _editManager.Current.Rotate(1);
            }
            switch (current.EditStatus)
            {
                case KHHEditStatus.Valid:
                    Debug.LogError("check valid: " + bubble.BubbleId);
                    _editManager.SetCurrent(null);
                    break;
                case KHHEditStatus.CanSwap:
                    _editManager.SetCurrent(null);
                    var swapDeco = current.ListOverlaps[0];
                    var swapDecoEdit = swapDeco.GetComponent<DecoEditDemo>();
                    swapDecoEdit.StartMove();
                    _editManager.SetCurrent(swapDecoEdit);

                    current.EndMove();
                    if (current.EditStatus != KHHEditStatus.Valid)
                    {
                        _editManager.SetCurrent(null);
                        _editManager.SetCurrent(current);
                    }

                    //for tile & wallpaper
                    var colliderLayer = swapDecoEdit.deco.gameObject.layer;
                    if (colliderLayer == (int)DemoColliderLayer.Tile || colliderLayer == (int)DemoColliderLayer.Wallpaper)
                    {
                        var lstAreaPieces = swapDecoEdit.deco.LstAreaPieces;
                        foreach (var piece in lstAreaPieces)
                        {
                            piece.Lock();
                            var cloneList = new List<Deco>(piece.LstChilds);
                            foreach (var deco in cloneList)
                            {
                                deco.Apply(null, null);
                                var moveData = _areaManager.Move(deco);
                                if (moveData != null)
                                {
                                    deco.Apply(moveData.piece, moveData.overlapPieces);
                                }
                            }
                            piece.Unlock();
                        }
                    }
                    swapDecoEdit.EndMove();
                    break;
            }
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
        public void OnClickSpawnUnpackingDeco()
        {
            var decoInfo = this.ParseInfo<DecoInfo>();
            var deco = _importDecoController.CreateDeco(decoInfo.Id, decoInfo.Color);
            deco.Info = new DecoInfo { Id = decoInfo.Id, Color = decoInfo.Color, IsUnpacking = true, IsBubble = false };
            deco.Position = IsoWorld.WorldToIso(Camera.main.transform.position, 0);
            var decoEdit = deco.GetComponent<DecoEditDemo>();
            if (_editManager.Current != null)
            {
                var current = _editManager.Current;
                foreach (var item in _toolUnpackingSetting.LstDecoItem)
                {
                    if (item.Deco == current.deco)
                    {
                        item.UnActiveImgCheck();
                        break;
                    }
                }
                _editManager.SetCurrent(null);
                current.deco.Remove();
            }
            if (_editManager.SetCurrent(decoEdit))
            {
                decoEdit.StartMove();
                decoEdit.EndMove();
                _editManager.editTool.SetValid(decoEdit.EditStatus);
            }
            this.Deco = deco;
            //foreach (var item in _toolUnpackingSetting.LstDecoItem)
            //{
            //    if (decoInfo.IsUnpacking)
            //    {
            //        Debug.LogError("unpacking nha");
            //        item.UnActiveImgCheck();
            //    }
            //}
            SetActiveImgCheck();
        }
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
                        var current = _editManager.Current;
                        if (item.Deco != null)
                        {
                            if (current != null && item.Deco == current.deco)
                            {
                                _editManager.SetCurrent(null);
                                item.Deco.Remove();
                            }
                            else item.Deco.Remove();
                        }
                        _toolUnpackingSetting.LstDecoItem.Remove(item);
                        break;
                    }                   
                }
                _toolUnpackingSetting.LstUnpackDeco.Remove(decoId);
            }
        }
        #endregion
    }

}
