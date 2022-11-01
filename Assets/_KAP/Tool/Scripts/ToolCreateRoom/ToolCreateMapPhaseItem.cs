using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace KAP.ToolCreateMap
{
    public class ToolCreateMapPhaseItem : MonoBehaviour
    {
        [SerializeField] private ToolCreateMapPhaseController _phaseController = null;
        [SerializeField] private Image _img = null;
        [SerializeField] private Text _name = null;
        [SerializeField] private Color _color;
        [SerializeField] private Color _defaultColor;
        public void OnItemPhaseClick()
        {

        }
    }
}

