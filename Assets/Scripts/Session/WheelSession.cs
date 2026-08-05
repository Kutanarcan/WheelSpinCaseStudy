using System;

namespace CaseStudy.WheelSpin
{
    public class WheelSession
    {
        private readonly IZoneProvider _zoneProvider;
        private readonly WheelSpinner _spinner;

        public int ZoneIndex { get; private set; }
        public long Accumulated { get; private set; }
        public Zone CurrentZone { get; private set; }
        public bool IsRunActive { get; private set; }

        /// <summary>Yeni zone hazir. Carki cizmek icin bunu dinle.</summary>
        public event Action<Zone> ZoneStarted;

        /// <summary>Spin cozuldu. SliceIndex animasyonun duracagi dilim.</summary>
        public event Action<SpinResult> SpinResolved;

        /// <summary>Ceza geldi. (ulasilan zone, kaybedilen birikim)</summary>
        public event Action<int, long> RunFailed;

        /// <summary>Oyuncu cikti. (ulasilan zone, cebe giren birikim)</summary>
        public event Action<int, long> RunCashedOut;

        /// <summary>Zone kalmadi. (son zone, cebe giren birikim)</summary>
        public event Action<int, long> RunCompleted;

        public WheelSession(IZoneProvider zoneProvider, WheelSpinner spinner)
        {
            _zoneProvider = zoneProvider ?? throw new ArgumentNullException(nameof(zoneProvider));
            _spinner = spinner ?? throw new ArgumentNullException(nameof(spinner));
        }

        public void StartRun()
        {
            ZoneIndex = 0;
            Accumulated = 0;
            CurrentZone = null;
            IsRunActive = true;

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
                long lost = Accumulated;
                int reached = ZoneIndex;

                Accumulated = 0;
                CurrentZone = null;
                IsRunActive = false;

                RunFailed?.Invoke(reached, lost);
                return true;
            }

            Accumulated += result.Amount;
            AdvanceZone();
            return true;
        }

        public long CashOut()
        {
            if (!IsRunActive)
                return 0;

            long banked = Accumulated;
            int reached = ZoneIndex;

            Accumulated = 0;
            CurrentZone = null;
            IsRunActive = false;

            RunCashedOut?.Invoke(reached, banked);
            return banked;
        }

        public void ClearListeners()
        {
            ZoneStarted = null;
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

                Accumulated = 0;
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