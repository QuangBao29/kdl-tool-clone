using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.IsoTools.DecoSystem
{
    public class Calculator 
    {
        public static Vector3 GetChildWorldPositionFollowParent(int childWorldDirect, Vector3 childFlLocalPos, Vector3 childFLIsoSize, int parentWorldDirect, Vector3 parentWorldPos, Vector3 parentWorldSize)
        {
            var childSize = childFLIsoSize;
            switch(childWorldDirect)
            {
                case IsoDirect.FR:
                case IsoDirect.BL:
                    childSize = new Vector3(childFLIsoSize.y, childFLIsoSize.x, childFLIsoSize.z);
                    break;
            }

            var pos = parentWorldPos;
            switch (parentWorldDirect)
            {
                case IsoDirect.FL:
                    pos = parentWorldPos + childFlLocalPos;
                    break;
                case IsoDirect.FR:
                    pos = parentWorldPos + new Vector3(childFlLocalPos.y, childFlLocalPos.x, childFlLocalPos.z);
                    break;
                case IsoDirect.BR:
                    pos = parentWorldPos + new Vector3(parentWorldSize.x - childFlLocalPos.x - childSize.x, parentWorldSize.y - childFlLocalPos.y - childSize.y, childFlLocalPos.z);
                    break;
                default:
                    pos = parentWorldPos + new Vector3(parentWorldSize.x - childFlLocalPos.y - childSize.x, parentWorldSize.y - childFlLocalPos.x - childSize.y, childFlLocalPos.z);
                    break;
            }
            return pos;
        }

        public static Vector3 GetChildFLLocalPositionFollowParent(int childWorldDirect, Vector3 childWorldPos, Vector3 childFlSize, int parentWorldDirect, Vector3 parentWorldPos, Vector3 parentWorldSize)
        {
            var childSize = childFlSize;
            switch(childWorldDirect)
            {
                case IsoDirect.FR:
                case IsoDirect.BL:
                    childSize = new Vector3(childFlSize.y, childFlSize.x, childSize.z);
                    break;
            }

            var flLocalPos = Vector3.zero;
            var deltaPos = childWorldPos - parentWorldPos;
            switch (parentWorldDirect)
            {
                case IsoDirect.FL:
                    flLocalPos = deltaPos;
                    break;
                case IsoDirect.FR:
                    flLocalPos = new Vector3(deltaPos.y, deltaPos.x, deltaPos.z);
                    break;
                case IsoDirect.BR:
                    flLocalPos = new Vector3(parentWorldSize.x - deltaPos.x - childSize.x, parentWorldSize.y - deltaPos.y - childSize.y, deltaPos.z);
                    break;
                default:
                    flLocalPos = new Vector3(parentWorldSize.y - deltaPos.y - childSize.y, parentWorldSize.x - deltaPos.x - childSize.x, deltaPos.z);
                    break;
            }
            return flLocalPos;
        }

        public static Vector3 GetNewPosWhenRotate(Vector3 pos, Vector3 size, int direct, int newDirect)
        {
            var newPosition = pos;
            switch (direct)
            {
                case IsoDirect.FL:
                    if (newDirect == IsoDirect.BR)
                    {
                        newPosition = new Vector3(pos.x - size.x + 1, pos.y, pos.z);
                    }
                    else if (newDirect == IsoDirect.BL)
                    {
                        newPosition = new Vector3(pos.x, pos.y - size.x + 1, pos.z);
                    }
                    break;
                case IsoDirect.FR:
                    if (newDirect == IsoDirect.BR)
                    {
                        newPosition = new Vector3(pos.x - size.y + 1, pos.y, pos.z);
                    }
                    else if (newDirect == IsoDirect.BL)
                    {
                        newPosition = new Vector3(pos.x, pos.y - size.y + 1, pos.z);
                    }
                    break;
                case IsoDirect.BR:
                    if (newDirect <= IsoDirect.FR)
                    {
                        newPosition = new Vector3(pos.x + size.x - 1, pos.y, pos.z);
                    }
                    else if (newDirect == IsoDirect.BL)
                    {
                        newPosition = new Vector3(pos.x + size.x - 1, pos.y - size.x + 1, pos.z);
                    }
                    break;
                default:
                    if (newDirect <= IsoDirect.FR)
                    {
                        newPosition = new Vector3(pos.x, pos.y + size.y - 1, pos.z);
                    }
                    else if (newDirect == IsoDirect.BR)
                    {
                        newPosition = new Vector3(pos.x - size.y + 1, pos.y + size.y - 1, pos.z);
                    }
                    break;
            }
            return newPosition;
        }

        public static int Rotate(int direct, int delta)
        {
            var newDirect = direct + delta;
            if (newDirect > IsoUtils.MaxDirectValue)
                newDirect = newDirect - IsoUtils.MaxDirectValue - 1;
            else if (newDirect < 0)
                newDirect = newDirect + IsoUtils.MaxDirectValue + 1;
            return newDirect;
        }

        public static int GetChildDirectMultiply(int parentDirect, int childDirect)
        {
            int result = -1;
            if ((parentDirect + childDirect) % 2 == 0)
                result = 1;
            return result;
        }

        public static Vector3 GetFLIsoSizeFromWorldSize(int worldDirect, Vector3 worldSize)
        {
            switch(worldDirect)
            {
                case IsoDirect.FR:
                case IsoDirect.BL:
                    return new Vector3(worldSize.y, worldSize.x, worldSize.z);
            }
            return worldSize;
        }
    }
}

