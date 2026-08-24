using System.Collections.Generic;
using UnityEngine;

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
            _view = view;
            _registry = registry;
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

        /// <summary>
        /// Makes sure the board has a slot for the item, opening it at zero on first win so the
        /// count-up starts from x0. Returns the slot rect the flying icons should aim at.
        /// </summary>
        public RectTransform BeginAdd(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return null;

            if (_entries.TryGetValue(itemId, out Entry existing))
                return existing.View.Rect;

            RewardView view = _view.Acquire();

            if (_registry.TryGet(itemId, out ItemAsset item))
                view.Bind(item.Icon, 0, item.RewardSettings);
            else
                view.Bind(null, 0, ItemViewSettings.Default);

            _entries[itemId] = new Entry { View = view, Amount = 0 };

            // The slot was just activated; without this its rect is still at the old layout
            // position and the icons would fly to the wrong place.
            _view.RebuildLayout();

            return view.Rect;
        }

        /// <summary>Counts the slot up by one icon's share and replays the stack punch.</summary>
        public void Tick(string itemId, int amount)
        {
            if (string.IsNullOrWhiteSpace(itemId) || amount <= 0)
                return;

            if (!_entries.TryGetValue(itemId, out Entry entry))
                return;

            entry.Amount += amount;

            entry.View.SetAmount(entry.Amount);
            entry.View.PlayStackFeedback();

            _entries[itemId] = entry;
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
