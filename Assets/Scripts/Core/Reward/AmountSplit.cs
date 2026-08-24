namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Splits a win into per-icon shares. Every share is equal except the last, which absorbs the
    /// remainder, so the shares always sum back to the exact amount and the counter lands on the
    /// real total instead of a rounded one.
    /// </summary>
    public static class AmountSplit
    {
        public static void Fill(int[] buffer, int count, int amount)
        {
            if (buffer == null || count <= 0 || count > buffer.Length)
                return;

            int share = amount / count;

            for (int i = 0; i < count; i++)
                buffer[i] = share;

            buffer[count - 1] = amount - share * (count - 1);
        }
    }
}
