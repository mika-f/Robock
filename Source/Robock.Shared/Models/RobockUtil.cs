using System;

using Microsoft.Win32;

using Robock.Interop.Win32;

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

        /// <summary>
        ///     Windows 10 1703 (Creators Update) 以前の場合は　true を返します。
        /// </summary>
        /// <returns></returns>
        public static bool IsOldWindows()
        {
            if (Environment.OSVersion.Version.Major < 10)
                return true;

            var release = int.Parse(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "1000").ToString());
            return release < 1709;
        }
    }
}