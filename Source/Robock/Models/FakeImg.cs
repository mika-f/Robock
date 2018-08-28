namespace Robock.Models
{
    public static class FakeImg
    {
        private const string Domain = "https://fakeimg.mochizuki.moe";

        public static string Default => HxW(1, 1);

        public static string HxW(int height, int width)
        {
            return $"{Domain}/{height}x{width}/000000%2C000/000000%2C000/";
        }

        public static string HxW((double, double) tuple)
        {
            return HxW((int) tuple.Item1, (int) tuple.Item2);
        }
    }
}