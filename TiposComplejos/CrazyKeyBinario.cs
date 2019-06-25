using Gabriel.Cat.S.Seguretat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class CrazyKeyBinario : ElementoIListBinario<ItemBinario>
    {
        public CrazyKeyBinario(LongitudBinaria unidadCantidadElementos = LongitudBinaria.UInt) : base(new CrazyKeyBinario(), unidadCantidadElementos)
        {
        }

        public CrazyKeyBinario(byte[] marcaFin) : base(new CrazyKeyBinario(), marcaFin)
        {
        }
        protected override byte[] IGetBytes(object obj)
        {
            CrazyKey ckey = obj as CrazyKey;
            if (ckey == null)
                throw new ArgumentException("tiene que ser un objeto " + (typeof(CrazyKey).FullName), "obj");

            return base.GetBytes(ckey.CrazyItems);
        }
        protected override object IGetObject(MemoryStream bytes)
        {
            CrazyKey ckey= new CrazyKey();
            ckey.CrazyItems.AddRange((CrazyKey.CrazyItem[])base.GetObject(bytes));
            return ckey;
        }
    }
    public class CrazyItemBinario : ElementoComplejoBinario
    {
        public CrazyItemBinario()
        {
            Partes.Add(ElementoBinario.ElementosTipoAceptado(Utilitats.Serializar.TiposAceptados.Byte));
            Partes.Add(ElementoBinario.ElementosTipoAceptado(Utilitats.Serializar.TiposAceptados.Byte));
            Partes.Add(ElementoBinario.ElementosTipoAceptado(Utilitats.Serializar.TiposAceptados.Byte));
        }
        protected override IList IGetPartsObject(object obj)
        {
            CrazyKey.CrazyItem item = obj as CrazyKey.CrazyItem;
            return new object[] { item.GenKey, (byte)(int)item.DataMethods, (byte)(int)item.PasswordMethods };
        }

        protected override object JGetObject(MemoryStream bytes)
        {
            object[] partes = GetPartsObject(bytes);
            CrazyKey.CrazyItem cKeyItem = new CrazyKey.CrazyItem();
            cKeyItem.GenKey = (byte)partes[0];
            cKeyItem.DataMethods = (CrazyKey.CrazyItem.MetodoEncrypt)(byte)partes[1];
            cKeyItem.PasswordMethods = (CrazyKey.CrazyItem.MetodoEncrypt)(byte)partes[2];
            return cKeyItem;
        }
    }
}
