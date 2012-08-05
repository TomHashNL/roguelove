using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roguelove
{
    public static class CollectionExtensions
    {
        public static T MaxT<T, TSelector>(this IEnumerable<T> source, Func<T, TSelector> selector)
            where TSelector : IComparable<TSelector>
        {
            return MinMaxT(source, selector, true);
        }

        public static T MinT<T, TSelector>(this IEnumerable<T> source, Func<T, TSelector> selector)
            where TSelector : IComparable<TSelector>
        {
            return MinMaxT(source, selector, false);
        }

        static T MinMaxT<T, TSelector>(this IEnumerable<T> source, Func<T, TSelector> selector, bool max)
            where TSelector : IComparable<TSelector>
        {
            bool set = false;
            T result = default(T);
            TSelector tSelector = default(TSelector);

            foreach (var t in source)
            {
                TSelector tSelectorNew = selector.Invoke(t);
                if (!set || tSelectorNew.CompareTo(tSelector) * (max ? +1 : -1) > 0)
                {
                    result = t;
                    tSelector = tSelectorNew;
                }
            }

            return result;
        }
    }
}
