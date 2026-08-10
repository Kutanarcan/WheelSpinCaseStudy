using UnityEditor;

namespace CaseStudy.WheelSpin.EditorTools
{
    internal static class WheelConfigLocator
    {
        private const float FallbackPenaltyChance = 0.25f;

        private static WheelConfigAsset _cached;

        public static WheelConfigAsset Find()
        {
            if (_cached != null)
                return _cached;

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(WheelConfigAsset)}");

            if (guids.Length == 0)
                return null;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);

            _cached = AssetDatabase.LoadAssetAtPath<WheelConfigAsset>(path);

            return _cached;
        }

        public static WheelTierRuleProvider Rules()
        {
            WheelConfigAsset config = Find();

            return config != null ? config.CreateTierRuleProvider() : new WheelTierRuleProvider();
        }

        public static float GlobalPenaltyChance()
        {
            WheelConfigAsset config = Find();

            return config != null ? config.PenaltyChance : FallbackPenaltyChance;
        }
    }
}
