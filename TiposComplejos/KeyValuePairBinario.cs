using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class KeyValuePairBinario<TKey, TValue> : ElementoBinario
    {
        public ElementoBinario FormatoKey { get; set; }
        public ElementoBinario FormatoValue { get; set; }

        protected override byte[] IGetBytes(object obj)
        {
            KeyValuePair<TKey, TValue> keyValuePair = (KeyValuePair<TKey, TValue>)obj;
            return new byte[0].AddArray(FormatoKey.GetBytes(keyValuePair.Key), FormatoValue.GetBytes(keyValuePair.Value));
        }

        protected override object IGetObject(MemoryStream bytes)
        {
            return new KeyValuePair<TKey, TValue>((TKey)FormatoKey.GetObject(bytes), (TValue)FormatoValue.GetObject(bytes));
        }
    }
}
