using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;
using Kawaii.IsoTools;
using KAP.ToolCreateMap;
using Kawaii.IsoTools.DecoSystem;
using UnityEngine.UI;
using KAP.Tools;
using System.Threading.Tasks;

namespace KAP
{
    public class DecoEditTool : MonoBehaviour
    {
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField]
        private EditManager _editManager = null;
        [SerializeField]
        private ToolCreateMapImportDeco _importDecoController = null;
        [SerializeField]
        private SpriteRenderer _sprCircle = null;
        [SerializeField]
        private SpriteRenderer _sprBtnOk = null;
        [SerializeField]
        private List<GameObject> _lstObjOffWhenMove = new List<GameObject>();

        [SerializeField] private InputField _inputfielGroup = null;
        [SerializeField] private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField] private ToolCreateMapUnpackingSetting _toolUnpackingSetting  = null;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        #region Buttons
        public void OnTapRotate(GestureRecognizer gesture)
        {
            switch (gesture.State)
            {
                case GestureRecognizerState.Ended:
                    if (_editManager.Current != null)
                        _editManager.Current.Rotate(1);
                    break;
            }
        }

        public void OnTapRemove(GestureRecognizer gesture)
        {
            switch (gesture.State)
            {
                case GestureRecognizerState.Ended:
                    if (_editManager.Current != null)
                    {
                        var current = _editManager.Current;
                        var info = (DecoInfo)current.deco.Info;
                        if (info.IsUnpacking)
                        {
                            foreach (var item in _toolUnpackingSetting.LstDecoItem)
                            {
                                if (item.Deco == current.deco)
                                {
                                    item.UnActiveImgCheck();
                                    break;
                                }
                            }
                        }
                        else if (info.IsBubble)
                        {
                            var currentBubble = current.gameObject.GetComponent<Bubble>();
                            if (currentBubble != null)
                            {
                                if (currentBubble.Prefab != null)
                                currentBubble.Prefab.UnActiveImgCheck();
                            }
                        }
                        _editManager.SetCurrent(null);
                        current.deco.Remove();
                    }
                    break;
            }
        }

        public void OnTapOk(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                var current = _editManager.Current;
                if (current == null)
                    return;
                int group = 0;
                if(int.TryParse(_inputfielGroup.text, out group))
                {
                    current.deco.Group = group != 0 ? group : 0;
                }
                switch (current.EditStatus)
                {
                    case KHHEditStatus.Valid:
                        if (ToolEditMode.Instance.CurrentPhaseMode == PhaseMode.Unpacking)
                        {
                            var info = current.deco.ParseInfo<DecoInfo>();
                            var decoId = info.Id + "_" + info.Color;
                            if (info.IsUnpacking && !_toolUnpackingSetting.LstUnpackDeco.Contains(decoId))
                                _toolUnpackingSetting.ButtonAddDecoToListUnpack();
                        }
                        else if (ToolEditMode.Instance.CurrentPhaseMode == PhaseMode.Bubble)
                        {
                            var currentbubble = current.gameObject.GetComponent<Bubble>();
                            if (currentbubble != null && currentbubble.Prefab == null)
                            {
                                Debug.LogError("cos component bubble va prefab null");
                                var bubbleInfo = (DecoInfo)current.deco.Info;
                                if (_toolBubbleSetting.CurrentBubble.DctDecoIdColor.ContainsKey(bubbleInfo.Id) &&
                                        _toolBubbleSetting.CurrentBubble.DctDecoIdColor[bubbleInfo.Id].Contains(bubbleInfo.Color))
                                {
                                    Debug.LogError("this deco is already in Bubble! o tren");
                                    return;
                                }
                            }
                            
                        }
                        _editManager.SetCurrent(null);
                        break;
                    case KHHEditStatus.CanSwap:
                        //_editManager.SetCurrent(null);
                        //var swapDeco = current.ListOverlaps[0];
                        //var swapDecoEdit = swapDeco.GetComponent<DecoEditDemo>();
                        //swapDecoEdit.StartMove();
                        //_editManager.SetCurrent(swapDecoEdit);

                        //current.EndMove();
                        //if (current.EditStatus != KHHEditStatus.Valid)
                        //{
                        //    _editManager.SetCurrent(null);
                        //    _editManager.SetCurrent(current);
                        //}

                        ////for tile & wallpaper

                        //var colliderLayer = swapDecoEdit.deco.gameObject.layer;
                        //if (colliderLayer == (int)DemoColliderLayer.Tile || colliderLayer == (int)DemoColliderLayer.Wallpaper)
                        //{
                        //    var lstAreaPieces = swapDecoEdit.deco.LstAreaPieces;
                        //    foreach (var piece in lstAreaPieces)
                        //    {
                        //        piece.Lock();
                        //        var cloneList = new List<Deco>(piece.LstChilds);
                        //        foreach (var deco in cloneList)
                        //        {
                        //            deco.Apply(null, null);
                        //            var moveData = _areaManager.Move(deco);
                        //            if (moveData != null)
                        //            {
                        //                deco.Apply(moveData.piece, moveData.overlapPieces);
                        //            }
                        //        }
                        //        piece.Unlock();
                        //    }
                        //}
                        //swapDecoEdit.EndMove();
                        break;
                }
                
                var newRoomIdx = current.deco.Root.ParseInfo<DecoInfo>().Id;
                int newBubbleIndex;
                Debug.LogError("newRoomIdx: " + newRoomIdx);
                var currentBubble = current.gameObject.GetComponent<Bubble>();
                if (currentBubble != null)
                {
                    var preRoomIndex = currentBubble.RoomIndex;
                    var preBubbleIndex = currentBubble.BubbleIndex;
                    var preBubbleId = currentBubble.BubbleId;

                    #region PHASE BUBBLE
                    //if (ToolEditMode.Instance.CurrentPhaseMode == PhaseMode.Bubble)
                    //{
                    //    var bubbleInfo = (DecoInfo)current.deco.Info;
                    //    if (bubbleInfo.IsBubble && currentBubble.Prefab == null)
                    //    {
                    //        Debug.LogError("Is Bubble va Prefab null");
                    //        if (!_toolBubbleSetting.CurrentBubble.DctDecoIdColor.ContainsKey(bubbleInfo.Id))
                    //        {
                    //            _toolBubbleSetting.CurrentBubble.DctDecoIdColor.Add(bubbleInfo.Id, new List<int>());
                    //            _toolBubbleSetting.CurrentBubble.DctDecoIdColor[bubbleInfo.Id].Add(bubbleInfo.Color);
                    //            currentBubble.Prefab = _toolBubbleDecoSetting.CreateDecoItems(bubbleInfo.Id, bubbleInfo.Color);
                    //            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
                    //            {
                    //                if (root.Key.BubbleId == _toolBubbleSetting.CurrentBubble.BubbleId)
                    //                {
                    //                    foreach (var item in root.Value)
                    //                    {
                    //                        item.UnActiveImgCheck();
                    //                    }
                    //                    break;
                    //                }
                    //            }
                    //            currentBubble.Prefab.SetActiveImgCheck();
                    //        }
                    //        else if (!_toolBubbleSetting.CurrentBubble.DctDecoIdColor[bubbleInfo.Id].Contains(bubbleInfo.Color))
                    //        {
                    //            _toolBubbleSetting.CurrentBubble.DctDecoIdColor[bubbleInfo.Id].Add(bubbleInfo.Color);
                    //            currentBubble.Prefab = _toolBubbleDecoSetting.CreateDecoItems(bubbleInfo.Id, bubbleInfo.Color);
                    //            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
                    //            {
                    //                if (root.Key.BubbleId == _toolBubbleSetting.CurrentBubble.BubbleId)
                    //                {
                    //                    foreach (var item in root.Value)
                    //                    {
                    //                        item.UnActiveImgCheck();
                    //                    }
                    //                    break;
                    //                }
                    //            }
                    //            currentBubble.Prefab.SetActiveImgCheck();
                    //        }
                    //        else
                    //        {
                    //            Debug.LogError("this deco is already in Bubble!");
                    //            return;
                    //        }
                    //    }
                    //}
                    
                    #endregion 

                    Debug.LogError("roomidx bubbleIdx bubbleId: " + preRoomIndex + " " + preBubbleIndex + " " + preBubbleId);
                    Debug.LogError("_areaManager.ListRooms.Count" + _areaManager.ListRooms.Count);
                    foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
                    {
                        Debug.LogError(_toolBubbleDecoSetting.DctRootDecoItems.Count);
                        if (root.Key.BubbleId == preBubbleId)
                        {
                            Debug.LogError("root.Key.BubbleId == preBubbleId");
                            if (root.Key.BubbleDeco != null && root.Key.BubbleDeco != currentBubble)
                            {
                                Debug.LogError("khac null");
                                var temp = root.Key.BubbleDeco;
                                temp.Remove();
                                root.Key.BubbleDeco = null;
                            }
                            root.Key.BubbleDeco = currentBubble;
                            break;
                        }
                    }
                    Debug.LogError("chuyen tiep");
                    if (newRoomIdx != preRoomIndex && ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                    {
                        Debug.LogError("run this shit 1");
                        _toolBubbleSetting.DctNumBubbleInRoom[preRoomIndex]--;
                        Debug.LogError("run this shit 2");
                        newBubbleIndex = _toolBubbleSetting.DctNumBubbleInRoom[newRoomIdx]++;
                        Debug.LogError("run this shit 3");
                        Debug.LogError("newBubbleIndex: " + newBubbleIndex);
                        Debug.LogError("num bubble in room after: " + _toolBubbleSetting.DctNumBubbleInRoom[newRoomIdx]);
                        //update currentBubble info
                        currentBubble.BubbleIndex = newBubbleIndex;
                        currentBubble.RoomIndex = newRoomIdx;
                        currentBubble.BubbleId = currentBubble.RoomIndex + "_" + currentBubble.BubbleIndex;
                        Debug.LogError("currentBubble.BubbleId: " + currentBubble.BubbleId);
                        currentBubble.Prefab.UpDateInfo(newRoomIdx, currentBubble.Position, newBubbleIndex);
                        currentBubble.Prefab.UpdateName();
                        currentBubble.Prefab.Prefab.UpdateInfo(currentBubble.Position, newRoomIdx, newBubbleIndex);
                        foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
                        {
                            if (root.Key.BubbleId == preBubbleId)
                            {
                                root.Key.UpDateInfo(newRoomIdx, currentBubble.Position, newBubbleIndex);
                                root.Key.gameObject.name = "Bubble: " + root.Key.BubbleId;
                                foreach (var value in root.Value)
                                {
                                    value.BubbleIndex = root.Key.BubbleIndex;
                                    value.RoomId = root.Key.RoomId;
                                    value.BubbleId = root.Key.BubbleId;
                                }
                                break;
                            }
                        }
                        var lstBubbleItem = _toolBubbleSetting.GetLstBubble();
                        for (var i = 0; i < lstBubbleItem.Count; i++)
                        {
                            if (lstBubbleItem[i].RoomIndex == preRoomIndex)
                            {
                                lstBubbleItem[i].UpdateIndexAfterDeleteBubble(preBubbleIndex);
                            }
                        }
                        foreach (var pair in _toolBubbleDecoSetting.DctRootDecoItems)
                        {
                            if (pair.Key.RoomId == preRoomIndex)
                            {
                                var newIdx = pair.Key.BubbleIndex > preBubbleIndex ? (pair.Key.BubbleIndex - 1) : pair.Key.BubbleIndex;
                                pair.Key.UpDateInfo(pair.Key.RoomId, pair.Key.BubblePosition, newIdx);
                                pair.Key.gameObject.name = "Bubble: " + pair.Key.BubbleId;
                                foreach (var clone in pair.Value)
                                {
                                    clone.BubbleIndex = pair.Key.BubbleIndex;
                                    clone.BubbleId = pair.Key.BubbleId;
                                }
                                if (pair.Key.BubbleDeco != null)
                                {
                                    pair.Key.BubbleDeco.BubbleIndex = pair.Key.BubbleIndex;
                                    pair.Key.BubbleDeco.BubbleId = pair.Key.BubbleId;
                                }
                            }
                        }
                    }
                    else if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play || (newRoomIdx == preRoomIndex && ToolEditMode.Instance.CurrentEditMode == EditMode.Home))
                    {
                        currentBubble.Prefab.BubblePosition = currentBubble.Position;
                        currentBubble.Prefab.Prefab.BubblePosition = currentBubble.Position;
                        foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
                        {
                            if (root.Key.BubbleId == preBubbleId)
                            {
                                root.Key.BubblePosition = currentBubble.Position;
                                break;
                            }
                        }
                    }
                    _toolBubbleSetting.SortListBubble();
                }
            }
        }

        public void OnTapCloneFL(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                CloneDecor(IsoDirect.FL);
            }
        }

        public void OnTapCloneFR(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                CloneDecor(IsoDirect.FR);
            }
        }

        public void OnTapCloneBL(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                CloneDecor(IsoDirect.BL);
            }
        }

        public void OnTapCloneBR(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                CloneDecor(IsoDirect.BR);
            }
        }

        void CloneDecor(int direct)
        {
            var current = _editManager.Current;
            if (current == null)
                return;
            if (current.EditStatus == KHHEditStatus.Invalid)
                return;
            var info = (DecoInfo)current.deco.Info;
            var clone = _importDecoController.CreateDeco(info.Id, info.Color);
            clone.Info = new DecoInfo { Id = info.Id, Color = info.Color, IsStatic = info.IsStatic};
            clone.WorldDirect = current.deco.WorldDirect;
            clone.Group = current.deco.Group;
            var size = clone.Size;
            var pos = IsoUtils.RemoveZOfPosition(current.deco.Position);
            switch (direct)
            {
                case IsoDirect.FL:
                    pos.x -= size.x;
                    break;
                case IsoDirect.FR:
                    pos.y -= size.y;
                    break;
                case IsoDirect.BL:
                    pos.y += size.y;
                    break;
                case IsoDirect.BR:
                    pos.x += size.x;
                    break;
            }
            clone.Position = pos;
            if (current.EditStatus == KHHEditStatus.CanSwap)
            {
                var swapDeco = current.ListOverlaps[0];
                swapDeco.Apply(null, null);
                var swapDecoEdit = swapDeco.GetComponent<DecoEditDemo>();
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

                current.EndMove();
                swapDeco.Remove();
            }

            var cloneEdit = clone.GetComponent<DecoEditDemo>();
            if (_editManager.SetCurrent(cloneEdit))
            {
                cloneEdit.StartMove();
                cloneEdit.EndMove();
                SetValid(cloneEdit.EditStatus);
            }
            else
            {
                Debug.LogError("Error when clone");
            }
        }

        #endregion

        public void OnPan(GestureRecognizer gesture)
        {
            var current = _editManager.Current;
            if (current == null)
                return;
            current.OnPanEvent(gesture);
        }

        public void Show()
        {
            var current = _editManager.Current;
            if (current == null)
                return;
            var pos = current.deco.transform.position;
            var center = current.deco.BoxCol.offset;
            transform.position = pos + new Vector3(center.x, center.y, 0);
            gameObject.SetActive(true);
            _inputfielGroup.text = current.deco.Group.ToString();
            foreach (var obj in _lstObjOffWhenMove)
                obj.SetActive(true);
            SetValid(current.EditStatus);
        }

        public void StartMove()
        {
            var current = _editManager.Current;
            if (current == null)
                return;
            transform.SetParent(current.transform);
            foreach (var obj in _lstObjOffWhenMove)
                obj.SetActive(false);
        }

        public void EndMove()
        {
            transform.SetParent(_editManager.transform);
            foreach (var obj in _lstObjOffWhenMove)
                obj.SetActive(true);
        }

        public void SetValid(KHHEditStatus status)
        {
            var color = Color.green;
            switch (status)
            {
                case KHHEditStatus.Invalid:
                    color = Color.red;
                    break;
                case KHHEditStatus.CanSwap:
                    color = Color.yellow;
                    break;
            }
            _sprBtnOk.color = _sprCircle.color = color;
        }

