using UnityEngine;

namespace CaseStudy.WheelSpin
{

    [CreateAssetMenu(menuName = "CaseStudy/WheelTierViewDatabase", fileName = "WheelTierViewDatabase")]
    public class WheelTierViewDatabase : ScriptableObject
    {
        [SerializeField] private TierSpritePack _defaultSpritePack;
        [SerializeField] private TierSpriteHolder[] _tierHolderArray;

        public TierSpritePack GetSprite(WheelTier tier)
        {

            for (int i = 0; i < _tierHolderArray.Length; i++)
            {
                var holder = _tierHolderArray[i];

                if (holder.Tier == tier)
                    return holder.Pack;
            }

            return _defaultSpritePack;
        }

    }

    [System.Serializable]
    public class TierSpritePack
    {
        public Sprite Wheel;
        public Sprite WheelIndicator;

    }

    [System.Serializable]
    public class TierSpriteHolder
    {
        public WheelTier Tier;
        public TierSpritePack Pack;
    }
}