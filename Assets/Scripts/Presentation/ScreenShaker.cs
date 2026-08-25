using DG.Tweening;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class ScreenShaker
    {
        private readonly RectTransform _root;

        private Tween _tween;
        private Vector2 _rest;
        private bool _restCaptured;

        public bool CanShake => _root != null;

        public ScreenShaker(RectTransform root) => _root = root;

        public void Play(float duration, float strength, int vibrato, float randomness)
        {
            if (!CanShake || duration <= 0f || strength <= 0f)
                return;

            CaptureRest();
            Stop();

            _tween = _root
                .DOShakeAnchorPos(duration, strength, vibrato, randomness, snapping: false, fadeOut: true)
                .SetLink(_root.gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(HandleComplete);
        }

        public void Stop()
        {
            _tween?.Kill();
            _tween = null;

            if (_restCaptured && _root != null)
                _root.anchoredPosition = _rest;
        }

        private void CaptureRest()
        {
            if (_restCaptured)
                return;

            _restCaptured = true;
            _rest = _root.anchoredPosition;
        }

        private void HandleComplete()
        {
            _tween = null;

            if (_root != null)
                _root.anchoredPosition = _rest;
        }
    }
}
