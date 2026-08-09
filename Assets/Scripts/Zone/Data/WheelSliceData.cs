using System;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [Serializable]
    public class WheelSliceData
    {
        [SerializeField] private ItemAsset _item;
        [SerializeField, Min(1)] private int _amount = 100;
        [SerializeField, Min(1)] private int _weight = 10;

        public ItemAsset Item => _item;
        public int Amount => _amount;
        public int Weight => _weight;

        public bool IsValid => _item != null;

        public WheelSlice ToRewardSlice() => WheelSlice.CreateReward(_item != null ? _item.ItemId : string.Empty, _amount, _weight);
    }
}