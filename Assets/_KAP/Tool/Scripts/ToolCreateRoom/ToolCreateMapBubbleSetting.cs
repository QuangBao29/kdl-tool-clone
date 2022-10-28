using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KAP.Tools;
using Kawaii.IsoTools;
using Kawaii.IsoTools.DecoSystem;
using System.Linq;
using Kawaii.ResourceManager;
//using Kawaii.ResourceManager.Editor;

namespace KAP.ToolCreateMap
{   
    public class ToolCreateMapBubbleSetting : MonoBehaviour
    {
        [SerializeField] private Bubble _prefabBubble = null;
        [SerializeField] private EditManager _editManager = null;
        [SerializeField] private GameObject _bubbleContent = null;
        [SerializeField] private InputField _inputMapId = null;
        [SerializeField] private AreaManager _areaManager = null;
        [SerializeField] private GameObject _panelListBubble = null;

        [Header("Deco Item")]
        [SerializeField] private Transform _transGrid = null;
        [SerializeField] private ToolCreateMapBubbleItem _prefabBubbleItem = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField] private ToolCreateMapUnpackingSetting _toolUnpackingSetting = null;
        [SerializeField] private ToolCreateMapConfigController _configController = null;

        private List<ToolCreateMapBubbleItem> _lstBubbleItems = new List<ToolCreateMapBubbleItem>();
        private Dictionary<int, int> _dctNumBubbleInRoom = new Dictionary<int, int>();
        private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";
        

