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
        [SerializeField] private Color _currentZoneColor = Color.black;

        [SerializeField, Range(0, 255)] private int _pastZoneAlpha = 40;

        public Color CurrentZoneColor => _currentZoneColor;

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
