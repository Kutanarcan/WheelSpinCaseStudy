using DG.Tweening;
using System;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class PenaltyPresenter
    {
        private readonly WheelView _wheelView;
        private readonly BombExplosionView _explosion;
        private readonly ScreenShaker _shaker;
        private readonly AudioManager _audio;
        private readonly PenaltyEffectSettings _settings;

        private readonly TweenCallback _onDetonate;
        private readonly TweenCallback _onSequenceComplete;
        private readonly TweenCallback _onExplosionEnded;

        private Tween _cleanupTween;
        private Sequence _sequence;
        private Action _pendingComplete;
        private WheelSliceView _bombView;
        private RectTransform _bombRect;

        public bool CanPlay => _explosion != null && _explosion.IsReady;

        public PenaltyPresenter(
            WheelView wheelView,
            BombExplosionView explosion,
            RectTransform shakeRoot,
            AudioManager audio,
            PenaltyEffectSettings settings)
        {
            _wheelView = wheelView;
            _explosion = explosion;
            _shaker = new ScreenShaker(shakeRoot);
            _audio = audio;
            _settings = settings;

            _onDetonate = HandleDetonate;
            _onSequenceComplete = HandleSequenceComplete;
            _onExplosionEnded = HandleExplosionEnded;
        }

        public void Initialize()
        {
            if (_explosion != null)
                _explosion.Initialize();
        }

        public void Deinitialize() => ResetForNewRun();

        public void ResetForNewRun() => Kill();

        public void Kill()
        {
            _sequence?.Kill();
            _sequence = null;

            _cleanupTween?.Kill();
            _cleanupTween = null;

            _pendingComplete = null;

            if (_explosion != null)
                _explosion.Hide();

            _shaker.Stop();
            RestoreBomb();
        }

        public void Play(int sliceIndex, Action onComplete)
        {
            Kill();

            _bombView = ResolveBombView(sliceIndex);
            _bombRect = _bombView != null ? _bombView.IconRect : null;

            if (!CanPlay || _bombRect == null)
            {
                onComplete?.Invoke();
                return;
            }

            _pendingComplete = onComplete;

            Sequence sequence = DOTween.Sequence();

            AppendSwell(sequence);

            float detonateTime = _settings.GrowDuration * _settings.DetonateAt;

            sequence.InsertCallback(detonateTime, _onDetonate);

            float tail = detonateTime + _settings.PopupDelay - sequence.Duration();

            if (tail > 0f)
                sequence.AppendInterval(tail);

            _sequence = sequence
                .SetLink(_wheelView.gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(_onSequenceComplete);
        }

        private void AppendSwell(Sequence sequence)
        {
            if (_settings.GrowDuration <= 0f)
                return;

            sequence.Append(_bombRect.DOScale(_settings.GrowScale, _settings.GrowDuration)
                .SetEase(_settings.GrowEase));

            if (_settings.ShakeRotation <= 0f)
                return;

            sequence.Join(_bombRect.DOPunchRotation(
                new Vector3(0f, 0f, _settings.ShakeRotation),
                _settings.GrowDuration,
                _settings.ShakeVibrato,
                _settings.ShakeElasticity));
        }

        private void HandleDetonate()
        {
            _explosion.PlayAt(_bombRect.position);

            if (_audio != null)
                _audio.PlayExplosion();

            if (_settings.HideBombOnExplode)
                _bombView.SetIconVisible(false);

            _cleanupTween?.Kill();
            _cleanupTween = DOVirtual
                .DelayedCall(_settings.ExplosionDuration, _onExplosionEnded)
                .SetLink(_wheelView.gameObject, LinkBehaviour.KillOnDestroy);

            _shaker.Play(
                _settings.ScreenShakeDuration,
                _settings.ScreenShakeStrength,
                _settings.ScreenShakeVibrato,
                _settings.ScreenShakeRandomness);
        }

        private void HandleExplosionEnded()
        {
            _cleanupTween = null;
            _explosion.Hide();
        }

        private void HandleSequenceComplete()
        {
            _sequence = null;

            RestoreBomb();

            Action callback = _pendingComplete;
            _pendingComplete = null;
            callback?.Invoke();
        }

        private void RestoreBomb()
        {
            if (_bombRect != null)
            {
                _bombRect.localScale = Vector3.one;
                _bombRect.localEulerAngles = Vector3.zero;
            }

            if (_bombView != null)
                _bombView.SetIconVisible(true);

            _bombView = null;
            _bombRect = null;
        }

        private WheelSliceView ResolveBombView(int sliceIndex)
        {
            WheelSliceView[] views = _wheelView != null ? _wheelView.SliceViewArray : null;

            if (views == null || sliceIndex < 0 || sliceIndex >= views.Length)
                return null;

            return views[sliceIndex];
        }
    }
}
