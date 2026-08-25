using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public sealed class ViewPool<T> where T : Component
    {
        private readonly List<T> _instances = new List<T>();
        private readonly T _prefab;
        private readonly Transform _content;

        private int _activeCount;
        private int _createdFromIndex;

        public ViewPool(T prefab, Transform content)
        {
            _prefab = prefab;
            _content = content;
        }

        public bool IsReady => _prefab != null && _content != null;

        public int Count => _instances.Count;

        public int ActiveCount => _activeCount;

        public T Get(int index) => index >= 0 && index < _instances.Count ? _instances[index] : null;

        public void Prewarm(int count)
        {
            DropDestroyed();

            _createdFromIndex = _instances.Count;

            while (_instances.Count < count && IsReady)
                _instances.Add(Create());

            ReleaseAll();
        }

        public T Acquire()
        {
            if (!IsReady)
                return null;

            if (_activeCount == _instances.Count)
                _instances.Add(Create());

            T instance = _instances[_activeCount];

            _activeCount++;

            instance.transform.SetAsLastSibling();
            instance.gameObject.SetActive(true);

            return instance;
        }

        public void Release(T instance)
        {
            if (instance != null)
                instance.gameObject.SetActive(false);
        }

        public void ReleaseAll()
        {
            DropDestroyed();

            for (int i = 0; i < _instances.Count; i++)
                _instances[i].gameObject.SetActive(false);

            _activeCount = 0;
        }

        public void DestroyCreated()
        {
            for (int i = _instances.Count - 1; i >= _createdFromIndex; i--)
            {
                if (_instances[i] != null)
                    Object.Destroy(_instances[i].gameObject);

                _instances.RemoveAt(i);
            }

            _activeCount = 0;
        }

        private void DropDestroyed()
        {
            for (int i = _instances.Count - 1; i >= 0; i--)
            {
                if (_instances[i] == null)
                    _instances.RemoveAt(i);
            }

            if (_createdFromIndex > _instances.Count)
                _createdFromIndex = _instances.Count;
        }

        private T Create()
        {
            T instance = Object.Instantiate(_prefab, _content, false);

            instance.gameObject.SetActive(false);

            return instance;
        }
    }
}
