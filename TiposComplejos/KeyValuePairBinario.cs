using Gabriel.Cat.S.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class KeyValuePairBinario<TKey, TValue> : ElementoComplejoBinario
    {
        public KeyValuePairBinario(ElementoBinario serializadorTKey,ElementoBinario serializadorTValue):base(new ElementoBinario[] {serializadorTKey,serializadorTValue }) { }
        protected override IList IGetPartsObject(object obj)
        {
            KeyValuePair<TKey, TValue> pair = (KeyValuePair<TKey, TValue>)obj;
            return new object[] { pair.Key, pair.Value };
        }
        public ElementoBinario SerializadorKey => Partes[0];
        public ElementoBinario SerializadorValue => Partes[1];

        protected override object JGetObject(MemoryStream bytes)
        {
            object[] partes = GetPartsObject(bytes);
            return new KeyValuePair<TKey, TValue>((TKey)partes[0], (TValue)partes[1]);
        }
        public override string ToString()
        {
            return $"TipoDatos=KeyValuePair<{typeof(TKey).Name},{typeof(TValue).Name}>Binario";
        }
        public override ElementoBinario Clon()
        {
            return new KeyValuePairBinario<TKey, TValue>(SerializadorKey, SerializadorValue);
        }
    }
}
