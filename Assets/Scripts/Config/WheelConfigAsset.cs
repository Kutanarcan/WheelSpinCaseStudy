using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName = "CaseStudy/Wheel Config", fileName = "WheelConfig")]
    public class WheelConfigAsset : ScriptableObject
    {
        [SerializeField, Min(1)] private int _goldEvery = 30;
        [SerializeField, Min(1)] private int _silverEvery = 5;
        [SerializeField, Min(1)] private int _sliceCount = 8;

        public int SliceCount => _sliceCount;

        public WheelTierRuleProvider CreateTierRuleProvider()
            => new WheelTierRuleProvider(_goldEvery, _silverEvery);
    }
}