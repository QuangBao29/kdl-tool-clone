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
        public virtual Dictionary<int, List<Dictionary<string, object>>> Export(object param, DecoRoot r, FindDecoMatch match = null)
        {
            var data = new Dictionary<int, List<Dictionary<string, object>>>();
            var rData = ExportData(r, param, match);
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
            return data;
        }

        public Dictionary<int, List<Dictionary<string, object>>> ExportData(Deco deco, object param, FindDecoMatch match)
        {
            var dataResult = new Dictionary<int, List<Dictionary<string, object>>>();
            ExportDataRecrusive(dataResult, deco, param, match);
            return dataResult;
        }

        void ExportDataRecrusive(Dictionary<int, List<Dictionary<string, object>>> dataResult, Deco deco, object param, FindDecoMatch match)
        {
            if (deco == null)
                return;
            var decoInfo = deco.ParseInfo<DecoInfo>();
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

        public virtual Dictionary<int, List<Dictionary<string, object>>> ExportPoolDeco(Dictionary<int, List<Dictionary<string, object>>> rData)
        {
            var data = new Dictionary<int, List<Dictionary<string, object>>>();
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
            return data;
        }
        public void ExportDataPoolDeco(Dictionary<int, List<Dictionary<string, object>>> dataResult, Deco deco)
        {
            if (deco == null)
            {
                Debug.LogError("deco null");
                return;
            }
            List<Dictionary<string, object>> lstByTreeLevel = null;
            var treeLevel = 0;
            if (!dataResult.TryGetValue(treeLevel, out lstByTreeLevel))
            {
                lstByTreeLevel = new List<Dictionary<string, object>>();
                dataResult[treeLevel] = lstByTreeLevel;
            }
            lstByTreeLevel.Add(deco.ExportDataSelf(null));
        }
    }
}

