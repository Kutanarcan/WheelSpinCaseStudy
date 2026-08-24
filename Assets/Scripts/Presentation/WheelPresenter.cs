using System;

namespace CaseStudy.WheelSpin
{
    public class WheelPresenter
    {
        private readonly WheelSlicePresenter _slicePresenter;
        private readonly ZonePresenter _zonePresenter;
        private readonly RewardPresenter _rewardPresenter;
        private readonly RewardFlightPresenter _flightPresenter;
        private readonly PopupPresenter _popupPresenter;
        private readonly AnimationGate _gate = new AnimationGate();

        private readonly Action _continueAfterSpin;
        private readonly Action _startZoneTransition;
        private readonly Action _onSequenceComplete;

        private SpinOutcome _outcome;
        private Action _zoneStripPart;
        private Action _wheelChangePart;
        private bool _isBusy;

        public event Action ClaimClicked
        {
            add => _popupPresenter.ClaimClicked += value;
            remove => _popupPresenter.ClaimClicked -= value;
        }

        public event Action ReviveClicked
        {
            add => _popupPresenter.ReviveClicked += value;
            remove => _popupPresenter.ReviveClicked -= value;
        }

        public event Action GiveUpClicked
        {
            add => _popupPresenter.GiveUpClicked += value;
            remove => _popupPresenter.GiveUpClicked -= value;
        }

        public bool IsBusy => _isBusy;

        public WheelPresenter(
            WheelSceneView view,
            ItemRegistry registry,
            WheelTierRuleProvider tierRules,
            WheelTierViewDatabase wheelTierViewDatabase,
            RewardLedger rewards)
        {
            _slicePresenter = new WheelSlicePresenter(
                view.WheelView, registry, view.PenaltySprite, view.PenaltyViewSettings,
                view.SpinSettings,
                wheelTierViewDatabase);

            _zonePresenter = new ZonePresenter(
                view.ZoneCountView, view.ZoneSelectorView, wheelTierViewDatabase, view.CurrentZoneColor,
                view.SpinSettings, tierRules);

            _rewardPresenter = new RewardPresenter(view.RewardHolderView, registry);

            _flightPresenter = new RewardFlightPresenter(
                view.RewardFlightView, _rewardPresenter, registry, view.FlightSettings);

            _popupPresenter = new PopupPresenter(view.CashoutPopup, view.RevivePopup, registry, rewards);

            _continueAfterSpin = ContinueAfterSpin;
            _startZoneTransition = StartZoneTransition;
            _onSequenceComplete = HandleSequenceComplete;
        }

        public void Initialize(int zoneCount)
        {
            _zonePresenter.Initialize(zoneCount);
            _rewardPresenter.Initialize();
            _flightPresenter.Initialize();
            _popupPresenter.Initialize();

            Finish();
        }

        public void Deinitialize()
        {
            _slicePresenter.Deinitialize();
            _zonePresenter.Deinitialize();
            _rewardPresenter.Deinitialize();
            _flightPresenter.Deinitialize();
            _popupPresenter.Deinitialize();

            Finish();
        }

        public void ResetForNewRun()
        {
            _slicePresenter.ResetForNewRun();
            _zonePresenter.ResetForNewRun();
            _rewardPresenter.ResetForNewRun();
            _flightPresenter.ResetForNewRun();
            _popupPresenter.ResetForNewRun();

            Finish();
        }

        public void Subscribe(WheelSession session)
        {
            session.ZoneStarted += CaptureZoneStarted;
            session.ZoneRefreshed += HandleZoneRefreshed;
            session.SpinResolved += CaptureSpinResolved;
            session.RunFailed += CaptureRunFailed;
            session.RunCashedOut += CaptureRunEnded;
            session.RunCompleted += CaptureRunEnded;
        }

        public void Unsubscribe(WheelSession session)
        {
            session.ZoneStarted -= CaptureZoneStarted;
            session.ZoneRefreshed -= HandleZoneRefreshed;
            session.SpinResolved -= CaptureSpinResolved;
            session.RunFailed -= CaptureRunFailed;
            session.RunCashedOut -= CaptureRunEnded;
            session.RunCompleted -= CaptureRunEnded;
        }

        public void PlayInitial()
        {
            if (_outcome.NextZone != null)
            {
                _slicePresenter.Bind(_outcome.NextZone);
                _zonePresenter.Show(_outcome.NextZone.Index, instant: true, onComplete: null);
            }

            Finish();
        }

        public void Play()
        {
            SetBusy(true);

            if (_outcome.HasResult)
            {
                _slicePresenter.PlaySpin(_outcome.Result.SliceIndex, _continueAfterSpin);
                return;
            }

            ContinueAfterSpin();
        }

        public void PlayRevive()
        {
            _popupPresenter.HideAll();
            _flightPresenter.Kill();

            Finish();
        }

        private void CaptureSpinResolved(SpinResult result)
        {
            _outcome.Result = result;
            _outcome.HasResult = true;
        }

        private void CaptureZoneStarted(Zone zone) => _outcome.NextZone = zone;

        private void CaptureRunFailed(int zone, long lost) => _outcome.RunFailed = true;

        private void CaptureRunEnded(int zone, long banked) => _outcome.RunEnded = true;

        private void HandleZoneRefreshed(Zone zone) => _slicePresenter.Bind(zone);

        /// <summary>
        /// Builds the post-spin timeline. Parts are reserved on the gate before anything starts, so
        /// the zone change can be handed its callback here and only launched later, when the first
        /// reward icon leaves the wheel.
        /// </summary>
        private void ContinueAfterSpin()
        {
            _gate.Begin(_onSequenceComplete);

            if (_outcome.HasZoneChange)
            {
                _zoneStripPart = _gate.Track();
                _wheelChangePart = _gate.Track();
            }

            if (_outcome.HasReward)
            {
                _flightPresenter.Play(
                    _outcome.Result.ItemId,
                    _outcome.Result.Amount,
                    _slicePresenter.SliceWorldPosition(_outcome.Result.SliceIndex),
                    _startZoneTransition,
                    _gate.Track());
            }
            else
            {
                StartZoneTransition();
            }

            _gate.Seal();
        }

        private void StartZoneTransition()
        {
            if (!_outcome.HasZoneChange)
                return;

            Zone next = _outcome.NextZone;

            _zonePresenter.Show(next.Index, instant: false, _zoneStripPart);
            _slicePresenter.PlayZoneChange(next, _wheelChangePart);
        }

        private void HandleSequenceComplete()
        {
            bool failed = _outcome.RunFailed;
            bool ended = _outcome.RunEnded;

            Finish();

            if (failed)
                _popupPresenter.ShowRevive();
            else if (ended)
                _popupPresenter.ShowCashout();
        }

        private void Finish()
        {
            _gate.Cancel();

            _zoneStripPart = null;
            _wheelChangePart = null;
            _outcome = default;

            SetBusy(false);
        }

        private void SetBusy(bool busy) => _isBusy = busy;
    }
}
