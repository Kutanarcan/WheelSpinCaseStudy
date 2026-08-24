using UnityEngine;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Decides where the win icons appear and how big they are: acquires them from the pool and
    /// scatters them around the given origin. Owns placement only — the flight timeline belongs to
    /// RewardFlightPresenter.
    /// </summary>
    public class RewardFlightSpawner
    {
        private readonly RewardFlightView _view;
        private readonly ItemRegistry _registry;
        private readonly RewardFlightSettings _settings;

        private RewardFlightIconView[] _iconArray = System.Array.Empty<RewardFlightIconView>();

        public RewardFlightSpawner(RewardFlightView view, ItemRegistry registry, RewardFlightSettings settings)
        {
            _view = view;
            _registry = registry;
            _settings = settings;
        }

        public RewardFlightIconView Get(int index)
            => index >= 0 && index < _iconArray.Length ? _iconArray[index] : null;

        public void Spawn(string itemId, int count, Vector3 origin)
        {
            EnsureCapacity(count);

            _view.ReleaseAll();

            Sprite sprite = null;
            Vector2 size = ItemViewSettings.DefaultSize;

            if (_registry != null && _registry.TryGet(itemId, out ItemAsset item))
            {
                sprite = item.Icon;
                size = item.FlightSize;
            }

            float step = 360f / count;
            float jitter = step * _settings.SpawnAngleJitter;

            float driftRatio = 1f - _settings.SpawnDrift;

            for (int i = 0; i < count; i++)
            {
                RewardFlightIconView icon = _view.Acquire();
                Vector2 offset = RandomOffset(i * step, jitter);

                icon.Bind(sprite, size);
                icon.Place(origin, offset * driftRatio, offset);

                _iconArray[i] = icon;
            }
        }

        public void Release(int index)
        {
            RewardFlightIconView icon = Get(index);

            if (icon != null)
                _view.Release(icon);
        }

        /// Even angular slots plus jitter: uniform random inside the circle would let icons overlap,
        /// a fixed slot per icon keeps them apart while still reading as scattered.
        private Vector2 RandomOffset(float slotAngle, float jitter)
        {
            float angle = (slotAngle + Random.Range(-jitter, jitter)) * Mathf.Deg2Rad;
            float radius = _settings.SpawnRadius
                           * Mathf.Sqrt(Random.Range(_settings.SpawnInnerRatio, 1f));

            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        private void EnsureCapacity(int count)
        {
            if (_iconArray.Length < count)
                _iconArray = new RewardFlightIconView[count];
        }
    }
}
