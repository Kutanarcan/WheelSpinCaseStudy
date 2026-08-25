using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class RewardPresenter
    {
        private readonly RewardHolderView _view;
        private readonly ItemRegistry _registry;
        private readonly Dictionary<string, Entry> _entries = new Dictionary<string, Entry>();

        private RewardScrollFocus _scrollFocus;

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

        private bool HasView => _view != null;

        public void Initialize()
        {
            _entries.Clear();

            if (!HasView)
                return;

            _view.Initialize();
            _scrollFocus = _view.CreateScrollFocus();
            SetCashOutActive(true);
        }

        public void Deinitialize()
        {
            _entries.Clear();
            _scrollFocus?.Kill();
            _scrollFocus = null;

            if (HasView)
                _view.Deinitialize();
        }

        public void ResetForNewRun()
        {
            _entries.Clear();

            if (!HasView)
                return;

            _view.ResetForNewRun();
            _scrollFocus?.Reset();
            SetCashOutActive(true);
        }

        public void SetCashOutActive(bool active)
        {
            if (HasView)
                _view.SetCashOutButtonRootActive(active);
        }

        public RectTransform BeginAdd(string itemId)
        {
            if (!HasView || string.IsNullOrWhiteSpace(itemId))
                return null;

            if (_entries.TryGetValue(itemId, out Entry existing))
            {
                _scrollFocus?.Focus(existing.View.Rect);
                return existing.View.Rect;
            }

            RewardView view = _view.Acquire();

            if (view == null)
                return null;

            if (_registry.TryGet(itemId, out ItemAsset item))
                view.Bind(item.Icon, 0, item.RewardSettings);
            else
                view.Bind(null, 0, ItemViewSettings.Default);

            _entries[itemId] = new Entry { View = view, Amount = 0 };

            _view.RebuildLayout();
            _scrollFocus?.Focus(view.Rect);

            return view.Rect;
        }

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
    }
}
