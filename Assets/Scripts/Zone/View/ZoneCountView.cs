using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public class ZoneCountView : MonoBehaviour
    {
        [SerializeField] private ZoneNumberView _zoneNumberViewPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private ScrollRect _scrollRect;

        private List<ZoneNumberView> _zoneNumberViewList = new List<ZoneNumberView>();
        private int _createdFromIndex;
        public int Count => _zoneNumberViewList.Count;

        public void Initialize(int zoneCount)
        {
            NeutralizeScrollRect();

            for (int i = _zoneNumberViewList.Count - 1; i >= 0; i--)
            {
                if (_zoneNumberViewList[i] == null) 
                    _zoneNumberViewList.RemoveAt(i);
            }

            _createdFromIndex = _zoneNumberViewList.Count;

            while (_zoneNumberViewList.Count < zoneCount)
            {
                _zoneNumberViewList.Add(Instantiate(_zoneNumberViewPrefab, _content));
            }

            for (int i = 0; i < _zoneNumberViewList.Count; i++)
            {
                bool used = i < zoneCount;
                _zoneNumberViewList[i].gameObject.SetActive(used);

                if (used) _zoneNumberViewList[i].SetNumber(i + 1);
            }

            if (_content != null) 
                LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        }

        public void Deinitialize()
        {
            for (int i = _zoneNumberViewList.Count - 1; i >= _createdFromIndex; i--)
            {
                if (_zoneNumberViewList[i] != null)
                    Destroy(_zoneNumberViewList[i].gameObject);

                _zoneNumberViewList.RemoveAt(i);
            }
        }

        public ZoneNumberView Get(int zoneNumber)
        {
            int i = zoneNumber - 1;
            return i >= 0 && i < _zoneNumberViewList.Count ? _zoneNumberViewList[i] : null;
        }

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