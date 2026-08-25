using System;

namespace CaseStudy.WheelSpin
{
    public sealed class SpinTimeline
    {
        private readonly WheelPresenterSet _presenters;
        private readonly SpinOutcomeBuffer _buffer;
        private readonly AnimationGate _gate = new AnimationGate();

        private readonly Action _startZoneTransition;
        private readonly Action<int> _onWheelAmountChanged;
        private readonly Action _onGateComplete;

        private Action _zoneStripPart;
        private Action _wheelChangePart;

        public event Action Completed;

        public SpinTimeline(WheelPresenterSet presenters, SpinOutcomeBuffer buffer)
        {
            _presenters = presenters;
            _buffer = buffer;

            _startZoneTransition = StartZoneTransition;
            _onWheelAmountChanged = HandleWheelAmountChanged;
            _onGateComplete = HandleGateComplete;
        }

        public void Start()
        {
            SpinOutcome outcome = _buffer.Outcome;

            _gate.Begin(_onGateComplete);

            if (outcome.HasZoneChange)
            {
                _zoneStripPart = _gate.Track();
                _wheelChangePart = _gate.Track();
            }

            if (outcome.HasReward)
                StartRewardFlight(outcome);
            else if (outcome.HasPenalty)
                StartPenalty(outcome);
            else
                StartZoneTransition();

            _gate.Seal();
        }

        public void Cancel()
        {
            _gate.Cancel();

            _zoneStripPart = null;
            _wheelChangePart = null;
        }

        private void StartRewardFlight(SpinOutcome outcome)
        {
            _presenters.Flight.Play(
                outcome.Result.ItemId,
                outcome.Result.Amount,
                _presenters.Slice.SliceWorldPosition(outcome.Result.SliceIndex),
                _startZoneTransition,
                _onWheelAmountChanged,
                _gate.Track());
        }

        private void StartPenalty(SpinOutcome outcome)
        {
            _presenters.Reward.SetCashOutActive(false);
            _presenters.Penalty.Play(outcome.Result.SliceIndex, _gate.Track());
        }

        private void StartZoneTransition()
        {
            SpinOutcome outcome = _buffer.Outcome;

            if (!outcome.HasZoneChange)
                return;

            _presenters.Zone.Show(outcome.NextZone.Index, instant: false, _zoneStripPart);
            _presenters.Slice.PlayZoneChange(outcome.NextZone, _wheelChangePart);
        }

        private void HandleWheelAmountChanged(int remaining)
            => _presenters.Slice.SetSliceAmount(_buffer.Outcome.Result.SliceIndex, remaining);

        private void HandleGateComplete() => Completed?.Invoke();
    }
}
