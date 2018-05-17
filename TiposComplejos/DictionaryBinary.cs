using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class DictionaryBinary<T, TKey, TValue> : ElementoIListBinario<KeyValuePairBinario<TKey, TValue>>
         where T :  IDictionary<TKey, TValue>,new()

    {
        public DictionaryBinary(LongitudBinaria unidadCantidadElementos = LongitudBinaria.UInt) : base(new KeyValuePairBinario<TKey, TValue>(), unidadCantidadElementos)
        {
        }


        protected override byte[] JGetBytes(object obj)
        {
            IDictionary<TKey, TValue> dic = (IDictionary<TKey, TValue>)obj;
            return base.GetBytes(dic.ToArray());
        }

        protected override object JGetObject(MemoryStream bytes)
        {
            KeyValuePair<TKey, TValue>[] partes =(KeyValuePair<TKey, TValue>[]) GetObject(bytes);
            T dic = new T();
            for (int i = 0; i < partes.Length; i++)
                dic.Add(partes[i]);
            return dic;
        }
    }
}
