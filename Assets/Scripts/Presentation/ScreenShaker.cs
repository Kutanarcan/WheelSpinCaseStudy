using DG.Tweening;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Shakes a UI content root and always puts it back. Owns the rest position itself, because a
    /// killed shake leaves the transform wherever the tween had pushed it.
    /// </summary>
    /// <remarks>
    /// Aim this at a child of the Canvas, never the Canvas itself: a Screen Space - Overlay canvas
    /// has its RectTransform rewritten by Unity every frame, which would erase the shake.
    /// </remarks>
    public class ScreenShaker
    {
        private readonly RectTransform _root;

        private Tween _tween;
        private Vector2 _rest;
        private bool _restCaptured;

        public bool IsShaking => _tween != null;

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

            // Re-checked rather than relying on the captured flag: on shutdown the root may already
            // be destroyed, and Unity's null check is what catches that.
            if (_restCaptured && _root != null)
                _root.anchoredPosition = _rest;
        }

        /// Read once, before the first shake: re-reading later could capture a position the shake
        /// itself had displaced.
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
