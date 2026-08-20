namespace CaseStudy.WheelSpin
{
    public readonly struct SpinResult
    {
        public readonly int SliceIndex;
        public readonly SliceType Type;
        public readonly string ItemId;
        public readonly int Amount;

        private SpinResult(int sliceIndex, SliceType type, string itemId, int amount)
        {
            SliceIndex = sliceIndex;
            Type = type;
            ItemId = itemId;
            Amount = amount;
        }

        public bool IsPenalty => Type == SliceType.Penalty;

        public static SpinResult Reward(int sliceIndex, string itemId, int amount)
            => new SpinResult(sliceIndex, SliceType.Reward, itemId, amount);

        public static SpinResult Penalty(int sliceIndex)
            => new SpinResult(sliceIndex, SliceType.Penalty, null, 0);
    }
}
