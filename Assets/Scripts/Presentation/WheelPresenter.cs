using System;

namespace CaseStudy.WheelSpin
{
    public class WheelPresenter
    {
        private readonly WheelSlicePresenter _slicePresenter;
        private readonly ZonePresenter _zonePresenter;
        private readonly RewardPresenter _rewardPresenter;

        private readonly Action _continueAfterSpin;
        private readonly Action _onPartComplete;

        private bool _hasResult;
        private SpinResult _result;
        private Zone _nextZone;
        private bool _runFailed;

        private int _pendingParts;

        private bool _isBusy;
        public event Action<bool> BusyChanged;

        public bool IsBusy => _isBusy;

        public WheelPresenter(
            WheelSceneView view,
            ItemRegistry registry,
            WheelTierRuleProvider tierRules,
            int sliceCount)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            _slicePresenter = new WheelSlicePresenter(
                view.WheelView, registry, view.PenaltySprite, view.PenaltyViewSettings,
                view.SpinSettings, sliceCount);

            _zonePresenter = new ZonePresenter(
                view.ZoneCountView, view.ZoneSelectorView, view.ZonePalette, view.SpinSettings, tierRules);

            _rewardPresenter = new RewardPresenter(view.RewardHolderView, registry);

            _continueAfterSpin = ContinueAfterSpin;
            _onPartComplete = HandlePartComplete;
        }

        public void Initialize(int zoneCount)
        {
            _zonePresenter.Initialize(zoneCount);
            _rewardPresenter.Initialize();

            ClearCaptured();
            _pendingParts = 0;
            _isBusy = false;
        }

        public void Deinitialize()
        {
            _slicePresenter.Deinitialize();
            _zonePresenter.Deinitialize();
            _rewardPresenter.Deinitialize();

            ClearCaptured();
            _pendingParts = 0;
            _isBusy = false;
            BusyChanged = null;
        }

        public void ResetForNewRun()
        {
            _slicePresenter.ResetForNewRun();
            _zonePresenter.ResetForNewRun();
            _rewardPresenter.ResetForNewRun();

            ClearCaptured();
            _pendingParts = 0;
            SetBusy(false);
        }

        public void Subscribe(WheelSession session)
        {
            session.ZoneStarted += CaptureZoneStarted;
            session.SpinResolved += CaptureSpinResolved;
            session.RunFailed += CaptureRunFailed;
        }

        public void Unsubscribe(WheelSession session)
        {
            session.ZoneStarted -= CaptureZoneStarted;
            session.SpinResolved -= CaptureSpinResolved;
            session.RunFailed -= CaptureRunFailed;
        }

        private void CaptureSpinResolved(SpinResult result)
        {
            _result = result;
            _hasResult = true;
        }

        private void CaptureZoneStarted(Zone zone) => _nextZone = zone;

        private void CaptureRunFailed(int zone, long lost) => _runFailed = true;

        private void ClearCaptured()
        {
            _hasResult = false;
            _result = default;
            _nextZone = null;
            _runFailed = false;
        }

        public void PlayInitial()
        {
            if (_nextZone != null)
            {
                _slicePresenter.Bind(_nextZone);
                _zonePresenter.Show(_nextZone.Index, instant: true, onComplete: null);
            }

            ClearCaptured();
            _pendingParts = 0;
            SetBusy(false);
        }

        public void Play()
        {
            SetBusy(true);

            if (_hasResult)
            {
                _slicePresenter.PlaySpin(_result.SliceIndex, _continueAfterSpin);
                return;
            }

            ContinueAfterSpin();
        }

        private void ContinueAfterSpin()
        {
            if (_hasResult && !_result.IsPenalty)
                _rewardPresenter.Add(_result.ItemId, _result.Amount);

            if (_runFailed)
                _rewardPresenter.Clear();

            if (_nextZone == null)
            {
                Finish();
                return;
            }

            // Finish -> ClearCaptured alani null'ladigi icin referansi simdi yakala.
            Zone next = _nextZone;

            // Sayaci HER IKI cagridan once kur: biri callback'ini senkron cagirabilir.
            _pendingParts = 2;

            _zonePresenter.Show(next.Index, instant: false, _onPartComplete);
            _slicePresenter.PlayZoneChange(next, _onPartComplete);
        }

        private void HandlePartComplete()
        {
            _pendingParts--;

            if (_pendingParts > 0)
                return;

            Finish();
        }

        private void Finish()
        {
            _pendingParts = 0;

            ClearCaptured();
            SetBusy(false);
        }

        private void SetBusy(bool busy)
        {
            if (_isBusy == busy) return;

            _isBusy = busy;
            BusyChanged?.Invoke(busy);
        }
    }
}