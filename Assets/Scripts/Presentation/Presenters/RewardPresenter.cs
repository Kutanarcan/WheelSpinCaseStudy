using System;
using System.Collections.Generic;

namespace CaseStudy.WheelSpin
{
    public class RewardPresenter
    {
        private readonly RewardHolderView _view;
        private readonly ItemRegistry _registry;
        private readonly Dictionary<string, Entry> _entries = new Dictionary<string, Entry>();

        private struct Entry
        {
            public RewardView View;
            public int Amount;
        }

        public RewardPresenter(RewardHolderView view, ItemRegistry registry)
        {
            _view = view != null ? view : throw new ArgumentNullException(nameof(view));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public void Initialize()
        {
            _entries.Clear();
            _view.Initialize();
        }

        public void Deinitialize()
        {
            _entries.Clear();
            _view.Deinitialize();
        }

        public void ResetForNewRun()
        {
            _entries.Clear();
            _view.ResetForNewRun();
        }

        public void Clear() => ResetForNewRun();

        public void Add(string itemId, int amount)
        {
            if (string.IsNullOrWhiteSpace(itemId) || amount <= 0)
                return;

            if (_entries.TryGetValue(itemId, out Entry entry))
            {
                entry.Amount += amount;

                entry.View.SetAmount(entry.Amount);
                entry.View.PlayStackFeedback();

                _entries[itemId] = entry;   
                return;
            }

            RewardView view = _view.Acquire();

            if (_registry.TryGet(itemId, out ItemAsset item))
                view.Bind(item.Icon, amount, item.RewardSettings);
            else
                view.Bind(null, amount, ItemViewSettings.Default);

            _entries[itemId] = new Entry { View = view, Amount = amount };
        }

        public bool TryGetAmount(string itemId, out long amount)
        {
            if (!string.IsNullOrWhiteSpace(itemId) && _entries.TryGetValue(itemId, out Entry entry))
            {
                amount = entry.Amount;
                return true;
            }

            amount = 0;
            return false;
        }
    }
}