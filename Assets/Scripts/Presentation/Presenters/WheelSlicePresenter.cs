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
        private readonly WheelSpinAnimator _animator;

        public bool IsSpinning => _animator.IsPlaying;

        public WheelSlicePresenter(
            WheelView view,
            ItemRegistry registry,
            Sprite penaltySprite,
            ItemViewSettings penaltySettings,
            WheelSpinSettings settings,
            int sliceCount)
        {
            _view = view;
            _registry = registry;
            _penaltySprite = penaltySprite;
            _penaltySettings = penaltySettings;

            _animator = new WheelSpinAnimator(view.WheelRect, settings, sliceCount);
        }

        public void ResetForNewRun() => _animator.Kill();

        public void Bind(Zone zone)
        {
            IReadOnlyList<WheelSliceView> views = _view.SliceViewArray;
            int count = Mathf.Min(views.Count, zone.Wheel.Length);

            for (int i = 0; i < count; i++)
            {
                WheelSlice slice = zone.Wheel[i];

                if (slice.Type == SliceType.Penalty)
                {
                    views[i].BindPenalty(_penaltySprite, _penaltySettings);
                    continue;
                }

                if (_registry.TryGet(slice.ItemId, out ItemAsset item))
                {
                    views[i].Bind(item.Icon, slice.Amount, item.WheelSettings);
                }
            }
        }

        public void PlaySpin(int sliceIndex, Action onComplete) => _animator.Play(sliceIndex, onComplete);

        public void Deinitialize() => _animator.Kill();

    }
}
