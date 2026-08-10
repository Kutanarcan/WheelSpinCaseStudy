using System;

namespace CaseStudy.WheelSpin
{
    public class WheelPresenter
    {
        private readonly WheelSlicePresenter _slicePresenter;
        private readonly ZonePresenter _zonePresenter;
        private readonly RewardPresenter _rewardPresenter;
        private readonly PopupPresenter _popupPresenter;
        private readonly Action _continueAfterSpin;
        private readonly Action _onPartComplete;

        private bool _hasResult;
        private SpinResult _result;
        private Zone _nextZone;
        private bool _runFailed;
        private bool _runEnded;

        private int _pendingParts;

        private bool _isBusy;
        public event Action<bool> BusyChanged;

        public event Action ClaimClicked;
        public event Action ReviveClicked;
        public event Action GiveUpClicked;

        public bool IsBusy => _isBusy;

        public WheelPresenter(
            WheelSceneView view,
            ItemRegistry registry,
            WheelTierRuleProvider tierRules,
            WheelTierViewDatabase wheelTierViewDatabase,
            RewardLedger rewards)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            _slicePresenter = new WheelSlicePresenter(
                view.WheelView, registry, view.PenaltySprite, view.PenaltyViewSettings,
                view.SpinSettings,
                wheelTierViewDatabase);

            _zonePresenter = new ZonePresenter(
                view.ZoneCountView, view.ZoneSelectorView, view.ZonePalette, view.SpinSettings, tierRules);

            _rewardPresenter = new RewardPresenter(view.RewardHolderView, registry);

            _popupPresenter = new PopupPresenter(view.CashoutPopup, view.RevivePopup, registry, rewards);
            _popupPresenter.ClaimClicked += HandleClaimClicked;
            _popupPresenter. ReviveClicked += HandleReviveClicked;
            _popupPresenter.GiveUpClicked += HandleGiveUpClicked;

            _continueAfterSpin = ContinueAfterSpin;
            _onPartComplete = HandlePartComplete;
        }

        public void Initialize(int zoneCount)
        {
            _zonePresenter.Initialize(zoneCount);
            _rewardPresenter.Initialize();
            _popupPresenter.Initialize();

            ClearCaptured();
            _pendingParts = 0;
            _isBusy = false;
        }

        public void Deinitialize()
        {
            _slicePresenter.Deinitialize();
            _zonePresenter.Deinitialize();
            _rewardPresenter.Deinitialize();

            _popupPresenter.ClaimClicked -= HandleClaimClicked;
            _popupPresenter.ReviveClicked -= HandleReviveClicked;
            _popupPresenter.GiveUpClicked -= HandleGiveUpClicked;
            _popupPresenter.Deinitialize();

            ClearCaptured();
            _pendingParts = 0;
            _isBusy = false;

            BusyChanged = null;
            ClaimClicked = null;
            ReviveClicked = null;
            GiveUpClicked = null;
        }

        public void ResetForNewRun()
        {
            _slicePresenter.ResetForNewRun();
            _zonePresenter.ResetForNewRun();
            _rewardPresenter.ResetForNewRun();
            _popupPresenter.ResetForNewRun();

            ClearCaptured();
            _pendingParts = 0;
            SetBusy(false);
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

        private void CaptureSpinResolved(SpinResult result)
        {
            _result = result;
            _hasResult = true;
        }

        private void CaptureZoneStarted(Zone zone) => _nextZone = zone;

        private void CaptureRunFailed(int zone, long lost) => _runFailed = true;

        private void CaptureRunEnded(int zone, long banked) => _runEnded = true;

        /// <summary>Revive sonrasi cark yeniden bind edilir; devre disi bomba solar.</summary>
        private void HandleZoneRefreshed(Zone zone) => _slicePresenter.Bind(zone);

        private void ClearCaptured()
        {
            _hasResult = false;
            _result = default;
            _nextZone = null;
            _runFailed = false;
            _runEnded = false;
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

        /// <summary>Revive'dan sonra normal akisa donus: popup kapanir, butonlar tekrar acilir.</summary>
        public void PlayRevive()
        {
            _popupPresenter.HideAll();

            ClearCaptured();
            Finish();
        }

        private void ContinueAfterSpin()
        {
            if (_hasResult && !_result.IsPenalty)
                _rewardPresenter.Add(_result.ItemId, _result.Amount);

            // Cark bombanin ustunde durdu; karar oyuncuda. Busy true kalir, butonlar kilitli.
            if (_runFailed)
            {
                _popupPresenter.ShowRevive();
                return;
            }

            // CashOut ya da zone'lar bitti: her iki durumda da ayni popup.
            if (_runEnded)
            {
                _popupPresenter.ShowCashout();
                return;
            }

            if (_nextZone == null)
            {
                Finish();
                return;
            }

            Zone next = _nextZone;
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

        private void HandleClaimClicked() => ClaimClicked?.Invoke();

        private void HandleReviveClicked() => ReviveClicked?.Invoke();

        private void HandleGiveUpClicked() => GiveUpClicked?.Invoke();
    }
}
