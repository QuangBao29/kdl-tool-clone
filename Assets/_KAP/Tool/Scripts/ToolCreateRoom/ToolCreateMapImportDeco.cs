using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;
using Kawaii.IsoTools;
using Pathfinding.Serialization.JsonFx;
using Kawaii.ResourceManager;
using KAP.Tools;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapImportDeco : MonoBehaviour
    {
        [SerializeField]
        private AreaManager _areaManager = null;
        [Space]

        [SerializeField]
        private Deco _prefabDeco = null;
        [SerializeField]
        private DecoRoot _prefabDecoRoot = null;

        [SerializeField]
        private ToolCreateMapConfigController _configController = null;

        [SerializeField]
        private string _textureAtlasPath = "Assets/_KAP/_GameResources/Atlas/Decos/";


        public Deco CreateDeco(int decoId, int colorId)
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
                FLSprite = atlas != null?atlas.GetSprite(config.Id + colorPath):null,
                BRSprite = atlas!= null?atlas.GetSprite(config.Id + "_b" + colorPath): null,
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

            var deco = SGUtils.InstantiateObject<Deco>(_prefabDeco, null);
            deco.gameObject.SetActive(true);
            deco.SetupParameters(parameters);
            deco.name = config.Id.ToString();
            return deco;
        }

        public DecoRoot CreateARoom(int index, Vector3 position, Vector3 size)
        {
            var root = SGUtils.InstantiateObject<DecoRoot>(_prefabDecoRoot, null);
            root.gameObject.SetActive(true);
            var decoParams = new DecoParameters
            {
                IsWallHang = false,
                SortingLayerName = "Default",
                FLSize = size,
                ListAreas = new List<AreaParameters> {
                    new AreaParameters {
                        Face = DemoAreaFace.Tile.GetHashCode(),
                        SortingLayerName = DemoSortingLayerName.Tile.ToString(),
                        PieceSize = size,
                        FLLocalPos = Vector3.zero,
                    },
                    new AreaParameters
                    {
                        Face = DemoAreaFace.Carpet.GetHashCode(),
                        SortingLayerName = DemoSortingLayerName.Carpet.ToString(),
                        PieceSize = size,
                        FLLocalPos = new Vector3(0,0,1)
                    },
                    new AreaParameters
                    {
                        Face = DemoAreaFace.Indoor.GetHashCode(),
                        SortingLayerName = DemoSortingLayerName.Indoor.ToString(),
                        PieceSize = size,
                         FLLocalPos = new Vector3(0,0,1)
                    }
                }
            };
            root.Root = root;
            root.Info = new DecoInfo { Id = index };
            root.SetupParameters(decoParams);
            root.Position = position;
            root.SetFLIsoPos(position);
            root.WorldDirect = IsoDirect.FL;
            root.Apply(null, null);
            _areaManager.AddRoom(root);
            _areaManager.SortRoom();
            return root;
        }

        public void Import(Dictionary<string, DecoDataArray[]> data)
        {
            var lstLevels = new List<int>();
            foreach (var iter in data)
            {
                int level = 0;
                if (int.TryParse(iter.Key, out level))
                    lstLevels.Add(level);
            }

            SGUtils.BubbleSort<int>(lstLevels, (l1, l2) => { return l1 < l2; });
            foreach (var level in lstLevels)
            {
                var lstDecos = data[level.ToString()];
                foreach (var deco in lstDecos)
                    ImportADeco(deco);
            }

        }

        public void ImportADeco(DecoDataArray data)
        {
            if (data == null)
                return;
            DecoInfo info = null;
            if(!string.IsNullOrEmpty(data.Info))
                info = JsonReader.Deserialize<DecoInfo>(data.Info);
            if (data.Size != null) //it's root
            {
                CreateARoom(info != null?info.Id:_areaManager.ListRooms.Count, data.Position != null ? data.Position.ToVector3() : Vector3.zero, data.Size.ToVector3());
                return;
            }
            var id = info.Id;
            var color = info.Color;
            var deco = CreateDeco(id, color);
            if (deco != null)
            {
                deco.Info = info;
                deco.Position = data.Position != null ? data.Position.ToVector3() : Vector3.zero;
                deco.WorldDirect = data.WorldDirect;
                deco.Group = data.Group;
                var moveData = _areaManager.Move(deco);
                if (moveData != null && moveData.piece != null)
                {
                    deco.Apply(moveData.piece, moveData.overlapPieces);
                }
                else
                {
                    Debug.LogError(string.Format("Import deco fail Id: {0}, Direct: {1}, Pos: {2}, Group: {3}, ", id, data.WorldDirect, deco.Position, data.Group));
                    deco.Remove();
                }
            }
        }

        public void ImportFromDataTree(DecoDataTree data)
        {
            if (data == null)
                return;
            Queue<DecoDataTree> queue = new Queue<DecoDataTree>();
            queue.Enqueue(data);
            while (queue.Count > 0)
                ImportFromDataTreeRecrusive(queue.Dequeue(), queue);
        }

        void ImportFromDataTreeRecrusive(DecoDataTree data, Queue<DecoDataTree> queue)
        {
            if (data.Size != null) //it's root
            {
                CreateARoom(_areaManager.ListRooms.Count, data.Position.ToVector3(), data.Size.ToVector3());
            }
            else
            {
                var info = JsonReader.Deserialize<DecoInfo>(data.Info);
                var id = info.Id;
                var color = info.Color;
                var deco = CreateDeco(id, color);
                if (deco != null)
                {
                    deco.Info = info;
                    deco.Position = data.Position.ToVector3();
                    deco.WorldDirect = data.WorldDirect;

                    var moveData = _areaManager.Move(deco);
                    if (moveData != null && moveData.piece != null)
                    {
                        deco.Apply(moveData.piece, moveData.overlapPieces);
                    }
                    else
                    {
                        Debug.LogError(string.Format("Import deco fail Id: {0}, Direct: {1}, Pos: {2} ", id, data.WorldDirect, data.Position.ToVector3()));
                        deco.Remove();
                    }
                }
            }

            foreach (var child in data.LstChilds)
            {
                queue.Enqueue(child);
            }
        }

        public Sprite GetSprite(string spriteName, int themeId)
        {
            if (themeId == 0)
                return null;
            Sprite sprite = null;
#if UNITY_EDITOR
            var atlas = Kawaii.ResourceManager.Editor.ResourceManagerEditor.LoadAtlas(_textureAtlasPath + themeId + ".asset", themeId.ToString());
            if (atlas != null)
                sprite = atlas.GetSprite(spriteName);
#endif
            return sprite;
        }
    }

}
