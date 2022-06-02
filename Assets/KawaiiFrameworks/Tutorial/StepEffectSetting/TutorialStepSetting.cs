using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaii.Tutorial
{
    public class TutorialStepSetting : MonoBehaviour
    {
        public virtual void Show()
        {

        }

        public virtual void Hide()
        {

        }

#if UNITY_EDITOR
        public virtual void SetupInspector()
        {

        }
#endif
    }

}
