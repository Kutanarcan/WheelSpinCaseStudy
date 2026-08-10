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


        public string ItemId => string.IsNullOrWhiteSpace(_itemId) ? name : _itemId;
        public Sprite Icon => _icon;

        public ItemViewSettings WheelSettings => _wheelSettings;

        public ItemViewSettings RewardSettings => _wheelSettings;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_itemId))
                _itemId = name;


            if (_wheelSettings.Size.x <= 0f || _wheelSettings.Size.y <= 0f)
                _wheelSettings.Size = ItemViewSettings.DefaultSize;

            if (_rewardSettings.Size.x <= 0f || _rewardSettings.Size.y <= 0f)
                _rewardSettings.Size = ItemViewSettings.DefaultSize;
        }
#endif
    }
}