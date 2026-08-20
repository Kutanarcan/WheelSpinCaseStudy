using System;

namespace CaseStudy.WheelSpin
{
    public class RandomWeightedResultCalculator : IWheelSpinResultCalculator
    {
        private readonly Random _random;

        public RandomWeightedResultCalculator(Random random)
        {
            _random = random;
        }

        public int Calculate(Wheel wheel)
        {
            int total = 0;

            for (int i = 0; i < wheel.Length; i++)
            {
                total += wheel[i].Weight;
            }

            if (total <= 0)
                return -1;

            int roll = _random.Next(total);
            int cursor = 0;

            for (int i = 0; i < wheel.Length; i++)
            {
                cursor += wheel[i].Weight;

                if (roll < cursor)
                    return i;
            }

            return wheel.Length - 1;
        }
    }
}
