namespace CaseStudy.WheelSpin
{
    public struct SpinOutcome
    {
        public bool HasResult;
        public SpinResult Result;
        public Zone NextZone;
        public bool RunFailed;
        public bool RunEnded;

        public bool HasReward => HasResult && !Result.IsPenalty && Result.Amount > 0;

        public bool HasPenalty => HasResult && Result.IsPenalty;

        public bool HasZoneChange => !RunFailed && !RunEnded && NextZone != null;
    }
}
