using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class RewardFlightView : MonoBehaviour
    {
        [SerializeField] private RewardFlightIconView _iconPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField, Min(0)] private int _prewarmCount = 8;

        private ViewPool<RewardFlightIconView> _pool;

        private ViewPool<RewardFlightIconView> Pool
            => _pool ??= new ViewPool<RewardFlightIconView>(_iconPrefab, _content);

        public bool IsReady => _iconPrefab != null && _content != null;

        public void Initialize() => Pool.Prewarm(_prewarmCount);

        public void Deinitialize() => Pool.DestroyCreated();

        public RewardFlightIconView Acquire() => Pool.Acquire();

        public void Release(RewardFlightIconView icon) => Pool.Release(icon);

        public void ReleaseAll() => Pool.ReleaseAll();

        private void OnValidate()
        {
            if (_content == null)
                _content = transform as RectTransform;
        }
    }
}
