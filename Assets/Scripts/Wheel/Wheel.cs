namespace CaseStudy.WheelSpin
{
    public sealed class Wheel
    {
        public readonly WheelSlice[] Slices;

        public Wheel(WheelSlice[] slices)
        {
            Slices = slices;
        }

        public int Length => Slices.Length;
        public WheelSlice this[int index] => Slices[index];
    }
}
