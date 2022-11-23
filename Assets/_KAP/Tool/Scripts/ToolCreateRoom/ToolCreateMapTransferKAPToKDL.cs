using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kawaii.IsoTools.DecoSystem;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapTransferKAPToKDL : MonoBehaviour
    {
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField]
        private ToolCreateMapConfigController _configController = null;
        //[SerializeField]
        //private InputField _inputRoomId = null;

        private List<int> _lstDecoIDBubble = new List<int>();
        private List<Vector3> _lstPosBubble = new List<Vector3>();
        private List<string> _lstUnpackingDeco = new List<string>();
        private Dictionary<string, string> _dctBubbleDecoIds = new Dictionary<string, string>();

        public List<Vector3> GetListPosition()
        {
            return _lstPosBubble;
        }
        public List<string> GetListUnpackingDeco()
        {
            return _lstUnpackingDeco;
        }
        public Dictionary<string, string> GetDctBubbleDecoIds()
        {
            return _dctBubbleDecoIds;
        }

        public void OnClickCreateBubbleBtn()
        {
            //1. chay ham chon loc deco to de lam bubble
            //maybe chon loc theo size: deco ko phai tuong/san (1 chi so > 6, 1 chi so > 5, 1 chi so > 2)
            //2. tao structure chua data save vao config.
            //3. duyet qua tat ca deco con lai, lay id_color cua chung vao list unpacking.
            GetListDecoIDForBubble();
            //save vo config. goi configcontroller chay ham.
            _configController.SaveConfigPlayKAPToKDL();
            
        }

        public void GetListDecoIDForBubble()
        {
            //kiem tra thu deco to cua 1 room to truoc.
            //1460007 
            //1010001 1441002 
            //test room 101005 1752001 1454001 1710013
            _lstDecoIDBubble.Clear();
            _lstPosBubble.Clear();
            _lstUnpackingDeco.Clear();
            _dctBubbleDecoIds.Clear();

            _lstDecoIDBubble.Add(1710013);
            _lstDecoIDBubble.Add(1454001);
            _lstDecoIDBubble.Add(1752001);
            var root = _areaManager.ListRooms[0];
            var infoRoot = (DecoInfo)root.Info;
            root.Foreach((deco) => {
                var info = (DecoInfo)deco.Info;
                if (info.Id != infoRoot.Id)
                {
                    if (_lstDecoIDBubble.Contains(info.Id))
                    {
                        var listDecoColor = _configController.ConfigDecoColor.GetListDecoColorsByDecoId(info.Id);
                        string bubbleDecoIds = "";
                        if (listDecoColor.Count >= 3)
                        {
                            for (var i = 0; i < 3; i++)
                            {
                                bubbleDecoIds += listDecoColor[i].Id + ";";
                            }
                        }
                        else
                        {
                            for (var i = 0; i < listDecoColor.Count; i++)
                            {
                                bubbleDecoIds += listDecoColor[i].Id + ";";
                            }
                        }

                        _dctBubbleDecoIds.Add(infoRoot.Id + "_" + _lstPosBubble.Count, bubbleDecoIds);
                        _lstPosBubble.Add(deco.Position);
                        Debug.LogError("Id: " + info.Id);
                        Debug.LogError("Position Deco: " + deco.Position);
                    }
                    else if (info.Id/100000 < 22)
                    {
                        Debug.LogError("id deco unpacking: " + info.Id + "_" + info.Color);
                        _lstUnpackingDeco.Add(info.Id + "_" + info.Color);
                    }
                }
            });

            foreach (var pair in _dctBubbleDecoIds)
            {
                Debug.LogError(pair.Key +" "+ pair.Value);
            }
            foreach (var id in _lstUnpackingDeco)
            {
                Debug.LogError("kiem tra id unpacking: " + id);
            }    
        }
    }
}
