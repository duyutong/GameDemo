namespace FlexiServer.Core
{
    public class RandomGenerator
    {
        private static Random random;
        public static void InitRandom()
        {
            int seed = DateTime.Now.Millisecond;
            random = new Random(seed);
        }
        public static int GetRandomInt(int min, int max)
        {
            if (random == null) InitRandom();
            return random.Next(min, max);
        }
        public static int GetRandomInt(int max)
        {
            if (random == null) InitRandom();
            return random.Next(max);
        }
        public static float GetRandomFloat(float min, float max)
        {
            if (random == null) InitRandom();
            return (float)(random.NextDouble() * (max - min) + min);
        }
        public static float GetRandomFloat(float max)
        {
            if (random == null) InitRandom();
            return (float)(random.NextDouble() * max);
        }
        public static double GetRandomDouble(double min, double max)
        {
            if (random == null) InitRandom();
            return random.NextDouble() * (max - min) + min;
        }
        public static void GetRandomBytes(byte[] buffer)
        {
            if (random == null) InitRandom();
            random.NextBytes(buffer);
        }
        public static void GetRandomBytes(Span<byte> buffer)
        {
            if (random == null) InitRandom();
            random.NextBytes(buffer);
        }
    }
}
