using System;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// The presentation layer's entry point: run lifecycle, and whether the game is currently
    /// showing something. The sequence that plays after a spin belongs to <see cref="SpinTimeline"/>.
    /// </summary>
    public class WheelPresenter
    {
        private readonly WheelPresenterSet _presenters;
        private readonly SpinOutcomeBuffer _buffer = new SpinOutcomeBuffer();
        private readonly SpinTimeline _timeline;
        private readonly Action _startTimeline;

        private bool _isBusy;

        public bool IsBusy => _isBusy;

        public PopupPresenter Popups => _presenters.Popup;

        public WheelPresenter(
            WheelSceneView view,
            ItemRegistry registry,
            WheelTierRuleProvider tierRules,
            WheelTierViewDatabase wheelTierViewDatabase,
            RewardLedger rewards)
        {
            _presenters = new WheelPresenterSet(
                view, registry, tierRules, wheelTierViewDatabase, rewards);

            _timeline = new SpinTimeline(_presenters, _buffer);
            _timeline.Completed += HandleTimelineComplete;

            _startTimeline = _timeline.Start;
        }

        public void Initialize(int zoneCount)
        {
            _presenters.Initialize(zoneCount);
            Finish();
        }

        public void Deinitialize()
        {
            _presenters.Deinitialize();
            Finish();
        }

        public void ResetForNewRun()
        {
            _presenters.ResetForNewRun();
            Finish();
        }

        /// ZoneRefreshed is handled straight away rather than buffered: a revive rebuilds the wheel
        /// that is already on screen, so it cannot wait for a spin to end.
        public void Subscribe(WheelSession session)
        {
            _buffer.Subscribe(session);
            session.ZoneRefreshed += HandleZoneRefreshed;
        }

        public void Unsubscribe(WheelSession session)
        {
            _buffer.Unsubscribe(session);
            session.ZoneRefreshed -= HandleZoneRefreshed;
        }

        public void PlayInitial()
        {
            Zone zone = _buffer.Outcome.NextZone;

            if (zone != null)
            {
                _presenters.Slice.Bind(zone);
                _presenters.Zone.Show(zone.Index, instant: true, onComplete: null);
            }

            Finish();
        }

        public void Play()
        {
            _isBusy = true;

            SpinOutcome outcome = _buffer.Outcome;

            if (outcome.HasResult)
            {
                _presenters.Slice.PlaySpin(outcome.Result.SliceIndex, _startTimeline);
                return;
            }

            _timeline.Start();
        }

        public void PlayRevive()
        {
            _presenters.Popup.HideAll();
            _presenters.Flight.Kill();
            _presenters.Penalty.Kill();
            _presenters.Reward.SetCashOutActive(true);

            Finish();
        }

        private void HandleZoneRefreshed(Zone zone) => _presenters.Slice.Bind(zone);

        /// Read before Finish clears the buffer, so the popup decision survives the reset.
        private void HandleTimelineComplete()
        {
            SpinOutcome outcome = _buffer.Outcome;

            Finish();

            if (outcome.RunFailed)
                _presenters.Popup.ShowRevive();
            else if (outcome.RunEnded)
                _presenters.Popup.ShowCashout();
        }

        private void Finish()
        {
            _timeline.Cancel();
            _buffer.Clear();

            _isBusy = false;
        }
    }
}
