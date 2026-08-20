using System;

namespace CaseStudy.WheelSpin
{
    public class WheelSession
    {
        private readonly IZoneProvider _zoneProvider;
        private readonly WheelSpinner _spinner;
        private readonly RewardLedger _rewards;

        public int ZoneIndex { get; private set; }
        public long Accumulated { get; private set; }
        public Zone CurrentZone { get; private set; }
        public bool IsRunActive { get; private set; }
        public bool IsAwaitingRevive { get; private set; }

        public RewardLedger Rewards => _rewards;

        public event Action<Zone> ZoneStarted;
        public event Action<Zone> ZoneRefreshed;
        public event Action<SpinResult> SpinResolved;
        public event Action<int, long> RunFailed;
        public event Action<int, long> RunCashedOut;
        public event Action<int, long> RunCompleted;

        public WheelSession(IZoneProvider zoneProvider, WheelSpinner spinner, RewardLedger rewards)
        {
            _zoneProvider = zoneProvider ?? throw new ArgumentNullException(nameof(zoneProvider));
            _spinner = spinner ?? throw new ArgumentNullException(nameof(spinner));
            _rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
        }

        public void StartRun()
        {
            ZoneIndex = 0;
            Accumulated = 0;
            CurrentZone = null;
            IsRunActive = true;
            IsAwaitingRevive = false;

            _rewards.Clear();

            AdvanceZone();
        }

        public bool TrySpin(out SpinResult result)
        {
            result = default;

            if (!IsRunActive || CurrentZone == null)
                return false;

            result = _spinner.Spin(CurrentZone.Wheel);

            SpinResolved?.Invoke(result);

            if (result.IsPenalty)
            {
                IsRunActive = false;
                IsAwaitingRevive = true;

                RunFailed?.Invoke(ZoneIndex, Accumulated);
                return true;
            }

            _rewards.Add(result.ItemId, result.Amount);
            Accumulated += result.Amount;

            AdvanceZone();
            return true;
        }

        public bool TryRevive()
        {
            if (!IsAwaitingRevive || CurrentZone == null)
                return false;

            Zone revived = _zoneProvider.GetZone(ZoneIndex, penaltyDisabled: true);

            if (revived == null)
                return false;

            CurrentZone = revived;
            IsAwaitingRevive = false;
            IsRunActive = true;

            ZoneRefreshed?.Invoke(revived);
            return true;
        }

        public void GiveUp()
        {
            if (!IsAwaitingRevive)
                return;

            IsAwaitingRevive = false;

            Accumulated = 0;
            CurrentZone = null;

            _rewards.Clear();
        }

        public long CashOut()
        {
            if (!IsRunActive)
                return 0;

            long banked = Accumulated;

            CurrentZone = null;
            IsRunActive = false;

            RunCashedOut?.Invoke(ZoneIndex, banked);
            return banked;
        }

        public void ClearListeners()
        {
            ZoneStarted = null;
            ZoneRefreshed = null;
            SpinResolved = null;
            RunFailed = null;
            RunCashedOut = null;
            RunCompleted = null;
        }

        private void AdvanceZone()
        {
            ZoneIndex++;

            Zone zone = _zoneProvider.GetZone(ZoneIndex);

            if (zone == null)
            {
                long banked = Accumulated;
                int lastZone = ZoneIndex - 1;

                ZoneIndex = lastZone;
                CurrentZone = null;
                IsRunActive = false;

                RunCompleted?.Invoke(lastZone, banked);
                return;
            }

            CurrentZone = zone;
            ZoneStarted?.Invoke(zone);
        }
    }
}
