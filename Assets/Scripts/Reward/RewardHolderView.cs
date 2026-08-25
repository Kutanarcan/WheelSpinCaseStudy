using DG.Tweening;
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

        [Header("Scroll")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _topAnchor;
        [SerializeField] private RectTransform _bottomAnchor;
        [SerializeField, Min(0f)] private float _scrollDuration = 0.2f;
        [SerializeField] private Ease _scrollEase = Ease.OutCubic;

        private ViewPool<RewardView> _pool;

        private ViewPool<RewardView> Pool
            => _pool ??= new ViewPool<RewardView>(_rewardViewPrefab, _rewardContentParent);

        private void OnValidate()
        {
            if (_scrollRect == null)
                _scrollRect = GetComponentInChildren<ScrollRect>(true);
        }

        public void Initialize() => Pool.Prewarm(_prewarmCount);

        public void Deinitialize() => Pool.DestroyCreated();

        public void ResetForNewRun() => Pool.ReleaseAll();

        public RewardView Acquire() => Pool.Acquire();

        public RewardScrollFocus CreateScrollFocus()
            => new RewardScrollFocus(_scrollRect, _topAnchor, _bottomAnchor, _scrollDuration, _scrollEase);

        public void SetCashOutButtonRootActive(bool active)
        {
            if (CashOutButtonViewRoot != null)
                CashOutButtonViewRoot.gameObject.SetActive(active);
        }

        public void RebuildLayout()
        {
            if (_rewardContentParent is RectTransform content)
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }
    }
}
