using System.Collections.Generic;

namespace CaseStudy.WheelSpin
{
    public class ScriptableObjectZoneProvider : IZoneProvider
    {
        private readonly IReadOnlyList<ZoneAsset> _assets;
        private readonly WheelTierRuleProvider _tierRules;
        private readonly int _penaltyWeight;

        private readonly Dictionary<int, Zone> _cache = new Dictionary<int, Zone>();

        public int ZoneCount => _assets != null ? _assets.Count : 0;

        public ScriptableObjectZoneProvider(
            IReadOnlyList<ZoneAsset> assets,
            WheelTierRuleProvider tierRules,
            int penaltyWeight)
        {
            _assets = assets;
            _tierRules = tierRules;
            _penaltyWeight = penaltyWeight;
        }

        public Zone GetZone(int index)
        {
            if (_cache.TryGetValue(index, out Zone cached))
                return cached;

            ZoneAsset asset = GetAsset(index);

            if (asset == null)
                return null;

            Zone zone = asset.ToZone(index, _tierRules, _penaltyWeight);

            _cache[index] = zone;

            return zone;
        }

        /// <summary>Cache'e yazmaz: revive tek bir run'a ozgudur, sonraki run'lara sizmamali.</summary>
        public Zone GetZoneWithPenaltyDisabled(int index)
        {
            ZoneAsset asset = GetAsset(index);

            return asset != null
                ? asset.ToZone(index, _tierRules, _penaltyWeight, penaltyDisabled: true)
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

            return true;
        }

        private ZoneAsset GetAsset(int index)
        {
            if (_assets == null || index < 1 || index > _assets.Count)
                return null;

            return _assets[index - 1];
        }
    }
}
