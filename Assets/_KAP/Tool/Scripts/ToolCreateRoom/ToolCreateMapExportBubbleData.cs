using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;
using UnityEngine.Rendering;
using KAP;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapExportBubbleData : MonoBehaviour
    {
        [SerializeField]
        private AreaManager _areaManager = null;

        public delegate bool FindDecoMatch(Deco deco);
        public virtual Dictionary<int, List<Dictionary<string, object>>> Export(object param, bool IsUnpacking = false, FindDecoMatch match = null)
        {
            var data = new Dictionary<int, List<Dictionary<string, object>>>();
            foreach (var r in _areaManager.ListRooms)
            {
                var rData = ExportData(r, param, match, IsUnpacking);
                foreach (var iter in rData)
                {
                    List<Dictionary<string, object>> lst = null;
                    if (data.TryGetValue(iter.Key, out lst))
                        lst.AddRange(iter.Value);
                    else
                    {
                        data[iter.Key] = iter.Value;
                    }
                }
            }
            return data;
        }

        public Dictionary<int, List<Dictionary<string, object>>> ExportData(Deco deco, object param, FindDecoMatch match, bool IsUnpacking = false)
        {
            var dataResult = new Dictionary<int, List<Dictionary<string, object>>>();
            if (IsUnpacking)
                ExportDataUnpackingRecrusive(dataResult, deco, param, match);
            else
                ExportDataRecrusive(dataResult, deco, param, match);
            
            return dataResult;
        }

        void ExportDataRecrusive(Dictionary<int, List<Dictionary<string, object>>> dataResult, Deco deco, object param, FindDecoMatch match)
        {
            if (deco == null)
                return;
            var decoInfo = deco.ParseInfo<DecoInfo>();
            if (decoInfo.IsBubble || decoInfo.IsUnpacking)
                return;
            List<Dictionary<string, object>> lstByTreeLevel = null;
            var treeLevel = deco.TreeLevel;
            if (match == null || match(deco))
            {
                if (!dataResult.TryGetValue(treeLevel, out lstByTreeLevel))
                {
                    lstByTreeLevel = new List<Dictionary<string, object>>();
                    dataResult[treeLevel] = lstByTreeLevel;
                }
                lstByTreeLevel.Add(deco.ExportDataSelf(param));
            }

            foreach (var piece in deco.LstAreaPieces)
            {
                foreach (var child in piece.LstChilds)
                    ExportDataRecrusive(dataResult, child, param, match);
            }
        }

        #region Export Json Including Unpacking
        void ExportDataUnpackingRecrusive(Dictionary<int, List<Dictionary<string, object>>> dataResult, Deco deco, object param, FindDecoMatch match)
        {
            if (deco == null)
                return;
            var decoInfo = deco.ParseInfo<DecoInfo>();
            if (decoInfo.IsBubble)
                return;
            List<Dictionary<string, object>> lstByTreeLevel = null;
            var treeLevel = deco.TreeLevel;
            if (match == null || match(deco))
            {
                if (!dataResult.TryGetValue(treeLevel, out lstByTreeLevel))
                {
                    lstByTreeLevel = new List<Dictionary<string, object>>();
                    dataResult[treeLevel] = lstByTreeLevel;
                }
                lstByTreeLevel.Add(deco.ExportDataSelf(param));
            }

            foreach (var piece in deco.LstAreaPieces)
            {
                foreach (var child in piece.LstChilds)
                    ExportDataUnpackingRecrusive(dataResult, child, param, match);
            }
        }

        #endregion
    }
}

