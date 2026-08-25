namespace CaseStudy.WheelSpin
{
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
