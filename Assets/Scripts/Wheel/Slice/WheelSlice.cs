
namespace CaseStudy.WheelSpin
{
    public sealed class WheelSlice
    {
        public SliceType Type;
        public string ItemId;
        public int Amount;
        public int Weight;

        public static WheelSlice CreateReward(string itemId, int amount, int weight)
        {
            return new WheelSlice
            {
                Type = SliceType.Reward,
                ItemId = itemId,
                Amount = amount,
                Weight = weight
            };
        }

        public static WheelSlice CreatePenalty(int weight)
        {
            return new WheelSlice
            {
                Type = SliceType.Penalty,
                ItemId = null,
                Amount = 0,
                Weight = weight
            };
        }
    }
}
