using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.IsoTools
{
    public class IsoRect
    {
        public float PosX;
        public float PosY;
        public float SizeX;
        public float SizeY;

        public IsoRect()
        {

        }

        public IsoRect(float posX, float posY, float sizeX, float sizeY)
        {
            PosX = posX;
            PosY = posY;
            SizeX = sizeX;
            SizeY = sizeY;
        }
    }
}

