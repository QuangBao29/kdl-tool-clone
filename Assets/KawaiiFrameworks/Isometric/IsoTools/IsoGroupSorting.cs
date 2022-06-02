using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.IsoTools
{
    public class IsoGroupSorting
    {
        protected List<IsoObject> _lstIsoObjs = new List<IsoObject>();
        public ReadOnlyCollection<IsoObject> ListIsoObjects;

        protected string _name;
        public IsoGroupSorting(string name)
        {
            _name = name;
            ListIsoObjects = _lstIsoObjs.AsReadOnly();
        }

        public void AddObject(IsoObject child)
        {
            _lstIsoObjs.Add(child);
        }

        public bool RemoveObject(IsoObject child)
        {
            return _lstIsoObjs.Remove(child);
        }

        public void ClearAll()
        {
            _lstIsoObjs.Clear();
        }

        public void Sort()
        {
            IsoUtils.BubbleSort<IsoObject>(_lstIsoObjs, (c1, c2) => {
                var pos1 = c1.Position;
                var pos2 = c2.Position;
                var size1 = c1.Size;
                var size2 = c2.Size;
                var maxX1 = pos1.x + size1.x;
                var maxY1 = pos1.y + size1.y;
                if (pos2.x < maxX1 && pos2.y < maxY1)
                {
                    var maxX2 = pos2.x + size2.x;
                    var maxY2 = pos2.y + size2.y;
                    if (pos1.x < maxX2 && pos1.y < maxY2) //overlap
                    {
                        if (pos2.z > pos1.z)
                            return true;
                        var center1 = new Vector2(pos1.x + size1.x / 2, pos1.y + size1.y / 2);
                        var center2 = new Vector2(pos2.x + size2.x / 2, pos2.y + size2.y / 2);
                        if (center2.x <= center1.x && center2.y <= center1.y)
                            return true;
                        if (size2.x * size2.y < size1.x * size1.y)
                            return true;
                        return false;
                    }
                    return true;
                }
                return false;
            });
            var i = 0;
            foreach (var iter in _lstIsoObjs)
            {
                iter.SortingLayerOrder = i;
                i = i + 1 + iter.SortStep;
            }
        }
    }

}
