using System;

using Robock.Shared.Win32;

namespace Robock.Shared.Models
{
    public static class RobockUtil
    {
        public static RECT AsRect(double top, double left, double height, double width, double multi = 1)
        {
            return new RECT
            {
                top = (int) (top * multi),
                left = (int) (left * multi),
                bottom = (int) ((top + height) * multi),
                right = (int) ((left + width) * multi)
            };
        }

        /// <summary>
        ///     height, width におけるアスペクト比を求めます。
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns>WxH</returns>
        public static (double, double) AspectRatio(double width, double height)
        {
            // ReSharper disable once RedundantAssignment
            var (a, b, remainder) = (height, width, .0);
            do
            {
                remainder = a % b;
                (a, b) = (b, remainder);
            } while (Math.Abs(remainder) > 0);
            return (width / a, height / a);
        }
    }
}