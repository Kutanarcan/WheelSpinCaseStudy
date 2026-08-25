namespace CaseStudy.WheelSpin
{
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
