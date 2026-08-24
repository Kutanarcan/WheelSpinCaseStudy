using DG.Tweening;
using DG.Tweening.Core;
using System;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class ZonePresenter
    {
        private readonly ZoneCountView _countView;
        private readonly RectTransform _selectorRect;
        private readonly WheelTierViewDatabase _tierViewDatabase;
        private readonly Color _currentZoneColor;
        private readonly WheelSpinSettings _settings;
        private readonly WheelTierRuleProvider _tierRules;

        private readonly DOGetter<float> _progressGetter;
        private readonly DOSetter<float> _progressSetter;
        private readonly TweenCallback _onTransitionComplete;

        private int _zoneCount;
        private int _targetZoneNumber;
        private Action _pendingCallback;

        private Tween _tween;
        private float _progress;
        private float _fromContentX;
        private float _toContentX;
        private Vector3 _selectorFromPosition;

        public bool IsMoving => _tween != null;

        public ZonePresenter(
            ZoneCountView countView,
            ZoneSelectorView selectorView,
            WheelTierViewDatabase tierViewDatabase,
            Color currentZoneColor,
            WheelSpinSettings settings,
            WheelTierRuleProvider tierRules)
        {
            _countView = countView;
            _tierViewDatabase = tierViewDatabase;
            _currentZoneColor = currentZoneColor;
            _settings = settings;
            _tierRules = tierRules;

            _selectorRect = selectorView != null ? selectorView.transform as RectTransform : null;

            _progressGetter = GetProgress;
            _progressSetter = SetProgress;
            _onTransitionComplete = HandleTransitionComplete;
        }

        public void Initialize(int zoneCount)
        {
            _zoneCount = zoneCount;
            _countView.Initialize(zoneCount);
        }

        public void Deinitialize()
        {
            KillTweens();
            _countView.Deinitialize();
        }

        public void ResetForNewRun() => KillTweens();

        public void Show(int zoneNumber, bool instant, Action onComplete)
        {
            RefreshColors(zoneNumber);
            KillTweens();

            _targetZoneNumber = zoneNumber;
            _pendingCallback = onComplete;

            if (instant || _settings.ScrollDuration <= 0f)
            {
                Snap(zoneNumber);
                CompletePending();
                return;
            }

            PlayTransition();
        }

        /// <summary>
        /// Scrolls the strip and walks the selector over it on one clock. The selector cannot use a
        /// tween of its own: its destination is a number that the scroll is moving at the same time,
        /// so both are driven from a single progress value, each through its own ease.
        /// </summary>
        private void PlayTransition()
        {
            _fromContentX = _countView.GetContentX();
            _toContentX = _countView.GetCenteredContentX(_targetZoneNumber);
            _selectorFromPosition = _selectorRect != null ? _selectorRect.position : Vector3.zero;
            _progress = 0f;

            _tween = DOTween
                .To(_progressGetter, _progressSetter, 1f, _settings.ScrollDuration)
                .SetEase(Ease.Linear)
                .SetLink(_countView.gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(_onTransitionComplete);
        }

        private float GetProgress() => _progress;

        private void SetProgress(float progress)
        {
            _progress = progress;

            float scrollT = DOVirtual.EasedValue(0f, 1f, progress, _settings.ScrollEase);
            _countView.SetContentX(Mathf.LerpUnclamped(_fromContentX, _toContentX, scrollT));

            // Read the number's position only after the strip has moved this frame, so the selector
            // chases where the number actually is rather than where it started.
            RectTransform target = GetNumberRect(_targetZoneNumber);

            if (_selectorRect == null || target == null)
                return;

            float selectorT = DOVirtual.EasedValue(0f, 1f, progress, _settings.SelectorStepEase);

            // Unclamped so an overshooting ease (OutBack) can carry the selector past the number
            // and back instead of being flattened at the ends.
            _selectorRect.position = Vector3.LerpUnclamped(_selectorFromPosition, target.position, selectorT);
        }

        private void HandleTransitionComplete()
        {
            _tween = null;

            Snap(_targetZoneNumber);
            CompletePending();
        }

        private void RefreshColors(int currentZone)
        {
            for (int zoneNumber = 1; zoneNumber <= _zoneCount; zoneNumber++)
            {
                ZoneNumberView view = _countView.Get(zoneNumber);
                if (view == null) continue;

                view.SetColor(ColorFor(zoneNumber, currentZone));
            }
        }

        private Color ColorFor(int zoneNumber, int currentZone)
        {
            if (zoneNumber == currentZone)
                return _currentZoneColor;

            WheelTier tier = _tierRules.TierFor(zoneNumber);

            return _tierViewDatabase.GetPack(tier).ZoneNumberColor;
        }

        private void Snap(int zoneNumber)
        {
            _countView.SetContentX(_countView.GetCenteredContentX(zoneNumber));
            PlaceSelectorOn(zoneNumber);
        }

        private void PlaceSelectorOn(int zoneNumber)
        {
            RectTransform target = GetNumberRect(zoneNumber);
            if (_selectorRect == null || target == null) return;

            _selectorRect.position = target.position;
        }

        private RectTransform GetNumberRect(int zoneNumber)
        {
            ZoneNumberView view = _countView.Get(zoneNumber);
            return view != null ? view.Rect : null;
        }

        private void CompletePending()
        {
            Action callback = _pendingCallback;
            _pendingCallback = null;
            callback?.Invoke();
        }

        private void KillTweens()
        {
            _tween?.Kill();
            _tween = null;

            _pendingCallback = null;
        }
    }
}
