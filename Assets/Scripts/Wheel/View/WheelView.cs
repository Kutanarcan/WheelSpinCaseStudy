using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public class WheelView : MonoBehaviour
    {
        public WheelSliceView[] SliceViewArray = new WheelSliceView[8];
        public RectTransform HolderRect;
        public RectTransform WheelRect;
        public SpinButtonView SpinButtonView;
        public Image SpinIndicatorImage;
        public Image WheelImage;

        private void OnValidate()
        {
            if (WheelRect == null) WheelRect = transform as RectTransform;

            if (HolderRect == null && WheelRect != null)
                HolderRect = WheelRect.parent as RectTransform;

            if (SliceViewArray == null || SliceViewArray.Length == 0 || SliceViewArray.Any(s => s == null))
            {
                SliceViewArray = GetComponentsInChildren<WheelSliceView>(true)
                    .OrderBy(s => s.transform.GetSiblingIndex())
                    .ToArray();
            }
        }
    }
}