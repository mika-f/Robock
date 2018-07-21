using System;
using System.Drawing;

namespace Robock.Models
{
    public static class AspectHelper
    {
        /// <summary>
        ///     height, width におけるアスペクト比を求めます。
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns>WxH</returns>
        public static string Calc(double height, double width)
        {
            // ReSharper disable once RedundantAssignment
            var (a, b, remainder) = (height, width, .0);
            do
            {
                remainder = a % b;
                (a, b) = (b, remainder);
            } while (Math.Abs(remainder) > 0);
            return $"{width / a}x{height / a}";
        }

        public static Size AsSize(string aspectRatio)
        {
            return new Size {Height = int.Parse(aspectRatio.Split('x')[1]), Width = int.Parse(aspectRatio.Split('x')[0])};
        }
    }
}