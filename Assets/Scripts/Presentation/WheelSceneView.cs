using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class WheelSceneView : MonoBehaviour
    {
        [Header("Audio")]
        public AudioManager AudioManager;

        [Header("Views")]
        public WheelView WheelView;
        public ZoneCountView ZoneCountView;
        public ZoneSelectorView ZoneSelectorView;
        public RewardHolderView RewardHolderView;
        public RewardFlightView RewardFlightView;
        public BombExplosionView BombExplosionView;

        public RectTransform ShakeRoot;

        [Header("Popups")]
        public CashoutPopup CashoutPopup;
        public RevivePopup RevivePopup;

        [Header("Settings")]
        public WheelSpinSettings SpinSettings;
        public RewardFlightSettings FlightSettings;
        public PenaltyEffectSettings PenaltySettings;

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
            float limit = SpinAliasing.MaxSafeDegreesPerSecond(sliceCount, DisplaySetup.TargetFrameRate);

            if (peak <= limit)
                return;

            Debug.LogWarning(
                $"[{nameof(WheelSceneView)}] Spin peaks at {peak:0} deg/s, above the {limit:0} deg/s " +
                $"that {sliceCount} slices can show at {DisplaySetup.TargetFrameRate} fps — the wheel " +
                "will look like it is turning backwards. Raise Duration or lower Max Turns. " +
                "Prevent Stroboscopic Aliasing stretches the spin at runtime to cover this.", this);
        }
#endif
    }
}