        [HideInInspector] public ToolCreateMapBubbleItem CurrentBubble = null;
        [HideInInspector] public List<ToolCreateMapBubbleItem> ListSwapBubble = new List<ToolCreateMapBubbleItem>();
        public Dictionary<int, int> DctNumBubbleInRoom
        {
            get => _dctNumBubbleInRoom;
        }
        #region Create Bubble
        public void OnAddBubbleClick()
        {
            UpdateListNumOfBubbleInARoom();
            ToolCreateMapBubbleItem item = SGUtils.InstantiateObject<ToolCreateMapBubbleItem>(_prefabBubbleItem, _transGrid);
            item.Setup(_dctNumBubbleInRoom.ElementAt(_dctNumBubbleInRoom.Count - 1).Value);
            item.gameObject.SetActive(true);
            _lstBubbleItems.Add(item);
            _toolBubbleDecoSetting.AddRootClone(item.RoomIndex, item.Index);
            DebugForCheck();
        }
        public void OnImportAddBubble(int RoomIndex, int index, Vector3 BubblePosition)
        {
            UpdateListNumOfBubbleInARoom();
            ToolCreateMapBubbleItem item = SGUtils.InstantiateObject<ToolCreateMapBubbleItem>(_prefabBubbleItem, _transGrid);
            item.Index = index;
            item.RoomIndex = RoomIndex;
            item.UpdateBubbleId();
            Debug.LogError("item.BubbleId: " + item.BubbleId);
            item.BubblePosition = BubblePosition;
            item.gameObject.SetActive(true);
            _lstBubbleItems.Add(item);
            Debug.LogError("num of room: " + _areaManager.ListRooms.Count);
            Debug.LogError("_dctNumBubbleInRoom.Count: " + _dctNumBubbleInRoom.Count);
            Debug.LogError("item.RoomIndex: " + item.RoomIndex);
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Play)
            {
                _dctNumBubbleInRoom[_dctNumBubbleInRoom.ElementAt(0).Key]++;
            }
            else
            {
                _dctNumBubbleInRoom[item.RoomIndex]++;
                Debug.LogError("homeeeeee");
            }
        }
        public void UpdateListNumOfBubbleInARoom()
        {
            if (_dctNumBubbleInRoom.Count < _areaManager.ListRooms.Count)
            {
                for (var i = 0; i < _areaManager.ListRooms.Count; i++)
                {
                    var info = (DecoInfo)_areaManager.ListRooms[i].Info;
                    if (!_dctNumBubbleInRoom.ContainsKey(info.Id))
                    {
                        _dctNumBubbleInRoom.Add(info.Id, 0);
                    }
                }
            }
        }
        public void DebugForCheck()
        {
            for (var i = 0; i < _dctNumBubbleInRoom.Count; i++)
            {
                Debug.LogError("num of bubble in room: " + _dctNumBubbleInRoom.ElementAt(i));
            }
        }
        public void ResetColorAllBubbleItems()
        {
            foreach (var item in _lstBubbleItems)
            {
                item.ResetColor();
            }
        }

        public List<ToolCreateMapBubbleItem> GetLstBubble()
        {
            return _lstBubbleItems;
        }

        public string GetBubbleId(int roomIndex, int bubbleIdx)
        {
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                return (roomIndex + "_" + bubbleIdx).ToString();
            else return (_inputMapId.text + "_" + bubbleIdx).ToString();
        }

        public int GetRoomId(int roomIndex)
        {
            var roomId = 0;
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                return roomIndex;
            else
            {
                if (!int.TryParse(_inputMapId.text, out roomId))
                    Debug.LogError("cannot parse RoomId");
                return roomId;
            }
        }

        public Bubble CreateDecoBubble(int decoId, int colorId)
        {
            var config = _configController.ConfigDeco.GetDecoById(decoId);
            if (config == null)
                return null;
            KawaiiAtlas atlas = null;
#if UNITY_EDITOR
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + config.ThemeId + ".asset", config.ThemeId.ToString());
#endif
            var colorPath = colorId > 0 ? "_" + colorId : "";
            var parameters = new DecoParameters
            {
                IsWallHang = config.IsWallHang,
                SortingLayerName = config.SortingLayerName.ToString(),
                FLSize = new Vector3(config.SizeX, config.SizeY, config.SizeZ),
                FLSprite = atlas != null ? atlas.GetSprite(config.Id + colorPath) : null,
                BRSprite = atlas != null ? atlas.GetSprite(config.Id + "_b" + colorPath) : null,
                CanInFaces = config.CanInAreaFaces,
                ColliderLayer = config.ColliderLayer.GetHashCode(),
                ListColliderRects = config.GetListFLLocalColliderRect()
            };

            if (parameters.BRSprite == null)
                parameters.BRSprite = parameters.FLSprite;

            var allAreas = _configController.ConfigDecoArea.GetListDecoAreaByDecoId(config.Id);
            if (allAreas != null)
            {
                foreach (var record in allAreas)
                {
                    var pieceParameters = new AreaParameters
                    {
                        IsWall = record.IsWall,
                        Face = record.Face.GetHashCode(),
                        SortingLayerName = record.SortingLayerName.ToString(),
                        FLLocalPos = new Vector3(record.FLLocalPosX, record.FLLocalPosY, record.FLLocalPosZ),
                        PieceSize = new Vector2(record.PieceSizeX, record.PieceSizeY),
                        UseRootSorting = record.UseRootSorting,
                        Alone = record.Alone
                    };
                    parameters.ListAreas.Add(pieceParameters);
                }
            }

            var deco = SGUtils.InstantiateObject<Bubble>(_prefabBubble, null);
            deco.gameObject.SetActive(true);
            deco.SetupParameters(parameters);
            deco.name = config.Id.ToString();
            return deco;
        }

        #endregion

        #region Remove Bubble
        public void OnDeleteBubbleClick()
        {
            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                if (root.Key.BubbleId == CurrentBubble.BubbleId)
                {
                    if (root.Key.BubbleDeco != null)
                    {
                        var temp = root.Key.BubbleDeco;
                        temp.Remove();
                        root.Key.BubbleDeco = null;
                    }
                    _toolBubbleDecoSetting.DctRootDecoItems.Remove(root.Key);
                    Destroy(root.Key.gameObject);
                    break;
                }
            }
            _lstBubbleItems.Remove(CurrentBubble);
            var bubble = CurrentBubble;
            CurrentBubble = null;
            Destroy(bubble.gameObject);

            var count = _lstBubbleItems.Count;
            for (var i = 0; i < count; i++)
            {
                if (_lstBubbleItems[i].RoomIndex == bubble.RoomIndex)
                    _lstBubbleItems[i].UpdateIndexAfterDeleteBubble(bubble.Index);
            }
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
                _dctNumBubbleInRoom[bubble.RoomIndex]--;
            else _dctNumBubbleInRoom[_dctNumBubbleInRoom.ElementAt(0).Key]--;
            foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                if (root.Key.RoomIndex == bubble.RoomIndex)
                {
                    if (root.Key.BubbleIndex > bubble.BubbleIndex)
                    {
                        root.Key.BubbleIndex -= 1;
                        root.Key.SetupBubbleId();
                        root.Key.gameObject.name = "Bubble: " + root.Key.BubbleId;
                        foreach (var value in root.Value)
                        {
                            value.BubbleIndex = root.Key.BubbleIndex;
                            value.BubbleId = root.Key.BubbleId;
                        }
                        if (root.Key.BubbleDeco != null)
                        {
                            root.Key.BubbleDeco.BubbleIndex = root.Key.BubbleIndex;
                            root.Key.BubbleDeco.BubbleId = root.Key.BubbleId;
                        }
                    }
                }
            }
            DebugForCheck();
        }
        public void RemoveAllBubbleInARoom(int roomIndex)
        {
            List<ToolCreateMapBubbleItem> listTemp = new List<ToolCreateMapBubbleItem>();
            if (!_dctNumBubbleInRoom.ContainsKey(roomIndex))
            {
                Debug.LogError("roomIndex out of range");
                return;
            }
            else _dctNumBubbleInRoom.Remove(roomIndex);
            foreach (var bubble in _lstBubbleItems)
            {
                if (bubble.RoomIndex == roomIndex)
                    listTemp.Add(bubble);
            }
            foreach (var bub in listTemp)
            {
                CurrentBubble = bub;
                OnDeleteBubbleClick();
            }
            //xoa cac bubbleItem tren man hinh
        }

        public void ClearAll()
        {
            var listTemp = _lstBubbleItems;
            if (_lstBubbleItems.Count > 0)
            {
                foreach (var bubble in _lstBubbleItems)
                {
                    foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
                    {
                        if (root.Key.BubbleId == bubble.BubbleId)
                        {
                            if (root.Key.BubbleDeco != null)
                            {
                                var temp = root.Key.BubbleDeco;
                                temp.Remove();
                                root.Key.BubbleDeco = null;
                            }
                            _toolBubbleDecoSetting.DctRootDecoItems.Remove(root.Key);
                            Destroy(root.Key.gameObject);
                            break;
                        }
                    }
                }
                foreach (var bubble in listTemp)
                {
                    Destroy(bubble.gameObject);
                }
                _lstBubbleItems.Clear();
            }
        }
        public void ClearBubbles()
        {
            _dctNumBubbleInRoom.Clear();
            ClearAll();
            _toolUnpackingSetting.ClearAll();
        }
        #endregion

        #region Utils
        public void ShowPanelListBubble()
        {
            _panelListBubble.SetActive(true);
        }

        public void HidePanelListBubble()
        {
            _panelListBubble.SetActive(false);
        }

        public void SwapBubbleInSameRoom()
        {
            var tempIndex_0 = ListSwapBubble[0].Index;
            var tempBubbleId_0 = ListSwapBubble[0].BubbleId;
            var tempIndex_1 = ListSwapBubble[1].Index;
            var tempBubbleId_1 = ListSwapBubble[1].BubbleId;
            ListSwapBubble[0].Index = tempIndex_1;
            ListSwapBubble[0].UpdateBubbleId();
            ListSwapBubble[1].Index = tempIndex_0;
            ListSwapBubble[1].UpdateBubbleId();
            int index0 = 0;
            int index1 = 0;
            foreach (var pair in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                if (pair.Key.BubbleId == tempBubbleId_0)
                {
                    pair.Key.BubbleIndex = tempIndex_1;
                    pair.Key.BubbleId = tempBubbleId_1;
                    pair.Key.gameObject.name = "Bubble: " + pair.Key.BubbleId;
                    foreach (var value in pair.Value)
                    {
                        value.BubbleIndex = pair.Key.BubbleIndex;
                        value.BubbleId = pair.Key.BubbleId;
                    }
                    if (pair.Key.BubbleDeco != null)
                    {
                        pair.Key.BubbleDeco.BubbleIndex = pair.Key.BubbleIndex;
                        pair.Key.BubbleDeco.BubbleId = pair.Key.BubbleId;
                    }
                    break;
                }
                index0++;
            }
            foreach (var pair in _toolBubbleDecoSetting.DctRootDecoItems)
            {
                if (pair.Key.BubbleId == tempBubbleId_1 && index1 != index0)
                {
                    pair.Key.BubbleIndex = tempIndex_0;
                    pair.Key.BubbleId = tempBubbleId_0;
                    pair.Key.gameObject.name = "Bubble: " + pair.Key.BubbleId;
                    foreach (var value in pair.Value)
                    {
                        value.BubbleIndex = pair.Key.BubbleIndex;
                        value.BubbleId = pair.Key.BubbleId;
                    }
                    if (pair.Key.BubbleDeco != null)
                    {
                        pair.Key.BubbleDeco.BubbleIndex = pair.Key.BubbleIndex;
                        pair.Key.BubbleDeco.BubbleId = pair.Key.BubbleId;
                    }
                    break;
                }
                index1++;
            }

            foreach (var item in ListSwapBubble)
            {
                item.ResetColorImgSwap();
            }
            ListSwapBubble.Clear();
            SortListBubble();
        }

        public void SortListBubble()
        {
            List<ToolCreateMapBubbleItem> sortedList = new List<ToolCreateMapBubbleItem>();
            List<int> lstRoomIndex = new List<int>();
            foreach (var item in _lstBubbleItems)
            {
                if (!lstRoomIndex.Contains(item.RoomIndex))
                    lstRoomIndex.Add(item.RoomIndex);
            }
            lstRoomIndex.Sort();
            
            foreach (var roomIndex in lstRoomIndex)
            {
                List<int> lstBubbleIndex = new List<int>();
                foreach (var item in _lstBubbleItems)
                {
                    if (item.RoomIndex == roomIndex)
                    {
                        lstBubbleIndex.Add(item.Index);
                    }
                }
                lstBubbleIndex.Sort();
                foreach (var index in lstBubbleIndex)
                {
                    foreach (var item in _lstBubbleItems)
                    {
                        if (item.Index == index && item.RoomIndex == roomIndex)
                            sortedList.Add(item);
                    }
                }
            }
            _lstBubbleItems = sortedList;
            var sortedArray = _lstBubbleItems.ToArray();
            for (var i = 0; i < sortedArray.Length; i++)
            {
                sortedArray[i].transform.SetSiblingIndex(i);
            }
        }
        #endregion
    }
}

