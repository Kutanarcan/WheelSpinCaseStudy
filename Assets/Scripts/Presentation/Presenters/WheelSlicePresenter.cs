using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class WheelSlicePresenter
    {
        private readonly WheelView _view;
        private readonly ItemRegistry _registry;
        private readonly Sprite _penaltySprite;
        private readonly ItemViewSettings _penaltySettings;
        private readonly WheelSpinSettings _settings;
        private readonly WheelSpinAnimator _animator;
        private readonly WheelTierViewDatabase _wheelTierViewDatabase;
        private readonly RectTransform _holder;  
        private readonly float _restAnchoredY;

        private readonly TweenCallback _onSwapPoint;
        private readonly TweenCallback _onZoneChangeComplete;

        private Sequence _sequence;
        private Zone _pendingZone;
        private Action _pendingCallback;

        public bool IsSpinning => _animator.IsPlaying;
        public bool IsChangingZone => _sequence != null;

        public WheelSlicePresenter(
            WheelView view,
            ItemRegistry registry,
            Sprite penaltySprite,
            ItemViewSettings penaltySettings,
            WheelSpinSettings settings,
            WheelTierViewDatabase wheelTierViewDatabase)
        {
            _view = view;
            _registry = registry;
            _penaltySprite = penaltySprite;
            _penaltySettings = penaltySettings;
            _settings = settings;
            _wheelTierViewDatabase = wheelTierViewDatabase;

            // Dilim sayisinin tek kaynagi sahnedeki view dizisi.
            _animator = new WheelSpinAnimator(view.WheelRect, settings, view.SliceViewArray.Length);

            _holder = view.HolderRect != null && view.HolderRect != view.WheelRect
                ? view.HolderRect
                : null;

            _restAnchoredY = _holder != null ? _holder.anchoredPosition.y : 0f;

            _onSwapPoint = HandleSwapPoint;
            _onZoneChangeComplete = HandleZoneChangeComplete;
        }

        public void ResetForNewRun()
        {
            _animator.ResetAngle();
            KillSequence();
            SnapHolderToRest();
        }

        public void Deinitialize()
        {
            _animator.Kill();
            KillSequence();
        }

        public void Bind(Zone zone)
        {
            IReadOnlyList<WheelSliceView> views = _view.SliceViewArray;
            int count = Mathf.Min(views.Count, zone.Wheel.Length);

            for (int i = 0; i < count; i++)
            {
                WheelSlice slice = zone.Wheel[i];

                if (slice.Type == SliceType.Penalty)
                {
                    views[i].BindPenalty(_penaltySprite, _penaltySettings, disabled: slice.IsDisabled);
                    continue;
                }

                if (_registry.TryGet(slice.ItemId, out ItemAsset item))
                {
                    views[i].Bind(item.Icon, slice.Amount, item.WheelSettings);
                }
            }

            var spritePack = _wheelTierViewDatabase.GetSprite(zone.Tier);
            
            _view.WheelImage.sprite = spritePack.Wheel;
            _view.SpinIndicatorImage.sprite = spritePack.WheelIndicator;
        }

        public void PlaySpin(int sliceIndex, Action onComplete) => _animator.Play(sliceIndex, onComplete);

        public void PlayZoneChange(Zone nextZone, Action onComplete)
        {
            KillSequence();

            if (nextZone == null)
            {
                onComplete?.Invoke();
                return;
            }

            _pendingZone = nextZone;
            _pendingCallback = onComplete;

            bool canAnimate = _holder != null
                              && _settings.DropDuration > 0f
                              && _settings.RiseDuration > 0f;

            if (!canAnimate)
            {
                HandleSwapPoint();
                HandleZoneChangeComplete();
                return;
            }

            float downY = _restAnchoredY - _settings.DropDistance;

            _sequence = DOTween.Sequence()
                .Append(_holder.DOAnchorPosY(downY, _settings.DropDuration)
                    .SetEase(_settings.DropEase))
                .Join(_holder.DOLocalRotate(new Vector3(0f, 0f, _settings.DropTilt), _settings.DropDuration)
                    .SetEase(_settings.DropEase))
                .AppendCallback(_onSwapPoint)
                .Append(_holder.DOAnchorPosY(_restAnchoredY, _settings.RiseDuration)
                    .SetEase(_settings.RiseEase))
                .Join(_holder.DOLocalRotate(Vector3.zero, _settings.RiseDuration)
                    .SetEase(_settings.RiseEase))
                .SetLink(_holder.gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(_onZoneChangeComplete);
        }

        private void HandleSwapPoint()
        {
            if (_pendingZone != null)
                Bind(_pendingZone);

            if (_settings.ResetAngleOnZoneChange)
                _animator.ResetAngle();
        }

        private void HandleZoneChangeComplete()
        {
            _sequence = null;
            _pendingZone = null;

            SnapHolderToRest();

            Action callback = _pendingCallback;
            _pendingCallback = null;
            callback?.Invoke();
        }

        private void KillSequence()
        {
            _sequence?.Kill();
            _sequence = null;
            _pendingZone = null;
            _pendingCallback = null;
        }

        private void SnapHolderToRest()
        {
            if (_holder == null)
                return;

            Vector2 position = _holder.anchoredPosition;
            position.y = _restAnchoredY;
            _holder.anchoredPosition = position;

            _holder.localEulerAngles = Vector3.zero;
        }
    }
}