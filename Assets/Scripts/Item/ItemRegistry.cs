using System.Collections.Generic;

namespace CaseStudy.WheelSpin
{
    public class ItemRegistry
    {
        private readonly Dictionary<string, ItemAsset> _items = new Dictionary<string, ItemAsset>();

        public ItemRegistry(IReadOnlyList<ItemAsset> assets)
        {
            if (assets == null) return;

            for (int i = 0; i < assets.Count; i++)
            {
                ItemAsset asset = assets[i];
                if (asset == null) continue;

                _items[asset.ItemId] = asset;
            }
        }

        public bool TryGet(string itemId, out ItemAsset asset)
            => _items.TryGetValue(itemId, out asset);
    }
}