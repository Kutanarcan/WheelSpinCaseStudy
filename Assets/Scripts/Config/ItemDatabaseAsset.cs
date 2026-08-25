using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName ="CaseStudy/Item Database", fileName = "ItemDatabase")]
    public class ItemDatabaseAsset : ScriptableObject
    {
        [SerializeField] private ItemAsset[] _items = Array.Empty<ItemAsset>();

        public IReadOnlyList<ItemAsset> Items => _items;

        public ItemRegistry CreateRegistry() => new ItemRegistry(_items);

    }
}
