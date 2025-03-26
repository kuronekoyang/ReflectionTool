using System;
using System.Collections.Generic;

namespace kuro.Core
{
    public static class CollectionUtils
    {
        public static bool IsEquals<T1, T2>(this IReadOnlyList<T1>? a, IReadOnlyList<T2>? b, Func<T1, T2, bool> f)
        {
            if (a == null != (b == null))
                return false;
            if (a == null)
                return true;
            int count = a.Count;
            if (count != b.Count)
                return false;
            for (int index = 0; index < count; ++index)
            {
                if (!f(a[index], b[index]))
                    return false;
            }

            return true;
        }
    }
}