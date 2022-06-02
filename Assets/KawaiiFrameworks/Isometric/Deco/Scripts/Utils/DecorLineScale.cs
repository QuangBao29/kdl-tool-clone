using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kawaii.IsoTools.DecoSystem
{
    [ExecuteInEditMode]
    public class DecorLineScale : MonoBehaviour
    {
        public Transform trans;
        public SpriteRenderer spriteRenderer;
        public Vector2 scale;
        public Vector2 size;
        public Vector2 yardSize;
        public float padding;
        public bool invert;
        public bool includeYard;

        public void Setup(int x, int y, int yardX, int yardY)
        {
            this.size = new Vector2(x, y);
            this.yardSize = new Vector2(yardX, yardY);

            var sizeX = x;
            var sizeY = y;
            if (includeYard)
            {
                sizeX += yardX;
                sizeY += yardY;
            }
            if (invert)
            {
                var tmp = sizeX;
                sizeX = sizeY;
                sizeY = tmp;
            }

            if (trans != null)
            {
                var newScale = trans.localScale;
                if (scale.x != 0f)
                {
                    newScale.x = scale.x * sizeX - padding;
                }
                if (scale.y != 0f)
                {
                    newScale.y = scale.y * sizeY - padding;
                }

                trans.localScale = newScale;
            }
            else if (spriteRenderer != null)
            {
                var newScale = spriteRenderer.size;
                if (scale.x != 0f)
                {
                    newScale.x = scale.x * sizeX - padding;
                }
                if (scale.y != 0f)
                {
                    newScale.y = scale.y * sizeY - padding;
                }
                spriteRenderer.size = newScale;
            }
        }



#if UNITY_EDITOR
        Vector2 oldScale;
        Vector2 oldSize = Vector2.zero;
        Vector2 oldYard = Vector2.zero;
        float oldPadding = 0;
        private void Update()
        {
            if (oldSize != size)
            {
                oldSize = size;
                Setup((int)size.x, (int)size.y, (int)yardSize.x, (int)yardSize.y);
            }
            if (oldYard != yardSize)
            {
                oldYard = yardSize;
                Setup((int)size.x, (int)size.y, (int)yardSize.x, (int)yardSize.y);
            }
            if (oldPadding != padding)
            {
                oldPadding = padding;
                Setup((int)size.x, (int)size.y, (int)yardSize.x, (int)yardSize.y);
            }
            if (oldScale != scale)
            {
                oldScale = scale;
                Setup((int)size.x, (int)size.y, (int)yardSize.x, (int)yardSize.y);
            }
        }
#endif
    }
}