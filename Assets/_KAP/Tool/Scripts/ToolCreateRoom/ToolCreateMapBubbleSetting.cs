using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KAP.Tools;
using Kawaii.IsoTools;
using Kawaii.IsoTools.DecoSystem;
using System.Linq;
using Kawaii.ResourceManager;

namespace KAP.ToolCreateMap
{   
    public class ToolCreateMapBubbleSetting : MonoBehaviour
    {
        [SerializeField] private Bubble _prefabBubble = null;
        [SerializeField] private EditManager _editManager = null;
        [SerializeField] private GameObject _bubbleContent = null;
        [SerializeField] private InputField _inputMapId = null;
        [SerializeField] private AreaManager _areaManager = null;

        [Header("Deco Item")]
        [SerializeField] private Transform _transGrid = null;
        [SerializeField] private ToolCreateMapBubbleItem _prefabBubbleItem = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        [SerializeField] private ToolCreateMapConfigController _configController = null;

        private List<ToolCreateMapBubbleItem> _lstBubbleItems = new List<ToolCreateMapBubbleItem>();
        private List<int> _lstNumBubbleInRoom = new List<int>();
        private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";

        [HideInInspector] public ToolCreateMapBubbleItem CurrentBubble = null;

        public List<int> LstNumBubbleInRoom
        {
            get => _lstNumBubbleInRoom;
        }

        #region Create Bubble
        public void OnAddBubbleClick()
        {
            UpdateListNumOfBubbleInARoom();
            ToolCreateMapBubbleItem item = SGUtils.InstantiateObject<ToolCreateMapBubbleItem>(_prefabBubbleItem, _transGrid);
            item.Setup(_lstNumBubbleInRoom[_areaManager.ListRooms.Count - 1]);
            item.gameObject.SetActive(true);
            _lstBubbleItems.Add(item);
            DebugForCheck();
        }

        public void UpdateListNumOfBubbleInARoom()
        {
            if (_lstNumBubbleInRoom.Count < _areaManager.ListRooms.Count)
            {
                int count = _areaManager.ListRooms.Count - _lstNumBubbleInRoom.Count;
                for (var i = 0; i < count; i++)
                    _lstNumBubbleInRoom.Add(0);
            }
        }
        public void DebugForCheck()
        {
            for (var i = 0; i < _lstNumBubbleInRoom.Count; i++)
            {
                Debug.LogError("num of bubble in room: " + _lstNumBubbleInRoom[i]);
            }
        }
        public void GetNewIndexBubbleInRoom()
        {

        }

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
                _lstNumBubbleInRoom[bubble.RoomIndex]--;
            else _lstNumBubbleInRoom[0]--;
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
            atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + config.ThemeId + ".asset", 
                config.ThemeId.ToString());

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
        //delete and remove all bubble in a room
        public void RemoveAllBubbleInARoom(int roomIndex)
        {
            List<ToolCreateMapBubbleItem> listTemp = new List<ToolCreateMapBubbleItem>();
            if (roomIndex >= _lstNumBubbleInRoom.Count)
            {
                Debug.LogError("roomIndex out of range");
                return;
            }
            else _lstNumBubbleInRoom.RemoveAt(roomIndex);
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
        #endregion
    }
}

