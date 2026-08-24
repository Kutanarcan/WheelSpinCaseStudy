using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Pool and spawn surface for the reward icons that fly from the wheel to the reward board.
    /// Icons are acquired for one flight and released together when that flight ends, so the pool
    /// settles at the largest icon count a single win ever used.
    /// </summary>
    public class RewardFlightView : MonoBehaviour
    {
        [SerializeField] private RewardFlightIconView _iconPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField, Min(0)] private int _prewarmCount = 8;

        private readonly List<RewardFlightIconView> _iconList = new List<RewardFlightIconView>();
        private int _activeCount;

        public bool IsReady => _iconPrefab != null && _content != null;

        public void Initialize()
        {
            if (!IsReady)
                return;

            while (_iconList.Count < _prewarmCount)
                _iconList.Add(Create());

            ReleaseAll();
        }

        public void Deinitialize()
        {
            for (int i = _iconList.Count - 1; i >= 0; i--)
            {
                if (_iconList[i] != null)
                    Destroy(_iconList[i].gameObject);
            }

            _iconList.Clear();
            _activeCount = 0;
        }

        public RewardFlightIconView Acquire()
        {
            if (_activeCount == _iconList.Count)
                _iconList.Add(Create());

            RewardFlightIconView icon = _iconList[_activeCount];

            _activeCount++;

            icon.transform.SetAsLastSibling();
            icon.gameObject.SetActive(true);

            return icon;
        }

        public void Release(RewardFlightIconView icon)
        {
            if (icon != null)
                icon.gameObject.SetActive(false);
        }

        public void ReleaseAll()
        {
            for (int i = 0; i < _iconList.Count; i++)
            {
                if (_iconList[i] != null)
                    _iconList[i].gameObject.SetActive(false);
            }

            _activeCount = 0;
        }

        private RewardFlightIconView Create()
        {
            RewardFlightIconView icon = Instantiate(_iconPrefab, _content, false);

            icon.gameObject.SetActive(false);

            return icon;
        }

        private void OnValidate()
        {
            if (_content == null)
                _content = transform as RectTransform;
        }
    }
}
