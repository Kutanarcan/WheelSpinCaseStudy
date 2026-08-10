using UnityEngine;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Penalty hedef olasiligi ile dilim agirligi arasindaki tek donusum noktasi.
    /// Runtime ve editor onizlemesi ayni sonucu uretsin diye ikisi de burayi kullanir.
    /// </summary>
    public static class PenaltyOdds
    {
        public const float MaxChance = 0.9f;

        /// <summary>Hedef olasiligi, diger dilimlerin agirlik toplamina gore mutlak agirliga cevirir.</summary>
        public static int WeightFor(int otherWeightSum, float chance)
        {
            chance = Mathf.Clamp(chance, 0f, MaxChance);

            if (chance <= 0f || otherWeightSum <= 0)
                return 0;

            return Mathf.Max(1, Mathf.RoundToInt(otherWeightSum * chance / (1f - chance)));
        }

        /// <summary>Yuvarlamadan sonra gercekte olusan olasilik. Editor bu degeri gosterir.</summary>
        public static float ChanceFor(int otherWeightSum, int penaltyWeight)
        {
            int total = otherWeightSum + penaltyWeight;

            return total > 0 ? penaltyWeight / (float)total : 0f;
        }
    }
}
