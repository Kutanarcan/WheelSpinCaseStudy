using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// A pooled win icon. Carries its own flight geometry: the spawn drift target and the quadratic
    /// bezier it follows to the board, driven by a single 0..1 progress value so the ease shapes the
    /// travel along the curve instead of along a straight line.
    /// </summary>
    public class RewardFlightIconView : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        private RectTransform _rect;
        private DOGetter<float> _progressGetter;
        private DOSetter<float> _progressSetter;

        private Vector2 _restAnchored;
        private Vector3 _restWorld;
        private Vector3 _flightControl;
        private Vector3 _flightTo;
        private float _progress;

        public RectTransform Rect => _rect != null ? _rect : _rect = transform as RectTransform;

        /// Where the icon settles after the spawn drift — the DOAnchorPos target of the scale-up.
        public Vector2 RestAnchored => _restAnchored;

        public DOGetter<float> ProgressGetter => _progressGetter ??= GetProgress;

        public DOSetter<float> ProgressSetter => _progressSetter ??= SetProgress;

        private void OnValidate()
        {
            if (_icon == null)
                _icon = GetComponent<Image>();
        }

        public void Bind(Sprite sprite, Vector2 size)
        {
            if (_icon != null)
            {
                _icon.sprite = sprite;
                _icon.enabled = sprite != null;
                _icon.raycastTarget = false;
                _icon.color = Color.white;
            }

            Rect.sizeDelta = size;
            Rect.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// Seats the icon at <paramref name="endOffset"/> to record its resting place, then pulls it
        /// back to <paramref name="startOffset"/> so the scale-up can glide it out. Recording the rest
        /// pose by actually visiting it avoids converting between canvas spaces by hand.
        /// </summary>
        public void Place(Vector3 worldOrigin, Vector2 startOffset, Vector2 endOffset)
        {
            Rect.position = worldOrigin;
            Rect.anchoredPosition += endOffset;

            _restAnchored = Rect.anchoredPosition;
            _restWorld = Rect.position;

            Rect.anchoredPosition = _restAnchored - endOffset + startOffset;
            Rect.localScale = Vector3.zero;
        }

        public void BeginFlight(Vector3 target, float arcHeightRatio)
        {
            _flightTo = target;
            _flightControl = ArcControl(_restWorld, target, arcHeightRatio);
            _progress = 0f;
        }

        /// Control point sits beside the midpoint, offset along the perpendicular of the straight
        /// line. The offset is a ratio of the flight distance, so the arc keeps its shape whatever
        /// the canvas scale is.
        private static Vector3 ArcControl(Vector3 from, Vector3 to, float heightRatio)
        {
            Vector3 direction = to - from;
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0f);

            if (perpendicular.sqrMagnitude > 0.0001f)
                perpendicular.Normalize();

            return (from + to) * 0.5f + perpendicular * (direction.magnitude * heightRatio);
        }

        private float GetProgress() => _progress;

        private void SetProgress(float progress)
        {
            _progress = progress;

            float inverse = 1f - progress;

            Rect.position = inverse * inverse * _restWorld
                            + 2f * inverse * progress * _flightControl
                            + progress * progress * _flightTo;
        }
    }
}
