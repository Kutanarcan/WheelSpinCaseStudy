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

        [Tooltip("Alpha of zone numbers the player has already passed, out of 255.")]
        [Range(0, 255)] public int PastZoneAlpha = 40;

        [Header("Animation")]
        public WheelSpinSettings SpinSettings = new WheelSpinSettings();

        [Header("Reward Flight")]
        public RewardFlightView RewardFlightView;
        public RewardFlightSettings FlightSettings = new RewardFlightSettings();

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