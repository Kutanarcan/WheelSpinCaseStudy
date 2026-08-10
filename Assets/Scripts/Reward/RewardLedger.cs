using System.Collections.Generic;

namespace CaseStudy.WheelSpin
{
    public class RewardLedger
    {
        public readonly struct Entry
        {
            public readonly string ItemId;
            public readonly int Amount;

            public Entry(string itemId, int amount)
            {
                ItemId = itemId;
                Amount = amount;
            }
        }

        private readonly List<Entry> _entries = new List<Entry>();
        private readonly Dictionary<string, int> _indexById = new Dictionary<string, int>();

        public IReadOnlyList<Entry> Entries => _entries;

        public bool IsEmpty => _entries.Count == 0;

        public void Add(string itemId, int amount)
        {
            if (string.IsNullOrWhiteSpace(itemId) || amount <= 0)
                return;

            if (_indexById.TryGetValue(itemId, out int index))
            {
                _entries[index] = new Entry(itemId, _entries[index].Amount + amount);
                return;
            }

            _indexById[itemId] = _entries.Count;
            _entries.Add(new Entry(itemId, amount));
        }

        public void Clear()
        {
            _entries.Clear();
            _indexById.Clear();
        }
    }
}
