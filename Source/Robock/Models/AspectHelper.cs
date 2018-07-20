using System;

namespace Robock.Models
{
    public static class AspectHelper
    {
        /// <summary>
        ///     アスペクト比求めるくん
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns>HxW</returns>
        public static string Calc(double height, double width)
        {
            // ReSharper disable once RedundantAssignment
            var (a, b, remainder) = (height, width, .0);
            do
            {
                remainder = a % b;
                (a, b) = (b, remainder);
            } while (Math.Abs(remainder) > 0);
            return $"{height / a}x{width / a}";
        }
    }
}