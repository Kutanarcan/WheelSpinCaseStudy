using DG.Tweening;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName = "CaseStudy/Settings/Penalty Effect", fileName = "PenaltyEffectSettings")]
    public class PenaltyEffectSettings : ScriptableObject
    {
        [Header("Wheel Slice")]
        public Sprite Sprite;

        public ItemViewSettings SliceView = ItemViewSettings.Default;

        [Header("Bomb Grow")]
        [Min(1f)] public float GrowScale = 1.6f;
        [Min(0f)] public float GrowDuration = 0.45f;
        public Ease GrowEase = Ease.InBack;

        [Header("Bomb Shake")]
        public float ShakeRotation = 14f;
        [Min(0)] public int ShakeVibrato = 10;
        [Range(0f, 1f)] public float ShakeElasticity = 0.6f;

        [Header("Explosion")]
        [Range(0f, 1f)] public float DetonateAt = 0.9f;
        public bool HideBombOnExplode = true;

        [Header("Screen Shake")]
        [Min(0f)] public float ScreenShakeStrength = 34f;
        [Min(0f)] public float ScreenShakeDuration = 0.45f;
        [Min(0)] public int ScreenShakeVibrato = 18;
        [Range(0f, 180f)] public float ScreenShakeRandomness = 90f;

        [Header("Handover")]
        [Min(0f)] public float PopupDelay = 0.15f;
        [Min(0f)] public float ExplosionDuration = 2f;
    }
}
