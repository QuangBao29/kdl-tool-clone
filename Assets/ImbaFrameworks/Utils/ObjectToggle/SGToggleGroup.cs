using UnityEngine;
using System.Collections;
namespace Imba.Utils
{
    public class SGToggleGroup : MonoBehaviour
    {
        SGToggle current;

        public SGToggle Current
        {
            get { return current; }
        }

        public void ChangeTab(SGToggle obj)
        {
            if (current != null)
                current.Visible = false;
            current = obj;
            current.Visible = true;
        }

        public void HideTab(SGToggle obj)
        {
            if (obj == current)
                current = null;
            obj.Visible = false;
        }
    }

}
