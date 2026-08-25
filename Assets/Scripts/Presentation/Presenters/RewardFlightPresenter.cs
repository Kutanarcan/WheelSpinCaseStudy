using DG.Tweening;
using System;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Plays the win effect timeline: icons pop in around the wheel one by one, then leave for the
    /// reward board on the same cadence. Each arrival ticks the board counter up by its share.
    /// </summary>
    public class RewardFlightPresenter
    {
        private readonly RewardFlightView _view;
        private readonly RewardFlightSpawner _spawner;
        private readonly RewardPresenter _rewards;
        private readonly AudioManager _audio;
        private readonly RewardFlightSettings _settings;

        private readonly TweenCallback _onFirstFlight;
        private readonly TweenCallback _onArrive;
        private readonly TweenCallback _onIconSpawned;
        private readonly TweenCallback _onSequenceComplete;

        private int[] _shareArray = Array.Empty<int>();

        private Sequence _sequence;
        private Action _pendingComplete;
        private Action _pendingFirstFlight;
        private string _itemId;
        private int _shareCount;
        private int _arrivedCount;
        private int _spawnedCount;
        private int _wheelRemaining;
        private Action<int> _pendingWheelAmount;

        public bool CanPlay => _view != null && _view.IsReady;

        public RewardFlightPresenter(
            RewardFlightView view,
            RewardPresenter rewards,
            ItemRegistry registry,
            AudioManager audio,
            RewardFlightSettings settings)
        {
            _view = view;
            _rewards = rewards;
            _audio = audio;
            _settings = settings;
            _spawner = new RewardFlightSpawner(view, registry, settings);

            _onFirstFlight = HandleFirstFlight;
            _onIconSpawned = HandleIconSpawned;
            _onArrive = HandleArrive;
            _onSequenceComplete = HandleSequenceComplete;
        }

        public void Initialize()
        {
            if (_view != null)
                _view.Initialize();
        }

        public void Deinitialize()
        {
            Kill();

            if (_view != null)
                _view.Deinitialize();
        }

        public void ResetForNewRun()
        {
            Kill();

            if (_view != null)
                _view.ReleaseAll();
        }

        public void Kill()
        {
            _sequence?.Kill();
            _sequence = null;
            _pendingComplete = null;
            _pendingFirstFlight = null;
            _pendingWheelAmount = null;
        }

        /// <param name="origin">World position the icons pop out of — the resting winning slice.</param>
        /// <param name="onFirstFlight">Raised the moment the first icon leaves the wheel.</param>
        /// <param name="onWheelAmountChanged">
        /// Raised with the amount still owed by the slice each time an icon pops out of it.
        /// </param>
        public void Play(
            string itemId,
            int amount,
            Vector3 origin,
            Action onFirstFlight,
            Action<int> onWheelAmountChanged,
            Action onComplete)
        {
            Kill();

            RectTransform target = _rewards.BeginAdd(itemId);

            if (!CanPlay || target == null || amount <= 0)
            {
                _rewards.Tick(itemId, amount);
                onWheelAmountChanged?.Invoke(0);
                onFirstFlight?.Invoke();
                onComplete?.Invoke();
                return;
            }

            _itemId = itemId;
            _pendingFirstFlight = onFirstFlight;
            _pendingWheelAmount = onWheelAmountChanged;
            _pendingComplete = onComplete;
            _arrivedCount = 0;
            _spawnedCount = 0;
            _wheelRemaining = amount;
            _shareCount = Mathf.Clamp(_settings.IconCount, 1, amount);

            if (_shareArray.Length < _shareCount)
                _shareArray = new int[_shareCount];

            AmountSplit.Fill(_shareArray, _shareCount, amount);
            _spawner.Spawn(itemId, _shareCount, origin);

            _sequence = BuildSequence(_shareCount, target);
        }

        private Sequence BuildSequence(int count, RectTransform target)
        {
            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < count; i++)
                InsertSpawn(sequence, i * _settings.SpawnInterval, _spawner.Get(i));

            float flyStart = (count - 1) * _settings.SpawnInterval
                             + _settings.ScaleUpDuration
                             + _settings.HoldDuration;

            sequence.InsertCallback(flyStart, _onFirstFlight);

            for (int i = 0; i < count; i++)
                InsertFlight(sequence, flyStart + i * _settings.FlightInterval, _spawner.Get(i), target.position);

            return sequence
                .SetLink(_view.gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(_onSequenceComplete);
        }

        /// Scale-up and glide run together: the icon grows while drifting out to its scatter slot.
        private void InsertSpawn(Sequence sequence, float at, RewardFlightIconView icon)
        {
            // Drawn from the slice as the icon appears, not as it lands — the wheel is still on
            // screen during the spawn phase, and has already dropped away by the time icons arrive.
            sequence.InsertCallback(at, _onIconSpawned);

            sequence.Insert(at, icon.Rect.DOScale(1f, _settings.ScaleUpDuration)
                .SetEase(_settings.ScaleUpEase));

            sequence.Insert(at, icon.Rect.DOAnchorPos(icon.RestAnchored, _settings.ScaleUpDuration)
                .SetEase(_settings.SpawnDriftEase));
        }

        private void InsertFlight(Sequence sequence, float at, RewardFlightIconView icon, Vector3 target)
        {
            icon.BeginFlight(target, ArcHeight());

            sequence.Insert(at, DOTween.To(icon.ProgressGetter, icon.ProgressSetter, 1f, _settings.FlightDuration)
                .SetEase(_settings.FlightEase)
                .OnComplete(_onArrive));

            sequence.Insert(at, icon.Rect.DOScale(_settings.ArriveScale, _settings.FlightDuration)
                .SetEase(_settings.FlightEase));
        }

        private float ArcHeight()
            => _settings.ArcHeight * UnityEngine.Random.Range(1f - _settings.ArcJitter, 1f + _settings.ArcJitter);

        private void HandleFirstFlight()
        {
            Action callback = _pendingFirstFlight;
            _pendingFirstFlight = null;
            callback?.Invoke();
        }

        /// Counts the slice down by the share this icon carries, so the wheel drains to zero over
        /// the spawn phase while the board fills up later, on arrival.
        private void HandleIconSpawned()
        {
            if (_spawnedCount >= _shareCount)
                return;

            _wheelRemaining -= _shareArray[_spawnedCount++];
            _pendingWheelAmount?.Invoke(_wheelRemaining);

            if (_audio != null)
                _audio.PlayRewardAppear();
        }

        /// Every flight has the same duration and a staggered start, so arrivals keep spawn order —
        /// a running index pairs an arrival with its icon and share, with no per-icon closure.
        private void HandleArrive()
        {
            if (_arrivedCount >= _shareCount)
                return;

            int index = _arrivedCount++;

            _spawner.Release(index);
            _rewards.Tick(_itemId, _shareArray[index]);

            if (_audio != null)
                _audio.PlayRewardImpact();
        }

        private void HandleSequenceComplete()
        {
            _sequence = null;
            _view.ReleaseAll();

            Action callback = _pendingComplete;
            _pendingComplete = null;
            callback?.Invoke();
        }
    }
}
