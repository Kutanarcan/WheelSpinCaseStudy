using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public class RewardScrollFocus
    {
        private readonly ScrollRect _scroll;
        private readonly RectTransform _topAnchor;
        private readonly RectTransform _bottomAnchor;
        private readonly float _duration;
        private readonly Ease _ease;

        private readonly Vector3[] _cornerBuffer = new Vector3[4];
        private readonly DOGetter<float> _contentYGetter;
        private readonly DOSetter<float> _contentYSetter;

        private Tween _tween;

        public RewardScrollFocus(
            ScrollRect scroll,
            RectTransform topAnchor,
            RectTransform bottomAnchor,
            float duration,
            Ease ease)
        {
            _scroll = scroll;
            _topAnchor = topAnchor;
            _bottomAnchor = bottomAnchor;
            _duration = duration;
            _ease = ease;

            _contentYGetter = GetContentY;
            _contentYSetter = SetContentY;
        }

        private bool IsReady
            => _scroll != null && _scroll.content != null && _topAnchor != null && _bottomAnchor != null;

        private RectTransform Viewport
            => _scroll.viewport != null ? _scroll.viewport : _scroll.transform as RectTransform;

        public void Focus(RectTransform target)
        {
            if (!IsReady || target == null)
                return;

            RectTransform viewport = Viewport;
            float delta = OutOfBandDelta(viewport, target);

            if (Mathf.Approximately(delta, 0f))
                return;

            Kill();
            _scroll.StopMovement();

            float to = ClampContentY(viewport, GetContentY() - delta);

            if (_duration <= 0f)
            {
                SetContentY(to);
                return;
            }

            _tween = DOTween.To(_contentYGetter, _contentYSetter, to, _duration)
                .SetEase(_ease)
                .SetLink(_scroll.gameObject, LinkBehaviour.KillOnDestroy);
        }

        public void Reset()
        {
            Kill();

            if (IsReady)
                SetContentY(0f);
        }

        public void Kill()
        {
            _tween?.Kill();
            _tween = null;
        }

        private float OutOfBandDelta(RectTransform viewport, RectTransform target)
        {
            float bandTop = LocalY(viewport, _topAnchor.position);
            float bandBottom = LocalY(viewport, _bottomAnchor.position);

            if (bandBottom > bandTop)
                (bandTop, bandBottom) = (bandBottom, bandTop);

            target.GetWorldCorners(_cornerBuffer);

            float targetBottom = LocalY(viewport, _cornerBuffer[0]);
            float targetTop = LocalY(viewport, _cornerBuffer[1]);

            if (targetTop > bandTop)
                return targetTop - bandTop;

            if (targetBottom < bandBottom)
                return targetBottom - bandBottom;

            return 0f;
        }

        private static float LocalY(RectTransform viewport, Vector3 worldPosition)
            => viewport.InverseTransformPoint(worldPosition).y;

        private float ClampContentY(RectTransform viewport, float y)
            => Mathf.Clamp(y, 0f, Mathf.Max(0f, _scroll.content.rect.height - viewport.rect.height));

        private float GetContentY() => _scroll.content.anchoredPosition.y;

        private void SetContentY(float y)
        {
            Vector2 position = _scroll.content.anchoredPosition;
            position.y = y;
            _scroll.content.anchoredPosition = position;
        }
    }
}
