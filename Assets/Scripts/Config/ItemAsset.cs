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

        [SerializeField] private ItemViewSettings _cashRewardSettings = ItemViewSettings.Default;

        [Header("Win Effect")]
        [SerializeField, Min(0f)] private float _flightScale = 1f;

        public string ItemId => string.IsNullOrWhiteSpace(_itemId) ? name : _itemId;
        public Sprite Icon => _icon;

        public ItemViewSettings WheelSettings => _wheelSettings;

        public ItemViewSettings RewardSettings => _rewardSettings;

        public ItemViewSettings CashRewardSettings => _cashRewardSettings;

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