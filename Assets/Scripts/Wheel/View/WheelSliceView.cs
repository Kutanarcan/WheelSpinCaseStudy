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
            {
                _amountText.text = ViewFormat.FormatSliceAmount(amount);
                _amountText.enabled = true;
            }
        }

        public void BindPenalty(Sprite penaltyIcon, ItemViewSettings settings)
        {
            ApplyIcon(penaltyIcon, settings);

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
            }

            if (_iconRect == null) return;

            _iconRect.sizeDelta = settings.Size;
            _iconRect.localEulerAngles = new Vector3(0f, 0f, settings.Rotation);
            _iconRect.anchoredPosition = settings.Offset;
        }
    }
}