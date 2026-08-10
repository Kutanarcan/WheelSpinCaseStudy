using System;
using DG.Tweening;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [Serializable]
    public class WheelSpinSettings
    {
        [Min(0f)] public float Duration = 2.5f;
        [Min(0)] public int MinTurns = 3;
        [Min(0)] public int MaxTurns = 5;
        public Ease Ease = Ease.OutQuart;
        [Min(0f)] public float SelectorStepDuration = 0.25f;
        public Ease SelectorStepEase = Ease.OutBack;
        [Min(0f)] public float ScrollDuration = 0.35f;
        public Ease ScrollEase = Ease.OutCubic;

        public float IndicatorAngle = 0f;
        public bool SlicesClockwise = true;
    }
}