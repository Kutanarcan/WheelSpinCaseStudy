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

        public IReadOnlyList<WheelSliceData> Rewards => _rewards;
        public int PenaltySlotIndex => _penaltySlotIndex;

        /// <param name="penaltyDisabled">Revive sonrasi: penalty dilimi kalir ama agirligi 0 olur.</param>
        public Zone ToZone(int index, WheelTierRuleProvider tierRules, int penaltyWeight, bool penaltyDisabled = false)
        {
            bool hasPenalty = tierRules.HasPenalty(index);
            var slices = new WheelSlice[SliceCount];

            for (int i = 0; i < SliceCount; i++)
            {
                slices[i] = hasPenalty && i == _penaltySlotIndex
                    ? WheelSlice.CreatePenalty(penaltyDisabled ? 0 : penaltyWeight)
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