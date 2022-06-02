using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SGShadero
{
    public class SGShaderoOutline : MonoBehaviour
    {
        public Material defaultMaterial;
        public Material effectMaterial;
        //public Color color;
        public Renderer sprRenderer;

        void OnEnable()
        {
            sprRenderer.material = effectMaterial;
        }

        void OnDisable()
        {
            sprRenderer.material = defaultMaterial;
        }

        public void SetColor(Color color)
        {
            //this.color = color;
            sprRenderer.material.SetColor("_Outline_Color_1", color);
        }
    }

}
