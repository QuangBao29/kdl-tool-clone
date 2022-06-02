using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Fingers;
using Kawaii.IsoTools;

namespace KAP.ToolCreateMap
{
    public class ToolIsoFingerRaycaster : FingerEventRaycaster2D
    {
        private readonly List<Transform> _lstLastCatchs = new List<Transform>();
        public ReadOnlyCollection<Transform> ListLastCatchs;

        private void Awake()
        {
            ListLastCatchs = _lstLastCatchs.AsReadOnly();
        }

        protected override FingerEventObjectTrigger Raycast(float screenX, float screenY)
        {
            _lstLastCatchs.Clear();
            FingerEventObjectTrigger nearest = null;
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, 0));
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero, Mathf.Infinity, ~IgnoreLayerMask);
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    var trans = hit.transform;
                    if (trans == null)
                        continue;
                    _lstLastCatchs.Add(trans);
                    var fingerTrigger = trans.GetComponent<FingerEventObjectTrigger>();
                    if (fingerTrigger == null)
                        continue;
                    if (nearest == null)
                    {
                        nearest = fingerTrigger;
                        continue;
                    }

                    var curLayer = trans.gameObject.layer;
                    var nearestLayer = nearest.gameObject.layer;
                    if (curLayer != nearestLayer)
                    {
                        if (curLayer > nearestLayer)
                        {
                            nearest = fingerTrigger;
                        }
                        continue;
                    }

                    var isoComp = trans.GetComponent<IsoObject>();
                    if (isoComp == null)
                        continue;

                    var nearestIsoComp = nearest.GetComponent<IsoObject>();
                    if (nearestIsoComp == null)
                    {
                        nearest = fingerTrigger;
                        continue;
                    }

                    if (IsoUtils.IsFront(isoComp, nearestIsoComp))
                    {
                        nearest = fingerTrigger;
                    }
                }
            }
            return nearest;
        }
    }

}
