using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName = "CaseStudy/Wheel Config", fileName = "WheelConfig")]
    public class WheelConfigAsset : ScriptableObject
    {
        [Header("Tier Rules")]
        [SerializeField, Min(1)] private int _goldEvery = 30;
        [SerializeField, Min(1)] private int _silverEvery = 5;
        [SerializeField, Min(1)] private int _sliceCount = 8;

        [Header("Penalty")]
        [SerializeField, Min(1)] private int _penaltyWeight = 10;

        [Header("Content")]
        [SerializeField] private ZoneSetAsset _zoneSet;

        public int SliceCount => _sliceCount;
        public int PenaltyWeight => _penaltyWeight;
        public ZoneSetAsset ZoneSet => _zoneSet;

        public WheelTierRuleProvider CreateTierRuleProvider()
            => new WheelTierRuleProvider(_goldEvery, _silverEvery);
    }
}