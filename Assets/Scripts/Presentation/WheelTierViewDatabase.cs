using UnityEngine;
using UnityEngine.Serialization;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName = "CaseStudy/WheelTierViewDatabase", fileName = "WheelTierViewDatabase")]
    public class WheelTierViewDatabase : ScriptableObject
    {
        [SerializeField, FormerlySerializedAs("_defaultSpritePack")]
        private TierViewPack _defaultViewPack = new TierViewPack();

        [SerializeField] private TierViewHolder[] _tierHolderArray;

        [Header("Zone Strip")]
        [Tooltip("Colour of the zone the player is on. Every other number takes its tier colour.")]
        [SerializeField] private Color _currentZoneColor = Color.black;

        [Tooltip("Alpha of zone numbers the player has already passed, out of 255.")]
        [SerializeField, Range(0, 255)] private int _pastZoneAlpha = 40;

        public Color CurrentZoneColor => _currentZoneColor;

        /// Authored out of 255 because that is how the colour picker shows it; handed out as the
        /// 0..1 alpha the renderer actually wants.
        public float PastZoneAlpha => _pastZoneAlpha / 255f;

        public TierViewPack GetPack(WheelTier tier)
        {
            if (_tierHolderArray != null)
            {
                for (int i = 0; i < _tierHolderArray.Length; i++)
                {
                    TierViewHolder holder = _tierHolderArray[i];

                    if (holder != null && holder.Tier == tier && holder.Pack != null)
                        return holder.Pack;
                }
            }

            return _defaultViewPack;
        }
    }

    [System.Serializable]
    public class TierViewPack
    {
        public Sprite Wheel;
        public Sprite WheelIndicator;
        public Color ZoneNumberColor = Color.white;
    }

    [System.Serializable]
    public class TierViewHolder
    {
        public WheelTier Tier;
        public TierViewPack Pack;
    }
}
