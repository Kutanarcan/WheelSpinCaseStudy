using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public class ZoneCountView : MonoBehaviour
    {
        [SerializeField] private ZoneNumberView _zoneNumberViewPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private ScrollRect _scrollRect;

        private ViewPool<ZoneNumberView> _pool;

        private ViewPool<ZoneNumberView> Pool
            => _pool ??= new ViewPool<ZoneNumberView>(_zoneNumberViewPrefab, _content);

        public void Initialize(int zoneCount)
        {
            NeutralizeScrollRect();

            Pool.Prewarm(zoneCount);

            for (int i = 0; i < zoneCount; i++)
            {
                ZoneNumberView view = Pool.Acquire();

                if (view == null)
                    break;

                view.SetNumber(i + 1);
            }

            if (_content != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        }

        public void Deinitialize() => Pool.DestroyCreated();

        public ZoneNumberView Get(int zoneNumber) => Pool.Get(zoneNumber - 1);

        public float GetContentX() => _content != null ? _content.anchoredPosition.x : 0f;

        public void SetContentX(float x)
        {
            if (_content == null)
                return;

            Vector2 position = _content.anchoredPosition;
            position.x = x;
            _content.anchoredPosition = position;
        }

        public float GetCenteredContentX(int zoneNumber)
        {
            if (_content == null || _viewport == null)
                return GetContentX();

            ZoneNumberView view = Get(zoneNumber);

            if (view == null || view.Rect == null)
                return GetContentX();

            RectTransform item = view.Rect;

            Vector3 itemWorld = item.TransformPoint(item.rect.center);
            Vector3 itemInViewport = _viewport.InverseTransformPoint(itemWorld);

            float delta = _viewport.rect.center.x - itemInViewport.x;
            return _content.anchoredPosition.x + delta;
        }

        private void NeutralizeScrollRect()
        {
            if (_scrollRect == null)
                return;

            _scrollRect.horizontal = false;
            _scrollRect.vertical = false;
            _scrollRect.inertia = false;
            _scrollRect.velocity = Vector2.zero;
        }

        private void OnValidate()
        {
            if (_scrollRect == null)
                _scrollRect = GetComponentInChildren<ScrollRect>(true);

            if (_scrollRect == null)
                return;

            if (_viewport == null)
                _viewport = _scrollRect.viewport;

            if (_content == null)
                _content = _scrollRect.content;
        }
    }
}
