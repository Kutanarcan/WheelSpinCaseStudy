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
            if (_icon != null)
            {
                _icon.sprite = icon;
                _icon.preserveAspect = false;
                _icon.enabled = icon != null;
            }

            if (_iconRect != null)
            {
                _iconRect.sizeDelta = settings.Size;
                _iconRect.localEulerAngles = new Vector3(0f, 0f, settings.Rotation);
                _iconRect.anchoredPosition = settings.Offset;
            }

            if (_amountText != null)
            {
                _amountText.text = ViewFormat.FormatSliceAmount(amount);
            }
        }
    }
}