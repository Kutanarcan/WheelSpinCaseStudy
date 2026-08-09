using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName = "CaseStudy/Item", fileName = "Item_")]
    public class ItemAsset : ScriptableObject
    {
        [SerializeField] private string _itemId;
        [SerializeField] private Sprite _icon;

        public string ItemId => string.IsNullOrWhiteSpace(_itemId) ? name : _itemId;
        public Sprite Icon => _icon;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_itemId)) _itemId = name;
        }
#endif
    }
}