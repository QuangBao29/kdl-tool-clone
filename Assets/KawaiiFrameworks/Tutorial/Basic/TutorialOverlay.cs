using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Kawaii.Tutorial
{
    public class TutorialOverlay : MonoBehaviour
    {
        [SerializeField]
        private Image _imgOverlay = null;
        [SerializeField]
        private float _tweenPlayTime = 0.5f;

        private Tweener _tweener = null;

        public void Show(float alpha, float delay, bool raycast, bool instant)
        {
            _imgOverlay.raycastTarget = raycast;
            if(_tweener != null)
                _tweener = null;
            gameObject.SetActive(true);
            if (instant)
                _imgOverlay.color = new Color(0,0,0, alpha);
            else
                _tweener = _imgOverlay.DOFade(alpha, _tweenPlayTime).SetDelay(delay);
        }

        public void Hide(bool instant)
        {
            if (_tweener != null)
                _tweener.Kill();

            if (instant)
                gameObject.SetActive(false);
            else
                _tweener = _imgOverlay.DOFade(0, _tweenPlayTime).OnComplete(() => {
                    gameObject.SetActive(false);
                });
        }

        private void OnDisable()
        {
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }
        }
    }
}

