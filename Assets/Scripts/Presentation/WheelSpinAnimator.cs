using System;
using DG.Tweening;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class WheelSpinAnimator
    {
        private readonly RectTransform _wheel;
        private readonly WheelSpinSettings _settings;
        private readonly int _sliceCount;
        private readonly TweenCallback _onTweenComplete;
        private float _currentAngle;
        private float _fromAngle;
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
            float to = TargetAngle(sliceIndex);

            _direction = _settings.SpinClockwise ? -1f : 1f;

            float delta = _settings.SpinClockwise
                ? Mathf.Repeat(_fromAngle - to, 360f)
                : Mathf.Repeat(to - _fromAngle, 360f);

            int turns = UnityEngine.Random.Range(_settings.MinTurns, _settings.MaxTurns + 1);
            float total = delta + 360f * turns;

            float duration = _settings.Duration;

            if (_settings.PreventStroboscopicAliasing)
            {
                duration = SpinAliasing.SafeDuration(
                    duration, total, _settings.Ease, _sliceCount, SpinAliasing.CurrentFrameRate());
            }

            _onComplete = onComplete;

            _tween = DOTween.To(() => 0f, Step, total, duration)
                .SetEase(_settings.Ease)
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

        private void Step(float travelled) => Apply(_fromAngle + _direction * travelled);

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
