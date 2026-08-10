using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName = "CaseStudy/Wheel Config", fileName = "WheelConfig")]
    public class WheelConfigAsset : ScriptableObject
    {
        [Header("Tier Rules")]
        [SerializeField, Min(1)] private int _goldEvery = 30;
        [SerializeField, Min(1)] private int _silverEvery = 5;

        [Header("Penalty")]
        [SerializeField, Range(0f, PenaltyOdds.MaxChance)] private float _penaltyChance = 0.25f;

        [Header("Content")]
        [SerializeField] private ZoneSetAsset _zoneSet;
        [SerializeField] private ItemDatabaseAsset _itemDatabase;
        [SerializeField] private WheelTierViewDatabase _wheelTierViewDatabase;

        public float PenaltyChance => _penaltyChance;
        public ZoneSetAsset ZoneSet => _zoneSet;
        public ItemDatabaseAsset ItemDatabase => _itemDatabase;
        public WheelTierViewDatabase WheelTierViewDatabase => _wheelTierViewDatabase;

        public WheelTierRuleProvider CreateTierRuleProvider()
            => new WheelTierRuleProvider(_goldEvery, _silverEvery);
    }
}
