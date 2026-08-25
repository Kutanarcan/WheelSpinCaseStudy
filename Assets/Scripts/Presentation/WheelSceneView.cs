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

        [Header("Penalty Effect")]
        public BombExplosionView BombExplosionView;

        [Tooltip("Content root shaken by the blast. This canvas is Screen Space - Overlay, so Unity " +
                 "rewrites the Canvas RectTransform every frame — point this at a child that holds " +
                 "the UI, not at the Canvas itself.")]
        public RectTransform ShakeRoot;
        public PenaltyEffectSettings PenaltySettings = new PenaltyEffectSettings();

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