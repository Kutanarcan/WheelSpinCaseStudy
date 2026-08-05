namespace CaseStudy.WheelSpin
{
    public class Zone
    {
        public readonly int Index;
        public readonly WheelTier Tier;
        public readonly Wheel Wheel;

        public Zone(int index, WheelTier tier, Wheel wheel)
        {
            Index = index;
            Tier = tier;
            Wheel = wheel;
        }
    }
}
