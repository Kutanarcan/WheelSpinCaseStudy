using System;

namespace CaseStudy.WheelSpin
{
    public class RewardPresenter
    {
        private readonly RewardHolderView _view;
        private readonly ItemRegistry _registry;

        public RewardPresenter(RewardHolderView view, ItemRegistry registry)
        {
            _view = view != null ? view : throw new ArgumentNullException(nameof(view));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public void Initialize() => _view.Initialize();

        public void Deinitialize() => _view.Deinitialize();

        public void ResetForNewRun() => _view.ResetForNewRun();

        public void Add(string itemId, int amount)
        {
            RewardView view = _view.Acquire();

            if (_registry.TryGet(itemId, out ItemAsset item))
                view.Bind(item.Icon, amount, item.RewardSettings);
            else
                view.Bind(null, amount, ItemViewSettings.Default);
        }

        public void Clear() => _view.ResetForNewRun();
    }
}