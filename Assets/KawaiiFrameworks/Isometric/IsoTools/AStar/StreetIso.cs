using UnityEngine;

namespace Kawaii.IsoTools
{
    [ExecuteInEditMode]
    public class StreetIso : MonoBehaviour
    {

        public IsoObject isoComp;
        public IsoObject realIso;

        void Awake()
        {
            isoComp = GetComponent<IsoObject>();
        }
    }
}
