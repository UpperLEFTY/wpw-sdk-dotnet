using System;
using System.Collections.Generic;

namespace Worldpay.Innovation.WPWithin.ThriftAdapters
{
    internal class CollectionUtils
    {

        public static Dictionary<TDestKey, TDestValue> Copy<TSourceKey, TSourceValue, TDestKey, TDestValue>(
            Dictionary<TSourceKey, TSourceValue> source, Func<TSourceKey, TDestKey> keyAdapter, 
            Func<TSourceValue, TDestValue> valueAdapater)
        {
            Dictionary<TDestKey, TDestValue> dest = new Dictionary<TDestKey, TDestValue>(source.Count);

            foreach (var keypair in source)
            {
                dest.Add(keyAdapter(keypair.Key), valueAdapater(keypair.Value));
            }

            return dest;
        }

        public static Dictionary<TKey, TDestValue> Copy<TKey, TSourceValue, TDestValue>(
            Dictionary<TKey, TSourceValue> source, Func<TSourceValue, TDestValue> valueAdapter)
        {
            return Copy(source, key => key, valueAdapter);
        }
    }
}
