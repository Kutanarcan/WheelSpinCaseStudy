using System;

namespace CaseStudy.WheelSpin
{
    public class WheelTierRuleProvider
    {
        public readonly int GoldEvery;
        public readonly int SilverEvery;

        public WheelTierRuleProvider(int goldEvery = 30, int silverEvery = 5)
        {
            GoldEvery = goldEvery;
            SilverEvery = silverEvery;
        }

        public WheelTier TierFor(int round)
        {
            if (round % GoldEvery == 0) 
                return WheelTier.Gold;

            if (round % SilverEvery == 0) 
                return WheelTier.Silver;

            return WheelTier.Bronze;
        }

        public bool HasPenalty(int round)
        {
            return TierFor(round) == WheelTier.Bronze;
        }
    }
}