#if UNITY_EDITOR
        float _cloneDelayTime = 0;

        private async void Update()
        {
            var cur = _editManager.Current;
            if (cur == null)
                return;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                cur.StartMove();
                var pos = cur.deco.Position;
                pos.x += 1f;
                cur.deco.Position = pos;
                cur.EndMove();
                Show();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                cur.StartMove();
                var pos = cur.deco.Position;
                pos.x -= 1f;
                cur.deco.Position = pos;
                cur.EndMove();
                Show();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                cur.StartMove();
                var pos = cur.deco.Position;
                pos.y += 1f;
                cur.deco.Position = pos;
                cur.EndMove();
                Show();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                cur.StartMove();
                var pos = cur.deco.Position;
                pos.y -= 1f;
                cur.deco.Position = pos;
                cur.EndMove();
                Show();
            }

            if(Input.GetKey(KeyCode.W))
            {
                if(Time.realtimeSinceStartup - _cloneDelayTime > 0.1f)
                {
                    _cloneDelayTime = Time.realtimeSinceStartup;
                    CloneDecor(IsoDirect.BR);
                }

            }
            if (Input.GetKey(KeyCode.A))
            {
                if (Time.realtimeSinceStartup - _cloneDelayTime > 0.1f)
                {
                    _cloneDelayTime = Time.realtimeSinceStartup;
                    CloneDecor(IsoDirect.BL);
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (Time.realtimeSinceStartup - _cloneDelayTime > 0.1f)
                {
                    _cloneDelayTime = Time.realtimeSinceStartup;
                    CloneDecor(IsoDirect.FL);
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (Time.realtimeSinceStartup - _cloneDelayTime > 0.1f)
                {
                    _cloneDelayTime = Time.realtimeSinceStartup;
                    CloneDecor(IsoDirect.FR);
                }
            }

        }
#endif


    }
}

