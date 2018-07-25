using System;
using System.Collections.Generic;

namespace Robock.Models
{
    internal class IgnoreComparator : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            if (x != 0 && y == 0)
                return true; // なぜか新しい値に0が降ってきて、それを通知すると死ぬ
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            throw new NotImplementedException();
        }
    }
}