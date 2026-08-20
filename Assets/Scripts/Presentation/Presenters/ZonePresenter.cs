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

        private readonly DOGetter<float> _contentXGetter;
        private readonly DOSetter<float> _contentXSetter;
        private readonly TweenCallback _onStepComplete;
        private readonly TweenCallback _onScrollUpdate;
        private readonly TweenCallback _onScrollComplete;

        private int _zoneCount;
        private int _targetZoneNumber;
        private Action _pendingCallback;

        private Tween _stepTween;
        private Tween _scrollTween;

        public bool IsMoving => _stepTween != null || _scrollTween != null;

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

            _contentXGetter = _countView.GetContentX;
            _contentXSetter = _countView.SetContentX;
            _onStepComplete = HandleStepComplete;
            _onScrollUpdate = HandleScrollUpdate;
            _onScrollComplete = HandleScrollComplete;
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

            if (instant)
            {
                Snap(zoneNumber);
                CompletePending();
                return;
            }

            StepSelector(zoneNumber);
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

        private void StepSelector(int zoneNumber)
        {
            RectTransform target = GetNumberRect(zoneNumber);
            float duration = _settings.SelectorStepDuration;

            if (_selectorRect == null || target == null || duration <= 0f)
            {
                PlaceSelectorOn(zoneNumber);
                ScrollToTarget();
                return;
            }

            _stepTween = _selectorRect
                .DOMove(target.position, duration)
                .SetEase(_settings.SelectorStepEase)
                .SetLink(_selectorRect.gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(_onStepComplete);
        }

        private void HandleStepComplete()
        {
            _stepTween = null;
            ScrollToTarget();
        }


        private void ScrollToTarget()
        {
            float targetX = _countView.GetCenteredContentX(_targetZoneNumber);
            float duration = _settings.ScrollDuration;

            if (duration <= 0f)
            {
                _countView.SetContentX(targetX);
                PlaceSelectorOn(_targetZoneNumber);
                CompletePending();
                return;
            }

            _scrollTween = DOTween
                .To(_contentXGetter, _contentXSetter, targetX, duration)
                .SetEase(_settings.ScrollEase)
                .SetLink(_countView.gameObject, LinkBehaviour.KillOnDestroy)
                .OnUpdate(_onScrollUpdate)
                .OnComplete(_onScrollComplete);
        }

        private void HandleScrollUpdate() => PlaceSelectorOn(_targetZoneNumber);

        private void HandleScrollComplete()
        {
            _scrollTween = null;
            PlaceSelectorOn(_targetZoneNumber);
            CompletePending();
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
            _stepTween?.Kill();
            _stepTween = null;

            _scrollTween?.Kill();
            _scrollTween = null;

            _pendingCallback = null;
        }
    }
}