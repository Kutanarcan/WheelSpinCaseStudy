using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CaseStudy.WheelSpin.EditorTools
{
    [CustomEditor(typeof(ZoneSetAsset))]
    public class ZoneSetAssetEditor : Editor
    {
        private SerializedProperty _zones;
        private ReorderableList _list;
        private WheelTierRuleProvider _rules;

        private void OnEnable()
        {
            _zones = serializedObject.FindProperty("_zones");
            _rules = WheelConfigLocator.Rules();

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
                $"Gold every {_rules.GoldEvery}, Silver every {_rules.SilverEvery}    " +
                $"Global penalty {WheelConfigLocator.GlobalPenaltyChance() * 100f:0.0}%   (* = zone override)",
                EditorStyles.miniLabel);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect)
            => EditorGUI.LabelField(rect, "Zone   Tier      Penalty        Expected");

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
            var fieldRect = new Rect(rect.x + 32f, rect.y, rect.width * 0.34f, rect.height);
            var tierRect = new Rect(fieldRect.xMax + 6f, rect.y, 55f, rect.height);
            var penRect = new Rect(tierRect.xMax + 6f, rect.y, 110f, rect.height);
            var expRect = new Rect(penRect.xMax + 6f, rect.y, Mathf.Max(30f, rect.xMax - penRect.xMax - 6f), rect.height);

            EditorGUI.LabelField(numberRect, zoneIndex.ToString());
            EditorGUI.PropertyField(fieldRect, element, GUIContent.none);

            Color previous = GUI.color;
            GUI.color = TierColor(tier);
            EditorGUI.LabelField(tierRect, tier.ToString());
            GUI.color = previous;

            EditorGUI.LabelField(penRect, PenaltyLabel(asset, hasPenalty));
            EditorGUI.LabelField(expRect, asset == null ? "-" : ExpectedReward(asset, hasPenalty).ToString("0.0"));
        }

        private static string PenaltyLabel(ZoneAsset asset, bool hasPenalty)
        {
            if (asset == null) return "-";
            if (!hasPenalty) return "none";

            float chance = ResolvedChance(asset);
            int weight = PenaltyOdds.WeightFor(asset.OtherWeightSum(), chance);
            float actual = PenaltyOdds.ChanceFor(asset.OtherWeightSum(), weight);

            string mark = asset.OverridesPenaltyChance ? "*" : string.Empty;

            return $"slot {asset.PenaltySlotIndex} · {actual * 100f:0.0}%{mark}";
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

        private static float ResolvedChance(ZoneAsset asset)
            => asset.ResolvePenaltyChance(WheelConfigLocator.GlobalPenaltyChance());

        private static float ExpectedReward(ZoneAsset asset, bool hasPenalty)
        {
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

            int otherSum = asset.OtherWeightSum();
            if (otherSum <= 0) return 0f;

            int penaltyWeight = PenaltyOdds.WeightFor(otherSum, ResolvedChance(asset));
            float penaltyChance = PenaltyOdds.ChanceFor(otherSum, penaltyWeight);

            int penaltyIndex = asset.PenaltySlotIndex;
            float expected = 0f;

            for (int i = 0; i < asset.Rewards.Count; i++)
            {
                if (i == penaltyIndex) continue;
                expected += asset.Rewards[i].Amount * (1f - penaltyChance) * asset.Rewards[i].Weight / otherSum;
            }

            return expected;
        }
    }
}
