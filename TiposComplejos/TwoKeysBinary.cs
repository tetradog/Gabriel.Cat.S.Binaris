using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class TwoKeysBinario<TKey1, TKey2> : KeyValuePairBinario<TKey1, TKey2>
    {
        public TwoKeysBinario(ElementoBinario serializadorTKey1, ElementoBinario serializadorTKey2) : base(serializadorTKey1, serializadorTKey2)
        {
        }
        protected override object IGetObject(MemoryStream bytes)
        {
            KeyValuePair<TKey1,TKey2> pair=(KeyValuePair<TKey1, TKey2>) base.IGetObject(bytes);
            return new TwoKeys<TKey1, TKey2>(pair.Key, pair.Value);
        }
        protected override IList IGetPartsObject(object obj)
        {
            TwoKeys<TKey1, TKey2> keys = (TwoKeys<TKey1, TKey2>)obj;
            return base.IGetPartsObject(new KeyValuePair<TKey1,TKey2>(keys.Key1,keys.Key2));
        }
        public override string ToString()
        {
            return $"TipoDatos=TwoKeys<{typeof(TKey1).Name},{typeof(TKey2).Name}>Binario";
        }
    }
}
