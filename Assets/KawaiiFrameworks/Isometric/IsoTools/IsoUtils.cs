using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kawaii.IsoTools
{
    public class IsoUtils
    {
        public static bool Collision(IsoObject obj1, IsoObject obj2)
        {
            if (obj1.Position.x < obj2.Position.x + obj2.Size.x &&
                 obj1.Position.y < obj2.Position.y + obj2.Size.y &&
                obj2.Position.x < obj1.Position.x + obj1.Size.x &&
                obj2.Position.y < obj1.Position.y + obj1.Size.y)
            {
                return true;
            }
            return false;
        }

        public static bool Collision(IsoRect obj1, IsoRect obj2)
        {
            if (obj1.PosX < obj2.PosX + obj2.SizeX &&
                 obj1.PosY < obj2.PosY + obj2.SizeY &&
                obj2.PosX < obj1.PosX + obj1.SizeX &&
                obj2.PosY < obj1.PosY + obj1.SizeY)
            {
                return true;
            }
            return false;
        }

        public static bool Collision(List<IsoRect> _lst1, List<IsoRect> _lst2)
        {
            foreach(var rect1 in _lst1)
            {
                foreach(var rect2 in _lst2)
                {
                    if (Collision(rect1, rect2))
                        return true;
                }
            }
            return false;
        }

        public static bool Collision(Vector2 pos1, Vector2 size1, Vector2 pos2, Vector2 size2)
        {
            //Debug.LogError(string.Format("{0},{1},{2},{3}",pos1, size1, pos2, size2));
            if (pos1.x < pos2.x + size2.x &&
                pos1.y < pos2.y + size2.y &&
                pos2.x < pos1.x + size1.x &&
                pos2.y < pos1.y + size1.y
                )
            {
                return true;
            }
            return false;
        }

        public static bool IsRect2DContainsOtherRect2D(Vector2 smallPos, Vector2 smallSize, Vector2 largePos, Vector2 largeSize)
        {
            if (smallPos.x >= largePos.x &&
               smallPos.y >= largePos.y &&
               smallPos.x + smallSize.x <= largePos.x + largeSize.x &&
               smallPos.y + smallSize.y <= largePos.y + largeSize.y)
                return true;
            return false;
        }

        public static bool IsRect2DContainsAPoint(Vector2 point, Vector2 rectPos, Vector2 rectSize)
        {
            if (point.x >= rectPos.x &&
               point.y >= rectPos.y &&
               point.x < rectPos.x + rectSize.x &&
               point.y < rectPos.y + rectSize.y)
                return true;
            return false;
        }

        public static bool IsRect3DContainsAPoint(Vector3 point, Vector3 rectPos, Vector3 rectSize)
        {
            if (point.x >= rectPos.x &&
              point.y >= rectPos.y &&
              point.x < rectPos.x + rectSize.x &&
              point.y < rectPos.y + rectSize.y &&
              point.z >= rectPos.z &&
              point.z <= rectPos.z + rectSize.z
              )
                return true;
            return false;
        }

        public static Vector3 RemoveZOfPosition(Vector3 pos)
        {
            return new Vector3(pos.x + pos.z, pos.y + pos.z, 0);
        }

        public static Vector3 AddZToIsoPosition(Vector3 pos, float z)
        {
            return new Vector3(pos.x - z, pos.y - z, z);
        }

        public static bool IsFront(IsoObject obj1, IsoObject obj2)
        {
            var pos1 = obj1.Position;
            var pos2 = obj2.Position;
            var maxX2 = pos2.x + obj2.Size.x;
            var maxY2 = pos2.y + obj2.Size.y;
            if (pos1.x < maxX2 && pos1.y < maxY2)
            {
                var maxX1 = pos1.x + obj1.Size.x;
                var maxY1 = pos1.y + obj1.Size.y;
                if (pos2.x < maxX1 && pos2.y < maxY1) //overlap
                {
                    if (pos1.z > pos2.z)
                        return true;
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool IsFront(Vector3 pos1, Vector3 size1, Vector3 pos2, Vector3 size2)
        {
            //Debug.LogError(string.Format("Pos1: {0} - Size1: {1} - Pos2: {2} - Size2: {3}", pos1, size1, pos2, size2));
            var maxX2 = pos2.x + size2.x;
            var maxY2 = pos2.y + size2.y;
            if (pos1.x < maxX2 && pos1.y < maxY2)
            {
                var maxX1 = pos1.x + size1.x;
                var maxY1 = pos1.y + size1.y;
                if (pos2.x < maxX1 && pos2.y < maxY1) //overlap
                {
                    if (pos1.z > pos2.z)
                        return true;
                    return false;
                }
                return true;
            }
            return false;
        }

        public delegate bool BubbleSortCompare<T>(T a, T b);
        public static void BubbleSort<T>(List<T> list, BubbleSortCompare<T> isHigher)
        {
            if (list == null)
                return;
            for (int i = list.Count - 1; i > 0; i--)
            {
                for (int j = 0; j <= i - 1; j++)
                {
                    if (isHigher(list[i], list[j]))
                    {
                        var highValue = list[i];

                        list[i] = list[j];
                        list[j] = highValue;
                    }
                }
            }
        }

        public static void BubbleSort<T>(T[] list, BubbleSortCompare<T> compare)
        {
            if (list == null)
                return;
            var length = list.Length;
            for (int i = length - 1; i > 0; i--)
            {
                for (int j = 0; j <= i - 1; j++)
                {
                    if (compare(list[i], list[j]))
                    {
                        var highValue = list[i];

                        list[i] = list[j];
                        list[j] = highValue;
                    }
                }
            }
        }

        public const int MaxDirectValue = 3;
    }

    public class IsoDirect
    {
        public const int FL = 0;
        public const int FR = 1;
        public const int BR = 2;
        public const int BL = 3;
    }

}
