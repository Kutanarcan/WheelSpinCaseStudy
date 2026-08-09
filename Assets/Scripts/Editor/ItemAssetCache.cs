using UnityEditor;
using UnityEngine;

namespace CaseStudy.WheelSpin.EditorTools
{
    public static class ItemAssetCache
    {
        private static ItemAsset[] _items;

        public static ItemAsset[] Items
        {
            get
            {
                if (_items == null) Refresh();
                return _items;
            }
        }

        public static void Refresh()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ItemAsset)}");
            _items = new ItemAsset[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                _items[i] = AssetDatabase.LoadAssetAtPath<ItemAsset>(path);
            }
        }

        public static void Invalidate() => _items = null;

        private class Watcher : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(
                string[] imported, string[] deleted, string[] moved, string[] movedFrom)
                => Invalidate();
        }
    }
}