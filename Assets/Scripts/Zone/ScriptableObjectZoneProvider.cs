using System;
using System.Collections.Generic;

namespace CaseStudy.WheelSpin
{
    public class ScriptableObjectZoneProvider : IZoneProvider
    {
        private readonly Dictionary<int, Zone> _zones = new Dictionary<int, Zone>();

        public int ZoneCount => _zones.Count;

        public ScriptableObjectZoneProvider(
            IReadOnlyList<ZoneAsset> assets,
            WheelTierRuleProvider tierRules,
            int penaltyWeight)
        {
            if (assets == null)
                return;

            if (tierRules == null)
                return;

            for (int i = 0; i < assets.Count; i++)
            {
                ZoneAsset asset = assets[i];

                if (asset == null)
                    continue;

                int zoneIndex = i + 1;

                _zones[zoneIndex] = asset.ToZone(zoneIndex, tierRules, penaltyWeight);
            }
        }

        public Zone GetZone(int index) => _zones.TryGetValue(index, out Zone zone) ? zone : null;

        public bool TryValidate(out string error)
        {
            error = null;

            if (_zones.Count == 0)
            {
                error = "Zone set is empty.";
                return false;
            }

            for (int i = 1; i <= _zones.Count; i++)
            {
                if (!_zones.ContainsKey(i))
                {
                    error = $"Zone index {i} is missing. Check for null entries in the zone set.";
                    return false;
                }
            }

            return true;
        }
    }
}