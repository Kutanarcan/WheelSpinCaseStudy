using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public class WheelSliceView : MonoBehaviour
    {
        public Image Icon;
        public RectTransform IconRect;
        public TextMeshProUGUI AmountText;

        private void OnValidate()
        {
            Icon = GetComponentInChildren<Image>();
            IconRect = Icon.GetComponent<RectTransform>();
            AmountText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
