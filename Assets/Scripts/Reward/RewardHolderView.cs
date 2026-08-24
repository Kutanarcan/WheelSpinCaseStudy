using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public class RewardHolderView : MonoBehaviour
    {
        public ActionButtonView CashOutButtonView;
        public RectTransform CashOutButtonViewRoot;

        [SerializeField, Min(0)] public int _prewarmCount = 12;
        [SerializeField] private Transform _rewardContentParent;
        [SerializeField] private RewardView _rewardViewPrefab;

        private List<RewardView> _rewardViewList = new List<RewardView>();
        private int _createdFromIndex;
        private int _activeCount;

        public void Initialize()
        {
            for (int i = _rewardViewList.Count - 1; i >= 0; i--)
            {
                if (_rewardViewList[i] == null)
                    _rewardViewList.RemoveAt(i);
            }

            _createdFromIndex = _rewardViewList.Count;

            while (_rewardViewList.Count < _prewarmCount)
            {
                _rewardViewList.Add(Create());
            }

            DeactivateAll();
        }

        public void Deinitialize()
        {
            for (int i = _rewardViewList.Count - 1; i >= _createdFromIndex; i--)
            {
                if (_rewardViewList[i] != null) 
                    Destroy(_rewardViewList[i].gameObject);

                _rewardViewList.RemoveAt(i);
            }
             
            _activeCount = 0;
        }

        public void ResetForNewRun() => DeactivateAll();

        public void SetCashOutButtonRootActive(bool active)
        {
            if (CashOutButtonViewRoot != null)
                CashOutButtonViewRoot.gameObject.SetActive(active);
        }

        /// Forces the layout group to place a freshly activated slot now, so callers can read its
        /// world position in the same frame instead of one frame late.
        public void RebuildLayout()
        {
            if (_rewardContentParent is RectTransform content)
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        public RewardView Acquire()
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
            for (int i = 0; i < _rewardViewList.Count; i++)
            {
                _rewardViewList[i].gameObject.SetActive(false);
            }

            _activeCount = 0;
        }

        private RewardView Create()
        {
            RewardView view = Instantiate(_rewardViewPrefab, _rewardContentParent, false);

            view.gameObject.SetActive(false);

            return view;
        }
    }
}
