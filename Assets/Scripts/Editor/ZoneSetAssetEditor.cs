using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CaseStudy.WheelSpin.EditorTools
{
    [CustomEditor(typeof(ZoneSetAsset))]
    public class ZoneSetAssetEditor : Editor
    {
        private const string PreviewKey = "CaseStudy.WheelSpin.PreviewPenalty";

        private SerializedProperty _zones;
        private ReorderableList _list;
        private WheelTierRuleProvider _rules;

        private void OnEnable()
        {
            _zones = serializedObject.FindProperty("_zones");
            _rules = ResolveRules();

            _list = new ReorderableList(serializedObject, _zones, true, true, true, true)
            {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawElement,
                elementHeight = EditorGUIUtility.singleLineHeight + 4f
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _list.DoLayoutList();

            EditorGUILayout.LabelField(
                $"Gold every {_rules.GoldEvery}, Silver every {_rules.SilverEvery}",
                EditorStyles.miniLabel);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect)
            => EditorGUI.LabelField(rect, "Zone   Tier      Penalty   Expected");

        private void DrawElement(Rect rect, int index, bool active, bool focused)
        {
            SerializedProperty element = _zones.GetArrayElementAtIndex(index);
            var asset = element.objectReferenceValue as ZoneAsset;

            int zoneIndex = index + 1;
            WheelTier tier = _rules.TierFor(zoneIndex);
            bool hasPenalty = _rules.HasPenalty(zoneIndex);

            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;

            var numberRect = new Rect(rect.x, rect.y, 30f, rect.height);
            var fieldRect = new Rect(rect.x + 32f, rect.y, rect.width * 0.40f, rect.height);
            var tierRect = new Rect(fieldRect.xMax + 6f, rect.y, 60f, rect.height);
            var penRect = new Rect(tierRect.xMax + 6f, rect.y, 60f, rect.height);
            var expRect = new Rect(penRect.xMax + 6f, rect.y, rect.xMax - penRect.xMax - 6f, rect.height);

            EditorGUI.LabelField(numberRect, zoneIndex.ToString());
            EditorGUI.PropertyField(fieldRect, element, GUIContent.none);

            Color previous = GUI.color;
            GUI.color = TierColor(tier);
            EditorGUI.LabelField(tierRect, tier.ToString());
            GUI.color = previous;

            EditorGUI.LabelField(penRect, asset == null ? "-" : hasPenalty ? $"slot {asset.PenaltySlotIndex}" : "none");
            EditorGUI.LabelField(expRect, asset == null ? "-" : ExpectedReward(asset, hasPenalty).ToString("0"));
        }

        private static Color TierColor(WheelTier tier)
        {
            switch (tier)
            {
                case WheelTier.Gold: return new Color(1f, 0.82f, 0.3f);
                case WheelTier.Silver: return new Color(0.75f, 0.78f, 0.85f);
                default: return Color.white;
            }
        }

        private static float ExpectedReward(ZoneAsset asset, bool hasPenalty)
        {
            float q = EditorPrefs.GetFloat(PreviewKey, 0.10f);

            int total = 0;
            for (int i = 0; i < asset.Rewards.Count; i++)
                total += asset.Rewards[i].Weight;

            if (total <= 0) return 0f;

            if (!hasPenalty)
            {
                float sum = 0f;
                for (int i = 0; i < asset.Rewards.Count; i++)
                    sum += asset.Rewards[i].Amount * asset.Rewards[i].Weight / (float)total;

                return sum;
            }

            int penaltyIndex = asset.PenaltySlotIndex;
            int totalWithout = total - asset.Rewards[penaltyIndex].Weight;
            if (totalWithout <= 0) return 0f;

            float expected = 0f;
            for (int i = 0; i < asset.Rewards.Count; i++)
            {
                if (i == penaltyIndex) continue;
                expected += asset.Rewards[i].Amount * (1f - q) * asset.Rewards[i].Weight / totalWithout;
            }

            return expected;
        }

        private static WheelTierRuleProvider ResolveRules()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(WheelConfigAsset)}");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var config = AssetDatabase.LoadAssetAtPath<WheelConfigAsset>(path);
                if (config != null) return config.CreateTierRuleProvider();
            }

            return new WheelTierRuleProvider();
        }
    }
}