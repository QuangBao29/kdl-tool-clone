using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

namespace Kawaii.Tutorial
{
    [RequireComponent(typeof(TutorialStep))]
    public class TutorialFadeSetting : TutorialStepSetting
    {
        [SerializeField]
        private CanvasGroup _canvasGroup = null;
        [SerializeField]
        private float _from = 0;
        [SerializeField]
        private float _to = 0;
        [SerializeField]
        private float _delay = 0;
        [SerializeField]
        private float _duration = 0.5f;
        [SerializeField]
        private int _loops = 1;
        [SerializeField]
        private bool _instant = false;
        [SerializeField]
        private bool _isHide = false;
        [SerializeField]
        private bool _isHideInstant = false;

        private Tweener _tweener = null;

        public async override void Show()
        {
            if (_canvasGroup == null)
                return;
            if (_tweener != null)
                _tweener.Kill();
            _canvasGroup.alpha = _from;
            if (_instant)
            {
                if (_delay > 0)
                    await Task.Delay((int)(_delay * 1000));
                _canvasGroup.alpha = 1;
            }
            else
                _tweener = _canvasGroup.DOFade(_to, _duration).SetDelay(_delay).SetLoops(_loops);         
        }

        public override void Hide()
        {
            if (_canvasGroup == null)
                return;
            if (_tweener != null)
                _tweener.Kill();
            if (!_isHide)
                return;
            if (_isHideInstant)
                _canvasGroup.alpha = 0;
            else
                _tweener = _canvasGroup.DOFade(0, _duration);
        }

        private void OnDisable()
        {
            if(_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }
        }
    }

}
