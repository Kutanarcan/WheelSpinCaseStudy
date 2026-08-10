namespace CaseStudy.WheelSpin
{
    public interface IZoneProvider
    {
        int ZoneCount { get; }

        Zone GetZone(int index);

        Zone GetZoneWithPenaltyDisabled(int index);
    }
}
