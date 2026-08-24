using Coffee.UIExtensions;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// The single explosion instance kept under the wheel. It is moved onto the bomb slice and
    /// replayed for each detonation rather than being spawned, so no pooling is involved.
    /// </summary>
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

            // Activate before playing: UIParticle collects its ParticleSystems on enable, and
            // playing while disabled would emit into a list that is not registered yet.
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
