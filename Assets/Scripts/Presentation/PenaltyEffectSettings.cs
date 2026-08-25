using DG.Tweening;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    /// <summary>Everything about the penalty slice: how it looks on the wheel, and how it blows up.</summary>
    [CreateAssetMenu(menuName = "CaseStudy/Settings/Penalty Effect", fileName = "PenaltyEffectSettings")]
    public class PenaltyEffectSettings : ScriptableObject
    {
        [Header("Wheel Slice")]
        [Tooltip("Icon drawn on the penalty slice, whichever zone it appears in.")]
        public Sprite Sprite;

        [Tooltip("Size, rotation and offset of that icon inside the slice.")]
        public ItemViewSettings SliceView = ItemViewSettings.Default;

        [Header("Bomb Grow")]
        [Tooltip("Scale the bomb icon swells to. 1 = no growth.")]
        [Min(1f)] public float GrowScale = 1.6f;
        [Min(0f)] public float GrowDuration = 0.45f;
        public Ease GrowEase = Ease.InBack;

        [Header("Bomb Shake")]
        [Tooltip("Degrees the bomb rocks back and forth while it swells. 0 = no shake.")]
        public float ShakeRotation = 14f;
        [Min(0)] public int ShakeVibrato = 10;
        [Range(0f, 1f)] public float ShakeElasticity = 0.6f;

        [Header("Explosion")]
        [Tooltip("Point in the swell where the bomb blows, as a fraction of Grow Duration. " +
                 "1 = at the very end, lower values cut the swell short mid-growth.")]
        [Range(0f, 1f)] public float DetonateAt = 0.9f;
        [Tooltip("Hide the bomb icon the moment the explosion starts.")]
        public bool HideBombOnExplode = true;

        [Header("Screen Shake")]
        [Tooltip("Peak offset in canvas units. 0 = no shake.")]
        [Min(0f)] public float ScreenShakeStrength = 34f;
        [Min(0f)] public float ScreenShakeDuration = 0.45f;
        [Min(0)] public int ScreenShakeVibrato = 18;
        [Tooltip("0 keeps the shake on one axis, 90 spreads it in every direction.")]
        [Range(0f, 180f)] public float ScreenShakeRandomness = 90f;

        [Header("Handover")]
        [Tooltip("Delay between the blast and the revive popup. The explosion keeps playing behind it.")]
        [Min(0f)] public float PopupDelay = 0.15f;
        [Tooltip("How long the explosion stays on screen before it is cleared. Set this to the " +
                 "effect's own length — clearing it early cuts the particles off mid-flight.")]
        [Min(0f)] public float ExplosionDuration = 2f;
    }
}
