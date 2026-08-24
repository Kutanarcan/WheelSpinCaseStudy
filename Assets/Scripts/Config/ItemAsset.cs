using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName = "CaseStudy/Item", fileName = "Item_")]
    public class ItemAsset : ScriptableObject
    {
        [SerializeField] private string _itemId;
        [SerializeField] private Sprite _icon;

        [SerializeField] private ItemViewSettings _wheelSettings = ItemViewSettings.Default;
        [SerializeField] private ItemViewSettings _rewardSettings = ItemViewSettings.Default;

        [Tooltip("Icon layout inside the cashout popup, where the reward list has its own scale.")]
        [SerializeField] private ItemViewSettings _cashRewardSettings = ItemViewSettings.Default;

        [Header("Win Effect")]
        [Tooltip("Multiplies the wheel icon size for the flying win icons. " +
                 "1 = same size as on the wheel, 2 = double. Aspect ratio is always kept.")]
        [SerializeField, Min(0f)] private float _flightScale = 1f;


        public string ItemId => string.IsNullOrWhiteSpace(_itemId) ? name : _itemId;
        public Sprite Icon => _icon;

        public ItemViewSettings WheelSettings => _wheelSettings;

        public ItemViewSettings RewardSettings => _rewardSettings;

        public ItemViewSettings CashRewardSettings => _cashRewardSettings;

        /// <summary>Size of one flying win icon, derived from the wheel icon so the two always match.</summary>
        public Vector2 FlightSize => _wheelSettings.Size * _flightScale;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_itemId))
                _itemId = name;
          
        }
#endif
    }
}