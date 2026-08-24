using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class WheelSpinAnimator
    {
        private readonly RectTransform _wheel;
        private readonly WheelSpinSettings _settings;
        private readonly int _sliceCount;
        private readonly TweenCallback _onTweenComplete;
        private readonly DOGetter<float> _travelledGetter;
        private readonly DOSetter<float> _travelledSetter;

        private float _currentAngle;
        private float _fromAngle;
        private float _travelled;
        private float _direction = -1f;

        private Tween _tween;
        private Action _onComplete;

        public bool IsPlaying => _tween != null;

        public WheelSpinAnimator(RectTransform wheel, WheelSpinSettings settings, int sliceCount)
        {
            _wheel = wheel;
            _settings = settings;
            _sliceCount = Mathf.Max(1, sliceCount);
            _onTweenComplete = HandleTweenComplete;
            _travelledGetter = GetTravelled;
            _travelledSetter = SetTravelled;

            _currentAngle = 0f;
            Apply(0f);
        }

        public float TargetAngle(int sliceIndex)
        {
            float step = 360f / _sliceCount;
            float sign = _settings.SliceOrderClockwise ? 1f : -1f;
            return Mathf.Repeat(_settings.IndicatorAngle + sign * step * sliceIndex, 360f);
        }

        public void Play(int sliceIndex, Action onComplete)
        {
            Kill();

            _fromAngle = _currentAngle;
            _direction = _settings.SpinClockwise ? -1f : 1f;
            _travelled = 0f;
            _onComplete = onComplete;

            float total = TravelDegrees(sliceIndex);
            float windup = ResolveBackswing(_settings.Windup, _settings.WindupDuration);
            float overshoot = ResolveBackswing(_settings.Overshoot, _settings.SettleDuration);

            Sequence sequence = DOTween.Sequence();

            // Negative travel is the same motion mirrored: the wheel is dragged against the spin
            // direction, so the launch and the settle are two ends of one mechanism.
            if (windup > 0f)
            {
                sequence.Append(DOTween.To(_travelledGetter, _travelledSetter, -windup, _settings.WindupDuration)
                    .SetEase(_settings.WindupEase));
            }

            sequence.Append(DOTween
                .To(_travelledGetter, _travelledSetter, total + overshoot, ResolveDuration(total + overshoot + windup))
                .SetEase(_settings.Ease));

            if (overshoot > 0f)
            {
                sequence.Append(DOTween.To(_travelledGetter, _travelledSetter, total, _settings.SettleDuration)
                    .SetEase(_settings.SettleEase));
            }

            _tween = sequence
                .SetLink(_wheel.gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(_onTweenComplete);
        }

        public void Snap(int sliceIndex) { Kill(); Apply(TargetAngle(sliceIndex)); }

        public void ResetAngle()
        {
            Kill();
            Apply(0f);
        }

        public void Kill()
        {
            _tween?.Kill();
            _tween = null;
            _onComplete = null;
        }

        private float TravelDegrees(int sliceIndex)
        {
            float to = TargetAngle(sliceIndex);

            float delta = _settings.SpinClockwise
                ? Mathf.Repeat(_fromAngle - to, 360f)
                : Mathf.Repeat(to - _fromAngle, 360f);

            int turns = UnityEngine.Random.Range(_settings.MinTurns, _settings.MaxTurns + 1);

            return delta + 360f * turns;
        }

        /// Capped at half a slice so neither end of the spin visibly reaches the neighbouring slice.
        private float ResolveBackswing(float degrees, float duration)
        {
            if (degrees <= 0f || duration <= 0f)
                return 0f;

            return Mathf.Min(degrees, 180f / _sliceCount);
        }

        private float ResolveDuration(float totalDegrees)
        {
            if (!_settings.PreventStroboscopicAliasing)
                return _settings.Duration;

            return SpinAliasing.SafeDuration(
                _settings.Duration, totalDegrees, _settings.Ease, _sliceCount, SpinAliasing.CurrentFrameRate());
        }

        private float GetTravelled() => _travelled;

        private void SetTravelled(float travelled)
        {
            _travelled = travelled;
            Apply(_fromAngle + _direction * travelled);
        }

        private void Apply(float angle)
        {
            _currentAngle = Mathf.Repeat(angle, 360f);
            _wheel.localEulerAngles = new Vector3(0f, 0f, _currentAngle);
        }

        private void HandleTweenComplete()
        {
            _tween = null;

            Action callback = _onComplete;
            _onComplete = null;
            callback?.Invoke();
        }
    }
}
