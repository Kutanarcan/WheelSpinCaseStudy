namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// The buffered input of the next presentation step: what the session reported between
    /// a spin request and the moment the presenter is allowed to animate it.
    /// </summary>
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
