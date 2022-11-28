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

        public List<int> LstDecoIDBubble = new List<int>();
        //Data for ConfigPlay
        public List<Vector3> LstPosBubble = new List<Vector3>();
        public List<string> LstUnpackingDeco = new List<string>();
        public Dictionary<string, string> DctBubbleDecoIds = new Dictionary<string, string>();
        //Data for ConfigHome
        public Dictionary<int, List<Vector3>> DctBubblePos = new Dictionary<int, List<Vector3>>();    //data for configbubblehomeposition
        public Dictionary<string, string> DctBubble = new Dictionary<string, string>();               //data for configbubblehome
        public Dictionary<int, int> DctNumOfBubbleInRoom = new Dictionary<int, int>();

        public void OnClickCreateBubbleBtn()
        {
            ClearAllList();

            //1. chay ham chon loc deco to de lam bubble
            //maybe chon loc theo size: deco ko phai tuong/san (1 chi so >= 6, 1 chi so >= 5, 1 chi so >= 2)
            GetListDecoIDForBubble();

            //2. tao structure chua data save vao config.
            //3. duyet qua tat ca deco con lai, lay id_color cua chung vao list unpacking.
            if (ToolEditMode.Instance.CurrentEditMode == EditMode.Home)
            {
                ConvertKAPToKDLHome();
                _configController.SaveConfigHomeKAPToKDL();
            }
            else
            {
                ConvertKAPToKDLPlay();
                _configController.SaveConfigPlayKAPToKDL();
            }
        }

        public void GetListDecoIDForBubble()
        {
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

        public void ConvertKAPToKDLHome()
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

        public void ConvertKAPToKDLPlay()
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
                            Debug.LogError("Id: " + info.Id);
                        }
                        else if (info.Id / 100000 < 22)
                        {
                            Debug.LogError("id deco unpacking: " + info.Id + "_" + info.Color);
                            LstUnpackingDeco.Add(info.Id + "_" + info.Color);
                        }
                    }
                });
            }

            foreach (var pair in DctBubbleDecoIds)
            {
                Debug.LogError(pair.Key + " " + pair.Value);
            }
            foreach (var id in LstUnpackingDeco)
            {
                Debug.LogError("kiem tra id unpacking: " + id);
            }
        }

        public bool CheckIfBothNumGreaterThan4And5(float a, float b)
        {
            if (a >= 5 && b >= 4 || a >= 4 && b >= 5)
                return true;
            else return false;
        }

        public void ClearAllList()
        {
            LstDecoIDBubble.Clear();
            LstPosBubble.Clear();
            LstUnpackingDeco.Clear();
            DctBubbleDecoIds.Clear();

            DctBubblePos.Clear();
            DctBubble.Clear();
            DctNumOfBubbleInRoom.Clear();
        }
    }
}
