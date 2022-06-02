using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.ConfigSystem;
using Kawaii.IsoTools;

namespace KAP.Config
{
    public class ConfigDecoRecord
    {
        public int Id;
        public string Name;
        public int ThemeId;
        public bool IsWallHang;
        public DemoSortingLayerName SortingLayerName = DemoSortingLayerName.Indoor;
        public int SizeX;
        public int SizeY;
        public int SizeZ;
        public bool Symmetric;
        public bool BackLikeFront;
        public int CanInAreaFaces;
        public DemoColliderLayer ColliderLayer = DemoColliderLayer.Indoor;
        public string FLColliderRects;
        private List<IsoRect> _lstFLLocalColliderRect = null;

        public List<IsoRect> GetListFLLocalColliderRect()
        {
            if (_lstFLLocalColliderRect == null)
            {
                _lstFLLocalColliderRect = new List<IsoRect>();
                var lstRectStr = SGUtils.ParseStringToList(FLColliderRects, ';');

                foreach (var rectStr in lstRectStr)
                {
                    var lstParam = SGUtils.ParseStringToListInt(rectStr, '_');
                    if (lstParam.Count == 4)
                    {
                        _lstFLLocalColliderRect.Add(new IsoRect(lstParam[0], lstParam[1], lstParam[2], lstParam[3]));
                    }
                }
            }

            return _lstFLLocalColliderRect;
        }
    }

    public class ConfigDeco : ConfigDataTable<ConfigDecoRecord>
    {
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("Id");
            RebuildIndexByField<int>("ThemeId", true);
        }

        public ConfigDecoRecord GetDecoById(int id)
        {
            return GetRecordByIndex<int>("Id", id);
        }

        public List<ConfigDecoRecord> GetListDecoByThemeId(int themeId)
        {
            return GetRecordsByIndex<int>("ThemeId", themeId);
        }
    }

}
