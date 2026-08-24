using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace CaseStudy.WheelSpin
{
    [Serializable]
    public class WheelSpinSettings
    {
        [Header("Spin")]
        [Min(0f)] public float Duration = 2.5f;
        [Min(0)] public int MinTurns = 2;
        [Min(0)] public int MaxTurns = 3;
        public Ease Ease = Ease.InOutQuad;

        [Header("Windup")]
        [Tooltip("Degrees the wheel is pulled back against the spin direction before it launches.")]
        [Min(0f)] public float Windup = 12f;
        [Min(0f)] public float WindupDuration = 0.25f;
        public Ease WindupEase = Ease.OutQuad;

        [Header("Settle")]
        [Min(0f)] public float Overshoot = 8f;
        [Min(0f)] public float SettleDuration = 0.45f;
        public Ease SettleEase = Ease.OutCubic;

        [Header("Zone Strip")]
        [Tooltip("Shared duration: the strip scroll and the selector run on this one clock.")]
        [Min(0f)] public float ScrollDuration = 0.35f;
        public Ease ScrollEase = Ease.OutCubic;
        [Tooltip("Ease of the selector along the same clock. An overshooting ease is allowed here.")]
        public Ease SelectorStepEase = Ease.OutBack;

        [Header("Zone Change")]
        [Min(0f)] public float DropDistance = 900f;
        [Min(0f)] public float DropDuration = 0.25f;
        public Ease DropEase = Ease.InBack;
        public float DropTilt = -12f;
        [Min(0f)] public float RiseDuration = 0.35f;
        public Ease RiseEase = Ease.OutBack;
        public bool ResetAngleOnZoneChange = true;

        [Header("Layout")]
        public float IndicatorAngle = 0f;

        public bool SpinClockwise = true;
        public bool SliceOrderClockwise = true;

        [Header("Safety")]
        public bool PreventStroboscopicAliasing = true;
    }
}
