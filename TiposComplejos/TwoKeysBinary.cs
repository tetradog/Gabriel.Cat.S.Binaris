using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class TwoKeysBinary<TKey1,TKey2>:ElementoBinario,ITwoPartsElement
    {
        public ElementoBinario FormatoKey1 { get; set; }
        public ElementoBinario FormatoKey2 { get; set; }
        ElementoBinario ITwoPartsElement.Part1 { get => FormatoKey1; set => FormatoKey1 = value; }
        ElementoBinario ITwoPartsElement.Part2 { get => FormatoKey2; set => FormatoKey2 = value; }

        protected override byte[] IGetBytes(object obj)
        {
            TwoKeys<TKey1, TKey2> keys = (TwoKeys<TKey1, TKey2>)obj;
            return new byte[0].AddArray(FormatoKey1.GetBytes(keys.Key1), FormatoKey2.GetBytes(keys.Key2));
        }

        protected override object IGetObject(MemoryStream bytes)
        {
            return new TwoKeys<TKey1, TKey2>((TKey1)FormatoKey1.GetObject(bytes), (TKey2)FormatoKey2.GetObject(bytes));
        }
    }
}
