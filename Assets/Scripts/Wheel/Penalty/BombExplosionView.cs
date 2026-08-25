using Coffee.UIExtensions;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class BombExplosionView : MonoBehaviour
    {
        [SerializeField] private UIParticle _particle;

        private RectTransform _rect;

        public RectTransform Rect => _rect != null ? _rect : _rect = transform as RectTransform;

        public bool IsReady => _particle != null;

        private void OnValidate()
        {
            if (_particle == null)
                _particle = GetComponentInChildren<UIParticle>(true);
        }

        public void Initialize() => Hide();

        public void PlayAt(Vector3 worldPosition)
        {
            if (!IsReady)
                return;

            Rect.position = worldPosition;

            gameObject.SetActive(true);

            _particle.Clear();
            _particle.Play();
        }

        public void Hide()
        {
            if (IsReady)
            {
                _particle.Stop();
                _particle.Clear();
            }

            gameObject.SetActive(false);
        }
    }
}
