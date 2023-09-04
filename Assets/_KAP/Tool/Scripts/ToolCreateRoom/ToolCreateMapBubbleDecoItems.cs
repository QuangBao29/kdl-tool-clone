using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KAP.Tools;
using KAP.Config;
using Fingers;
using Kawaii.IsoTools.DecoSystem;
using Kawaii.IsoTools;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapBubbleDecoItems : Deco
    {
        [SerializeField] private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField] private ToolCreateMapImportDeco _importDecoController = null;
        [SerializeField] private EditManager _editManager = null;
        [SerializeField] private GameObject _imgCheck = null;
        [SerializeField] private SGPanZoom _sgPanZoom = null;
        [SerializeField] private Camera _cam = null;
        [Header("Config Data")]
        [SerializeField] private InputField _inputStar = null;
        [SerializeField] private InputField _inputIndex = null;
        [SerializeField] private InputField _inputPrice = null;
        [Space]
        public Image Image = null;
        public Text Name = null;
        public float _editCameraZoom = 6.5f;

        private int _bubbleIndex;
        private string _bubbleId;
        private int _roomId;
        private Vector3 _bubblePosition;
        private Bubble _bubble = null;
        
        public Bubble BubbleDeco
        {
            set => _bubble = value;
            get => _bubble;
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
        public int RoomId
        {
            get => _roomId;
            set => _roomId = value;
        }
        public Vector3 BubblePosition
        {
            get => _bubblePosition;
            set => _bubblePosition = value;
        }

        public void SetupBubbleId()
        {
            _bubbleId = _roomId + "_" + _bubbleIndex;
        }
        public string GetStar()
        {
            return _inputStar.text;
        }
        public void SetStar(string star)
        {
            _inputStar.text = star;
        }
        public string GetIndex()
        {
            return _inputIndex.text;
        }
        public void SetIndex(string idx)
        {
            _inputIndex.text = idx;
        }
        public string GetPrice()
        {
            return _inputPrice.text;
        }
        public void SetPrice(string price)
        {
            _inputPrice.text = price;
        }
        #region Bubble Deco

        public void OnClickTargetDecoItem()
        {
            var deco = _toolBubbleSetting.DctDecoInRoom[BubbleId];
            if (deco == null)
            {
                Debug.LogError("deco null");
                return;
            }
            if (_editManager.Current != null)
            {
                var cur = _editManager.Current;
                cur.deco.Remove();
                _editManager.SetCurrent(null);
            }
            var decoInfo = (DecoInfo)deco.Info;
            var curInfo = (DecoInfo)this.Info;
            if (_cam.orthographicSize > _editCameraZoom)
                _sgPanZoom.ZoomSmooth(_cam.orthographicSize, _editCameraZoom, 0.5f, null);
            var centerPos = deco.Position;
            var flyWorldPos = IsoWorld.IsoToWorld(centerPos);
            _sgPanZoom.FlyTo(flyWorldPos, true, 0.5f);
            if (decoInfo.Id == curInfo.Id && decoInfo.Color == curInfo.Color)
            {
                Debug.LogError("dung deco");
                _editManager.SetCurrent(deco.GetComponent<DecoEditDemo>());
            }
            else
            {
                Debug.LogError("case 2");
                var allChilds = deco.GetAllChilds();
                if (allChilds != null)
                {
                    foreach (var child in allChilds)
                    {
                        var childDecoInfo = child.ParseInfo<DecoInfo>();
                        string childId = childDecoInfo.Id.ToString() + "_" + childDecoInfo.Color.ToString();
                        if (childDecoInfo != null) _toolBubbleSetting.LstDecoBoxID.Add(childId);
                        else Debug.LogError("child info not found");
                    }
                }
                deco.Remove();
                var newDeco = _importDecoController.CreateDeco(curInfo.Id, curInfo.Color);
                newDeco.Info = new DecoInfo { Id = curInfo.Id, Color = curInfo.Color, IsBubble = true };
                newDeco.Position = deco.Position;
                newDeco.WorldDirect = deco.WorldDirect;
                var decoEdit = newDeco.GetComponent<DecoEditDemo>();

                var moveData = _areaManager.Move(newDeco);
                if (moveData.ListOverlaps != null)
                {
                    foreach (var decoItem in moveData.ListOverlaps)
                    {
                        if (decoItem == newDeco)
                        {
                            continue;
                        }
                        var curDeco = decoItem;
                        var children = curDeco.GetAllChilds();
                        foreach (var child in children)
                        {
                            var childDecoInfo = child.ParseInfo<DecoInfo>();
                            string childId = childDecoInfo.Id.ToString() + "_" + childDecoInfo.Color.ToString();
                            if (childDecoInfo != null) _toolBubbleSetting.LstDecoBoxID.Add(childId);
                            else Debug.LogError("child info not found");
                            child.Remove();
                        }
                        var itemInfo = curDeco.ParseInfo<DecoInfo>();
                        string itemId = itemInfo.Id.ToString() + "_" + itemInfo.Color.ToString();
                        _toolBubbleSetting.LstDecoBoxID.Add(itemId);
                        curDeco.Remove();
                    }
                }

                if (_editManager.SetCurrent(decoEdit))
                {
                    decoEdit.StartMove();
                    decoEdit.EndMove();
                }
                if (decoEdit.EditStatus == KHHEditStatus.Valid) _editManager.SetCurrent(null);
                _toolBubbleSetting.DctDecoInRoom[BubbleId] = newDeco;
            }
        }
        public void UpDateInfo(int RoomIndex, Vector3 BubblePosition, int BubbleIndex)
        {
            //_roomId = _toolBubbleSetting.GetRoomId(RoomIndex);
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
        public void OnChangeInputIndex()
        {
            BubbleId = RoomId + "_" + _inputIndex.text;
        }
        public void OnChangeInputStar()
        {
            _toolBubbleDecoSetting.OnChangeInputStars(_inputStar.text);
        }
        public void OnButtonRemoveItemClick()
        {
            var deco = _toolBubbleSetting.DctDecoInRoom[BubbleId];
            if (deco == null)
            {
                Debug.LogError("deco null");
                return;
            }
            
            //if (deco != null)
            //{
            //    var info = (DecoInfo)deco.Info;
            //    info.IsBubble = false;
            //    deco.Info = info;
            //}
            
            _editManager.SetCurrent(null);
            var curIdx = SGUtils.ParseStringToListInt(BubbleId, '_')[1];
            Debug.LogError("cur Idx removed: " + curIdx);
            //foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            //{
            //    if (root.Key.RoomId == RoomId)
            //    {
            //        foreach (var item in root.Value)
            //        {
            //            var index = SGUtils.ParseStringToListInt(item.BubbleId, '_')[1];
            //            if (index > curIdx)
            //            {
            //                index--;
            //                item.BubbleId = item.RoomId + "_" + index;
            //                item.SetIndex(index.ToString());
            //            }
            //        }
            //    }
            //}
            //foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            //{
            //    if (root.Key.RoomId == RoomId)
            //    {
            //        if (root.Value.Contains(this))
            //        {
            //            root.Value.Remove(this);
            //            Destroy(this.gameObject);
            //            break;
            //        }
            //    }
            //}
        }

        
        
        public void OnOkCLick(Bubble bubble)
        {
            var current = bubble.GetComponent<DecoEditDemo>();
            if (_editManager.SetCurrent(current))
            {
                current.StartMove();
                current.EndMove();
                _editManager.editTool.SetValid(current.EditStatus);
            }
            if (current.deco.IsWallHang && current.EditStatus != KHHEditStatus.Valid)
            {
                _editManager.Current.Rotate(1);
            }
            switch (current.EditStatus)
            {
                case KHHEditStatus.Valid:
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
    }

}
