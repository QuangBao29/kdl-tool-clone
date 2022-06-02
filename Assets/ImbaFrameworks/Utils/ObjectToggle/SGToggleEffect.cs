using UnityEngine;
using System.Collections;

namespace Imba.Utils
{
    public class SGToggleEffect : MonoBehaviour
    {

        public virtual void Visable()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }
    }

}
