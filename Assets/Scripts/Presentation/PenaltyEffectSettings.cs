using System;
using DG.Tweening;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [Serializable]
    public class PenaltyEffectSettings
    {
        [Header("Bomb Grow")]
        [Tooltip("Scale the bomb icon reaches before it detonates.")]
        [Min(1f)] public float GrowScale = 1.8f;
        [Min(0f)] public float GrowDuration = 0.45f;
        public Ease GrowEase = Ease.InBack;

        [Header("Explosion")]
        [Tooltip("How long the explosion is given to play out before the revive popup opens.")]
        [Min(0f)] public float ExplosionDuration = 1.2f;
        [Tooltip("Hide the bomb icon the moment the explosion starts.")]
        public bool HideBombOnExplode = true;
    }
}
