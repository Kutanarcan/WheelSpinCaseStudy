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
        public Ease Ease = Ease.OutQuad;

        [Header("Zone Strip")]
        [Min(0f)] public float SelectorStepDuration = 0.25f;
        public Ease SelectorStepEase = Ease.OutBack;
        [Min(0f)] public float ScrollDuration = 0.35f;
        public Ease ScrollEase = Ease.OutCubic;

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
