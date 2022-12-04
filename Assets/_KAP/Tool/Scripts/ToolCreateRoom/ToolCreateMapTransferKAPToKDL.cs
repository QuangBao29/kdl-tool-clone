using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kawaii.IsoTools.DecoSystem;
using KAP.Tools;
using System.Linq;
using System.Collections.Specialized;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapTransferKAPToKDL : MonoBehaviour
    {
        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField]
        private ToolCreateMapConfigController _configController = null;
        [SerializeField]
        private ToolCreateMapListRooms _toolListRooms = null;
        [SerializeField]
        private InputField _inputMapId = null;

        [HideInInspector]
        public List<int> LstDecoIDBubble = new List<int>();
        //Data for ConfigPlay
        [HideInInspector]
        public List<Vector3> LstPosBubble = new List<Vector3>();
        [HideInInspector]
        public List<string> LstUnpackingDeco = new List<string>();
        [HideInInspector]
        public Dictionary<string, string> DctBubbleDecoIds = new Dictionary<string, string>();
        //Data for ConfigHome
        [HideInInspector]
        public Dictionary<int, List<Vector3>> DctBubblePos = new Dictionary<int, List<Vector3>>();    //data for configbubblehomeposition
        [HideInInspector]
        public Dictionary<string, List<string>> DctBubble = new Dictionary<string, List<string>>();               //data for configbubblehome
        [HideInInspector]
        public Dictionary<int, int> DctNumOfBubbleInRoom = new Dictionary<int, int>();

        private List<int> _lstID = new List<int>();
        public void OnClickCreateRoomPlay()
        {
            _lstID.Clear();
            //GetListDecoIDForBubble();
            foreach (var rec in _configController.ListConfigRoomRecords)
            {
                if (rec.Id != 101000 && rec.Id != 101001)
                    _lstID.Add(rec.Id);
            }
            Debug.LogError("count of lstID: " + _lstID.Count);
            for (var i = 0; i < _lstID.Count; i++)
            {
                ClearAllList();
                _inputMapId.text = _lstID[i].ToString();
                _toolListRooms.OnButtonImportNewClick();
                ConvertKAPToKDLPlay();
                _configController.SaveConfigPlayKAPToKDL();
            }
        }
        public void OnClickCreateRoomHome()
        {
            ClearAllList();
            //GetListDecoIDForBubble();
            ConvertKAPToKDLHome();
            _configController.SaveConfigHomeKAPToKDL();
        }
        //private void GetListDecoIDForBubble()
        //{
        //    if (LstDecoIDBubble.Count > 0) return;
        //    var LstConfigDecoRecords = _configController.ListConfigDecoRecords;
        //    foreach (var rec in LstConfigDecoRecords)
        //    {
        //        float x = rec.SizeX;
        //        float y = rec.SizeY;
        //        float z = rec.SizeZ;
        //        if (CheckIfBothNumGreaterThan4And5(x, y) && z >= 2 || CheckIfBothNumGreaterThan4And5(x, z) && y >= 2 || CheckIfBothNumGreaterThan4And5(y, z) && x >= 2)
        //        {
        //            LstDecoIDBubble.Add(rec.Id);
        //        }
        //    }
        //    Debug.LogError("Count of List: " + LstDecoIDBubble.Count);
        //}

        private void ConvertKAPToKDLHome()
        {
            foreach (var root in _areaManager.ListRooms)
            {
                var infoRoot = (DecoInfo)root.Info;
                if (!DctNumOfBubbleInRoom.ContainsKey(infoRoot.Id))
                    DctNumOfBubbleInRoom.Add(infoRoot.Id, 0);
                List<Deco> lstDeco = new List<Deco>();
                root.Foreach((deco) => {
                    var info = (DecoInfo)deco.Info;
                    if (info.Id != infoRoot.Id)
                    {
                        float v = Volume(deco.FLIsoSize);
                        if (!CheckIfDecoIdInListOrNot(lstDeco, deco) && v >= 40)
                        {
                            if (lstDeco.Count >= 5)
                            {
                                var t = Sort(lstDeco, deco);
                                lstDeco.Clear();
                                lstDeco.AddRange(t);
                            }
                            else
                            {
                                lstDeco.Add(deco);
                            }
                        }
                        
                    }
                });
                foreach (var deco in lstDeco)
                {
                    var Info = (DecoInfo)deco.Info;
                }

                DctNumOfBubbleInRoom[infoRoot.Id] += lstDeco.Count;
                for (var i = 0; i < lstDeco.Count; i++)
                {
                    var deco = lstDeco[i];
                    var info = (DecoInfo)deco.Info;
                    if (DctBubblePos.ContainsKey(infoRoot.Id))
                    {
                        DctBubblePos[infoRoot.Id].Add(deco.Position - root.Position);
                    }
                    else
                    {
                        DctBubblePos.Add(infoRoot.Id, new List<Vector3> { deco.Position - root.Position });
                    }

                    var listDecoColor = _configController.ConfigDecoColor.GetListDecoColorsByDecoId(info.Id);
                    string bubbleDecoIds = "";
                    string BubbleId = "";
                    string price = "";
                    for (var j = 0; j < listDecoColor.Count; j++)
                    {
                        bubbleDecoIds += listDecoColor[j].Id + ";";
                        if (listDecoColor[j].ColorId == 0)
                            price += "10;";
                        else price += "20;";
                    }
                    BubbleId = infoRoot.Id + "_" + i;
                    DctBubble.Add(BubbleId, new List<string> { bubbleDecoIds, price });
                }
            }
        }

        private void ConvertKAPToKDLPlay()
        {
            foreach (var root in _areaManager.ListRooms)
            {
                var infoRoot = (DecoInfo)root.Info;
                root.Foreach((deco) => {
                    var info = (DecoInfo)deco.Info;
                    if (info.Id != infoRoot.Id)
                    {
                        if (LstDecoIDBubble.Contains(info.Id))
                        {
                            var listDecoColor = _configController.ConfigDecoColor.GetListDecoColorsByDecoId(info.Id);
                            if (listDecoColor == null)
                            {
                                Debug.LogError("cai list color null: " + info.Id);
                            }
                            else
                            {
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

                                DctBubbleDecoIds.Add(infoRoot.Id + "_" + LstPosBubble.Count, bubbleDecoIds);
                                LstPosBubble.Add(deco.Position);
                            }
                        }
                        else if (info.Id / 100000 < 22)
                        {
                            LstUnpackingDeco.Add(info.Id + "_" + info.Color);
                        }
                    }
                });
            }
        }

        private bool CheckIfBothNumGreaterThan4And5(float a, float b)
        {
            if (a >= 5 && b >= 4 || a >= 4 && b >= 5)
                return true;
            else return false;
        }

        public List<Deco> Sort(List<Deco> lst, Deco deco)
        {
            int i = 0;
            List<Deco> temp = new List<Deco>();
            for (i = 0; i < lst.Count; i++)
            {
                if (Volume(deco.FLIsoSize) > Volume(lst[i].FLIsoSize))
                {
                    lst.Insert(i, deco);
                    break;
                }
            }
            if (lst.Count > 5)
            {
                for (var j = 0; j < 5; j++)
                {
                    temp.Add(lst[j]);
                }
            }
            return temp;
        }

        public bool CheckIfDecoIdInListOrNot(List<Deco> lst, Deco deco)
        {
            bool check = false;
            var info = (DecoInfo)deco.Info;
            foreach (var d in lst)
            {
                var Info = (DecoInfo)d.Info;
                if (Info.Id == info.Id)
                {
                    check = true;
                    break;
                }
            }
            return check;
        }
        public float Volume(Vector3 vec)
        {
            return vec.x * vec.y * vec.z;
        }
        private void ClearAllList()
        {
            //_lstID.Clear();
            LstPosBubble.Clear();
            LstUnpackingDeco.Clear();
            DctBubbleDecoIds.Clear();

            DctBubblePos.Clear();
            DctBubble.Clear();
            DctNumOfBubbleInRoom.Clear();
        }
    }
}
