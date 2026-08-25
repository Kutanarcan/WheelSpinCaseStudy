using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class CashoutPopup : PopupView
    {
        public RewardView RewardViewPrefab;
        public ActionButtonView ClaimButton;
        public Transform Content;

        private ViewPool<RewardView> _pool;

        private ViewPool<RewardView> Pool
            => _pool ??= new ViewPool<RewardView>(RewardViewPrefab, Content);

        public void Bind(IReadOnlyList<RewardLedger.Entry> entries, ItemRegistry registry)
        {
            Pool.ReleaseAll();

            if (entries == null)
                return;

            for (int i = 0; i < entries.Count; i++)
            {
                RewardLedger.Entry entry = entries[i];
                RewardView view = Pool.Acquire();

                if (view == null)
                    return;

                if (registry != null && registry.TryGet(entry.ItemId, out ItemAsset item))
                    view.Bind(item.Icon, entry.Amount, item.CashRewardSettings);
                else
                    view.Bind(null, entry.Amount, ItemViewSettings.Default);
            }
        }

        public void Clear() => Pool.ReleaseAll();
    }
}
