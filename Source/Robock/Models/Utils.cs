namespace Robock.Models
{
    // いけない匂いがするぞ
    public static class Utils
    {
        public static (int, int) GetAspectRatio(int x, int y)
        {
            int CalcGcd(int a, int b)
            {
                return b == 0 ? a : CalcGcd(b, a % b);
            }

            var gcd = CalcGcd(x, y);
            return (x / gcd, y / gcd);
        }
    }
}