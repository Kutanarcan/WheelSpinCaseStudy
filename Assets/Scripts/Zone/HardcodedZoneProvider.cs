namespace CaseStudy.WheelSpin
{
    public class HardcodedZoneProvider : IZoneProvider
    {
        private readonly Zone[] _zones;

        public int ZoneCount => _zones.Length;

        public HardcodedZoneProvider(WheelTierRuleProvider tierRules)
        {
            _zones = new[]
            {
                Build(tierRules, 1,
                    R("coin",   100, 10),
                    R("coin",   150, 8),
                    R("wood",     5, 16),
                    P(40),
                    R("gem",      1,  6),
                    R("coin",   200, 12),
                    R("iron",     3, 4),
                    R("potion",   2, 10)),

                Build(tierRules, 2,
                    R("coin",   200, 18),
                    R("wood",     8, 16),
                    R("iron",     5, 14),
                    R("coin",   250, 16),
                    R("gem",      2,  6),
                    R("potion",   3, 12),
                    P(10),
                    R("coin",   300, 12)),

                Build(tierRules, 3,
                    R("iron",     8, 16),
                    P(10),
                    R("coin",   350, 18),
                    R("wood",    12, 14),
                    R("gem",      3,  6),
                    R("coin",   400, 14),
                    R("potion",   5, 12),
                    R("iron",    10, 14)),

                Build(tierRules, 4,
                    R("coin",   500, 18),
                    R("gem",      4,  8),
                    R("iron",    14, 14),
                    R("wood",    18, 14),
                    R("coin",   600, 16),
                    P(10),
                    R("potion",   8, 12),
                    R("coin",   700, 12)),

                Build(tierRules, 5,
                    R("coin",  1000, 16),
                    R("gem",      8, 10),
                    R("iron",    25, 14),
                    R("wood",    30, 14),
                    R("coin",  1250, 14),
                    R("potion",  12, 12),
                    R("gem",     12,  8),
                    R("coin",  1500, 12))
            };
        }

        public Zone GetZone(int index)
        {
            if (index < 1 || index > _zones.Length)
                return null;

            return _zones[index - 1];
        }

        private static Zone Build(WheelTierRuleProvider tierRules, int index, params WheelSlice[] slices)
        {
            return new Zone(index, tierRules.TierFor(index), new Wheel(slices));
        }

        private static WheelSlice R(string itemId, int amount, int weight)
            => WheelSlice.CreateReward(itemId, amount, weight);

        private static WheelSlice P(int weight)
            => WheelSlice.CreatePenalty(weight);
    }
}