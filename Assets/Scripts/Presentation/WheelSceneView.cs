using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class WheelSceneView : MonoBehaviour
    {
        [Header("Views")]
        public WheelView WheelView;
        public ZoneCountView ZoneCountView;
        public ZoneSelectorView ZoneSelectorView;
        public RewardHolderView RewardHolderView;

        [Header("Popups")]
        public CashoutPopup CashoutPopup;
        public RevivePopup RevivePopup;

        [Header("Penalty")]
        public Sprite PenaltySprite;
        public ItemViewSettings PenaltyViewSettings = ItemViewSettings.Default;

        [Header("Palette")]
        public Color CurrentZoneColor = Color.black;

        [Header("Animation")]
        public WheelSpinSettings SpinSettings = new WheelSpinSettings();

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (WheelView == null || WheelView.SliceViewArray == null || SpinSettings == null)
                return;

            int sliceCount = WheelView.SliceViewArray.Length;

            if (sliceCount <= 0)
                return;

            if (SpinAliasing.Overshoots(SpinSettings.Ease))
                return;

            float maxTotal = 359f + 360f * SpinSettings.MaxTurns;
            float peak = SpinAliasing.PeakDegreesPerSecond(maxTotal, SpinSettings.Duration, SpinSettings.Ease);
            float limit = SpinAliasing.MaxSafeDegreesPerSecond(sliceCount, 60);
        }
#endif
    }
}