using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName = "CaseStudy/Zone", fileName = "Zone_")]
    public class ZoneAsset : ScriptableObject
    {
        public const int SliceCount = 8;

        [SerializeField] private WheelSliceData[] _rewards = new WheelSliceData[SliceCount];
        [SerializeField, Range(0, SliceCount - 1)] private int _penaltySlotIndex;

        [Tooltip("Kapaliyken WheelConfig'teki global penalty olasiligi kullanilir.")]
        [SerializeField] private bool _overridePenaltyChance;
        [SerializeField, Range(0f, PenaltyOdds.MaxChance)] private float _penaltyChanceOverride = 0.25f;

        public IReadOnlyList<WheelSliceData> Rewards => _rewards;
        public int PenaltySlotIndex => _penaltySlotIndex;
        public bool OverridesPenaltyChance => _overridePenaltyChance;

        /// <summary>Bu zone icin gecerli olasilik: override acikca ayarlandiysa o, yoksa global.</summary>
        public float ResolvePenaltyChance(float globalChance)
            => _overridePenaltyChance ? _penaltyChanceOverride : globalChance;

        /// <summary>Penalty slotu haric kalan dilimlerin agirlik toplami.</summary>
        public int OtherWeightSum()
        {
            int sum = 0;

            for (int i = 0; i < SliceCount; i++)
            {
                if (i == _penaltySlotIndex)
                    continue;

                if (_rewards[i] != null)
                    sum += _rewards[i].Weight;
            }

            return sum;
        }

        /// <param name="penaltyDisabled">Revive sonrasi: penalty dilimi kalir ama agirligi 0 olur.</param>
        public Zone ToZone(int index, WheelTierRuleProvider tierRules, float globalPenaltyChance, bool penaltyDisabled = false)
        {
            bool hasPenalty = tierRules.HasPenalty(index);

            int penaltyWeight = penaltyDisabled
                ? 0
                : PenaltyOdds.WeightFor(OtherWeightSum(), ResolvePenaltyChance(globalPenaltyChance));

            var slices = new WheelSlice[SliceCount];

            for (int i = 0; i < SliceCount; i++)
            {
                slices[i] = hasPenalty && i == _penaltySlotIndex
                    ? WheelSlice.CreatePenalty(penaltyWeight, penaltyDisabled)
                    : _rewards[i].ToRewardSlice();
            }

            return new Zone(index, tierRules.TierFor(index), new Wheel(slices));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_rewards == null || _rewards.Length != SliceCount)
                Array.Resize(ref _rewards, SliceCount);

            for (int i = 0; i < _rewards.Length; i++)
                if (_rewards[i] == null) _rewards[i] = new WheelSliceData();
        }
#endif
    }
}
