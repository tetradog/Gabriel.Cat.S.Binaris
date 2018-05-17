using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class KeyValuePairBinario<TKey, TValue> : ElementoBinario,ITwoPartsElement
    {
        public ElementoBinario FormatoKey { get; set; }
        public ElementoBinario FormatoValue { get; set; }
        ElementoBinario ITwoPartsElement.Part1 { get => FormatoKey; set => FormatoKey = value; }
        ElementoBinario ITwoPartsElement.Part2 { get => FormatoValue; set => FormatoValue = value; }

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
