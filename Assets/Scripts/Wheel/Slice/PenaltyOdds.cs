using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public static class PenaltyOdds
    {
        public const float MaxChance = 0.9f;

        public static int WeightFor(int otherWeightSum, float chance)
        {
            chance = Mathf.Clamp(chance, 0f, MaxChance);

            if (chance <= 0f || otherWeightSum <= 0)
                return 0;

            return Mathf.Max(1, Mathf.RoundToInt(otherWeightSum * chance / (1f - chance)));
        }

        public static float ChanceFor(int otherWeightSum, int penaltyWeight)
        {
            int total = otherWeightSum + penaltyWeight;

            return total > 0 ? penaltyWeight / (float)total : 0f;
        }
    }
}
