using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public class WheelView : MonoBehaviour
    {
        public WheelSliceView[] SliceViewArray = new WheelSliceView[8];
        public SpinButtonView SpinButtonView;

        public Image SpinIndicatorImage;

        private void OnValidate()
        {
            SliceViewArray = GetComponentsInChildren<WheelSliceView>();
        }
    }
}
