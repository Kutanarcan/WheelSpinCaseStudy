namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Collects what the session reports during a spin and holds it until the presentation layer is
    /// ready to play it. The rules resolve the moment a spin is requested, but the wheel is still
    /// turning then — nothing may be shown until it stops, so the report has to wait somewhere.
    /// </summary>
    public sealed class SpinOutcomeBuffer
    {
        private SpinOutcome _outcome;

        public SpinOutcome Outcome => _outcome;

        public void Subscribe(WheelSession session)
        {
            session.ZoneStarted += CaptureZoneStarted;
            session.SpinResolved += CaptureSpinResolved;
            session.RunFailed += CaptureRunFailed;
            session.RunCashedOut += CaptureRunEnded;
            session.RunCompleted += CaptureRunEnded;
        }

        public void Unsubscribe(WheelSession session)
        {
            session.ZoneStarted -= CaptureZoneStarted;
            session.SpinResolved -= CaptureSpinResolved;
            session.RunFailed -= CaptureRunFailed;
            session.RunCashedOut -= CaptureRunEnded;
            session.RunCompleted -= CaptureRunEnded;
        }

        public void Clear() => _outcome = default;

        private void CaptureSpinResolved(SpinResult result)
        {
            _outcome.Result = result;
            _outcome.HasResult = true;
        }

        private void CaptureZoneStarted(Zone zone) => _outcome.NextZone = zone;

        private void CaptureRunFailed(int zone, long lost) => _outcome.RunFailed = true;

        private void CaptureRunEnded(int zone, long banked) => _outcome.RunEnded = true;
    }
}
