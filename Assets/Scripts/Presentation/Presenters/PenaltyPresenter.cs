using DG.Tweening;
using System;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Plays the loss effect: the bomb that came up swells on the wheel, then detonates through the
    /// one explosion instance kept under the wheel. Runs to completion before the revive popup opens.
    /// </summary>
    public class PenaltyPresenter
    {
        private readonly WheelView _wheelView;
        private readonly BombExplosionView _explosion;
        private readonly PenaltyEffectSettings _settings;

        private readonly TweenCallback _onDetonate;
        private readonly TweenCallback _onSequenceComplete;

        private Sequence _sequence;
        private Action _pendingComplete;
        private RectTransform _bombRect;

        public bool IsPlaying => _sequence != null;

        public bool CanPlay => _explosion != null && _explosion.IsReady;

        public PenaltyPresenter(
            WheelView wheelView, BombExplosionView explosion, PenaltyEffectSettings settings)
        {
            _wheelView = wheelView;
            _explosion = explosion;
            _settings = settings;

            _onDetonate = HandleDetonate;
            _onSequenceComplete = HandleSequenceComplete;
        }

        public void Initialize()
        {
            if (_explosion != null)
                _explosion.Initialize();
        }

        public void Deinitialize() => ResetForNewRun();

        public void ResetForNewRun()
        {
            Kill();

            if (_explosion != null)
                _explosion.Hide();
        }

        public void Kill()
        {
            _sequence?.Kill();
            _sequence = null;
            _pendingComplete = null;

            RestoreBomb();
        }

        public void Play(int sliceIndex, Action onComplete)
        {
            Kill();

            _bombRect = ResolveBombRect(sliceIndex);

            if (!CanPlay || _bombRect == null)
            {
                onComplete?.Invoke();
                return;
            }

            _pendingComplete = onComplete;

            _sequence = DOTween.Sequence()
                .Append(_bombRect.DOScale(_settings.GrowScale, _settings.GrowDuration)
                    .SetEase(_settings.GrowEase))
                .AppendCallback(_onDetonate)
                .AppendInterval(_settings.ExplosionDuration)
                .SetLink(_wheelView.gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(_onSequenceComplete);
        }

        private void HandleDetonate()
        {
            _explosion.PlayAt(_bombRect.position);

            if (_settings.HideBombOnExplode)
                _bombRect.localScale = Vector3.zero;
        }

        private void HandleSequenceComplete()
        {
            _sequence = null;

            _explosion.Hide();
            RestoreBomb();

            Action callback = _pendingComplete;
            _pendingComplete = null;
            callback?.Invoke();
        }

        /// The slice views are reused across zones, so the swell has to be undone whatever path the
        /// sequence leaves by — otherwise the next zone inherits a giant icon.
        private void RestoreBomb()
        {
            if (_bombRect != null)
                _bombRect.localScale = Vector3.one;

            _bombRect = null;
        }

        private RectTransform ResolveBombRect(int sliceIndex)
        {
            WheelSliceView[] views = _wheelView != null ? _wheelView.SliceViewArray : null;

            if (views == null || sliceIndex < 0 || sliceIndex >= views.Length || views[sliceIndex] == null)
                return null;

            return views[sliceIndex].IconRect;
        }
    }
}
