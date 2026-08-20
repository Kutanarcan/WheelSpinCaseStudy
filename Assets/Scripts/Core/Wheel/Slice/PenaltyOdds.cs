using System;

namespace CaseStudy.WheelSpin
{
    public static class PenaltyOdds
    {
        public const float MaxChance = 0.9f;

        public static int WeightFor(int otherWeightSum, float chance)
        {
            if (chance < 0f) chance = 0f;
            else if (chance > MaxChance) chance = MaxChance;

            if (chance <= 0f || otherWeightSum <= 0)
                return 0;

            int rounded = (int)Math.Round(otherWeightSum * chance / (1f - chance));

            return Math.Max(1, rounded);
        }

        public static float ChanceFor(int otherWeightSum, int penaltyWeight)
        {
            int total = otherWeightSum + penaltyWeight;

            return total > 0 ? penaltyWeight / (float)total : 0f;
        }
    }
}
