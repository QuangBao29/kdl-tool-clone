using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kawaii.IsoTools.DecoSystem
{
    public class DecoDataTree
    {
        public string Info;
        public int RoomId;
        public DecoVector3 Position;
        public int WorldDirect;
        public DecoVector3 Size;
        public int Group;
        public List<DecoDataTree> LstChilds = new List<DecoDataTree>();
    }

    public class DecoDataArray
    {
        public string Info;
        public DecoVector3 Position;
        public int WorldDirect;
        public DecoVector3 Size;
        public int Group;

        public Dictionary<string, object> ToJsonObject
        {
            get
            {
                var data = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(Info))
                    data["Info"] = Info;

                if(Position != null)
                {
                    var posJson = Position.ToJsonObject;
                    if (posJson != null)
                        data["Position"] = posJson;
                }
               
                if (WorldDirect > 0)
                    data["WorldDirect"] = WorldDirect;

                if(Size != null)
                {
                    var sizeJson = Size.ToJsonObject;
                    if (sizeJson != null)
                        data["Size"] = sizeJson;
                }
               
                if(Group >= 0)
                {
                    data["Group"] = Group;
                }

                return data;
            }
        }
    }

    public class DecoVector3
    {
        public float X;
        public float Y;
        public float Z;

        public DecoVector3()
        {

        }

        public DecoVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public DecoVector3(Vector3 v3)
        {
            X = v3.x;
            Y = v3.y;
            Z = v3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public Dictionary<string, object> ToJsonObject
        {
            get
            {
                var data = new Dictionary<string, object>();
                if (X != 0)
                    data["X"] = X;
                if (Y != 0)
                    data["Y"] = Y;
                if (Z != 0)
                    data["Z"] = Z;
                return data.Count > 0 ? data : null;
            }
        }
    }
}
