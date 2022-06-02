using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KAP
{
    public class SpriteOutlineEffect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sprRender = null;
        [SerializeField] private Material _originMaterial = null;
        [SerializeField] private Material _outlineMaterial = null;

        public void SetupEffect(bool isShow)
        {
            if(isShow)
            {
                _sprRender.material = _outlineMaterial;
            }
            else
            {
                _sprRender.material = _originMaterial;
            }
        }
    }
}