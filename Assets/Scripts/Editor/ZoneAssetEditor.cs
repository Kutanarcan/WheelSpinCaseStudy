using UnityEditor;
using UnityEngine;

namespace CaseStudy.WheelSpin.EditorTools
{
    [CustomEditor(typeof(ZoneAsset))]
    public class ZoneAssetEditor : Editor
    {
        private SerializedProperty _rewards;
        private SerializedProperty _penaltySlotIndex;
        private SerializedProperty _overridePenaltyChance;
        private SerializedProperty _penaltyChanceOverride;

        private void OnEnable()
        {
            _rewards = serializedObject.FindProperty("_rewards");
            _penaltySlotIndex = serializedObject.FindProperty("_penaltySlotIndex");
            _overridePenaltyChance = serializedObject.FindProperty("_overridePenaltyChance");
            _penaltyChanceOverride = serializedObject.FindProperty("_penaltyChanceOverride");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_penaltySlotIndex, new GUIContent("Penalty Slot"));

            DrawPenaltyChance(out float resolvedChance);

            EditorGUILayout.Space();
            DrawTable(resolvedChance);
            EditorGUILayout.Space();
            DrawWarnings();
            EditorGUILayout.Space();
            DrawTools();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPenaltyChance(out float resolvedChance)
        {
            float global = WheelConfigLocator.GlobalPenaltyChance();

            EditorGUILayout.PropertyField(_overridePenaltyChance, new GUIContent("Override Penalty Chance"));

            using (new EditorGUI.DisabledScope(!_overridePenaltyChance.boolValue))
                EditorGUILayout.PropertyField(_penaltyChanceOverride, new GUIContent("Penalty Chance"));

            resolvedChance = _overridePenaltyChance.boolValue ? _penaltyChanceOverride.floatValue : global;

            int otherSum = OtherWeightSum();
            int penaltyWeight = PenaltyOdds.WeightFor(otherSum, resolvedChance);
            float actual = PenaltyOdds.ChanceFor(otherSum, penaltyWeight);

            string source = _overridePenaltyChance.boolValue ? "bu zone" : "global";

            EditorGUILayout.HelpBox(
                $"Bronze penalty: {Percent(actual)} ({source})\n" +
                $"Turetilen agirlik: {penaltyWeight}   Diger dilimlerin toplami: {otherSum}",
                MessageType.None);
        }

        private void DrawTable(float resolvedChance)
        {
            int penaltyIndex = _penaltySlotIndex.intValue;

            int total = 0;
            for (int i = 0; i < _rewards.arraySize; i++)
                total += WeightAt(i);

            int otherSum = OtherWeightSum();
            int penaltyWeight = PenaltyOdds.WeightFor(otherSum, resolvedChance);

            // Runtime ile birebir ayni sayi: yuvarlanmis agirliktan geri hesaplanan olasilik.
            float actualPenalty = PenaltyOdds.ChanceFor(otherSum, penaltyWeight);

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("#", GUILayout.Width(20f));
            GUILayout.Label("Item", GUILayout.Width(150f));
            GUILayout.Label("Amount", GUILayout.Width(60f));
            GUILayout.Label("Weight", GUILayout.Width(55f));
            GUILayout.Label("Bronze", GUILayout.Width(60f));
            GUILayout.Label("Silver+", GUILayout.Width(60f));
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < _rewards.arraySize; i++)
            {
                SerializedProperty element = _rewards.GetArrayElementAtIndex(i);
                SerializedProperty item = element.FindPropertyRelative("_item");
                SerializedProperty amount = element.FindPropertyRelative("_amount");
                SerializedProperty weight = element.FindPropertyRelative("_weight");

                bool isPenalty = i == penaltyIndex;

                EditorGUILayout.BeginHorizontal();

                GUI.color = isPenalty ? new Color(1f, 0.65f, 0.6f) : Color.white;
                GUILayout.Label(i.ToString(), GUILayout.Width(20f));
                GUI.color = Color.white;

                EditorGUILayout.PropertyField(item, GUIContent.none, GUILayout.Width(150f));
                EditorGUILayout.PropertyField(amount, GUIContent.none, GUILayout.Width(60f));

                // Penalty slotunun weight'i Bronze'da kullanilmaz; sadece Silver+ icin anlamli.
                EditorGUILayout.PropertyField(weight, GUIContent.none, GUILayout.Width(55f));

                string bronze = isPenalty
                    ? Percent(actualPenalty)
                    : Percent((1f - actualPenalty) * weight.intValue / Mathf.Max(1, otherSum));

                string silver = Percent((float)weight.intValue / Mathf.Max(1, total));

                GUILayout.Label(bronze, GUILayout.Width(60f));
                GUILayout.Label(silver, GUILayout.Width(60f));

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField(
                $"Total weight: {total}    Bronze havuzu: {otherSum} + {penaltyWeight} (penalty)",
                EditorStyles.miniLabel);
        }

        private void DrawWarnings()
        {
            int penaltyIndex = _penaltySlotIndex.intValue;

            for (int i = 0; i < _rewards.arraySize; i++)
            {
                SerializedProperty item = _rewards.GetArrayElementAtIndex(i).FindPropertyRelative("_item");
                if (item.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox($"Slot {i} has no item assigned.", MessageType.Error);
                    return;
                }
            }

            SerializedProperty penaltySlotItem = _rewards
                .GetArrayElementAtIndex(penaltyIndex)
                .FindPropertyRelative("_item");

            if (penaltySlotItem.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox(
                    $"Slot {penaltyIndex} holds a reward that is unreachable on Bronze zones. " +
                    "It only appears on Silver and Gold. Its weight is ignored on Bronze - " +
                    "the penalty weight is derived from Penalty Chance instead.",
                    MessageType.Info);
            }
        }

        private void DrawTools()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Randomize Items"))
                RandomizeItems();

            if (GUILayout.Button("Refresh Item Cache"))
                ItemAssetCache.Refresh();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(
                $"{ItemAssetCache.Items.Length} item assets found in project.",
                EditorStyles.miniLabel);
        }

        private void RandomizeItems()
        {
            ItemAsset[] pool = ItemAssetCache.Items;
            if (pool.Length == 0) return;

            Undo.RecordObject(target, "Randomize Zone Items");

            for (int i = 0; i < _rewards.arraySize; i++)
            {
                SerializedProperty element = _rewards.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("_item").objectReferenceValue = pool[Random.Range(0, pool.Length)];
                element.FindPropertyRelative("_weight").intValue = Random.Range(5, 21);
            }
        }

        private int OtherWeightSum()
        {
            int penaltyIndex = _penaltySlotIndex.intValue;
            int sum = 0;

            for (int i = 0; i < _rewards.arraySize; i++)
            {
                if (i == penaltyIndex) continue;
                sum += WeightAt(i);
            }

            return sum;
        }

        private int WeightAt(int index)
            => _rewards.GetArrayElementAtIndex(index).FindPropertyRelative("_weight").intValue;

        private static string Percent(float value) => $"{value * 100f:0.0}%";
    }
}
