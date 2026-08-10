using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class CashoutPopup : MonoBehaviour
    {
        public RewardView RewardViewPrefab;
        public ActionButtonView ClaimButton;
        public Transform Content;

        public Transform PanelRoot;

        private readonly List<RewardView> _rewardViewList = new List<RewardView>();
        private int _activeCount;

        public void Show()
        {
            PanelRoot.gameObject.SetActive(true);
        }

        public void Hide()
        {
            PanelRoot.gameObject.SetActive(false);
        }

        public void Bind(IReadOnlyList<RewardLedger.Entry> entries, ItemRegistry registry)
        {
            DeactivateAll();

            if (entries == null)
                return;

            for (int i = 0; i < entries.Count; i++)
            {
                RewardLedger.Entry entry = entries[i];
                RewardView view = Acquire();

                if (registry != null && registry.TryGet(entry.ItemId, out ItemAsset item))
                    view.Bind(item.Icon, entry.Amount, item.RewardSettings);
                else
                    view.Bind(null, entry.Amount, ItemViewSettings.Default);
            }
        }

        public void Clear() => DeactivateAll();

        private RewardView Acquire()
        {
            if (_activeCount == _rewardViewList.Count)
                _rewardViewList.Add(Create());

            RewardView view = _rewardViewList[_activeCount];

            _activeCount++;

            view.transform.SetAsLastSibling();
            view.gameObject.SetActive(true);

            return view;
        }

        private void DeactivateAll()
        {
            for (int i = _rewardViewList.Count - 1; i >= 0; i--)
            {
                if (_rewardViewList[i] == null)
                {
                    _rewardViewList.RemoveAt(i);
                    continue;
                }

                _rewardViewList[i].gameObject.SetActive(false);
            }

            _activeCount = 0;
        }

        private RewardView Create()
        {
            RewardView view = Instantiate(RewardViewPrefab, Content, false);

            view.gameObject.SetActive(false);

            return view;
        }
    }
}
