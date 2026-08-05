using System;

namespace CaseStudy.WheelSpin
{
    public class RandomWeightedResultCalculator : IWheelSpinResultCalculator
    {
        private readonly Random _random;
        private readonly int _sliceCount;

        public RandomWeightedResultCalculator(Random random, int sliceCount = 8)
        {
            _random = random;
            _sliceCount = sliceCount;
        }

        public int Calculate(Wheel wheel)
        {
            int total = 0;

            for (int i = 0; i < _sliceCount; i++)
            {
                total += wheel[i].Weight;
            }

            if (total <= 0)
                return -1;

            int roll = _random.Next(total);
            int cursor = 0;

            for (int i = 0; i < _sliceCount; i++)
            {
                cursor += wheel[i].Weight;

                if (roll < cursor)
                    return i;
            }

            return _sliceCount - 1;
        }
    }
}
