using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Kawaii.Tutorial
{
    public class TutorialOnOffObjectSetting : TutorialStepSetting
    {
        [SerializeField]
        private List<GameObject> _lstObjs = null;
        [SerializeField]
        private int _delayMilisecond = 0;
        [SerializeField]
        private bool _isHide = false;

        public async override void Show()
        {
            foreach (var obj in _lstObjs)
                obj.SetActive(false);
            if (_delayMilisecond > 0)
                await Task.Delay(_delayMilisecond);
            foreach (var obj in _lstObjs)
                obj.SetActive(true);
        }

        public override void Hide()
        {
            if(_isHide)
            {
                foreach (var obj in _lstObjs)
                    obj.SetActive(false);
            }
        }
    }

}
