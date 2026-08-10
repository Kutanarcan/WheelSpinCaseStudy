using TMPro;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class ZoneNumberView : MonoBehaviour
    {
        public TextMeshProUGUI NumberText; 
        public RectTransform Rect;

        public void SetNumber(int zoneNumber)
        {
            if (NumberText != null)
                NumberText.text = zoneNumber.ToString();
        }

        public void SetColor(Color color)
        {
            if (NumberText != null)
                NumberText.color = color;
        }

        private void OnValidate()
        {
            if (NumberText == null) 
                NumberText = GetComponentInChildren<TextMeshProUGUI>(true);

            if (Rect == null)
                Rect = transform as RectTransform;
        }
    }
}
