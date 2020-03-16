using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class DictionaryBinary<T, TKey, TValue> : ElementoIListBinario<KeyValuePair<TKey, TValue>>
         where T :class,  IDictionary<TKey, TValue>,  new()

    {
        public DictionaryBinary(ElementoBinario serializadorKey,ElementoBinario serializadorValue) : this(serializadorKey,serializadorValue,LongitudBinaria.UInt)
        {
        }

        public DictionaryBinary(ElementoBinario serializadorKey, ElementoBinario serializadorValue,LongitudBinaria longitud) : base(new KeyValuePairBinario<TKey, TValue>(serializadorKey, serializadorValue), longitud)
        {
        }

        protected override byte[] JGetBytes(object obj)
        {
            T dic=obj as T;
            if (dic == null)
                throw new Exception($"Tiene que ser un {typeof(T).Name}");

            return base.JGetBytes(dic.ToArray());
        }

        protected override object JGetObject(MemoryStream bytes)
        {
            KeyValuePair<TKey, TValue>[] partes =(KeyValuePair<TKey, TValue>[])base.JGetObject(bytes);
            T dic = new T();
            for (int i = 0; i < partes.Length; i++)
                dic.Add(partes[i]);
            return dic;
        }
        public override string ToString()
        {
            return $"TipoDatos=DiccionarioBinario<{typeof(TKey).Name},{typeof(TValue).Name}>";
        }
    }
}
