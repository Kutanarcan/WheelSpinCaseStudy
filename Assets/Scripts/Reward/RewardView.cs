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
        [Range(0f, 1f)][SerializeField] private float _punchRiseRatio = 0.35f;
        [SerializeField] private Ease _punchRiseEase = Ease.OutQuad;
        [SerializeField] private Ease _punchFallEase = Ease.OutBack;

        private RectTransform _rect;
        private Tween _punchTween;
        private TweenCallback _onPunchKill;

        public RectTransform Rect => _rect != null ? _rect : _rect = transform as RectTransform;

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

            ApplyIcon(icon, settings);
            SetAmount(amount);
        }

        public void SetAmount(int amount)
        {
            if (_amountText != null)
                _amountText.SetText($"{amount}");
        }

        public void PlayStackFeedback()
        {
            if (Rect == null || _punchScale <= 0f || _punchDuration <= 0f)
                return;

            KillPunch();

            float riseDuration = _punchDuration * _punchRiseRatio;

            _punchTween = DOTween.Sequence()
                .Append(Rect.DOScale(1f + _punchScale, riseDuration).SetEase(_punchRiseEase))
                .Append(Rect.DOScale(1f, _punchDuration - riseDuration).SetEase(_punchFallEase))
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .OnKill(_onPunchKill ??= HandlePunchKill);
        }

        private void KillPunch()
        {
            Tween tween = _punchTween;
            _punchTween = null;

            tween?.Kill();

            if (Rect != null)
                Rect.localScale = Vector3.one;
        }

        private void HandlePunchKill() => _punchTween = null;

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
            _iconRect.localEulerAngles = new Vector3(0f, settings.Rotation, 0f);
        }
    }
}
