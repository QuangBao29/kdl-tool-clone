using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kawaii.IsoTools
{
    public class AStarManager : MonoBehaviour
    {

        public Index IsoToIndex(Vector3 pos)
        {
            return new Index(startIsoPos.x - pos.x, startIsoPos.y - pos.y, pos.z);
        }

        public Vector3 IndexToIso(Index index)
        {
            return new Vector3(-index.x + startIsoPos.x, -index.y + startIsoPos.y, index.z);
        }

        public Vector3 IndexToIso(int x, int y, int z)
        {
            return new Vector3(-x + startIsoPos.x, -y + startIsoPos.y, z);
        }

        public delegate bool CheckHasCollider(object obs);
        public class Node
        {
            public int key;
            public Index index;
            public Vector3 isoPos;
            public bool isLock;
            public bool virtualPos;
            public Node emptyNearlest;
            public Node virtualNode;

            public List<Node> emptyNodes = new List<Node>();

            public Node from;
            public float g;
            public float f;
            public float h;
        }

        static AStarManager instance;
        public static AStarManager Instance
        {
            get { return instance; }
        }
        public Vector2 startIsoPos;
        public Vector2 size;

        Node[,] nodeGrid;
        public bool diagonal;
        public bool invert;
        public bool saveEmptyNode;

        List<Node> emptyNodes;
        public List<Node> EmptyNodes
        {
            get
            {
                return emptyNodes;
            }
            set
            {
                emptyNodes = value;
            }
        }
        bool isSetup;

        public Node RandomEmptyPos
        {
            get
            {
                if (emptyNodes != null && emptyNodes.Count > 0)
                {
                    var randSystem = new System.Random();
                    int ran = randSystem.Next(emptyNodes.Count);
                    return emptyNodes[ran];
                }
                return null;
            }
        }

        int GetArrayIndex(Index index)
        {
            return (int)(index.z * (size.x * size.y) + index.x * size.x + index.y);
        }


        int GetArrayIndex(int x, int y)
        {
            return (int)(x * size.x + y);
        }

        void Awake()
        {
            instance = this;
            nodeGrid = new Node[(int)size.x, (int)size.y];
            if (saveEmptyNode)
                emptyNodes = new List<Node>();

        }



        public void Init()
        {
            isSetup = false;
            for (int x = 0; x < size.x; ++x)
                for (int y = 0; y < size.y; ++y)
                {
                    Node n = new Node();
                    //n.obs = null;
                    n.key = GetArrayIndex(x, y);
                    n.index = new Index(x, y, 0);
                    n.isoPos = IndexToIso(x, y, 0);
                    n.isLock = invert;
                    nodeGrid[x, y] = n;
                }
        }

        public bool Add(StreetIso street)
        {
            if (street.realIso == null)
            {
                return Add(street.isoComp, false);
            }

            var isoComp = street.isoComp;
            var obsSize = Vector3.one;
            Index index = IsoToIndex(isoComp.Position);
            if (index.x - obsSize.x < -1 || index.x >= size.x
                || index.y - obsSize.y < -1 || index.y >= size.y
                )
            {
                Debug.Log(street.name + ":" + index.ToString()
                    + "\n" + index.y + "," + size.y
                    + "\n" + index.x + "-" + obsSize.x
                    + "\n" + index.x + "-" + size.x
                    + "\n" + index.y + "-" + obsSize.y

                );
                return false;
            }

            for (int x = index.x; x > index.x - obsSize.x; --x)
                for (int y = index.y; y > index.y - obsSize.y; --y)
                {
                    Node n = new Node();
                    //n.obs = isoComp;
                    n.key = GetArrayIndex(x, y);
                    n.index = new Index(x, y, 0);
                    n.isoPos = street.realIso.Position;
                    n.isLock = false;
                    n.virtualPos = true;

                    var virtualIndex = IsoToIndex(n.isoPos);
                    nodeGrid[virtualIndex.x, virtualIndex.y].virtualNode = n;
                    nodeGrid[x, y] = n;
                }

            return true;

        }

        public bool Add(Vector3 pos, Vector3 obsSize, bool isLock = true)
        {
            Index index = IsoToIndex(pos);
            if (index.x - obsSize.x < -1 || index.x >= size.x
                || index.y - obsSize.y < -1 || index.y >= size.y)
            {
                Debug.Log(pos + ":" + index.ToString()
                    + "\n" + index.x + " - " + obsSize.x
                    + "\n" + index.x + " - " + size.x
                    + "\n" + index.y + " - " + obsSize.y
                    + "\n" + index.y + " - " + size.y
                );
                return false;
            }

            for (int x = index.x; x > index.x - obsSize.x; --x)
                for (int y = index.y; y > index.y - obsSize.y; --y)
                {
                    Node n = new Node();

                    n.key = GetArrayIndex(x, y);
                    n.index = new Index(x, y, 0);
                    n.isoPos = IndexToIso(x, y, 0);

                    n.isLock = isLock;
                    nodeGrid[x, y] = n;
                }

            return true;
        }

        public bool Add(IsoObject obstacle, bool isLock = true)
        {
            var obsSize = obstacle.Size;
            //Debug.LogError (obstacle.name +"," + obsSize);

            Index index = IsoToIndex(obstacle.Position);
            //Debug.LogError (name);
            if (index.x - obsSize.x < -1 || index.x >= size.x
                || index.y - obsSize.y < -1 || index.y >= size.y)
            {
                Debug.Log(obstacle.name + ":" + index.ToString()
                + "\n" + index.x + " - " + obsSize.x
                + "\n" + index.x + " - " + size.x
                + "\n" + index.y + " - " + obsSize.y
                + "\n" + index.y + " - " + size.y
                );
                return false;
            }

            for (int x = index.x; x > index.x - obsSize.x; --x)
                for (int y = index.y; y > index.y - obsSize.y; --y)
                {
                    Node n = new Node();

                    n.key = GetArrayIndex(x, y);
                    n.index = new Index(x, y, 0);
                    n.isoPos = IndexToIso(x, y, 0);

                    n.isLock = isLock;
                    //if(isLock)
                    //  n.obs = obstacle;
                    nodeGrid[x, y] = n;
                }

            return true;

        }

        public bool Relation(Node cur, Node b)
        {
            if (b == null)
                return false;
            bool result = true;
            if (!cur.isLock)
            {
                if (!b.isLock)
                {
                    cur.emptyNodes.Add(b);
                    result = false;
                }
                else
                {
                    if (b.emptyNearlest == null)
                        b.emptyNearlest = cur;
                    else if (Distance(b, cur) < Distance(b, b.emptyNearlest))
                        b.emptyNearlest = cur;

                }
            }
            else if (cur.emptyNearlest == null)
            {
                if (!b.isLock)
                    cur.emptyNearlest = b;
                else
                    cur.emptyNearlest = b.emptyNearlest;
            }
            else
            {
                if (!b.isLock)
                {
                    if (Distance(cur, b) < Distance(cur, cur.emptyNearlest))
                        cur.emptyNearlest = b;
                }
                else if (b.emptyNearlest != null && Distance(cur, b.emptyNearlest) < Distance(cur, cur.emptyNearlest))
                    cur.emptyNearlest = b.emptyNearlest;
            }
            return result;

        }

        public void Setup()
        {
            foreach (var iter in nodeGrid)
            {
                if (iter == null) continue;
                Node cur = iter;
                cur.emptyNodes.Clear();
                //cur.obsNodes.Clear();
                bool xSub = true;
                bool xAdd = true;
                bool ySub = true;
                bool yAdd = true;

                int x = cur.index.x;
                int y = cur.index.y;

                if (y > 0)
                {
                    ySub = Relation(cur, nodeGrid[x, y - 1]);
                }

                if (y + 1 < size.y)
                {
                    yAdd = Relation(cur, nodeGrid[x, y + 1]);
                }

                if (x > 0)
                {
                    xSub = Relation(cur, nodeGrid[x - 1, y]);
                }

                if (x + 1 < size.x)
                {
                    xAdd = Relation(cur, nodeGrid[x + 1, y]);
                }

                if (x > 0 && y > 0 && (diagonal || (!xSub && !ySub)))
                    Relation(cur, nodeGrid[x - 1, y - 1]);

                if (x + 1 < size.x && y > 0 && (diagonal || (!xAdd && !ySub)))
                    Relation(cur, nodeGrid[x + 1, y - 1]);

                if (x > 0 && y + 1 < size.y && (diagonal || (!xSub && !yAdd)))
                    Relation(cur, nodeGrid[x - 1, y + 1]);

                if (x + 1 < size.x && y + 1 < size.y && (diagonal || (!xAdd && !yAdd)))
                    Relation(cur, nodeGrid[x + 1, y + 1]);
                if (saveEmptyNode && !cur.isLock && !cur.virtualPos)
                    emptyNodes.Add(cur);
            }
            isSetup = true;
        }

        Node GetValidNode(Node node, IsoObject deco)
        {
            if (node == null)
                return null;
            if (deco != null)
                return node;
            if (!node.isLock)
                return node;

            if (node.virtualNode != null)
                return node.virtualNode;
            return node.emptyNearlest;
        }

        public class PathStack
        {
            public Stack<Vector3> stack = new Stack<Vector3>();
            public Vector3 endPos;
        }

        public PathStack FindByIsoPos(Vector3 start, Vector3 end, IsoObject deco)
        {
            PathStack result = null;

            if (isSetup)
            {
                if (start == end)
                    return new PathStack();
                Index startIndex = IsoToIndex(start);
                Index endIndex = IsoToIndex(end);

                if (startIndex.x < 0)
                    startIndex.x = 0;
                if (startIndex.x >= size.x)
                    startIndex.x = (int)size.x - 1;
                if (startIndex.y < 0)
                    startIndex.y = 0;
                if (startIndex.y >= size.y)
                    startIndex.y = (int)size.y - 1;

                if (endIndex.x < 0)
                    endIndex.x = 0;
                if (endIndex.x >= size.x)
                    endIndex.x = (int)size.x - 1;
                if (endIndex.y < 0)
                    endIndex.y = 0;
                if (endIndex.y >= size.y)
                    endIndex.y = (int)size.y - 1;


                Node nodeStart = nodeGrid[startIndex.x, startIndex.y];
                Node nodeEnd = nodeGrid[endIndex.x, endIndex.y];
                result = Find(GetValidNode(nodeStart, null), GetValidNode(nodeEnd, deco), deco);
            }
            return result;
        }

        public Node GetRandomNode()
        {
            Node randomNode = nodeGrid[(int)Random.Range(0, size.x), (int)Random.Range(0, size.y)];
            return randomNode;
        }

        float Distance(Node start, Node end)
        {
            var startPos = start.isoPos;
            var endPos = end.isoPos;

            return (startPos.x - endPos.x) * (startPos.x - endPos.x) + (startPos.y - endPos.y) * (startPos.y - endPos.y);
        }

        bool EndFind(Node current, Node end, IsoObject target)
        {
            if (target == null)
                return current == end;
            var pos = current.isoPos;
            var targetPos = target.Position;
            var size = target.Size;
            var targetStart = new Vector2(targetPos.x - 1, targetPos.y - 1);
            var targetEnd = new Vector2(targetPos.x + size.x, targetPos.y + size.y);
            //Debug.LogError (size + "," + targetStart + "," + targetEnd + "," + pos);
            return pos.x >= targetStart.x && pos.x <= targetEnd.x && pos.y >= targetStart.y && pos.y <= targetEnd.y;
        }

        PathStack Find(Node start, Node end, IsoObject except)
        {
            if (start == null || end == null) return null;

            //if (end.virtualPos)
            //  return null;

            Dictionary<int, Node> Open = new Dictionary<int, Node>();

            Dictionary<int, Node> Close = new Dictionary<int, Node>();
            //Debug.LogError ("AAAA");
            start.g = 0;
            start.h = Distance(start, end);
            start.f = start.g + start.h;
            Open[start.key] = start;
            while (Open.Count != 0)
            {
                //Find node in open list have min f
                Node current = null;
                float minF = 0;
                foreach (KeyValuePair<int, Node> iter in Open)
                {
                    if (minF == 0 || iter.Value.f < minF)
                    {
                        current = iter.Value;
                        minF = current.f;
                    }
                }
                //Remove current from open list
                Open.Remove(current.key);
                //Add curent to close list
                Close[current.key] = current;
                if (EndFind(current, end, except))
                { //(current == end) {
                  //Return path
                    Open.Clear();
                    Close.Clear();

                    return ReconstructPath(start, current);
                }
                else
                {
                    //with next node in current.next

                    foreach (Node i_node in current.emptyNodes)
                    {
                        if (Close.ContainsKey(i_node.key))
                            continue;//if node in close will do nothing

                        float tmp_current_g = current.g + 1;
                        bool addedToOpen = Open.ContainsKey(i_node.key);
                        if (!addedToOpen || tmp_current_g < i_node.g)
                        {
                            i_node.from = current;
                            i_node.g = tmp_current_g;
                            i_node.h = Distance(i_node, end);
                            i_node.f = i_node.g + i_node.h;
                            if (!addedToOpen)
                            {
                                Open[i_node.key] = i_node;
                            }
                        }
                    }
                }
            }

            return null;
        }

        static PathStack ReconstructPath(Node s, Node t)
        {
            var result = new PathStack();
            bool addEndPos = false;
            Node tmp = t;
            int i = 0;
            while (tmp != null && tmp != s)
            {
                result.stack.Push(tmp.isoPos);
                if (!addEndPos)
                {
                    result.endPos = tmp.isoPos;
                    addEndPos = true;
                }
                Node back = tmp.from;
                tmp.from = null;
                tmp = back;
                i++;
            }
            //Debug.LogError (path.Count);
            return result;

        }

    }

    public class Index
    {
        public Index()
        {
        }

        public Index(float x, float y, float z)
        {
            this.x = Mathf.RoundToInt(x);
            this.y = Mathf.RoundToInt(y);
            this.z = z;
        }
        public Index(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Index(Vector3 isoPos)
        {
            this.x = Mathf.RoundToInt(isoPos.x);
            this.y = Mathf.RoundToInt(isoPos.y);
            this.z = isoPos.z;
        }

        public Index(Vector2 isoPos)
        {
            this.x = Mathf.RoundToInt(isoPos.x);
            this.y = Mathf.RoundToInt(isoPos.y);
            this.z = 0;
        }

        public int x;
        public int y;
        public float z;

        public override bool Equals(object obj)
        {
            var other = (Index)obj;
            return x == other.x && y == other.y && System.Math.Abs(z - other.z) < 0.01f; 
        }

        public override int GetHashCode()
        {
            return x;
        }

        public string debug()
        {
            return x + "," + y + "," + z;
        }
    }
}

