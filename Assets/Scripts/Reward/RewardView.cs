using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public class RewardView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private RectTransform _iconRect;
        [SerializeField] private TextMeshProUGUI _amountText;

        [Header("Stack Feedback")]
        [SerializeField, Min(0f)] private float _punchScale = 0.18f;
        [SerializeField, Min(0f)] private float _punchDuration = 0.22f;

        private RectTransform _rect;
        private Tween _punchTween;

        private RectTransform Rect => _rect != null ? _rect : _rect = transform as RectTransform;

        private void OnValidate()
        {
            if (_icon == null)
                _icon = GetComponentInChildren<Image>(true);

            if (_icon != null && _iconRect == null)
                _iconRect = _icon.rectTransform;

            if (_amountText == null)
                _amountText = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        private void OnDisable() => KillPunch();

        private void OnDestroy() => KillPunch();

        public void Bind(Sprite icon, int amount, ItemViewSettings settings)
        {
            KillPunch();

            if (Rect != null)
                Rect.localScale = Vector3.one;

            ApplyIcon(icon, settings);
            SetAmount(amount);
        }

        public void SetAmount(int amount)
        {
            if (_amountText != null)
                _amountText.text = ViewFormat.FormatSliceAmount(amount);
        }

        /// <summary>Miktar artinca oyuncunun fark etmesi icin kucuk bir punch.</summary>
        public void PlayStackFeedback()
        {
            if (Rect == null || _punchScale <= 0f || _punchDuration <= 0f)
                return;

            KillPunch();
            Rect.localScale = Vector3.one;

            _punchTween = Rect
                .DOPunchScale(Vector3.one * _punchScale, _punchDuration, vibrato: 6, elasticity: 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .OnKill(() => _punchTween = null);
        }

        private void KillPunch()
        {
            _punchTween?.Kill();
            _punchTween = null;
        }

        private void ApplyIcon(Sprite icon, ItemViewSettings settings)
        {
            if (_icon != null)
            {
                _icon.sprite = icon;
                _icon.preserveAspect = false;
                _icon.enabled = icon != null;
            }

            if (_iconRect == null)
                return;

            _iconRect.sizeDelta = settings.Size;
            _iconRect.localEulerAngles = new Vector3(0f, 0f, settings.Rotation);
            _iconRect.anchoredPosition = settings.Offset;
        }
    }
}