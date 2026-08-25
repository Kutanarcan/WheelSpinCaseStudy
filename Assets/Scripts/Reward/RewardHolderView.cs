using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public class RewardHolderView : MonoBehaviour
    {
        public ActionButtonView CashOutButtonView;
        public RectTransform CashOutButtonViewRoot;

        [SerializeField, Min(0)] private int _prewarmCount = 12;
        [SerializeField] private Transform _rewardContentParent;
        [SerializeField] private RewardView _rewardViewPrefab;

        private ViewPool<RewardView> _pool;

        private ViewPool<RewardView> Pool
            => _pool ??= new ViewPool<RewardView>(_rewardViewPrefab, _rewardContentParent);

        public void Initialize() => Pool.Prewarm(_prewarmCount);

        public void Deinitialize() => Pool.DestroyCreated();

        public void ResetForNewRun() => Pool.ReleaseAll();

        public RewardView Acquire() => Pool.Acquire();

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
    }
}
