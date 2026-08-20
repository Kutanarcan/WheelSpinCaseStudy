namespace CaseStudy.WheelSpin
{
    public interface IZoneProvider
    {
        int ZoneCount { get; }

        Zone GetZone(int index, bool penaltyDisabled = false);
    }
}
