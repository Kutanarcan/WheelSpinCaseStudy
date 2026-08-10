using System;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [Serializable]
    public class ZonePalette
    {
        public Color Current = Color.black;
        public Color Bronze = Color.white;
        public Color Silver = new Color(0.35f, 0.85f, 0.45f);
        public Color Gold = new Color(1f, 0.82f, 0.25f);

        public Color For(WheelTier tier, bool isCurrent)
        {
            if (isCurrent) return Current;

            switch (tier)
            {
                case WheelTier.Gold: return Gold;
                case WheelTier.Silver: return Silver;
                default: return Bronze;
            }
        }
    }

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
        public ZonePalette ZonePalette = new ZonePalette();

        [Header("Animation")]
        public WheelSpinSettings SpinSettings = new WheelSpinSettings();
    }
}