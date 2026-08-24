using System;
using DG.Tweening;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [Serializable]
    public class RewardFlightSettings
    {
        [Header("Icons")]
        [Min(1)] public int IconCount = 8;

        [Header("Spawn")]
        [Min(0f)] public float SpawnRadius = 70f;
        [Range(0f, 1f)] public float SpawnInnerRatio = 0.35f;
        [Range(0f, 0.5f)] public float SpawnAngleJitter = 0.35f;
        [Min(0f)] public float SpawnInterval = 0.1f;
        [Min(0f)] public float ScaleUpDuration = 0.25f;
        public Ease ScaleUpEase = Ease.OutBack;

        [Tooltip("How far back toward the origin an icon starts before gliding out. 0 = no glide.")]
        [Range(0f, 1f)] public float SpawnDrift = 0.35f;
        public Ease SpawnDriftEase = Ease.OutCubic;

        [Header("Flight")]
        [Min(0f)] public float HoldDuration = 0.1f;
        [Min(0f)] public float FlightInterval = 0.1f;
        [Min(0f)] public float FlightDuration = 0.45f;
        public Ease FlightEase = Ease.InBack;
        [Min(0f)] public float ArriveScale = 0.6f;

        [Tooltip("Arc bulge as a ratio of the flight distance. Flip the sign to bulge the other way.")]
        [Range(-1f, 1f)] public float ArcHeight = 0.25f;
        [Range(0f, 1f)] public float ArcJitter = 0.35f;
    }
}
