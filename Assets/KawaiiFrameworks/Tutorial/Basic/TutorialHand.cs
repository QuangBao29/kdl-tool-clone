using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

namespace Kawaii.Tutorial
{
    public class TutorialHand : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rTrans = null;
        [SerializeField]
        private List<GameObject> _lstObjCircles = null;
        [SerializeField]
        private Animator _animator = null;

        private Tweener _tweenerMove = null;

        public async void ShowTouch(RectTransform transContainer, float eulerAngleZ, Vector3 deltaPosition, Vector2 localScale, int delayMilisecond)
        {

            _rTrans.localEulerAngles = new Vector3(0, 0, eulerAngleZ);
            _rTrans.localScale = localScale;
            _animator.enabled = true;
            foreach (var circle in _lstObjCircles)
                circle.SetActive(true);
            var rootPosition = transContainer != null ? transContainer.position : Vector3.zero;
            _rTrans.position = rootPosition + deltaPosition;
           
            if (delayMilisecond > 0)
                await Task.Delay(delayMilisecond);
            gameObject.SetActive(true);
        }

        public async void ShowMove(Transform transContainer, float eulerAngleZ, Vector3 fromPos, Vector3 toPos, Vector2 localScale, float duration, int delayMilisecond, int loops, Ease ease)
        {
            _rTrans.localEulerAngles = new Vector3(0, 0, eulerAngleZ);
            _rTrans.localScale = localScale;
            _animator.enabled = false;
            foreach (var circle in _lstObjCircles)
                circle.SetActive(false);
            var rootPosition = transContainer != null ? transContainer.position : Vector3.zero;
            _rTrans.position = rootPosition + fromPos;
            if (delayMilisecond > 0)
                await Task.Delay(delayMilisecond);
            gameObject.SetActive(true);

            if (_tweenerMove != null)
                _tweenerMove.Kill();

            _tweenerMove = _rTrans.DOMove(rootPosition + toPos, duration).SetLoops(loops).SetEase(ease);
        }

        public void Hide()
        {
            if (_tweenerMove != null)
            {
                _tweenerMove.Kill();
                _tweenerMove = null;
            }
            gameObject.SetActive(false);
        }
    }
}
