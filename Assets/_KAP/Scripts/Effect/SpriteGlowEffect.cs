using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace KAP
{
    public class SpriteGlowEffect : MonoBehaviour
    {
        public bool IsRunning { get; private set; }

        [SerializeField] private SpriteRenderer _target = null;

        [SerializeField] private Material _originMaterial = null;
        [SerializeField] private Material _glowMaterial = null;
        [SerializeField] private string _glowPropName = "GlowPropertyName";
        [SerializeField] private float _targetGlowValue = 0.4f;
        [SerializeField] private float _originGlowValue = 1f;

        [Space]
        [SerializeField] private bool _isLoop = false;
        [SerializeField] private float _loopTime = 1f;

        private int _glowPropId;
        private float _curGlowValue;
        private string _effectId = "SpriteGlowEffect";

        private void Start()
        {
            _glowPropId = Shader.PropertyToID(_glowPropName);
        }

        public void SetupEffect(bool isShow)
        {
            if (IsRunning && _isLoop)
                UpdateGlowLoop();

            if (isShow && !IsRunning)
            {
                IsRunning = true;
                _target.material = _glowMaterial;

                if (_isLoop)
                    StartGLowTweenLoop();
                else
                    StartGlowEffect();
            }
            else if(!isShow && IsRunning)
            {
                IsRunning = false;
                if (_isLoop)
                    StopGlowTweenLoop();
                else
                    StopGlowEffect();
            }
        }

        private void StartGlowEffect()
        {
            _glowMaterial.SetFloat(_glowPropId, _targetGlowValue);
        }

        private void StopGlowEffect()
        {
            RemoveSpriteEffect();
        }

        private void StartGLowTweenLoop()
        {
            _curGlowValue = _originGlowValue;
            DOTween.To(() => _curGlowValue, x => _curGlowValue = x, _targetGlowValue, _loopTime)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetId(_effectId);
        }

        private void UpdateGlowLoop()
        {
            _glowMaterial.SetFloat(_glowPropId, _curGlowValue);
        }

        private void StopGlowTweenLoop()
        {
			DOTween.Kill(_effectId);
			RemoveSpriteEffect();
        }

        private void RemoveSpriteEffect()
        {
            _target.material = _originMaterial;
            _glowMaterial.SetFloat(_glowPropId, _originGlowValue);
        }
    }
}
