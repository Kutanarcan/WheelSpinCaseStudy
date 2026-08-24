using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private RectTransform _iconRect;
        [SerializeField] private TextMeshProUGUI _amountText;

        [SerializeField] private Color _disabledIconColor = new Color(1f, 1f, 1f, 0.35f);

        /// Where the reward art actually sits on the wheel — the win effect spawns from here.
        public RectTransform IconRect => _iconRect;

        /// Hides the art without touching the transform, so a scale tween still running on this
        /// slice cannot draw it again on the next frame.
        public void SetIconVisible(bool visible)
        {
            if (_icon != null)
                _icon.enabled = visible;
        }

        private void OnValidate()
        {
            if (_icon == null)
                _icon = GetComponentInChildren<Image>(true);

            if (_icon != null && _iconRect == null)
                _iconRect = _icon.rectTransform;

            if (_amountText == null)
                _amountText = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        public void Bind(Sprite icon, int amount, ItemViewSettings settings)
        {
            ApplyIcon(icon, settings);

            if (_amountText != null)
                _amountText.enabled = true;

            SetAmount(amount);
        }

        /// Rewrites just the number, so the win effect can count the slice down as it drains.
        public void SetAmount(int amount)
        {
            if (_amountText != null)
                _amountText.text = ViewFormat.FormatSliceAmount(amount);
        }

        public void BindPenalty(Sprite penaltyIcon, ItemViewSettings settings, bool disabled)
        {
            ApplyIcon(penaltyIcon, settings);

            if (_icon != null && disabled)
                _icon.color = _disabledIconColor;

            if (_amountText != null)
                _amountText.enabled = false;
        }

        private void ApplyIcon(Sprite icon, ItemViewSettings settings)
        {
            if (_icon != null)
            {
                _icon.sprite = icon;
                _icon.preserveAspect = false;
                _icon.enabled = icon != null;
                _icon.color = Color.white;
            }

            if (_iconRect == null) return;

            _iconRect.sizeDelta = settings.Size;
            _iconRect.localEulerAngles = new Vector3(0f, 0f, settings.Rotation);
            _iconRect.anchoredPosition = settings.Offset;
        }
    }
}