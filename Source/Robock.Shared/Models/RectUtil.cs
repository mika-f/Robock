using Robock.Shared.Win32;

namespace Robock.Shared.Models
{
    public static class RectUtil
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
    }
}