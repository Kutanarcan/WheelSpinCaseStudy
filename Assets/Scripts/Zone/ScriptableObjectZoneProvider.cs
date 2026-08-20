using System.Collections.Generic;

namespace CaseStudy.WheelSpin
{
    public class ScriptableObjectZoneProvider : IZoneProvider
    {
        private readonly IReadOnlyList<ZoneAsset> _assets;
        private readonly WheelTierRuleProvider _tierRules;
        private readonly float _penaltyChance;

        public int ZoneCount => _assets != null ? _assets.Count : 0;

        public ScriptableObjectZoneProvider(
            IReadOnlyList<ZoneAsset> assets,
            WheelTierRuleProvider tierRules,
            float penaltyChance)
        {
            _assets = assets;
            _tierRules = tierRules;
            _penaltyChance = penaltyChance;
        }

        public Zone GetZone(int index, bool penaltyDisabled = false)
        {
            ZoneAsset asset = GetAsset(index);

            return asset != null
                ? asset.ToZone(index, _tierRules, _penaltyChance, penaltyDisabled)
                : null;
        }

        public bool TryValidate(out string error)
        {
            error = null;

            if (_tierRules == null)
            {
                error = "Tier rules are missing.";
                return false;
            }

            if (_assets == null || _assets.Count == 0)
            {
                error = "Zone set is empty.";
                return false;
            }

            for (int i = 0; i < _assets.Count; i++)
            {
                if (_assets[i] == null)
                {
                    error = $"Zone index {i + 1} is missing. Check for null entries in the zone set.";
                    return false;
                }
            }

            for (int i = 0; i < _assets.Count; i++)
            {
                int zoneIndex = i + 1;
                Zone zone = GetZone(zoneIndex);

                if (TotalWeight(zone) > 0)
                    continue;

                error = $"Zone index {zoneIndex} has no positive slice weight; the wheel could never resolve a slice.";
                return false;
            }

            return true;
        }

        private static int TotalWeight(Zone zone)
        {
            if (zone == null)
                return 0;

            int total = 0;

            for (int i = 0; i < zone.Wheel.Length; i++)
                total += zone.Wheel[i].Weight;

            return total;
        }

        private ZoneAsset GetAsset(int index)
        {
            if (_assets == null || index < 1 || index > _assets.Count)
                return null;

            return _assets[index - 1];
        }
    }
}
