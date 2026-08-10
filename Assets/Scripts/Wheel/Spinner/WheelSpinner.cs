using System;

namespace CaseStudy.WheelSpin
{
    public class WheelSpinner
    {
        private readonly IWheelSpinResultCalculator _calculator;
        private readonly Random _random;

        public WheelSpinner(IWheelSpinResultCalculator calculator, Random random)
        {
            _calculator = calculator;
            _random = random;
        }

        public SpinResult Spin(Wheel wheel)
        {
            int index = _calculator.Calculate(wheel);

            if (index < 0 || index >= wheel.Length)
            {
                UnityEngine.Debug.LogError(
                    $"[{nameof(WheelSpinner)}] Invalid slice index {index}; wheel weights may all be zero.");

                index = 0;
            }

            WheelSlice slice = wheel[index];

            if (slice.Type == SliceType.Penalty)
                return SpinResult.Penalty(index);

            return SpinResult.Reward(index, slice.ItemId, slice.Amount);
        }
    }
}
