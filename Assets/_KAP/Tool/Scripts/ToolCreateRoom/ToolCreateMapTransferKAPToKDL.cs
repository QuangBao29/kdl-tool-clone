using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kawaii.IsoTools.DecoSystem;
using KAP.Tools;

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
        public Dictionary<string, string> DctBubble = new Dictionary<string, string>();               //data for configbubblehome
        [HideInInspector]
        public Dictionary<int, int> DctNumOfBubbleInRoom = new Dictionary<int, int>();

        private List<int> _lstID = new List<int>();
        public void OnClickCreateRoomPlay()
        {
            _lstID.Clear();
            GetListDecoIDForBubble();
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
            GetListDecoIDForBubble();
            ConvertKAPToKDLHome();
            _configController.SaveConfigHomeKAPToKDL();
        }
        private void GetListDecoIDForBubble()
        {
            if (LstDecoIDBubble.Count > 0) return;
            var LstConfigDecoRecords = _configController.ListConfigDecoRecords;
            foreach (var rec in LstConfigDecoRecords)
            {
                float x = rec.SizeX;
                float y = rec.SizeY;
                float z = rec.SizeZ;
                if (CheckIfBothNumGreaterThan4And5(x, y) && z >= 2 || CheckIfBothNumGreaterThan4And5(x, z) && y >= 2 || CheckIfBothNumGreaterThan4And5(y, z) && x >= 2)
                {
                    LstDecoIDBubble.Add(rec.Id);
                }
            }
            Debug.LogError("Count of List: " + LstDecoIDBubble.Count);
        }

        private void ConvertKAPToKDLHome()
        {
            foreach (var root in _areaManager.ListRooms)
            {
                var infoRoot = (DecoInfo)root.Info;
                Debug.LogError("position of root: " + infoRoot.Id + " " + root.Position);
                if (!DctNumOfBubbleInRoom.ContainsKey(infoRoot.Id))
                    DctNumOfBubbleInRoom.Add(infoRoot.Id, 0);
                root.Foreach((deco) => {
                    var info = (DecoInfo)deco.Info;
                    if (info.Id != infoRoot.Id)
                    {
                        if (LstDecoIDBubble.Contains(info.Id))
                        {
                            if (DctBubblePos.ContainsKey(infoRoot.Id))
                            {
                                DctBubblePos[infoRoot.Id].Add(deco.Position - root.Position);
                            }
                            else
                            {
                                DctBubblePos.Add(infoRoot.Id, new List<Vector3> { deco.Position - root.Position });
                            }
                            DctNumOfBubbleInRoom[infoRoot.Id]++;

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

                            string BubbleId = "";
                            BubbleId = infoRoot.Id + "_" + (DctNumOfBubbleInRoom[infoRoot.Id] - 1);
                            DctBubble.Add(BubbleId, bubbleDecoIds);
                        }
                    }
                });
            }

            //Debug.LogError("Count of DctBubble: " + DctBubble.Count);
            //Debug.LogError("Count of DctNumOfBubbleInRoom: " + DctNumOfBubbleInRoom.Count);
            //foreach (var bubble in DctBubblePos)
            //{
            //    Debug.LogError("DctBubblePos:  key: " + bubble.Key + " value: " + bubble.Value);
            //}
            //foreach (var bubble in DctBubble)
            //{
            //    Debug.LogError("DctBubble:     key: " + bubble.Key + " value: " + bubble.Value);
            //}
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
